package tashjik.baton

//import org.specs2._
import org.specs2.mutable._
import org.specs2.time.NoTimeConversions
import org.junit.runner.RunWith
import org.specs2.runner.JUnitRunner

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
@RunWith(classOf[JUnitRunner])
class ExampleSpec extends Specification with  NoTimeConversions {
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
          val expectNothingProbe = TestProbe()
          val parentsLeftAdjacentProbe = TestProbe()
              
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
          
          parentLeftRoutingTable += (parentNumber-1 -> RoutingTableEntry(Some(expectNothingProbe.ref), None, Some(childOneProbe.ref), -1, -1))
          parentLeftRoutingTable += (parentNumber-2 -> RoutingTableEntry(Some(expectNothingProbe.ref), Some(childTwoProbe.ref), None, -1, -1))
          
          parentRightRoutingTable += (parentNumber+1 -> RoutingTableEntry(Some(expectNothingProbe.ref), None, None, -1, -1))
          parentRightRoutingTable += (parentNumber+2 -> RoutingTableEntry(Some(expectNothingProbe.ref), None, None, -1, -1))
          parentRightRoutingTable += (parentNumber+4 -> RoutingTableEntry(Some(expectNothingProbe.ref), Some(childThreeProbe.ref), None, -1, -1))
  
          val batonNode1 = system.actorOf(Props(new BatonNode(Some(parentProbe.ref), None)))
          parentProbe.expectMsg(500 millis, Join()) 
         
          parentProbe.reply(ParentForJoinFound(parentProbe.ref, isLeftChild, BatonNodeState(parentLevel, parentNumber, Some(expectNothingProbe.ref), None, 
		     Some(expectNothingProbe.ref), Some(parentsLeftAdjacentProbe.ref), Some(expectNothingProbe.ref),
		     parentLeftRoutingTable.toMap, parentRightRoutingTable.toMap)))
		
          expectNothingProbe.expectNoMsg(500 millis)
          
          batonNode1 ! GetState()

          expectMsgPF() {
            case BatonNodeState(level1: Int, number1: Int, parent: Option[ActorRef], leftChild: Option[ActorRef], 
            		rightChild: Option[ActorRef], leftAdjacent: Option[ActorRef], rightAdjacent: Option[ActorRef],
            		leftRoutingTable: Map[Int, RoutingTableEntry], rightRoutingTable: Map[Int, RoutingTableEntry]) => 
              level==level1 && number==number1 && parent.get==parentProbe.ref && leftChild==None && 
             	rightChild==None && leftAdjacent.get==parentsLeftAdjacentProbe.ref && rightAdjacent.get==parentProbe.ref //&&
              leftRoutingTable(number-1)==RoutingTableEntry(Some(childOneProbe.ref), None, None, -1, -1) &&
              leftRoutingTable(number-2)==RoutingTableEntry(Some(childTwoProbe.ref), None, None, -1, -1) &&
              rightRoutingTable(number+1)==RoutingTableEntry(None, None, None, -1, -1) &&
              rightRoutingTable(number+2)==RoutingTableEntry(None, None, None, -1, -1) &&
              rightRoutingTable(number+4)==RoutingTableEntry(Some(childThreeProbe.ref), None, None, -1, -1)
            case _ => false  
          }
          
          
		
          
        }
    }
  }
  
  
  
  
}