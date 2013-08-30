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
case class GetSuccessorOfId(queryId: String)
case class GetId()
case class SetNewSuccessor(successor: NodeRep)
case class SetNewPredecessor(predecessor: NodeRep)

case class NodeRep(node: ActorRef, id: String)

class Node(bootstrapNode: Option[NodeRep]) extends Actor {
  
  val id: String = DigestUtils.sha(UUID.randomUUID().toString()).toString()
  Logger.info("Hash of UUID of newly created node = " + id)
   
  var predecessor = NodeRep(context.self, id)
  var successor   = NodeRep(context.self, id)
  
  bootstrapNode match {
    case None => Logger.info("[Node::constructor()]: No bootstrap node available")
    case Some(bootstrapNode) => {
       
       implicit val timeout = Timeout(35 seconds)
       
       successor =   Await.result((bootstrapNode.node ? GetSuccessorOfId(id)), (35 seconds)).asInstanceOf[NodeRep]
       predecessor = Await.result((successor.node ? GetPredecessor()), (35 seconds)).asInstanceOf[NodeRep]
       Logger.info("[Node::constructor()]: successor and predecessor received")
       
       successor.node   ! SetNewPredecessor(NodeRep(context.self, id))
       predecessor.node ! SetNewSuccessor(NodeRep(context.self, id))
       Logger.info("[Node::constructor()]: Notified successor of new predecessor")
       Logger.info("[Node::constructor()]: Notified predecessor of new successor")
    }
  }
  
  Logger.info("------------------------ Node " + id + " ------------------------")
  Logger.info("Predecessor: " + predecessor)
  Logger.info("Successor: " + successor)
  Logger.info("-----------------------------------------------------------------")
 
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
    case GetSuccessorOfId(queryId) => {
      
      implicit val timeout = Timeout(35 seconds)
      
      (id.compareTo(successor.id) < 0) match {
        case true => {
          (queryId.compareTo(id) >= 0 && queryId.compareTo(successor.id) < 0 ) match {
            case true => sender ! successor
            case false =>sender ! Await.result((successor.node ? GetSuccessorOfId(queryId)), (35 seconds)).asInstanceOf[NodeRep]
          }
        }
        case false => {
          ((queryId.compareTo(id) >= 0 && queryId.compareTo("[B@ffffffff") <= 0) ||
            (queryId.compareTo("[B@00000000") >= 0 && queryId.compareTo(successor.id) < 0) ) match {
          case true => sender ! successor
          case false => sender ! Await.result((successor.node ? GetSuccessorOfId(queryId)), (35 seconds)).asInstanceOf[NodeRep]
          }
        }
      }
    
    }
      
    case _      => Logger.info("received unknown message")
  }
  
}
