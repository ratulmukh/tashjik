package models

import play.api._
import play.libs.Akka
import akka.actor.{Actor, ActorRef, Props}
import akka.event.Logging
import scala.concurrent.duration._
import akka.util.Timeout
import akka.pattern.ask
import scala.concurrent.Await

case class StartSimulation(nodeCount: Int)



class NodeManager extends Actor {
  
  //val myActor1: ActorRef = Akka.system().actorOf(Props[Node]);
  

 // val log = Logging(context.system, this)
  
  def receive = {
    case StartSimulation(nodeCount: Int) => { 
      Logger.info("Received new simulation request: Node count = " + nodeCount)
      
      var bootstrapNode= None : Option[NodeRep]
      
      for(a <- 1 to nodeCount)
      {
        implicit val timeout = Timeout(35 seconds)
        //val t: Iterable[ActorRef] = context.children
          val node: ActorRef = context.actorOf(Props(new Node(bootstrapNode))/*, name = "Node"+a */) 
        	//val future = node ? "test"
        	//val result = Await.result(future, (35 seconds)).asInstanceOf[String]
        	//Logger.info("returned after testing child with status = " + result)
        	
        	bootstrapNode = Some(NodeRep(node, Await.result(node.ask(GetId())(335 seconds), (335 seconds)).asInstanceOf[String]))
    	  
      }
    }  
    case _      => Logger.info("received unknown message")
  }
} 