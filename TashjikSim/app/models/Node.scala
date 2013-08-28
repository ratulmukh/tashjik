package models

import play.api._
import akka.actor.{Actor, ActorRef, Props}
import akka.event.Logging
import scala.concurrent.duration._
import akka.pattern.ask
import scala.concurrent.Await
import java.util.UUID
import org.apache.commons.codec.digest.DigestUtils

case class GetPredecessor()
case class GetSuccessor()
case class GetSuccessorOfId(id: String)
case class GetId()

case class NodeRep(node: ActorRef, id: String)

class Node(bootstrapNode: Option[NodeRep]) extends Actor {
  
  val id: String = DigestUtils.sha(UUID.randomUUID().toString()).toString()
  Logger.info("Hash of UUID of newly created node = " + id)
   
  var predecessor: NodeRep = NodeRep(self, id)
  var successor:   NodeRep = NodeRep(self, id)
  
  bootstrapNode match {
    case None => Logger.info("No bootstrap node available")
    case Some(value) => {
       val future1 = value.node.ask(GetPredecessor())(5 seconds)
       predecessor = Await.result(future1, (5 seconds)).asInstanceOf[NodeRep]
       
       
       val future2 = value.node.ask(GetSuccessor())(5 seconds)
       successor = Await.result(future1, (5 seconds)).asInstanceOf[NodeRep]
    }
  }
 
  def receive = {
    case "test" => {
      Logger.info("received test")
      sender ! "All Ok"
    }
    case GetPredecessor() => {
      sender ! predecessor
    }
    case GetId() => {
      sender ! id
    }
    case GetSuccessor() => {
      sender ! successor
    }
    
    case GetSuccessorOfId(id) => {
      Logger.info("Query id = " + id )
      Logger.info("Successor id = " + successor.id)
      if(id.compareTo(successor.id) < 0) {
        Logger.info("Query id < successor.id")}
      else if(id.compareTo(successor.id) > 0) {
        Logger.info("Query id > successor.id")}
      else if(id.compareTo(successor.id) == 0) {
        Logger.info("Query id == successor.id")}
        
      
      
      sender ! successor
    }
    case _      => Logger.info("received unknown message")
  }
  
}
