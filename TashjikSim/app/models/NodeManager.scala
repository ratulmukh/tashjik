package models

import play.api._
import play.libs.Akka
import akka.actor.{Actor, ActorRef, Props}
import akka.event.Logging
import scala.concurrent.duration._
import akka.util.Timeout 
import akka.pattern.ask
import scala.concurrent.Await
import java.util.UUID
import org.apache.commons.codec.digest.DigestUtils
import javax.xml.bind.annotation.adapters.HexBinaryAdapter

case class StartSimulation(nodeCount: Int)

object NodeManager {
  var sessionCount = 0
  
}

class NodeManager extends Actor {
  
  //val myActor1: ActorRef = Akka.system().actorOf(Props[Node]);
  

 // val log = Logging(context.system, this)
  
  def receive = {
    case StartSimulation(nodeCount: Int) => { 
      Logger.info("Received new simulation request: Node count = " + nodeCount)
      
      NodeManager.sessionCount = NodeManager.sessionCount + 1
      
      var bootstrapNode= None : Option[NodeRep]
      var nodeList = List[NodeRep]()
      implicit val timeout = Timeout(35 seconds)
      for(a <- 1 to nodeCount)
      {
        
        //val t: Iterable[ActorRef] = context.children
        val id: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
          val node: ActorRef = context.actorOf(Props(new Node(id, bootstrapNode)).withDispatcher("my-dispatcher"), name = "Node-"+ NodeManager.sessionCount + "-" + a) 
           Await.result(node.ask(Init())(335 seconds), (335 seconds))
          //val future = node ? "test"
        	//val result = Await.result(future, (35 seconds)).asInstanceOf[String]
        	//Logger.info("returned after testing child with status = " + result)
        	
        	//bootstrapNode = Some(NodeRep(node, Await.result(node.ask(GetId())(335 seconds), (335 seconds)).asInstanceOf[String]))
          bootstrapNode = Some(NodeRep(node, id))
          bootstrapNode match {
          case None => Logger.info("BootstrapNode is None: Unable to send any message to it")
          case Some(bootstrapnode) => nodeList = bootstrapnode :: nodeList
          //Thread.sleep(1000)
        }
        Logger.info("NODE COUNT = " + a)
        //Logger.info("Nodelist = " + nodeList)
        	 
/*        	bootstrapNode match {
          case None => 
          case Some(g) => nodeList + g
  */  	  
      }
      var jumper = nodeList.head 
   for(a <- 1 to nodeCount)   
   {
     
      var successor = Await.result((jumper.node ? GetSuccessor()), (35 seconds)).asInstanceOf[NodeRep] 
      var predecessor = Await.result((successor.node ? GetPredecessor()), (35 seconds)).asInstanceOf[NodeRep]
      if(jumper.id.compareTo(predecessor.id) == 0)
        Logger.info("pointing correctly: successor-predeccor: id = " + jumper.id)
      else
        Logger.info("ERROR!! - pointing wrongly: WRONG successor-predeccor: id = " + jumper.id)
        
      if(jumper.id.compareTo(successor.id) < 0)
        Logger.info("in sequence: successor-predeccor: id = " + jumper.id)
      else
        Logger.info("ERROR!! - NOT in sequence: successor-predeccor: id = " + jumper.id)
        
      jumper = successor  
   }  
      
   val dataStoreCount = 1
   for(a <- 1 to dataStoreCount)
   {
      Await.result((jumper.node ? Store(DigestUtils.sha1Hex(UUID.randomUUID().toString()), "howdy")), (35 seconds))
   }     
      
 //     Thread.sleep(10000)
  /*    val dataStoreCount = 100
      for(a <- 1 to dataStoreCount)
      {
        Logger.info("Random val = " +  (Math.random()*nodeCount).round.toInt.toString())
    */    
/*        for(node <- nodeList) {
          
          val key = DigestUtils.sha1Hex(UUID.randomUUID().toString())
          //Logger.info("Random val = " +  (Math.random()*nodeCount).round.toInt.toString() + " Key=" + key)
          node.node ! Store(key, "howdy")
          //Thread.sleep(1000)
  /*      node match {
          case None => Logger.info("BootastrapNode is None: Unable to send any message to it")
          case Some(bootstrapnode) => bootstrapnode.node ! Store(DigestUtils.sha(UUID.randomUUID().toString()).toString(), "howdy")
        }*/
        }
      Thread.sleep(10000)
 */  //   }
    }  
    case _      => Logger.info("received unknown message")
  }
} 