package models

import play.api._
import akka.actor.{Actor, ActorRef, Props}
import akka.event.Logging
import scala.concurrent.duration._
import akka.pattern.ask
import scala.concurrent.Await

case class GetPredecessor()
case class GetSuccessor()

class Node(bootstrapNode: Option[ActorRef]) extends Actor {
  
  var predecessor: ActorRef = self
  var successor:   ActorRef = self
  
  bootstrapNode match {
    case None => Logger.info("No bootstrap node available")
    case Some(value) => {
       val future1 = value.ask(GetPredecessor())(5 seconds)
       predecessor = Await.result(future1, (5 seconds)).asInstanceOf[ActorRef]
       
       
       val future2 = value.ask(GetSuccessor())(5 seconds)
       successor = Await.result(future1, (5 seconds)).asInstanceOf[ActorRef]
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
    case GetSuccessor() => {
      sender ! successor
    }
    case _      => Logger.info("received unknown message")
  }
  
}
