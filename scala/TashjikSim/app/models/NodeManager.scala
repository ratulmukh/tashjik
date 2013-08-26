package models

import play.api._
import play.libs.Akka
import akka.actor.{Actor, ActorRef, Props}
import akka.event.Logging

class NodeManager extends Actor {
  
  //val myActor1: ActorRef = Akka.system().actorOf(Props[Node]);
  case class StartSimulation()

  val log = Logging(context.system, this)
  
  def receive = {
    case StartSimulation => Logger.info("received test")
    case _      => log.info("received unknown message")
  }
}