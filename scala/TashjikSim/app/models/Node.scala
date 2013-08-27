package models

import play.api._
import akka.actor.{Actor, ActorRef, Props}
import akka.event.Logging
import scala.concurrent.duration._
import akka.pattern.ask
import scala.concurrent.Await

case class GetPredecessor()
case class GetSuccessor()

class Node(bootstrapNode: ActorRef) extends Actor {
  
  val future1 = bootstrapNode.ask(GetPredecessor())(5 seconds)
  val predecessor: Node = Await.result(future1, (5 seconds)).asInstanceOf[Node]
  
  val future2 = bootstrapNode.ask(GetSuccessor())(5 seconds)
  val successor: Node = Await.result(future2, (5 seconds)).asInstanceOf[Node]
  
  val log = Logging(context.system, this)
  def receive = {
    case "test" => {
      Logger.info("received test")
      sender ! "All Ok"
    }
    case GetPredecessor() => {
      sender ! predecessor
    }
    case GetSuccessor() => {
      sender ! successor
    }
    case _      => Logger.info("received unknown message")
  }
  
}
