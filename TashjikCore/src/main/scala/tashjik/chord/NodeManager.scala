package tashjik.chord

import akka.actor.{Actor, ActorRef, Props}
import akka.event.Logging
import scala.concurrent.duration._
import akka.util.Timeout 
import akka.pattern.ask
import scala.concurrent.Await
import java.util.UUID
import org.apache.commons.codec.digest.DigestUtils
import javax.xml.bind.annotation.adapters.HexBinaryAdapter

case class StartSimulation(nodeCount: Int, dataStoreCount: Int)

object NodeManager {
  var sessionCount = 0
  
}

class NodeManager extends Actor {
  
   val log = Logging(context.system, this)
  //val myActor1: ActorRef = Akka.system().actorOf(Props[Node]);
  

 // val log = Logging(context.system, this)
  
  def receive = {
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