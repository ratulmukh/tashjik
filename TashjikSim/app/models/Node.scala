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


	case class GetPredecessorMsg()
	case class GetSuccessorMsg()
	case class GetSuccessorOfIdSyncMsg(queryId: String)
	case class GetSuccessorOfIdMsg(originalRequestor: ActorRef, query: QueryMsg)
	case class GetIdMsg()
	case class SetNewSuccessorMsg(successor: NodeRep)
	case class SetNewPredecessorMsg(predecessor: NodeRep)

	case class DataRetrievedMsg(store: Store)
	case class RetrieveLocalMsg(requestor: ActorRef, key: String)
    case class RetrieveResultMsg(key: String, value: Option[String])
        
	case class StoreSyncMsg(key: String, value: String)
	case class QueryDestinationFoundMsg(destinationNodeRep: NodeRep, query: QueryMsg)
	case class StoreLocalMsg(key: String, value: String)
	case class StoreLocalSyncMsg(key: String, value: String)
	case class InitMsg()
	case class SuccessMsg()
	case class QueryMsg(key: String, queryType: Either[Store, Retrieve])

	case class NodeRep(node: ActorRef, id: String)
	case class Store(value: String)
	case class Retrieve(originalRequestor: ActorRef)


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

       successor =   Await.result((bootstrapNode.node ? GetSuccessorOfIdSyncMsg(id)), (35 seconds)).asInstanceOf[NodeRep]
       predecessor = Await.result((successor.node ? GetPredecessorMsg()), (35 seconds)).asInstanceOf[NodeRep]
       Logger.info("[Node-" + id + "::constructor()]: successor and predecessor received")
       
       Await.result(successor.node ? SetNewPredecessorMsg(NodeRep(context.self, id)), (35 seconds))
       Await.result(predecessor.node ? SetNewSuccessorMsg(NodeRep(context.self, id)), (35 seconds))
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

  def GetSuccessorOfIdSync(queryId: String): NodeRep = {
    Logger.info("[Node-" + id + "::GetSuccessorOfIdSync(" + queryId + ")]: ENTER")
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
            Await.result((successor.node ? GetSuccessorOfIdSyncMsg(queryId)), (35 seconds)).asInstanceOf[NodeRep]
          }
        }
      }
  }
  
   def GetSuccessorOfId(originalRequestor: ActorRef, query: QueryMsg) = {
    Logger.info("[Node-" + id + "::GetSuccessorOfId(" + query.key + ")]: ENTER")
        
      if((this.id.compareTo(successor.id) == 0) && (this.id.compareTo(predecessor.id) == 0))
      {
         Logger.debug("this.id =" + this.id)
         Logger.debug("successor.id =" + successor.id)
         Logger.debug("predecessor.id =" + predecessor.id)
         Logger.debug(this.id.compareTo(successor.id) + "+" + this.id.compareTo(predecessor.id))
         originalRequestor ! QueryDestinationFoundMsg(successor, query)
      }
      else
      {
        isSuccessorSameFor(query.key) match {
          case true  => originalRequestor ! QueryDestinationFoundMsg(successor, query)
          case false => {
            implicit val timeout = Timeout(35 seconds)
            successor.node ! GetSuccessorOfIdMsg(originalRequestor, query)
          }  
        }
      }
  }
    
  def receive = {
    case InitMsg() => {
      init()
      sender ! SuccessMsg()
    }
    case "test" => {
      Logger.info("received test")
      sender ! "All Ok"
    }
    case GetPredecessorMsg() => {
      sender ! predecessor
    }
    case GetIdMsg() => {
      sender ! id
    }
    case GetSuccessorMsg() => {
      sender ! successor
    }
    case SetNewSuccessorMsg(successor: NodeRep) => {
      this.successor = successor 
      sender ! SuccessMsg()
    }
    
    case SetNewPredecessorMsg(predecessor: NodeRep) => {
      this.predecessor = predecessor  
      sender ! SuccessMsg()
    }
    
    case GetSuccessorOfIdSyncMsg(queryId: String) => {
      sender ! GetSuccessorOfIdSync(queryId)
    }  
    
    case GetSuccessorOfIdMsg(originalRequestor: ActorRef, query: QueryMsg) => {
       GetSuccessorOfId(originalRequestor, query)
    } 
    

    case StoreLocalSyncMsg(key: String, value: String) => {
      Logger.info("Node-" + id + "::receive()->StoreLocal]: Home of [" + key + ", " + value + "] found:" + id)
      keyValueStore + (key->value)
      sender ! SuccessMsg()
    }
    
    case StoreLocalMsg(key: String, value: String) => {
      Logger.info("Node-" + id + "::receive()->StoreLocal]: Home of [" + key + ", " + value + "] found:" + id)
      keyValueStore + (key->value)
    }
    
    case RetrieveLocalMsg(requestor: ActorRef, key: String) => {
      Logger.info("Node-" + id + "::receive()->RetrieveLocal]: Request for key[" + key)
      keyValueStore.contains(key) match {
        case true  => requestor ! RetrieveResultMsg(key, Some(keyValueStore(key)))
        case false => requestor ! RetrieveResultMsg(key, None)
      }  
    }
    
    

    case StoreSyncMsg(key: String, value: String) => {
      Logger.info("[Node-" + id + "::receive()->StoreSync]: ENTER")
      Logger.info("key = " + key + " && value = " + value )
      implicit val timeout = Timeout(35 seconds)
      val keySuccessor =   GetSuccessorOfIdSync(key)
      Logger.info("keySuccessor.id = " + keySuccessor.id)
      Await.result((keySuccessor.node ? StoreLocalMsg(key, value)), (35 seconds))
      sender ! SuccessMsg()
    }
    
    case QueryMsg(key: String, queryType: Either[Store, Retrieve]) => {
      Logger.info("[Node-" + id + "::receive()->QueryMsg]: ENTER")
      //Logger.info("key = " + key + " && value = " + value )
      implicit val timeout = Timeout(35 seconds)
      val requestor = sender
      
      GetSuccessorOfId(self, QueryMsg(key, queryType))
      
    }
    
    case QueryDestinationFoundMsg(destinationNodeRep: NodeRep, query: QueryMsg) => {
      Logger.info("[Node-" + id + "::receive()->StoreDestinationFound]: ENTER")
      
      implicit val timeout = Timeout(35 seconds)
      query.queryType match {
        case Left(storeQuery)     => destinationNodeRep.node ! StoreLocalMsg(query.key, storeQuery.value)
        case Right(retrieveQuery) => destinationNodeRep.node ! RetrieveLocalMsg(retrieveQuery.originalRequestor, query.key)
        
      }
    }
    
/*    
    case Retrieve(key: String) => {
      Logger.info("[Node-" + id + "::receive()->Retrieve]: ENTER")
      Logger.info("key = " + key)
      implicit val timeout = Timeout(35 seconds)
      val requestor = sender
      GetSuccessorOfId(self, Store(key, null))
    }
 */     
    case _      => Logger.info("received unknown message")
  }
  
}
