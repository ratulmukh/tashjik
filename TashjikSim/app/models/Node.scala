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
case class GetSuccessorOfId1(queryId: String)
case class GetId()
case class SetNewSuccessor(successor: NodeRep)
case class SetNewPredecessor(predecessor: NodeRep)

case class Store(key: String, value: String)
case class StoreLocal(key: String, value: String)
case class Retrieve(key: String)
case class RtrievedData(key: String, value: String)
case class Init()
case class Success()
case class NodeRep(node: ActorRef, id: String)

class Node(id: String, bootstrapNode: Option[NodeRep]) extends Actor {
  
 
  //Logger.info("[Node-" + id + "::constructor()]: Hash of UUID of newly created node = " + id)
   
  val keyValueStore: Map[String, String] = Map[String, String]() 
  
  var predecessor = NodeRep(context.self, id)
  var successor   = NodeRep(context.self, id)
 
 def init() {
  Logger.info("[Node-" + id + "::init()]: ENTER")
  bootstrapNode match {
    case None => Logger.info("[Node-" + id + "::constructor()]: No bootstrap node available")
    case Some(bootstrapNode) => {
       
       implicit val timeout = Timeout(35 seconds)
       
 //      try {
         successor =   Await.result((bootstrapNode.node ? GetSuccessorOfId(id)), (35 seconds)).asInstanceOf[NodeRep]
   /*    }
       catch {
              case e: Exception => {
                Logger.info("[Node-" + id + "::receive()->GetSuccessorOf(" + id + ")]: TIMEOUT EXCEPTION HAPPENED .... RETRYING AGAIN")
                sender ! Await.result((bootstrapNode.node ? GetSuccessorOfId(id)), (35 seconds)).asInstanceOf[NodeRep]
              }
            }  

*/     
       predecessor = Await.result((successor.node ? GetPredecessor()), (35 seconds)).asInstanceOf[NodeRep]
       Logger.info("[Node-" + id + "::constructor()]: successor and predecessor received")
       
       Await.result(successor.node ? SetNewPredecessor(NodeRep(context.self, id)), (35 seconds))
       Await.result(predecessor.node ? SetNewSuccessor(NodeRep(context.self, id)), (35 seconds))
       Logger.debug("[Node-" + id + "::constructor()]: Notified successor(" + successor + ") of new predecessor(" + id + ")")
       Logger.debug("[Node-" + id + "::constructor()]: Notified predecessor(" + predecessor + ") of new successor(" + id + ")")
    }
  }
  
  Logger.info("-----------------------------------------------------------------")
  Logger.info("Node: " + id)
  Logger.info("Predecessor: " + predecessor)
  Logger.info("Successor: " + successor)
  Logger.info("-----------------------------------------------------------------")
  
}
  
  def isSuccessorSameFor(queryId: String): Boolean = {
    Logger.info("[Node-" + id + "::isSuccessorSameFor(" + queryId + ")]: ENTER")
    
    if(this.id.compareTo(successor.id) < 0 ) {
      if(queryId.compareTo(successor.id) < 0)
        true
      else
        false
    }
    else {
      if(this.id.compareTo(queryId) < 0 || queryId.compareTo(successor.id) < 0)
        true
      else
          false
    }
  }

  def receive = {
    case Init() => {
      init()
      sender ! Success()
    }
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
      sender ! Success()
    }
    
    case SetNewPredecessor(predecessor: NodeRep) => {
      this.predecessor = predecessor  
      sender ! Success()
    }
    
    case GetSuccessorOfId(queryId) => {
      Logger.info("[Node-" + id + "::receive()->GetSuccessorOfId(" + queryId + ")]: ENTER")
      if((this.id.compareTo(successor.id) == 0) && (this.id.compareTo(predecessor.id) == 0))
      {
         Logger.debug("this.id =" + this.id)
         Logger.debug("successor.id =" + successor.id)
         Logger.debug("predecessor.id =" + predecessor.id)
         Logger.debug(this.id.compareTo(successor.id) + "+" + this.id.compareTo(predecessor.id))
        sender ! successor
      }
      else
      {
        isSuccessorSameFor(queryId) match {
          case true  => sender ! successor
          case false => {
            implicit val timeout = Timeout(35 seconds)
            sender ! Await.result((successor.node ? GetSuccessorOfId(queryId)), (35 seconds)).asInstanceOf[NodeRep]
          }
        }
      }
    }  
    
    case GetSuccessorOfId1(queryId) => {
      Logger.info("[Node-" + id + "::receive()->GetSuccessorOf(" + queryId + ")]: Entering GetSuccessorOfId msg handler - queryID=" + queryId)
      implicit val timeout = Timeout(35 seconds)
       
      (id.compareTo(successor.id) < 0) match {
        case true => {
          Logger.info("[Node-" + id + "::receive()->GetSuccessorOf(" + queryId + ")]: ID(" + id + ") is less than successor.id(" + successor.id + ")")
          (queryId.compareTo(id) > 0 && queryId.compareTo(successor.id) <= 0 ) match {
            case true => {
              Logger.info("[Node-" + id + "::receive()->GetSuccessorOfId]: query ID(" + queryId + ") is inbetween ID(" + id + ") and successor.id(" + successor.id + ")")
              sender ! successor
            }
            case false => {
              Logger.info("[Node-" + id + "::receive()->GetSuccessorOf(" + queryId + ")]: query ID(" + queryId + ") is NOT inbetween ID(" + id + ") and successor.id(" + successor.id + ")")
              try {
                sender ! Await.result((successor.node ? GetSuccessorOfId(queryId)), (35 seconds)).asInstanceOf[NodeRep]
              }
              catch {
              case e: Exception => {
                Logger.info("[Node-" + id + "::receive()->GetSuccessorOf(" + queryId + ")]: TIMEOUT EXCEPTION HAPPENED .... RETRYING AGAIN")
                sender ! Await.result((successor.node ? GetSuccessorOfId(queryId)), (35 seconds)).asInstanceOf[NodeRep]
              }
            }
            }
          }
        }
        case false => {
          Logger.info("[Node-" + id + "::receive()->GetSuccessorOf(" + queryId + ")]: Id(" + id + ") is NOT less than successor.id(" + successor.id + ")")
          ((queryId.compareTo(id) > 0 && queryId.compareTo("[B@ffffffff") <= 0) ||
            (queryId.compareTo("[B@00000000") >= 0 && queryId.compareTo(successor.id) <= 0) ) match {
          case true => {
            Logger.info(
                "[Node-" + id + "::receive()->GetSuccessorOf(" + queryId + ")]: query ID(" + queryId + ") is inbetween ID(" + id + ") and successor.id(" + successor.id + ") BUT ULTA PULTA")
            sender ! successor
          }
          case false => {
            Logger.info("[Node-" + id + "::receive()->GetSuccessorOf(" + queryId + ")]: query ID(" + queryId + ") is NOT inbetween ID(" + id + ") and successor.id(" + successor.id + ") BUT ULTA PULTA")
            try {
              sender ! Await.result((successor.node ? GetSuccessorOfId(queryId)), (35 seconds)).asInstanceOf[NodeRep]
            }
            catch {
              case e: Exception => {
                Logger.info("[Node-" + id + "::receive()->GetSuccessorOf(" + queryId + ")]: TIMEOUT EXCEPTION HAPPENED .... RETRYING AGAIN")
                sender ! Await.result((successor.node ? GetSuccessorOfId(queryId)), (35 seconds)).asInstanceOf[NodeRep]
              }
            }
          }
          }
        }
      }
    
    }
    /*case Store(key: String, value: String) => {
      Logger.info("[Node-" + id + "::receive()->Store]: Entering Store msg handler - key=" + key)
      implicit val timeout = Timeout(35 seconds)
      
      val retrievedSuccessorFuture = (context.self ? (GetSuccessorOfId(key))).mapTo[NodeRep]
      val f2 = retrievedSuccessorFuture map { retrievedSuccessor =>
        retrievedSuccessor.asInstanceOf[NodeRep].node ! StoreLocal(key, value)
      }
    }*/
    case StoreLocal(key: String, value: String) => {
      keyValueStore + (key->value)
      Logger.info("Node-" + id + "::receive()->Store]: Home of [" + key + ", " + value + "] found:" + id)
    }
    
    case Store(key: String, value: String) => {
      Logger.info("[Node-" + id + "::receive()->Store]: Entering Store msg handler - key=" + key + " predecessor=" + predecessor + " id="+ id + " successor=" + successor)
      implicit val timeout = Timeout(35 seconds)
      (predecessor.id.compareTo(id) <= 0) match {
        case true => {
          Logger.info("Node-" + id + "::receive()->Store]: key(" + key + ") predecessor(" + predecessor + ") is less than id(" + id + ")")
          (key.compareTo(predecessor.id) > 0 && key.compareTo(id) <= 0) match {
            case true => {
              keyValueStore + key->value
              Logger.info("Node-" + id + "::receive()->Store]: Home of key(" + key + ") value pair found:" + id)
            }
            case false => {
              Logger.info("Node-" + id + "::receive()->Store]: key(" + key + ") Relaying GetSuccessorOfId call")
              val retrievedSuccessorFuture = (successor.node ? (GetSuccessorOfId(key))).mapTo[NodeRep]
              val f2 = retrievedSuccessorFuture map { x =>
                Logger.info("Node-" + id + "::receive()->Store]: key(" + key + ") Successfully received successor(" + successor + ")")
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
              Logger.info("Node-" + id + "::receive()->Store]: Home of key(" + key + ") value pair found:" + id)
            }
            case false => {
              Logger.info("Node-" + id + "::receive()->Store]: key(" + key + ") Relaying GetSuccessorOfId call")
              val retrievedSuccessorFuture = (successor.node ? (GetSuccessorOfId(key))).mapTo[NodeRep]
              val f2 = retrievedSuccessorFuture map { x =>
                Logger.info("Node-" + id + "::receive()->Store]: key(" + key + ") Successfully received successor(" + successor + ")")
                x.asInstanceOf[NodeRep].node ! Store(key, value)
              }
            }
          } 
        }
      }        
    }
    case Retrieve(key: String) => {
      
    }
      
    case _      => Logger.info("received unknown message")
  }
  
}
