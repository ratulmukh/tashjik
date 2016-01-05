package tashjik.chord

/*
import org.scalatest._

import scala.collection.mutable.ListBuffer
import scala.concurrent.Await
import scala.concurrent.duration._
import org.apache.commons.codec.digest.DigestUtils
import javax.xml.bind.annotation.adapters.HexBinaryAdapter
import java.util.UUID

import akka.util.Timeout 
import akka.actor._
import akka.pattern.ask

class NodeTest extends FlatSpec with Matchers {

  "A Node" should "get InitMsgialized and return SuccessMsg message" in {
    
    val bootstrapNode= None : Option[NodeRep]
    implicit val timeout = Timeout(35 seconds)
    val id: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val system = ActorSystem.create("MySystem")
    val node: ActorRef = system.actorOf(Props(new Node(id, bootstrapNode, null)).withDispatcher("my-dispatcher")) 

    val future = node.ask(InitMsg())(335 seconds)
    val result = Await.result(future, (35 seconds)).asInstanceOf[SuccessMsg]
    
    system.shutdown
    result should be (SuccessMsg())
  }
  
  "Second node" should "point to first node for SuccessMsgor and predecessor and vice versa" in {
    
    val system = ActorSystem.create("MySystem")
    implicit val timeout = Timeout(35 seconds)
    
    val id1: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val node1: ActorRef = system.actorOf(Props(new Node(id1, None, null)).withDispatcher("my-dispatcher")) 

    Await.result(node1.ask(InitMsg())(335 seconds), (35 seconds)).asInstanceOf[SuccessMsg]
    
    val id2: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val node2: ActorRef = system.actorOf(Props(new Node(id2, Some(NodeRep(node1, id1)), null)).withDispatcher("my-dispatcher")) 

    Await.result(node2.ask(InitMsg())(335 seconds), (35 seconds)).asInstanceOf[SuccessMsg]
    
    val result1 = Await.result(node1.ask(GetPredecessorMsg())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    val result2 = Await.result(node1.ask(GetSuccessorMsg())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]

    val result3 = Await.result(node2.ask(GetPredecessorMsg())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    val result4 = Await.result(node2.ask(GetSuccessorMsg())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
        
    system.shutdown
    
    result1 should be (NodeRep(node2, id2))
    result2 should be (NodeRep(node2, id2))
 
    result3 should be (NodeRep(node1, id1))
    result4 should be (NodeRep(node1, id1))
    
  }
 
  "Three nodes" should "have their Successors and precessors pointing in sequence correctly when bootstraps are fed in sequence" in {
    
    val system = ActorSystem.create("MySystem")
    implicit val timeout = Timeout(35 seconds)
    
    val id1: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val node1: ActorRef = system.actorOf(Props(new Node(id1, None, null)).withDispatcher("my-dispatcher")) 

    Await.result(node1.ask(InitMsg())(335 seconds), (35 seconds)).asInstanceOf[SuccessMsg]
    
    val id2: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val node2: ActorRef = system.actorOf(Props(new Node(id2, Some(NodeRep(node1, id1)), null)).withDispatcher("my-dispatcher")) 

    Await.result(node2.ask(InitMsg())(335 seconds), (35 seconds)).asInstanceOf[SuccessMsg]
    
    val id3: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val node3: ActorRef = system.actorOf(Props(new Node(id3, Some(NodeRep(node2, id2)), null)).withDispatcher("my-dispatcher")) 

    Await.result(node3.ask(InitMsg())(335 seconds), (35 seconds)).asInstanceOf[SuccessMsg]
    
    
    
    val node1SuccessorMsg = Await.result(node1.ask(GetSuccessorMsg())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    val predecessorOf_node1SuccessorMsg = Await.result(node1SuccessorMsg.node.ask(GetPredecessorMsg())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    
    predecessorOf_node1SuccessorMsg should be (NodeRep(node1, id1))
    
    val node2InSeq = node1SuccessorMsg
    val node2InSeqSuccessMsgor = Await.result(node2InSeq.node.ask(GetSuccessorMsg())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    val predecessorOf_Node2InSeqSuccessMsgor = Await.result(node2InSeqSuccessMsgor.node.ask(GetPredecessorMsg())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    
    predecessorOf_Node2InSeqSuccessMsgor should be (node2InSeq)
    
    val node3InSeq = node2InSeqSuccessMsgor
    val node3InSeqSuccessMsgor = Await.result(node3InSeq.node.ask(GetSuccessorMsg())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    val predecessorOf_node3InSeqSuccessMsgor = Await.result(node3InSeqSuccessMsgor.node.ask(GetPredecessorMsg())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    
    predecessorOf_node3InSeqSuccessMsgor should be (node3InSeq)
    
    
        
    system.shutdown
    
       
  }
  
  "Three nodes" should "have their Successors and precessors pointing in sequence correctly when bootstraps are NOT fed in sequence" in {
    
    val system = ActorSystem.create("MySystem")
    implicit val timeout = Timeout(35 seconds)
    
    val id1: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val node1: ActorRef = system.actorOf(Props(new Node(id1, None, null)).withDispatcher("my-dispatcher")) 

    Await.result(node1.ask(InitMsg())(335 seconds), (35 seconds)).asInstanceOf[SuccessMsg]
    
    val id2: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val node2: ActorRef = system.actorOf(Props(new Node(id2, Some(NodeRep(node1, id1)), null)).withDispatcher("my-dispatcher")) 

    Await.result(node2.ask(InitMsg())(335 seconds), (35 seconds)).asInstanceOf[SuccessMsg]
    
    val id3: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val node3: ActorRef = system.actorOf(Props(new Node(id3, Some(NodeRep(node1, id1)), null)).withDispatcher("my-dispatcher")) 

    Await.result(node3.ask(InitMsg())(335 seconds), (35 seconds)).asInstanceOf[SuccessMsg]
    
    
    
    val node1SuccessorMsg = Await.result(node1.ask(GetSuccessorMsg())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    val predecessorOf_node1SuccessorMsg = Await.result(node1SuccessorMsg.node.ask(GetPredecessorMsg())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    
    predecessorOf_node1SuccessorMsg should be (NodeRep(node1, id1))
    
    val node2InSeq = node1SuccessorMsg
    val node2InSeqSuccessMsgor = Await.result(node2InSeq.node.ask(GetSuccessorMsg())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    val predecessorOf_Node2InSeqSuccessMsgor = Await.result(node2InSeqSuccessMsgor.node.ask(GetPredecessorMsg())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    
    predecessorOf_Node2InSeqSuccessMsgor should be (node2InSeq)
    
    val node3InSeq = node2InSeqSuccessMsgor
    val node3InSeqSuccessMsgor = Await.result(node3InSeq.node.ask(GetSuccessorMsg())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    val predecessorOf_node3InSeqSuccessMsgor = Await.result(node3InSeqSuccessMsgor.node.ask(GetPredecessorMsg())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    
    predecessorOf_node3InSeqSuccessMsgor should be (node3InSeq)
    
    
        
    system.shutdown
    
       
  }
  
  
  "Three nodes" should "be ordered by their ids when bootstraps are NOT fed in sequence" in {
    
    val system = ActorSystem.create("MySystem")
    implicit val timeout = Timeout(35 seconds)
    var unOrderedCount = 0
    
    
    val id1: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val node1: ActorRef = system.actorOf(Props(new Node(id1, None, null)).withDispatcher("my-dispatcher")) 

    Await.result(node1.ask(InitMsg())(335 seconds), (35 seconds)).asInstanceOf[SuccessMsg]
    
    val id2: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val node2: ActorRef = system.actorOf(Props(new Node(id2, Some(NodeRep(node1, id1)), null)).withDispatcher("my-dispatcher")) 

    Await.result(node2.ask(InitMsg())(335 seconds), (35 seconds)).asInstanceOf[SuccessMsg]
    
    val id3: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val node3: ActorRef = system.actorOf(Props(new Node(id3, Some(NodeRep(node1, id1)), null)).withDispatcher("my-dispatcher")) 

    Await.result(node3.ask(InitMsg())(335 seconds), (35 seconds)).asInstanceOf[SuccessMsg]
    
    
    
    val node1Id = Await.result(node1.ask(GetIdMsg())(335 seconds), (35 seconds)).asInstanceOf[String]
    
    val node1SuccessorMsg = Await.result(node1.ask(GetSuccessorMsg())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    val node1SuccessorMsgId = Await.result(node1SuccessorMsg.node.ask(GetIdMsg())(335 seconds), (35 seconds)).asInstanceOf[String]
    
    if(node1Id > node1SuccessorMsgId)
      unOrderedCount += 1
        
    val node2InSeq = node1SuccessorMsg
    val node2InSeqSuccessMsgor = Await.result(node2InSeq.node.ask(GetSuccessorMsg())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    val node2SuccessMsgorId = Await.result(node2InSeqSuccessMsgor.node.ask(GetIdMsg())(335 seconds), (35 seconds)).asInstanceOf[String]
    
    if(node1SuccessorMsgId > node2SuccessMsgorId)
      unOrderedCount += 1
    
    val node3InSeq = node2InSeqSuccessMsgor
    val node3InSeqSuccessMsgor = Await.result(node3InSeq.node.ask(GetSuccessorMsg())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    val node3SuccessMsgorId = Await.result(node3InSeqSuccessMsgor.node.ask(GetIdMsg())(335 seconds), (35 seconds)).asInstanceOf[String]
    
    if(node2SuccessMsgorId > node3SuccessMsgorId)
      unOrderedCount += 1
      
    unOrderedCount should be (1)
    
    system.shutdown
    
       
  }
       
  
  "500 nodes" should "be ordered by their ids when bootstraps are NOT fed in sequence" in {
    
    val system = ActorSystem.create("MySystem")
    implicit val timeout = Timeout(35 seconds)
    var unOrderedCount = 0
    val r = new scala.util.Random
    
    
    val nodeList = ListBuffer[NodeRep]()
    
    val id1: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) 
    val node1: ActorRef = system.actorOf(Props(new Node(id1, None, null)).withDispatcher("my-dispatcher")) 
    nodeList += NodeRep(node1, id1)
    Await.result(node1.ask(InitMsg())(335 seconds), (35 seconds)).asInstanceOf[SuccessMsg]
    
    for (a <- 1 until 500)
    {  
      val idx: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) 
      val nodex: ActorRef = system.actorOf(Props(new Node(idx, Some(nodeList(math.abs(r.nextInt(nodeList.length-1)))), null)).withDispatcher("my-dispatcher")) 
      nodeList += NodeRep(nodex, idx)
      Await.result(nodex.ask(InitMsg())(335 seconds), (35 seconds)).asInstanceOf[SuccessMsg]
    
    }
    
    var node = nodeList(0)
    
    for (a <- 1 until nodeList.length)
    {
    val nodeId = Await.result(node.node.ask(GetIdMsg())(335 seconds), (35 seconds)).asInstanceOf[String]
    
    val nodeSuccessMsgor = Await.result(node.node.ask(GetSuccessorMsg())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    val nodeSuccessMsgorId = Await.result(nodeSuccessMsgor.node.ask(GetIdMsg())(335 seconds), (35 seconds)).asInstanceOf[String]
    
    if(nodeId > nodeSuccessMsgorId)
      unOrderedCount += 1
      
    node = nodeSuccessMsgor
    }
      
    unOrderedCount should be (1)
    
    system.shutdown
    
       
  }
  
  
  "Data stored on a 3 node network" should "should be retrievable" in {
    
    val system = ActorSystem.create("MySystem")
    implicit val timeout = Timeout(35 seconds)
    
    val id1: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val node1: ActorRef = system.actorOf(Props(new Node(id1, None, null)).withDispatcher("my-dispatcher")) 

    Await.result(node1.ask(InitMsg())(335 seconds), (35 seconds)).asInstanceOf[SuccessMsg]
    
    val id2: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val node2: ActorRef = system.actorOf(Props(new Node(id2, Some(NodeRep(node1, id1)), null)).withDispatcher("my-dispatcher")) 

    Await.result(node2.ask(InitMsg())(335 seconds), (35 seconds)).asInstanceOf[SuccessMsg]
    
    val id3: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val node3: ActorRef = system.actorOf(Props(new Node(id3, Some(NodeRep(node2, id2)), null)).withDispatcher("my-dispatcher")) 

    Await.result(node3.ask(InitMsg())(335 seconds), (35 seconds)).asInstanceOf[SuccessMsg]
    
    
    //case QueryMsg(key: String, queryType: Either[Store, Retrieve])
    val key = DigestUtils.sha1Hex(UUID.randomUUID().toString())
    val value = "value-of-" + key
    node1 ! QueryMsg(key, Left(Store(value)))
    Thread.sleep(10000)
    node1 ! QueryMsg(key, Right(Retrieve(node3)))

    
    Thread.sleep(10000)
        
    system.shutdown
    
       
  }
  
}
*/