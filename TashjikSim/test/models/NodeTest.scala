package models


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

  "A Node" should "get initialized and return Success message" in {
    
    val bootstrapNode= None : Option[NodeRep]
    implicit val timeout = Timeout(35 seconds)
    val id: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val system = ActorSystem.create("MySystem")
    val node: ActorRef = system.actorOf(Props(new Node(id, bootstrapNode)).withDispatcher("my-dispatcher")) 

    val future = node.ask(Init())(335 seconds)
    val result = Await.result(future, (35 seconds)).asInstanceOf[Success]
    
    system.shutdown
    result should be (Success())
  }
  
  "Second node" should "point to first node for successor and predecessor and vice versa" in {
    
    val system = ActorSystem.create("MySystem")
    implicit val timeout = Timeout(35 seconds)
    
    val id1: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val node1: ActorRef = system.actorOf(Props(new Node(id1, None)).withDispatcher("my-dispatcher")) 

    Await.result(node1.ask(Init())(335 seconds), (35 seconds)).asInstanceOf[Success]
    
    val id2: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val node2: ActorRef = system.actorOf(Props(new Node(id2, Some(NodeRep(node1, id1)))).withDispatcher("my-dispatcher")) 

    Await.result(node2.ask(Init())(335 seconds), (35 seconds)).asInstanceOf[Success]
    
    val result1 = Await.result(node1.ask(GetPredecessor())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    val result2 = Await.result(node1.ask(GetSuccessor())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]

    val result3 = Await.result(node2.ask(GetPredecessor())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    val result4 = Await.result(node2.ask(GetSuccessor())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
        
    system.shutdown
    
    result1 should be (NodeRep(node2, id2))
    result2 should be (NodeRep(node2, id2))
 
    result3 should be (NodeRep(node1, id1))
    result4 should be (NodeRep(node1, id1))
    
  }
 
  "Three nodes" should "have their successors and precessors pointing in sequence correctly when bootstraps are fed in sequence" in {
    
    val system = ActorSystem.create("MySystem")
    implicit val timeout = Timeout(35 seconds)
    
    val id1: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val node1: ActorRef = system.actorOf(Props(new Node(id1, None)).withDispatcher("my-dispatcher")) 

    Await.result(node1.ask(Init())(335 seconds), (35 seconds)).asInstanceOf[Success]
    
    val id2: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val node2: ActorRef = system.actorOf(Props(new Node(id2, Some(NodeRep(node1, id1)))).withDispatcher("my-dispatcher")) 

    Await.result(node2.ask(Init())(335 seconds), (35 seconds)).asInstanceOf[Success]
    
    val id3: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val node3: ActorRef = system.actorOf(Props(new Node(id3, Some(NodeRep(node2, id2)))).withDispatcher("my-dispatcher")) 

    Await.result(node3.ask(Init())(335 seconds), (35 seconds)).asInstanceOf[Success]
    
    
    
    val node1Successor = Await.result(node1.ask(GetSuccessor())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    val predecessorOf_Node1Successor = Await.result(node1Successor.node.ask(GetPredecessor())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    
    predecessorOf_Node1Successor should be (NodeRep(node1, id1))
    
    val node2InSeq = node1Successor
    val node2InSeqSuccessor = Await.result(node2InSeq.node.ask(GetSuccessor())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    val predecessorOf_Node2InSeqSuccessor = Await.result(node2InSeqSuccessor.node.ask(GetPredecessor())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    
    predecessorOf_Node2InSeqSuccessor should be (node2InSeq)
    
    val node3InSeq = node2InSeqSuccessor
    val node3InSeqSuccessor = Await.result(node3InSeq.node.ask(GetSuccessor())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    val predecessorOf_node3InSeqSuccessor = Await.result(node3InSeqSuccessor.node.ask(GetPredecessor())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    
    predecessorOf_node3InSeqSuccessor should be (node3InSeq)
    
    
        
    system.shutdown
    
       
  }
  
  "Three nodes" should "have their successors and precessors pointing in sequence correctly when bootstraps are NOT fed in sequence" in {
    
    val system = ActorSystem.create("MySystem")
    implicit val timeout = Timeout(35 seconds)
    
    val id1: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val node1: ActorRef = system.actorOf(Props(new Node(id1, None)).withDispatcher("my-dispatcher")) 

    Await.result(node1.ask(Init())(335 seconds), (35 seconds)).asInstanceOf[Success]
    
    val id2: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val node2: ActorRef = system.actorOf(Props(new Node(id2, Some(NodeRep(node1, id1)))).withDispatcher("my-dispatcher")) 

    Await.result(node2.ask(Init())(335 seconds), (35 seconds)).asInstanceOf[Success]
    
    val id3: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val node3: ActorRef = system.actorOf(Props(new Node(id3, Some(NodeRep(node1, id1)))).withDispatcher("my-dispatcher")) 

    Await.result(node3.ask(Init())(335 seconds), (35 seconds)).asInstanceOf[Success]
    
    
    
    val node1Successor = Await.result(node1.ask(GetSuccessor())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    val predecessorOf_Node1Successor = Await.result(node1Successor.node.ask(GetPredecessor())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    
    predecessorOf_Node1Successor should be (NodeRep(node1, id1))
    
    val node2InSeq = node1Successor
    val node2InSeqSuccessor = Await.result(node2InSeq.node.ask(GetSuccessor())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    val predecessorOf_Node2InSeqSuccessor = Await.result(node2InSeqSuccessor.node.ask(GetPredecessor())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    
    predecessorOf_Node2InSeqSuccessor should be (node2InSeq)
    
    val node3InSeq = node2InSeqSuccessor
    val node3InSeqSuccessor = Await.result(node3InSeq.node.ask(GetSuccessor())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    val predecessorOf_node3InSeqSuccessor = Await.result(node3InSeqSuccessor.node.ask(GetPredecessor())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    
    predecessorOf_node3InSeqSuccessor should be (node3InSeq)
    
    
        
    system.shutdown
    
       
  }
  
  
  "Three nodes" should "be ordered by their ids when bootstraps are NOT fed in sequence" in {
    
    val system = ActorSystem.create("MySystem")
    implicit val timeout = Timeout(35 seconds)
    var unOrderedCount = 0
    
    
    val id1: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val node1: ActorRef = system.actorOf(Props(new Node(id1, None)).withDispatcher("my-dispatcher")) 

    Await.result(node1.ask(Init())(335 seconds), (35 seconds)).asInstanceOf[Success]
    
    val id2: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val node2: ActorRef = system.actorOf(Props(new Node(id2, Some(NodeRep(node1, id1)))).withDispatcher("my-dispatcher")) 

    Await.result(node2.ask(Init())(335 seconds), (35 seconds)).asInstanceOf[Success]
    
    val id3: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) //.toString()
    val node3: ActorRef = system.actorOf(Props(new Node(id3, Some(NodeRep(node1, id1)))).withDispatcher("my-dispatcher")) 

    Await.result(node3.ask(Init())(335 seconds), (35 seconds)).asInstanceOf[Success]
    
    
    
    val node1Id = Await.result(node1.ask(GetId())(335 seconds), (35 seconds)).asInstanceOf[String]
    
    val node1Successor = Await.result(node1.ask(GetSuccessor())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    val node1SuccessorId = Await.result(node1Successor.node.ask(GetId())(335 seconds), (35 seconds)).asInstanceOf[String]
    
    if(node1Id > node1SuccessorId)
      unOrderedCount += 1
        
    val node2InSeq = node1Successor
    val node2InSeqSuccessor = Await.result(node2InSeq.node.ask(GetSuccessor())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    val node2SuccessorId = Await.result(node2InSeqSuccessor.node.ask(GetId())(335 seconds), (35 seconds)).asInstanceOf[String]
    
    if(node1SuccessorId > node2SuccessorId)
      unOrderedCount += 1
    
    val node3InSeq = node2InSeq
    val node3InSeqSuccessor = Await.result(node3InSeq.node.ask(GetSuccessor())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    val node3SuccessorId = Await.result(node3InSeqSuccessor.node.ask(GetId())(335 seconds), (35 seconds)).asInstanceOf[String]
    
    if(node2SuccessorId > node3SuccessorId)
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
    val node1: ActorRef = system.actorOf(Props(new Node(id1, None)).withDispatcher("my-dispatcher")) 
    nodeList += NodeRep(node1, id1)
    Await.result(node1.ask(Init())(335 seconds), (35 seconds)).asInstanceOf[Success]
    
    for (a <- 1 until 500)
    {  
      val idx: String = DigestUtils.sha1Hex(UUID.randomUUID().toString()) 
      val nodex: ActorRef = system.actorOf(Props(new Node(idx, Some(nodeList(r.nextInt(nodeList.length-1))))).withDispatcher("my-dispatcher")) 
      nodeList += NodeRep(nodex, idx)
      Await.result(nodex.ask(Init())(335 seconds), (35 seconds)).asInstanceOf[Success]
    
    }
    
    var node = nodeList(0)
    
    for (a <- 1 until nodeList.length)
    {
    val nodeId = Await.result(node.node.ask(GetId())(335 seconds), (35 seconds)).asInstanceOf[String]
    
    val nodeSuccessor = Await.result(node.node.ask(GetSuccessor())(335 seconds), (35 seconds)).asInstanceOf[NodeRep]
    val nodeSuccessorId = Await.result(nodeSuccessor.node.ask(GetId())(335 seconds), (35 seconds)).asInstanceOf[String]
    
    if(nodeId > nodeSuccessorId)
      unOrderedCount += 1
      
    node = nodeSuccessor
    }
      
    unOrderedCount should be (1)
    
    system.shutdown
    
       
  }
}