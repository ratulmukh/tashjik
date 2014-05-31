package tashjik.chord

import play.api.libs.json._
import akka.actor.{Actor, ActorRef, Props}
import akka.event.Logging
import scala.concurrent.duration._
import akka.util.Timeout 
import akka.pattern.ask
import scala.concurrent.Await
import java.util.UUID
import org.apache.commons.codec.digest.DigestUtils
import javax.xml.bind.annotation.adapters.HexBinaryAdapter
import play.api.libs.iteratee.Enumerator
import play.api.libs.iteratee.Iteratee
import play.api.libs.iteratee.Concurrent
import play.api.libs.concurrent.Execution.Implicits._
import play.api.libs.functional.syntax._

case class StartSimulation(nodeCount: Int, dataStoreCount: Int)
case class CancelSimulation
case class GetIterateeAndEnumerator
case class IterateeAndEnumerator(in: Iteratee[String,Unit], out: Enumerator[String])
case class WebsocketMsg(msg: String)
case class Circle(cx: Double, cy: Double, r: Double)
case class Circles(circleList: List[Circle])


object NodeManager {
  var sessionCount = 0
  
}


class NodeManager extends Actor {
  implicit class PathAdditions(path: JsPath) {

  def readNullableIterable[A <: Iterable[_]](implicit reads: Reads[A]): Reads[A] =
    Reads((json: JsValue) => path.applyTillLast(json).fold(
      error => error,
      result => result.fold(
        invalid = (_) => reads.reads(JsArray()),
        valid = {
          case JsNull => reads.reads(JsArray())
          case js => reads.reads(js).repath(path)
        })
    ))

  def writeNullableIterable[A <: Iterable[_]](implicit writes: Writes[A]): OWrites[A] =
    OWrites[A]{ (a: A) =>
      if (a.isEmpty) Json.obj()
      else JsPath.createObj(path -> writes.writes(a))
    }

  /** When writing it ignores the property when the collection is empty,
    * when reading undefined and empty jsarray becomes an empty collection */
  def formatNullableIterable[A <: Iterable[_]](implicit format: Format[A]): OFormat[A] =
    OFormat[A](r = readNullableIterable(format), w = writeNullableIterable(format))

}
  implicit val circleWrites: Writes[Circle] = (
     (__ \ "cx").write[Double] and
     (__ \ "cy").write[Double] and
     (__ \ "r").write[Double]  
 )(unlift(Circle.unapply))
 
 //implicit val circlesWrites: Writes[Circles] = (
   //  (__ \ "circleList").write[Array[Circle]]
 //)(unlift(Circle.unapply))
 //val writes: Writes[Circles] = (
 // (__ \ "circleList").writeNullableIterable[List[Circle]]
//)(unlift(Something.unapply))

   val log = Logging(context.system, this)
  //val myActor1: ActorRef = Akka.system().actorOf(Props[Node]);
   val (out,channel) = Concurrent.broadcast[String]

   val in = Iteratee.foreach[String] {
      msg => println(msg)
             //the Enumerator returned by Concurrent.broadcast subscribes to the channel and will 
             //receive the pushed messages
             log.info("Entered Iteratee closure")
      		 context.self ! WebsocketMsg(msg)
    }

 
    def receive = {
     case GetIterateeAndEnumerator => {
       sender ! IterateeAndEnumerator(in, out)

     } 
     case WebsocketMsg(msg: String) => {
       val msgJson = Json.parse(msg)
       val messageType: String = (msgJson \ "msgType").as[String]
       
       messageType match {
         case "startSim" => 
           channel push(Json.toJson(List(Circle(483.12, 401.5, 10), Circle(483.12, 402.5, 10))).toString)
           //channel push(Json.toJson(Circles(List(Circle(483.12, 401.5, 10), Circle(483.12, 402.5, 10)))).toString)
           //channel push(Json.toJson(Circle(483.12, 401.5, 10)).toString)
           
           
           context.self ! StartSimulation((msgJson \ "nodeCount").as[Int], (msgJson \ "dataObjectsCount").as[Int])
           //channel push("Simulation started on server")
         case "cancelSim" => context.self ! CancelSimulation
         case _ => throw new Exception()  
       }
       
       
       
     }
     
    case StartSimulation(nodeCount: Int, dataStoreCount: Int) => { 
      log.info("Received new simulation request: Node count = " + nodeCount)
      
      NodeManager.sessionCount = NodeManager.sessionCount + 1
      
      var bootstrapNode= None : Option[NodeRep]
      var nodeList = List[NodeRep]()
      implicit val timeout = Timeout(35 seconds)
      for(a <- 1 to nodeCount)
      {
        
        //val t: Iterable[ActorRef] = context.children
        val id: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
          val node: ActorRef = context.actorOf(Props(new Node(id, bootstrapNode)).withDispatcher("my-dispatcher"), name = "Node-"+ NodeManager.sessionCount + "-" + a) 
           Await.result(node.ask(InitMsg())(335 seconds), (335 seconds))
          //val future = node ? "test"
        	//val result = Await.result(future, (35 seconds)).asInstanceOf[String]
        	//log.info("returned after testing child with status = " + result)
        	
        	//bootstrapNode = Some(NodeRep(node, Await.result(node.ask(GetId())(335 seconds), (335 seconds)).asInstanceOf[String]))
          bootstrapNode = Some(NodeRep(node, id))
          bootstrapNode match {
          case None => log.info("BootstrapNode is None: Unable to send any message to it")
          case Some(bootstrapnode) => nodeList = bootstrapnode :: nodeList
          //Thread.sleep(1000)
        }
        log.info("NODE COUNT = " + a)
        //log.info("Nodelist = " + nodeList)
        	 
/*        	bootstrapNode match {
          case None => 
          case Some(g) => nodeList + g
  */  	  
      }
      var jumper = nodeList.head 
               
   //val dataStoreCount = 500
   for(a <- 1 to dataStoreCount)
   {
//      Await.result((jumper.node ? Store(DigestUtils.sha1Hex(UUID.randomUUID().toString()), "howdy")), (35 seconds))
        jumper.node ! QueryMsg(DigestUtils.sha1Hex(UUID.randomUUID().toString()), Left(Store("howdy")))
   }     
      
 //     Thread.sleep(10000)
  /*    val dataStoreCount = 100
      for(a <- 1 to dataStoreCount)
      {
        log.info("Random val = " +  (Math.random()*nodeCount).round.toInt.toString())
    */    
/*        for(node <- nodeList) {
          
          val key = DigestUtils.sha1Hex(UUID.randomUUID().toString())
          //log.info("Random val = " +  (Math.random()*nodeCount).round.toInt.toString() + " Key=" + key)
          node.node ! Store(key, "howdy")
          //Thread.sleep(1000)
  /*      node match {
          case None => log.info("BootastrapNode is None: Unable to send any message to it")
          case Some(bootstrapnode) => bootstrapnode.node ! Store(DigestUtils.sha(UUID.randomUUID().toString()).toString(), "howdy")
        }*/
        }
      Thread.sleep(10000)
 */  //   }
    }  
    case _      => log.info("received unknown message")
  }
} 