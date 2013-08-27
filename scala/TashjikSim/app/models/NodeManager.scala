package models

import play.api._
import play.libs.Akka
import akka.actor.{Actor, ActorRef, Props}
import akka.event.Logging
import scala.concurrent.duration._
import akka.pattern.ask
import scala.concurrent.Await

case class StartSimulation(nodeCount: Int)

class NodeManager extends Actor {
  
  //val myActor1: ActorRef = Akka.system().actorOf(Props[Node]);
  

 // val log = Logging(context.system, this)
  
  def receive = {
    case StartSimulation(nodeCount: Int) => { 
      Logger.info("Received new simulation request: Node count = " + nodeCount)
      
      var bootstrapNode: ActorRef = null
      
      for(a <- 1 to nodeCount)
      {
        val t: Iterable[ActorRef] = context.children
          val node: ActorRef = context.actorOf(Props(new Node(bootstrapNode)), name = "myChild"+a ) 
        	val future = node.ask("test")(5 seconds)
        	val result = Await.result(future, (5 seconds)).asInstanceOf[String]
        	Logger.info("returned after testing child with status = " + result)
        	
        	bootstrapNode = node
    	  
      } 
    }  
    case _      => Logger.info("received unknown message")
  }
} 