package models

import akka.actor.{Actor, ActorRef, Props}
import akka.event.Logging

class Node extends Actor {
  
  //case class

  val log = Logging(context.system, this)
  def receive = {
    case "test" => log.info("received test")
    case _      => log.info("received unknown message")
  }
  
}