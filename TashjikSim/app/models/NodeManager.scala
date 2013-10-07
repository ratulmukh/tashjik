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

case class StartSimulation(nodeCount: Int)
 


class NodeManager extends Actor {
  
  //val myActor1: ActorRef = Akka.system().actorOf(Props[Node]);
  

 // val log = Logging(context.system, this)
  
  def receive = {
    case StartSimulation(nodeCount: Int) => { 
      Logger.info("Received new simulation request: Node count = " + nodeCount)
      
      var bootstrapNode= None : Option[NodeRep]
      var nodeList = List[NodeRep]()
      for(a <- 1 to nodeCount)
      {
        implicit val timeout = Timeout(35 seconds)
        //val t: Iterable[ActorRef] = context.children
        val id: String = DigestUtils.sha512(UUID.randomUUID().toString()).toString()
          val node: ActorRef = context.actorOf(Props(new Node(id, bootstrapNode)).withDispatcher("my-dispatcher"), name = "Node-"+id.substring(3) ) 
        	//val future = node ? "test"
        	//val result = Await.result(future, (35 seconds)).asInstanceOf[String]
        	//Logger.info("returned after testing child with status = " + result)
        	
        	bootstrapNode = Some(NodeRep(node, Await.result(node.ask(GetId())(335 seconds), (335 seconds)).asInstanceOf[String]))
        	bootstrapNode match {
          case None => Logger.info("BootastrapNode is None: Unable to send any message to it")
          case Some(bootstrapnode) => nodeList = bootstrapnode :: nodeList
          Thread.sleep(1000)
        }
        Logger.info("NODE COUNT = " + a)
        Logger.info("Nodelist = " + nodeList)
        	
/*        	bootstrapNode match {
          case None => 
          case Some(g) => nodeList + g
  */  	  
      }
      Thread.sleep(10000)
  /*    val dataStoreCount = 100
      for(a <- 1 to dataStoreCount)
      {
        Logger.info("Random val = " +  (Math.random()*nodeCount).round.toInt.toString())
    */    
        for(node <- nodeList) {
          
          val key = DigestUtils.sha512(UUID.randomUUID().toString()).toString()
          //Logger.info("Random val = " +  (Math.random()*nodeCount).round.toInt.toString() + " Key=" + key)
          node.node ! Store(key, "howdy")
          //Thread.sleep(1000)
  /*      node match {
          case None => Logger.info("BootastrapNode is None: Unable to send any message to it")
          case Some(bootstrapnode) => bootstrapnode.node ! Store(DigestUtils.sha(UUID.randomUUID().toString()).toString(), "howdy")
        }*/
        }
   //   }
    }  
    case _      => Logger.info("received unknown message")
  }
} 