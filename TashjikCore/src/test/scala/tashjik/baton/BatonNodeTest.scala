package tashjik.baton

import org.specs2.mutable._
import org.specs2.time.NoTimeConversions
 
import akka.actor._
import akka.testkit._
import scala.concurrent.duration._
 
/* A tiny class that can be used as a Specs2 'context'. */
abstract class AkkaTestkitSpecs2Support extends TestKit(ActorSystem()) 
                                           with After 
                                           with ImplicitSender {
  // make sure we shut down the actor system after all tests have run
  def after = system.shutdown()
}


/* Both Akka and Specs2 add implicit conversions for adding time-related
   methods to Int. Mix in the Specs2 NoTimeConversions trait to avoid a clash. */
class ExampleSpec extends Specification with NoTimeConversions {
  sequential // forces all tests to be run sequentially
 
  "Baton node" should {
    "have correct state if it is 1st node in network" in new AkkaTestkitSpecs2Support {
        within(1 second) {
          
          val batonNode1 = system.actorOf(Props(new BatonNode(None, None)))
          batonNode1 ! Join()

          expectMsgPF() {
            case ParentForJoinFound(parentForJoin: ActorRef, assignedLeftChild: Boolean, parentState: BatonNodeState) => 
              parentState.level==0 && parentState.number==1 && parentState.parent==None && parentState.leftChild==None && parentState.rightChild==None && parentState.leftAdjacent==None && parentState.rightAdjacent==None && parentState.leftRoutingTable.size==0 && parentState.rightRoutingTable.size==0
            case _ => false  
          }
          expectNoMsg(500 millis)
          //expectMsgType[ParentForJoinFound](500 millis)
          //expectMsgType[String] must be equalTo "hallo"
        }
    }
    
    "join network properly when it gets a parent" in new AkkaTestkitSpecs2Support {
        within(3 second) {
          
          val parentProbe = TestProbe()
          val parentParentProbe = TestProbe()
          val parentRightChildProbe = TestProbe()
          val parentLeftAdjacentProbe = TestProbe()
          val parentRightAdjacentProbe = TestProbe()
          
          val childOneProbe = TestProbe()
          val childTwoProbe = TestProbe()
          val childThreeProbe = TestProbe()
          
          val isLeftChild = true
          val parentLevel = 3
          val parentNumber = 4
          val level = parentLevel + 1
          val number = isLeftChild match {
            case true => parentNumber*2-1
            case false => parentNumber*2
          }
          
          
          val parentLeftRoutingTable  = scala.collection.mutable.Map[Int, RoutingTableEntry]()
          val parentRightRoutingTable = scala.collection.mutable.Map[Int, RoutingTableEntry]()
          
          parentLeftRoutingTable += (parentNumber-1 -> RoutingTableEntry(null, None, Some(childOneProbe.ref), -1, -1))
          parentLeftRoutingTable += (parentNumber-2 -> RoutingTableEntry(null, Some(childTwoProbe.ref), None, -1, -1))
          
          parentRightRoutingTable += (parentNumber+1 -> RoutingTableEntry(null, None, None, -1, -1))
          parentRightRoutingTable += (parentNumber+2 -> RoutingTableEntry(null, None, None, -1, -1))
          parentRightRoutingTable += (parentNumber+4 -> RoutingTableEntry(null, Some(childThreeProbe.ref), None, -1, -1))
  
          
          
		  val batonNode1 = system.actorOf(Props(new BatonNode(Some(parentProbe.ref), None)))
          
          parentProbe.expectMsg(500 millis, Join()) 
       
          
          parentProbe.reply(ParentForJoinFound(parentProbe.ref, isLeftChild, BatonNodeState(parentLevel, parentNumber, Some(parentParentProbe.ref), None, 
		Some(parentRightChildProbe.ref), Some(parentLeftAdjacentProbe.ref), Some(parentRightAdjacentProbe.ref),
		parentLeftRoutingTable.toMap, parentRightRoutingTable.toMap)))
		
          parentRightChildProbe.expectNoMsg(500 millis)
          parentLeftAdjacentProbe.expectNoMsg(500 millis)
          parentRightAdjacentProbe.expectNoMsg(500 millis)
          
		
          
        }
    }
  }
  
  
  
  
}