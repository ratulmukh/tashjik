package models


import org.scalatest._

import scala.concurrent.Await
import scala.concurrent.duration._
import org.apache.commons.codec.digest.DigestUtils
import javax.xml.bind.annotation.adapters.HexBinaryAdapter
import java.util.UUID

import akka.util.Timeout 
import akka.actor._
import akka.pattern.ask

class ExampleTest extends FlatSpec with Matchers {

  "A Node" should "get initialized and return Success message" in {
    
    val bootstrapNode= None : Option[NodeRep]
    implicit val timeout = Timeout(35 seconds)
    val id: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val system = ActorSystem.create("MySystem")
    val node: ActorRef = system.actorOf(Props(new Node(id, bootstrapNode)).withDispatcher("my-dispatcher")) 

    val future = node.ask(Init())(335 seconds)
    val result = Await.result(future, (35 seconds)).asInstanceOf[Success]
    result should be (Success())
  }
}