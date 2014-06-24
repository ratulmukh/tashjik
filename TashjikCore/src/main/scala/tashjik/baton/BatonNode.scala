package tashjik.baton

import akka.actor.{Actor, ActorRef, Props}
import akka.event.Logging
import scala.collection.mutable.MutableList
import scala.concurrent.{ Future, ExecutionContext }
import akka.pattern.ask
import akka.util.Timeout 
import scala.concurrent.duration._
import ExecutionContext.Implicits.global
import scala.util.{Success, Failure}

case class RoutingTableEntry(batonNode: ActorRef, leftChild: Option[ActorRef], rightChild: Option[ActorRef], lowerBound: Int, upperBound: Int)
case class Join()
case class ParentForJoinFound(parentForJoin: ActorRef, assignedLeftChild: Boolean, parentState: BatonNodeState)
case class BatonNodeState(level: Int, number: Int, parent: Option[ActorRef], leftChild: Option[ActorRef], 
		rightChild: Option[ActorRef], leftAdjacent: Option[ActorRef], rightAdjacent: Option[ActorRef],
		leftRoutingTable: List[RoutingTableEntry], rightRoutingTable: List[RoutingTableEntry])
case class NewChildCreated(isUdateForAdjacentNodes: Boolean, assignedLeftChild: Boolean, childLevel: Int, childNumber: Int, child: ActorRef)		
	

class BatonNode(bootstrapNode: Option[ActorRef], nodeMgr: Option[ActorRef]) extends Actor {
  implicit val timeout = Timeout(35 seconds)
  val log = Logging(context.system, this)

  var parent        = None : Option[ActorRef]
  var leftChild     = None : Option[ActorRef]
  var rightChild    = None : Option[ActorRef]
  var leftAdjacent  = None : Option[ActorRef]
  var rightAdjacent = None : Option[ActorRef]
  
  val leftRoutingTable  = scala.collection.mutable.Map[Int, RoutingTableEntry]()
  val rightRoutingTable = scala.collection.mutable.Map[Int, RoutingTableEntry]()
  var (level, number) = (-1, -1)
  
  bootstrapNode match {
    case None => level = 0; number = 1
    case Some(batonNode) => 
      val future: Future[ParentForJoinFound] = ask(batonNode, Join()).mapTo[ParentForJoinFound]
      future onComplete {
      	case Success(parentForJoinFound)  => {
      		parent = Some(parentForJoinFound.parentForJoin)
    		level = parentForJoinFound.parentState.level + 1
    		  
    		if(parentForJoinFound.assignedLeftChild) {
    			leftAdjacent = parentForJoinFound.parentState.leftAdjacent
    		    rightAdjacent = Some(parentForJoinFound.parentForJoin)
    		    number = parentForJoinFound.parentState.number*2 - 1
    		}
    		else {
    		    rightAdjacent = parentForJoinFound.parentState.rightAdjacent
    			leftAdjacent = Some(parentForJoinFound.parentForJoin)
    			number = parentForJoinFound.parentState.number*2
    		}
    	}
    	case Failure(failure) => throw new Exception()
    }
  }
  
   
   def receive = {
    case NewChildCreated(isUdateForAdjacentNodes: Boolean, assignedLeftChild: Boolean, childLevel: Int, childNumber: Int, child: ActorRef) => {
    
      if(isUdateForAdjacentNodes)
      {
    	  if(assignedLeftChild)
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
        	val j = (math.log(childNumber-number)/math.log(2) + 1).toInt
        	rightRoutingTable += (j -> RoutingTableEntry(child, None, None, -1, -1))
        }
        else
        {
            val j = (math.log(number-childNumber)/math.log(2) + 1).toInt
        	leftRoutingTable += (j -> RoutingTableEntry(child, None, None, -1, -1))
        }
        
      }
      
    }	
    
   	case Join() => {
   	  log.info("Join msg received")
   	  
   	  val leftRoutingTableFull = leftRoutingTable.filter(_.batonNode==None).size==0
   	  val rightRoutingTableFull = rightRoutingTable.filter(_.batonNode==None).size==0
   	  
   	  if(leftRoutingTableFull && rightRoutingTableFull && (leftChild==None || rightChild==None))
   	  {
   	    //accept new node as child
   	    
   	    val isLeftChildToBeAssigned: Boolean = leftChild==None
   	    sender ! ParentForJoinFound(context.self, isLeftChildToBeAssigned, BatonNodeState(level, number, parent, leftChild, rightChild, 
      		leftAdjacent, rightAdjacent, leftRoutingTable.toList, rightRoutingTable.toList))
      		
      	val childNumber = isLeftChildToBeAssigned match {
   	      case true => number*2 - 1
   	      case false => number*2
   	    }
   	    
      	isLeftChildToBeAssigned match {
   	      case true => {
   	        leftChild = Some(sender)
   	    	leftAdjacent match {
   	    		case None =>
   	    		case Some(batonNode) => batonNode ! NewChildCreated(true, true, level+1, childNumber,  sender)
   	    	}
   	    	leftAdjacent = Some(sender)
   	      }
   	      case false => {
   	        rightChild = Some(sender)
   	    	rightAdjacent match {
   	    		case None =>
   	    		case Some(batonNode) => batonNode ! NewChildCreated(true, false, level+1, childNumber, sender)
   	    	}
   	    	rightAdjacent = Some(sender)
   	      }
   	    }	
   	    
   	    leftRoutingTable.foreach( leftRoutingTableEntry => {
   	      sendChildNotificationToRTChildren(leftRoutingTableEntry.leftChild, leftRoutingTableEntry.rightChild, isLeftChildToBeAssigned, level+1, childNumber, sender) 
   	    })
      	
   	    rightRoutingTable.foreach( rightRoutingTableEntry => {
   	    	sendChildNotificationToRTChildren(rightRoutingTableEntry.leftChild, rightRoutingTableEntry.rightChild, isLeftChildToBeAssigned, level+1, childNumber, sender) 
     	})

   	    
   	  }
   	  else 
   	  {
   	    if(!leftRoutingTableFull || !rightRoutingTableFull)
   	    {
   	    	//forward JOIN request to parent
   	    	parent match {
   	        	case None => throw new Exception()  //what to do in this case?
   	        	case Some(parent) => parent forward Join()
   	    	}
   	    }	
   	    else
   	    {
   	    	var m = leftRoutingTable.find(x => x.leftChild==None || x.rightChild==None)
   	    	if(m==None)
   	    		m = rightRoutingTable.find(x => x.leftChild==None || x.rightChild==None)
   	    	m match {
   	    	  case Some(routingTable) => routingTable.batonNode forward Join()
   	    	  case None => if(leftAdjacent!=None)
   	    		  				leftAdjacent.get forward Join()
   	    	  			   else if(rightAdjacent!=None)
   	    		  				rightAdjacent.get forward Join()
   	    	  			   else throw new Exception()
   	    	}

   	    }
   	  } 
   	  
    }
   	
  } 
   
  def sendChildNotificationToRTChildren(leftChild: Option[ActorRef], rightChild: Option[ActorRef], isLeftChildAssigned: Boolean, childLevel: Int, childNumber: Int, sender: ActorRef) = {
	   	if(leftChild.isDefined) 
	   		leftChild.get ! NewChildCreated(false, isLeftChildAssigned, childLevel, childNumber,  sender)
   	    if(rightChild.isDefined)
   	        rightChild.get ! NewChildCreated(false, isLeftChildAssigned, childLevel, childNumber,  sender)

  } 
  
  
  
}