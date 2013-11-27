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
case class GetSuccessorOfIdAsync(originalRequestor: ActorRef, storeRequest: Store)
case class GetId()
case class SetNewSuccessor(successor: NodeRep)
case class SetNewPredecessor(predecessor: NodeRep)

case class Store(key: String, value: String)
case class StoreSync(key: String, value: String)
case class Store1(key: String, value: String)
case class StoreDestinationFound(destinationNodeRep: NodeRep, storeRequest: Store)
case class StoreLocal(key: String, value: String)
case class StoreLocalSync(key: String, value: String)
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
      if(this.id.compareTo(queryId) < 0 && queryId.compareTo(successor.id) <= 0)
        true
      else
        false
    }
    else {
      if(this.id.compareTo(queryId) < 0 || queryId.compareTo(successor.id) <= 0)
        true
      else
          false
    }
  }

  def GetSuccessorOfIdFunc(queryId: String): NodeRep = {
    Logger.info("[Node-" + id + "::GetSuccessorOfId(" + queryId + ")]: ENTER")
      if((this.id.compareTo(successor.id) == 0) && (this.id.compareTo(predecessor.id) == 0))
      {
         Logger.debug("this.id =" + this.id)
         Logger.debug("successor.id =" + successor.id)
         Logger.debug("predecessor.id =" + predecessor.id)
         Logger.debug(this.id.compareTo(successor.id) + "+" + this.id.compareTo(predecessor.id))
        successor
      }
      else
      {
        isSuccessorSameFor(queryId) match {
          case true  => successor
          case false => {
            implicit val timeout = Timeout(35 seconds)
            Await.result((successor.node ? GetSuccessorOfId(queryId)), (35 seconds)).asInstanceOf[NodeRep]
          }
        }
      }
  }
  
   def GetSuccessorOfIdAsyncFunc_next2(originalRequestor: ActorRef, storeRequest: Store) = {
    Logger.info("[Node-" + id + "::GetSuccessorOfIdAsyncFunc_next2(" + storeRequest.key + ")]: ENTER")
    val queryId = storeRequest.key
    
      if((this.id.compareTo(successor.id) == 0) && (this.id.compareTo(predecessor.id) == 0))
      {
         Logger.debug("this.id =" + this.id)
         Logger.debug("successor.id =" + successor.id)
         Logger.debug("predecessor.id =" + predecessor.id)
         Logger.debug(this.id.compareTo(successor.id) + "+" + this.id.compareTo(predecessor.id))
         originalRequestor ! StoreDestinationFound(successor, storeRequest)
      }
      else
      {
        isSuccessorSameFor(queryId) match {
          case true  => originalRequestor ! StoreDestinationFound(successor, storeRequest)
          case false => {
            implicit val timeout = Timeout(35 seconds)
            successor.node ! GetSuccessorOfIdAsync(originalRequestor, storeRequest)
            
            
          }  
        }
      }
  }
  def GetSuccessorOfIdAsyncFunc(requestor: ActorRef, storeRequest: Store, queryId: String) = {
    Logger.info("[Node-" + id + "::GetSuccessorOfIdAsyncFunc(" + queryId + ")]: ENTER")
      if((this.id.compareTo(successor.id) == 0) && (this.id.compareTo(predecessor.id) == 0))
      {
         Logger.debug("this.id =" + this.id)
         Logger.debug("successor.id =" + successor.id)
         Logger.debug("predecessor.id =" + predecessor.id)
         Logger.debug(this.id.compareTo(successor.id) + "+" + this.id.compareTo(predecessor.id))
        requestor ! StoreDestinationFound(successor, storeRequest)
      }
      else
      {
        isSuccessorSameFor(queryId) match {
          case true  => requestor ! StoreDestinationFound(successor, storeRequest)
          case false => {
            implicit val timeout = Timeout(35 seconds)
            val future = successor.node ? GetSuccessorOfIdAsyncFunc_next2(self, storeRequest)
            future onSuccess {
              case destinationNode: NodeRep => requestor ! StoreDestinationFound(destinationNode, storeRequest)
              case _ => throw new Exception()
            }
            
            future onFailure {
              case ise: IllegalStateException if ise.getMessage == "OHNOES" =>
              //OHNOES! We are in deep trouble, do something!
              case e: Exception => throw e
              //Do something else
            }
            
          }  
        }
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
    
    case GetSuccessorOfId(queryId: String) => {
      sender ! GetSuccessorOfIdFunc(queryId)
    }  
    
    case GetSuccessorOfIdAsync(originalRequestor: ActorRef, storeRequest: Store) => {
       GetSuccessorOfIdAsyncFunc_next2(originalRequestor, storeRequest)
    } 
    

    case StoreLocalSync(key: String, value: String) => {
      Logger.info("Node-" + id + "::receive()->StoreLocal]: Home of [" + key + ", " + value + "] found:" + id)
      keyValueStore + (key->value)
      sender ! Success()
    }
    
    case StoreLocal(key: String, value: String) => {
      Logger.info("Node-" + id + "::receive()->StoreLocal]: Home of [" + key + ", " + value + "] found:" + id)
      keyValueStore + (key->value)
    }
    
    case StoreSync(key: String, value: String) => {
      Logger.info("[Node-" + id + "::receive()->StoreSync]: ENTER")
      Logger.info("key = " + key + " && value = " + value )
      implicit val timeout = Timeout(35 seconds)
      val keySuccessor =   GetSuccessorOfIdFunc(key)
      Logger.info("keySuccessor.id = " + keySuccessor.id)
      Await.result((keySuccessor.node ? StoreLocal(key, value)), (35 seconds))
      sender ! Success()
    }
    
    case Store(key: String, value: String) => {
      Logger.info("[Node-" + id + "::receive()->Store]: ENTER")
      Logger.info("key = " + key + " && value = " + value )
      implicit val timeout = Timeout(35 seconds)
      val requestor = sender
      GetSuccessorOfIdAsyncFunc_next2(self, Store(key, value))
      
    }
    
    case StoreDestinationFound(destinationNodeRep: NodeRep, storeRequest: Store) => {
      Logger.info("[Node-" + id + "::receive()->StoreDestinationFound]: ENTER")
      
      implicit val timeout = Timeout(35 seconds)
      destinationNodeRep.node ! StoreLocal(storeRequest.key, storeRequest.value)
    }
    
    
    case Retrieve(key: String) => {
      
    }
      
    case _      => Logger.info("received unknown message")
  }
  
}
