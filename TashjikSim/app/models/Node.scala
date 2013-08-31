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
import scala.concurrent.{ Future, ExecutionContext }
import ExecutionContext.Implicits.global
import akka.dispatch.OnSuccess

case class GetPredecessor()
case class GetSuccessor()
case class GetSuccessorOfId(queryId: String)
case class GetId()
case class SetNewSuccessor(successor: NodeRep)
case class SetNewPredecessor(predecessor: NodeRep)
case class NewSuccessorSuccessfullySet()
case class NewPredecessorSuccessfullySet()

case class Store(key: String, value: String)
case class StoreLocal(key: String, value: String)
case class Retrieve(key: String)
case class RtrievedData(key: String, value: String)

case class NodeRep(node: ActorRef, id: String)

class Node(bootstrapNode: Option[NodeRep]) extends Actor {
  
  val id: String = DigestUtils.sha(UUID.randomUUID().toString()).toString()
  Logger.info("[Node-" + id + "::constructor()]: Hash of UUID of newly created node = " + id)
   
  val keyValueStore: Map[String, String] = Map[String, String]() 
  
  var predecessor = NodeRep(context.self, id)
  var successor   = NodeRep(context.self, id)
  
  bootstrapNode match {
    case None => Logger.info("[Node-" + id + "::constructor()]: No bootstrap node available")
    case Some(bootstrapNode) => {
       
       implicit val timeout = Timeout(35 seconds)
       
       successor =   Await.result((bootstrapNode.node ? GetSuccessorOfId(id)), (35 seconds)).asInstanceOf[NodeRep]
       predecessor = Await.result((successor.node ? GetPredecessor()), (35 seconds)).asInstanceOf[NodeRep]
       Logger.info("[Node-" + id + "::constructor()]: successor and predecessor received")
       
       Await.result(successor.node ? SetNewPredecessor(NodeRep(context.self, id)), (35 seconds))
       Await.result(predecessor.node ? SetNewSuccessor(NodeRep(context.self, id)), (35 seconds))
       Logger.info("[Node-" + id + "::constructor()]: Notified successor of new predecessor")
       Logger.info("[Node-" + id + "::constructor()]: Notified predecessor of new successor")
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
      sender ! NewSuccessorSuccessfullySet()
    }
    
    case SetNewPredecessor(predecessor: NodeRep) => {
      this.predecessor = predecessor  
      sender ! NewPredecessorSuccessfullySet()
    }
    case GetSuccessorOfId(queryId) => {
      Logger.info("[Node-" + id + "::receive()->GetSuccessorOfId]: Entering GetSuccessorOfId msg handler - queryID=" + queryId)
      implicit val timeout = Timeout(35 seconds)
      
      (id.compareTo(successor.id) <= 0) match {
        case true => {
          (queryId.compareTo(id) > 0 && queryId.compareTo(successor.id) <= 0 ) match {
            case true => sender ! successor
            case false =>sender ! Await.result((successor.node ? GetSuccessorOfId(queryId)), (35 seconds)).asInstanceOf[NodeRep]
          }
        }
        case false => {
          ((queryId.compareTo(id) > 0 && queryId.compareTo("[B@ffffffff") <= 0) ||
            (queryId.compareTo("[B@00000000") >= 0 && queryId.compareTo(successor.id) <= 0) ) match {
          case true => sender ! successor
          case false => sender ! Await.result((successor.node ? GetSuccessorOfId(queryId)), (35 seconds)).asInstanceOf[NodeRep]
          }
        }
      }
    
    }
    case Store(key: String, value: String) => {
      Logger.info("[Node-" + id + "::receive()->Store]: Entering Store msg handler - key=" + key)
      implicit val timeout = Timeout(35 seconds)
      
      val retrievedSuccessorFuture = (context.self ? (GetSuccessorOfId(key))).mapTo[NodeRep]
      val f2 = retrievedSuccessorFuture map { retrievedSuccessor =>
        retrievedSuccessor.asInstanceOf[NodeRep].node ! StoreLocal(key, value)
      }
    }
    case StoreLocal(key: String, value: String) => {
      keyValueStore + key->value
      Logger.info("Node-" + id + "::receive()->Store]: Home of [" + key + ", " + value + "] found:" + id)
    }
    
  /*    (predecessor.id.compareTo(id) <= 0) match {
        case true => {
          (key.compareTo(predecessor.id) > 0 && key.compareTo(id) <= 0) match {
            case true => {
              keyValueStore + key->value
              Logger.info("Node::receive()->Store]: Home of key value pair found:" + id)
            }
            case false => {
              Logger.info("Node::receive()->Store]: Relaying GetSuccessorOfId call")
              val retrievedSuccessorFuture = (successor.node ? (GetSuccessorOfId(key))).mapTo[NodeRep]
              val f2 = retrievedSuccessorFuture map { x =>
                x.asInstanceOf[NodeRep].node ! Store(key, value)
              }
            }
          }  
        }
        case false => {
          Logger.info("ULTA PULTA")
          ((key.compareTo(predecessor.id) > 0 && key.compareTo("[B@ffffffff") <= 0) ||
            (key.compareTo("[B@00000000") >= 0 && key.compareTo(successor.id) <= 0) ) match {
            case true => {
              keyValueStore + key->value
              Logger.info("Node::receive()->Store]: Home of key value pair found:" + id)
            }
            case false => {
              Logger.info("Node::receive()->Store]: Relaying GetSuccessorOfId call")
              val retrievedSuccessorFuture = (successor.node ? (GetSuccessorOfId(key))).mapTo[NodeRep]
              val f2 = retrievedSuccessorFuture map { x =>
                x.asInstanceOf[NodeRep].node ! Store(key, value)
              }
            }
          } 
        }
      }        
    }*/
    case Retrieve(key: String) => {
      
    }
      
    case _      => Logger.info("received unknown message")
  }
  
}
