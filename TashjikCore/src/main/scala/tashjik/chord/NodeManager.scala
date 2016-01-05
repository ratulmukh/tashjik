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
case class CancelSimulation()
case class GetIterateeAndEnumerator()
case class IterateeAndEnumerator(in: Iteratee[String,Unit], out: Enumerator[String])
case class WebsocketMsg(msg: String)
case class Circle(SVGType: String, cx: Double, cy: Double)
case class Line(SVGType: String, x1: Double, y1: Double, x2: Double, y2: Double)
case class Circles(circleList: List[Circle])
case class HopCountDyn(SVGType: String, letter: String, frequency: Int)

case class ChordMsgSent(sender: NodeRep, receiever: NodeRep)

object NodeManager {
  var sessionCount = 0
  
}


class NodeManager extends Actor {
  
  var nodeMap = Map[NodeRep, Circle]()
  var circleList = List[Circle]()
  var hopCountMap = Map[Int, Int]()
      
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
     (__ \ "SVGType").write[String] and      
     (__ \ "cx").write[Double] and
     (__ \ "cy").write[Double]
 )(unlift(Circle.unapply))
 
 implicit val lineWrites: Writes[Line] = (
     (__ \ "SVGType").write[String] and 
     (__ \ "x1").write[Double] and
     (__ \ "y1").write[Double] and
     (__ \ "x2").write[Double] and
     (__ \ "y2").write[Double]
 )(unlift(Line.unapply))
 
 implicit val hopCountDynWrites: Writes[HopCountDyn] = (
     (__ \ "SVGType").write[String] and 
     (__ \ "letter").write[String] and
     (__ \ "count").write[Int]
 )(unlift(HopCountDyn.unapply))
 
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

  	
    def hex2dec(hex: String): BigInt = {
  hex.toLowerCase().toList.map(
    "0123456789abcdef".indexOf(_)).map(
    BigInt(_)).reduceLeft( _ * 16 + _)
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
           //channel push(Json.toJson(List(Circle(483.12, 401.5), Circle(483.12, 402.5))).toString)

           
           
           context.self ! StartSimulation((msgJson \ "nodeCount").as[Int], (msgJson \ "dataObjectsCount").as[Int])
           //channel push("Simulation started on server")
         case "cancelSim" => context.self ! CancelSimulation
         case _ => throw new Exception()  
       }
       
       
       
     }
     
     case HopCount(count) => {
       log.info("Query found destination: hop count = " + count) 
       val existingCount = hopCountMap(count)
       hopCountMap.contains(count) match {
         case false => hopCountMap += (count -> 0)
         case true => hopCountMap += (count -> (hopCountMap(count)+1))
       }
       
       var hopeCountDyList = scala.collection.mutable.MutableList[HopCountDyn]()
       hopCountMap.keys.foreach{
    	   key => hopeCountDyList += HopCountDyn("HopCount", key.toString, hopCountMap(key))
       }
       log.info("hopeCountDyList = " + hopeCountDyList.toString)
       
       channel push Json.toJson(hopeCountDyList).toString
       
     }
     
    case ChordMsgSent(sender: NodeRep, receiever: NodeRep) => {
      log.info("ChordMsgSent received by NodeMgr") 
      val senderCircle = nodeMap(sender)
      log.info("senderCircle cx=" + senderCircle.cx + "cy=" + senderCircle.cy) 
      val receieverCircle = nodeMap(receiever)
      log.info("receieverCircle cx=" + receieverCircle.cx + "cy=" + receieverCircle.cy) 
      //Thread.sleep(1000)
      channel push Json.toJson(Line("line", senderCircle.cx, senderCircle.cy, receieverCircle.cx, receieverCircle.cy)).toString
      
    } 
    
    case StartSimulation(nodeCount: Int, dataStoreCount: Int) => { 
      log.info("Received new simulation request: Node count = " + nodeCount)
      
      NodeManager.sessionCount = NodeManager.sessionCount + 1
      
      var bootstrapNode = None : Option[NodeRep]
      
      
      channel push Json.toJson(Circle("circle", 0,0)).toString      
      channel push Json.toJson(Circle("circle", 0,0)).toString
      
      
      
      implicit val timeout = Timeout(35 seconds)
      for(a <- 1 to nodeCount)
      {
        
        //val t: Iterable[ActorRef] = context.children
        val id: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
        log.info("03cedf84011dd11e38ff0800200c9a66".toList.map("0123456789abcdef".indexOf(_)).map(BigInt(_)).reduceLeft( _ * 16 + _).toString)
        val currentPointInCircle = hex2dec(id)
        val fullCircle = hex2dec("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF") //FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF")
        val degrees =  ((currentPointInCircle*100/fullCircle).doubleValue)*360/100
        var circle = Circle("circle", 0, 0)
        if(degrees>=0 && degrees<90) {
        	val cy = 223 * math.cos(degrees)
        	val cx = 223 * math.sin(degrees)
        	circle = Circle("circle", cx+290, cy+290)
        	
        }
        else if(degrees>=91 && degrees<180) {
        	val cy = 223 * math.cos(180-degrees)
        	val cx = 223 * math.sin(180-degrees)
        	circle = Circle("circle", cx+290, 290-cy)
        	
        }
        else if(degrees>=180 && degrees<270) {
        	val cx = 223 * math.cos(270-degrees)
        	val cy = 223 * math.sin(270-degrees)
        	circle = Circle("circle", 290-cx, 290-cy)
        	
        }
        else {
            val cx = 223 * math.cos(360-degrees)
        	val cy = 223 * math.sin(360-degrees)
        	circle = Circle("circle", 290-cx, 290+cy)
        	
        }
        channel push Json.toJson(circle).toString
        //circleList = Circle("circle", cx+290, cy+290) :: circleList 
        //channel push Json.toJson(Circle("circle", cx+290, cy+290)).toString
        
          val node: ActorRef = context.actorOf(Props(new Node(id, bootstrapNode, context.self)).withDispatcher("my-dispatcher"), name = "Node-"+ NodeManager.sessionCount + "-" + a) 
           Await.result(node.ask(InitMsg())(335 seconds), (335 seconds))
          //val future = node ? "test"
        	//val result = Await.result(future, (35 seconds)).asInstanceOf[String]
        	//log.info("returned after testing child with status = " + result)
        	
        	//bootstrapNode = Some(NodeRep(node, Await.result(node.ask(GetId())(335 seconds), (335 seconds)).asInstanceOf[String]))
          bootstrapNode = Some(NodeRep(node, id))
          bootstrapNode match {
          case None => log.info("BootstrapNode is None: Unable to send any message to it")
          case Some(bootstrapnode) => nodeMap += (bootstrapnode -> circle)
          //Thread.sleep(1000)
        }
        log.info("NODE COUNT = " + a)
        //log.info("Nodelist = " + nodeList)
        	 
/*        	bootstrapNode match {
          case None => 
          case Some(g) => nodeList + g
  */  	  
      }
      circleList = Circle("circle", 0,0) :: circleList
      circleList = Circle("circle", 0,0) :: circleList
      //channel push(Json.toJson(circleList).toString)
      
      //var jumper = nodeMap..head 
               
   //val dataStoreCount = 500
   for(a <- 1 to dataStoreCount)
   {
        bootstrapNode match {
          case None => log.info("BootstrapNode is None: Unable to send any message to it")
          case Some(bootstrapnode) => bootstrapnode.node ! QueryMsg(DigestUtils.sha1Hex(UUID.randomUUID().toString()), Left(Store("howdy")))
//      Await.result((jumper.node ? Store(DigestUtils.sha1Hex(UUID.randomUUID().toString()), "howdy")), (35 seconds))
        }
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