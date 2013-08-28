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
case class GetPredecessorOfId(id: String)
case class GetId()
case class SetNewSuccessor(successor: NodeRep)
case class SetNewPredecessor(predecessor: NodeRep)

case class NodeRep(node: ActorRef, id: String)

class Node(bootstrapNode: Option[NodeRep]) extends Actor {
  
  val id: String = DigestUtils.sha(UUID.randomUUID().toString()).toString()
  Logger.info("Hash of UUID of newly created node = " + id)
   
  var predecessor = None : Option[NodeRep]
  var successor   = None : Option[NodeRep]
  
  bootstrapNode match {
    case None => Logger.info("No bootstrap node available")
    case Some(bootstrapNode) => {
       
       implicit val timeout = Timeout(5 seconds)
       
       successor =   Await.result((bootstrapNode.node ? GetSuccessorOfId(id)), (5 seconds)).asInstanceOf[Option[NodeRep]]
       
       successor match {
         case None => {
           predecessor = Await.result((bootstrapNode.node ? GetPredecessorOfId(id)), (5 seconds)).asInstanceOf[Option[NodeRep]]
           predecessor match {
             case None => {
               Logger.error("SHOULD NOT REACH HERE")
             }
             case Some(predecessor) => {
               predecessor.node ! SetNewSuccessor(NodeRep(context.self, id))
             }
           }
         }
         case Some(successor) => {
           predecessor = Await.result((successor.node     ? GetPredecessor()),     (5 seconds)).asInstanceOf[Option[NodeRep]]
           predecessor match {
             case None => {
               successor.node   ! SetNewPredecessor(NodeRep(context.self, id))
             }
             case Some(predecessor) => {
               successor.node   ! SetNewPredecessor(NodeRep(context.self, id))
               predecessor.node ! SetNewSuccessor(NodeRep(context.self, id))
             }
           }
         }
       }
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
    case SetNewSuccessor(successor: Option[NodeRep]) => {
      this.successor = successor 
    }
    
    case SetNewPredecessor(predecessor: Option[NodeRep]) => {
      this.predecessor = predecessor  
    }
    
    case GetPredecessorOfId(queryId) => {
      
      implicit val timeout = Timeout(5 seconds)
      
      val noPredecessor = None : Option[NodeRep]
      successor match {
        case None => {
          predecessor match {
          	case None => {
          	  if(queryId.compareTo(id) > 0)
          	    sender ! Some(NodeRep(context.self, id))
          	}
          	case Some(predecessor) => {
          	  if(queryId.compareTo(predecessor.id) > 0)
          	    sender ! Some(predecessor)
          	  else
          	    sender ! Await.result((predecessor.node ? GetPredecessorOfId(queryId)), (5 seconds)).asInstanceOf[Option[NodeRep]]
          	}
          }
        }
        case Some(successor) => {
          if(queryId.compareTo(id) > 0 && queryId.compareTo(successor.id) < 0)
            sender ! Some(NodeRep(context.self, id))
          else if(queryId.compareTo(successor.id) > 0)
            sender ! Await.result((successor.node ? GetPredecessorOfId(queryId)), (5 seconds)).asInstanceOf[Option[NodeRep]]
          else
          {
            predecessor match {
              case None => {
                Logger.info("No predecessor")
                sender ! noPredecessor
              }
              case Some(predecessor) => {
                if(queryId.compareTo(id) < 0 && queryId.compareTo(predecessor.id) > 0)
                  sender ! Some(predecessor)
                else if(queryId.compareTo(predecessor.id) < 0 )
                  sender ! Await.result((predecessor.node ? GetPredecessorOfId(queryId)), (5 seconds)).asInstanceOf[Option[NodeRep]]
              }
            }
          }
        }
        

      }
    }
    
    case GetSuccessorOfId(queryId) => {
      
      implicit val timeout = Timeout(5 seconds)
      
      val noSuccessor = None : Option[NodeRep]
      successor match {
        case None => {
          predecessor match {
          	case None => {
          	  if(queryId.compareTo(id) > 0)
          	  {
          	    Logger.info("No successor")
          	    sender ! noSuccessor
          	  }
          	  else if(queryId.compareTo(id) < 0)
          	    sender ! Some(NodeRep(context.self, id))
          	}
          	case Some(predecessor) => {
          	  if(queryId.compareTo(id) <0 && queryId.compareTo(predecessor.id) > 0)
          	    sender ! Some(NodeRep(context.self, id))
          	  else if(queryId.compareTo(predecessor.id) < 0)
          	    sender ! Await.result((predecessor.node ? GetSuccessorOfId(queryId)), (5 seconds)).asInstanceOf[Option[NodeRep]]
          	}
          }
        }
        case Some(successor) => {
          if(queryId.compareTo(id) > 0 && queryId.compareTo(successor.id) < 0)
            sender ! Some(successor)
          else if(queryId.compareTo(successor.id) > 0)
            sender ! Await.result((successor.node ? GetSuccessorOfId(queryId)), (5 seconds)).asInstanceOf[Option[NodeRep]]
          else
          {
            predecessor match {
              case None => {
                sender ! Some(NodeRep(context.self, id))
              }
              case Some(predecessor) => {
                if(queryId.compareTo(id) < 0 && queryId.compareTo(predecessor.id) > 0)
                  sender ! Some(NodeRep(context.self, id))
                else if(queryId.compareTo(predecessor.id) < 0 )
                  sender ! Await.result((predecessor.node ? GetSuccessorOfId(queryId)), (5 seconds)).asInstanceOf[Option[NodeRep]]
              }
            }
          }
        }
        

      }
    }

    case _      => Logger.info("received unknown message")
  }
  
}