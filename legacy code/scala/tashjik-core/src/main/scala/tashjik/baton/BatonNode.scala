package tashjik.baton

import akka.actor.{Actor, ActorRef, Props}
import akka.event.Logging
import scala.collection.mutable.MutableList
import scala.concurrent.{ Future, ExecutionContext }
import akka.pattern.ask
import akka.util.Timeout 
import scala.concurrent.Await
import scala.concurrent.duration._
import ExecutionContext.Implicits.global
import scala.util.{Success, Failure}
import scala.util.control.Breaks
import ChildPosition._
import scala.collection.mutable.ArrayBuffer



//--------ENUMS-------------------------------------------------------
sealed trait RoutingTableType
case object LeftRoutingTable extends RoutingTableType
case object RightRoutingTable extends RoutingTableType
//--------------------------------------------------------------------   

case class RoutingTableEntry(index: Integer, batonNode: Option[ActorRef], leftChild: Option[ActorRef], rightChild: Option[ActorRef], lowerBound: Int, upperBound: Int)

case class JoinMsg()
case class ParentForJoinFoundMsg(parent: ActorRef, assignedChildPosition: ChildPosition, parentState: BatonNodeStateMsg)
case class BatonNodeStateMsg(level: BigInt, number: BigInt, parent: Option[ActorRef], leftChild: Option[ActorRef], 
		rightChild: Option[ActorRef], leftAdjacent: Option[ActorRef], rightAdjacent: Option[ActorRef], 
		leftRoutingTable: ArrayBuffer[RoutingTableEntry], rightRoutingTable: ArrayBuffer[RoutingTableEntry])
case class ChildCreatedMsg(isUdateForAdjacentNodes: Boolean, assignedChildPosition: ChildPosition, childLevel: BigInt, childNumber: BigInt, child: ActorRef)		
case class GetStateMsg()	


class BatonNode(bootstrapNode: Option[ActorRef], nodeMgr: Option[ActorRef]) extends Actor {

  implicit val timeout = Timeout(35 seconds)
  val log = Logging(context.system, this)

  // ---------BATON node state-------------------------------------------------------
  var parent        = None : Option[ActorRef]
  var leftChild     = None : Option[ActorRef]
  var rightChild    = None : Option[ActorRef]
  var leftAdjacent  = None : Option[ActorRef]
  var rightAdjacent = None : Option[ActorRef]
  
  var (level, number) = (BigInt(-1), BigInt(-1))
  
  val leftRoutingTable  = ArrayBuffer[RoutingTableEntry]()
  val rightRoutingTable  = ArrayBuffer[RoutingTableEntry]()
  //----------------------------------------------------------------------------------
    
  bootstrap()
  
  
  
  def receive = {
    case ChildCreatedMsg(isUdateForAdjacentNodes: Boolean, assignedChildPosition: ChildPosition, childLevel: BigInt, childNumber: BigInt, child: ActorRef) => {
      newChildCreatedMsg(isUdateForAdjacentNodes, assignedChildPosition, childLevel, childNumber, child)
    }	
    case GetStateMsg() => sender ! BatonNodeStateMsg(level, number, parent, leftChild, rightChild, leftAdjacent, rightAdjacent, leftRoutingTable, rightRoutingTable)
   	case JoinMsg() => join()
   	case _ => assert(false)   
  } 
  
  def bootstrap() {
    bootstrapNode match {
      case None => initFirstNodeInNetwork()
      case Some(batonNode) => { 
        //retrieve parent state
        val parentForJoinFoundMsg =   Await.result((batonNode ? JoinMsg()), (35 seconds)).asInstanceOf[ParentForJoinFoundMsg]
      
        //set state based on parent's state ------------------------------------------------------------
        parent = Some(parentForJoinFoundMsg.parent)
    	  level = parentForJoinFoundMsg.parentState.level + 1
    		  
    	  parentForJoinFoundMsg.assignedChildPosition match {
          case LeftChild => {
            leftAdjacent = parentForJoinFoundMsg.parentState.leftAdjacent
    		    rightAdjacent = Some(parentForJoinFoundMsg.parent)
    		    number = parentForJoinFoundMsg.parentState.number*2 - 1
          }
          case RightChild => {
            rightAdjacent = parentForJoinFoundMsg.parentState.rightAdjacent
      		  leftAdjacent = Some(parentForJoinFoundMsg.parent)
    	  	  number = parentForJoinFoundMsg.parentState.number*2
          }
        }
      
        setupRoutingTable(LeftRoutingTable, parentForJoinFoundMsg.parentState)
        setupRoutingTable(RightRoutingTable, parentForJoinFoundMsg.parentState)

        log.info("Left routing table after joining network:")
     		leftRoutingTable.foreach (entry => log.info("Entry=" + entry.toString))
   	  	log.info("Right routing table after joining network:")
   		  rightRoutingTable.foreach (entry => log.info("Entry=" + entry.toString))
        //---------------------------------------------------------------------------------------------
      }     
    }
  }
  
  def setupRoutingTable(routingTableType: RoutingTableType, parentState: BatonNodeStateMsg) {

    var routingTable = routingTableType match {
      case LeftRoutingTable  => leftRoutingTable
      case RightRoutingTable => rightRoutingTable
    }
    
    var index = 0
    var siblingNumber = number
    var siblingParentNumber = number         //this is the number of the sibling's parent
    var continueLoop = true
    
    while (continueLoop)
    {
      siblingNumber = calculateSiblingNumber(index, routingTableType)
      siblingParentNumber = if (siblingNumber % 2 == 0) siblingNumber/2  else (siblingNumber/2 + 1)          
        
      val extractedSiblingNode = extractSiblingNodeFromParentState(index, parentState.number, siblingNumber, siblingParentNumber, parentState, routingTableType)
      routingTable += RoutingTableEntry(index, extractedSiblingNode, None, None, -1, -1)
        
      //updating loop parameters
    	index = index + 1
    	if ((routingTableType==LeftRoutingTable && siblingNumber<=0) ||
    	    (routingTableType==RightRoutingTable && siblingNumber> BigInt(2).pow(level.intValue())))
    	  continueLoop = false
    	
    }
  }
  
  def calculateSiblingNumber(index: Int, routingTableType: RoutingTableType): BigInt = {
    routingTableType match {
      case LeftRoutingTable => number - BigInt(2).pow(index)
      case RightRoutingTable =>   number + BigInt(2).pow(index)
    }
  }
  
  
  def extractSiblingNodeFromParentState(index: Integer, parentNumber: BigInt, siblingNumber: BigInt, siblingParentNumber: BigInt, parentState: BatonNodeStateMsg, routingTableType: RoutingTableType): Option[ActorRef] = {
    if(siblingParentNumber == parentState.number) {     //sibling and this node have the same parent; so no need to extract from routing table
      if(siblingNumber%2 == 0)
        parentState.rightChild
      else
        parentState.leftChild
    }
    else {                                              //sibling needs to be extracted from the routing table
      val routingTable: ArrayBuffer[RoutingTableEntry] =  routingTableType match {
        case LeftRoutingTable => parentState.leftRoutingTable
        case RightRoutingTable =>   parentState.rightRoutingTable
      }

      val routingTableIndex = (scala.math.log10((parentNumber - siblingParentNumber).abs.doubleValue())/scala.math.log10(2)).intValue()

      log.debug(" parentNumber=" + parentNumber + " siblingParentNumber=" + siblingParentNumber + " routingTableIndex = " + routingTableIndex)
      routingTable.foreach { entry => log.debug(entry.toString) }
      
      val foundSiblingArray = routingTable.filter { r: RoutingTableEntry => r.index == routingTableIndex }
      
      assert(foundSiblingArray.size<2)
      
      if(foundSiblingArray.size==0)
        None
      else {  
        if(siblingNumber%2 == 0)
          foundSiblingArray(0).rightChild
        else
          foundSiblingArray(0).leftChild
      }    
    }
  }
  
  
  
  
  
  
  def newChildCreatedMsg(isUdateForAdjacentNodes: Boolean, assignedChildPosition: ChildPosition, childLevel: BigInt, childNumber: BigInt, child: ActorRef) {
    if(isUdateForAdjacentNodes)
    {
     if(assignedChildPosition==LeftChild)
       rightAdjacent = Some(child)
     else
       leftAdjacent = Some(child)
    }
    else 
    {
      //update is for routing table children to update their routing tables to point to new kid on the block 
      assert(level==childLevel)
      assert(number!=childNumber)
      
        if(childNumber > number)
        {
        	val j = (math.log((childNumber-number).doubleValue())/math.log(2) + 1).toInt
        	rightRoutingTable += RoutingTableEntry(j, Some(child), None, None, -1, -1)
        }
        else
        {
            val j = (math.log((childNumber-number).doubleValue())/math.log(2) + 1).toInt
        	leftRoutingTable += RoutingTableEntry(j, Some(child), None, None, -1, -1)
        }
      }
  }
  
 
  
  def join() {
     log.info("Join msg received")
   	  
   	  val leftRoutingTableFull = leftRoutingTable.size==0
   	  val rightRoutingTableFull = rightRoutingTable.size==0
   	  
   	  
   	  //Node join algorithm from BATON paper: Algorithm 1 Join(node n)
   	  if(leftRoutingTableFull && rightRoutingTableFull && (leftChild==None || rightChild==None))
   	    acceptNewNodeAsChild()
   	  else 
   	  {
   	    if(!leftRoutingTableFull || !rightRoutingTableFull)
   	      forward_JOIN_Request(parent)
   	    else
   	    {
   	      findSomeNodeNotHavingEnoughChildren() match {
   	        case None       => forward_JOIN_RequestToEither(leftAdjacent, rightAdjacent)
   	        case Some(node) => forward_JOIN_Request(Some(node))
   	      }  
   	    }
   	  } 
  }
   
  def forward_JOIN_RequestToEither(node1: Option[ActorRef], node2: Option[ActorRef]) {
    if(node1!=None)
   	  forward_JOIN_Request(node1)
   	else if(node2!=None)
   	  forward_JOIN_Request(node2)
   	else 
   	  throw new Exception()
  }
   
  def findSomeNodeNotHavingEnoughChildren(): Option[ActorRef] = {
    var m = leftRoutingTable.find(entry => entry.leftChild==None || entry.rightChild==None)
   	    	if(m==None)
   	    		m = rightRoutingTable.find(entry => entry.leftChild==None || entry.rightChild==None)
   	    		
   	    		m match { 
   	    	    case None => None
   	    	    case Some(x) => x.batonNode
   	    	}
   	    		

   	    	
  }
   
  def forward_JOIN_Request(node: Option[ActorRef]) {
    node match {
   	  case None => throw new Exception()  //what to do in this case?
   	  case Some(x) => x forward JoinMsg()
   	}
  }
  
  
  def initFirstNodeInNetwork() {
    level = 0 
    number = 1
  }
  
  def acceptNewNodeAsChild() {
    
   	    sender ! ParentForJoinFoundMsg(context.self, if(leftChild==None) LeftChild else RightChild, BatonNodeStateMsg(level, number, parent, leftChild, rightChild, 
      		leftAdjacent, rightAdjacent, leftRoutingTable, rightRoutingTable))
      		
      	val childNumber: BigInt = leftChild==None match {
   	      case true => number*2 - 1
   	      case false => number*2
   	    }
   	    
      	leftChild==None match {
   	      case true => {
   	        leftChild = Some(sender)
   	    	  leftAdjacent match {
   	    		  case None =>
   	    		  case Some(batonNode) => batonNode ! ChildCreatedMsg(true, LeftChild, level+1, childNumber,  sender)
   	    	  }
   	    	  leftAdjacent = Some(sender)
   	      }
   	      case false => {
   	        rightChild = Some(sender)
   	    	  rightAdjacent match {
   	    		  case None =>
   	    		  case Some(batonNode) => batonNode ! ChildCreatedMsg(true, RightChild, level+1, childNumber, sender)
   	    	  }
   	    	  rightAdjacent = Some(sender)
   	      }
   	    }	
   	    
   	    leftRoutingTable.foreach( leftRoutingTableEntry => {
   	      sendChildNotificationToRTChildren(leftRoutingTableEntry.leftChild, leftRoutingTableEntry.rightChild, if(leftChild==None) LeftChild else RightChild, level+1, childNumber, sender) 
   	    })
      	
   	    rightRoutingTable.foreach( rightRoutingTableEntry => {
   	    	sendChildNotificationToRTChildren(rightRoutingTableEntry.leftChild, rightRoutingTableEntry.rightChild, if(leftChild==None) LeftChild else RightChild, level+1, childNumber, sender) 
     	})
    
  }
  
  
  def sendChildNotificationToRTChildren(leftChild: Option[ActorRef], rightChild: Option[ActorRef], assignedChildPosition: ChildPosition, childLevel: BigInt, childNumber: BigInt, sender: ActorRef) = {
	   	if(leftChild.isDefined) 
	   		leftChild.get ! ChildCreatedMsg(false, assignedChildPosition, childLevel, childNumber, sender)
   	    if(rightChild.isDefined)
   	        rightChild.get ! ChildCreatedMsg(false, assignedChildPosition, childLevel, childNumber, sender)

  } 
  
  
  
}