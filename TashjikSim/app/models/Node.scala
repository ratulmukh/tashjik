package models

import play.api._
import akka.actor.{Actor, ActorRef, Props}
import akka.event.Logging
import scala.concurrent.duration._
import akka.util.Timeout
import akka.pattern.ask
import scala.concurrent.Await
import java.util.UUID
import org.apache.commons.codec.digest.DigestUtils

case class GetPredecessor()
case class GetSuccessor()
case class GetSuccessorOfId(id: String)
case class GetId()
case class SetNewSuccessor(successor: NodeRep)
case class SetNewPredecessor(predecessor: NodeRep)

case class NodeRep(node: ActorRef, id: String)

class Node(bootstrapNode: Option[NodeRep]) extends Actor {
  
  val id: String = DigestUtils.sha(UUID.randomUUID().toString()).toString()
  Logger.info("Hash of UUID of newly created node = " + id)
   
  var predecessor: NodeRep = NodeRep(self, id)
  var successor:   NodeRep = NodeRep(self, id)
  
  bootstrapNode match {
    case None => Logger.info("No bootstrap node available")
    case Some(bootstrapNode) => {
       
       implicit val timeout = Timeout(5 seconds)
       
       successor =   Await.result((bootstrapNode.node ? GetSuccessorOfId(id)), (5 seconds)).asInstanceOf[NodeRep]
       predecessor = Await.result((successor.node     ? GetPredecessor()),     (5 seconds)).asInstanceOf[NodeRep]
       
       successor.node   ! SetNewPredecessor(NodeRep(context.self, id))
       predecessor.node ! SetNewSuccessor(NodeRep(context.self, id))
           
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
    case SetNewSuccessor(successor: NodeRep) => {
      this.successor = successor 
    }
    
    case SetNewPredecessor(predecessor: NodeRep) => {
      this.predecessor = predecessor  
    }
    
    case GetSuccessorOfId(id) => {
      Logger.info("Query id = " + id )
      Logger.info("Successor id = " + successor.id)
      
      var querySuccessor: NodeRep = NodeRep(context.self, id)
      
      if( id.compareTo(predecessor.id) == 0 || id.compareTo(successor.id) == 0 ) 
        Logger.warn("id matches predecessor or successor")
      if( id.compareTo(this.id) > 0 && id.compareTo(successor.id) <= 0 ) {
        Logger.info("Query success")
        querySuccessor = NodeRep(context.self, id)
      }
      else if(id.compareTo(successor.id) > 0) {
        implicit val timeout = Timeout(5 seconds)
        Logger.info("Query needs to be relayed: id > successor.id")
        querySuccessor = Await.result((successor.node ? GetSuccessorOfId(id)), (5 seconds)).asInstanceOf[NodeRep]
      }
      else if(id.compareTo(this.id) < 0) {
        implicit val timeout = Timeout(5 seconds)
        Logger.info("Query needs to be relayed: id < successor.id")
        querySuccessor = Await.result((predecessor.node ? GetSuccessorOfId(id)), (5 seconds)).asInstanceOf[NodeRep]
      }
      
      
        
      
      
      sender ! successor
    }
    case _      => Logger.info("received unknown message")
  }
  
}
