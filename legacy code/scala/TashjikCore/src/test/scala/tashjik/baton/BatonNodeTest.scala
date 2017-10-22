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
    
    "join as leftChild of mid tree parent" in new AkkaTestkitSpecs2Support {
        within(9 second) {
          
          val parentProbe = TestProbe()
          val expectNothingProbe = TestProbe()
          val parentsLeftAdjacentProbe = TestProbe()
              
          val childOneProbe = TestProbe()
          val childTwoProbe = TestProbe()
          val childThreeProbe = TestProbe()
          val childFourProbe = TestProbe()
          val childFiveProbe = TestProbe()
          val childSixProbe = TestProbe()
          val childSevenProbe = TestProbe()
          
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
          
          parentLeftRoutingTable += (parentNumber-1 -> RoutingTableEntry(Some(expectNothingProbe.ref), Some(childOneProbe.ref), Some(childTwoProbe.ref), -1, -1))
          parentLeftRoutingTable += (parentNumber-2 -> RoutingTableEntry(Some(expectNothingProbe.ref), Some(childThreeProbe.ref), None, -1, -1))
          
          parentRightRoutingTable += (parentNumber+1 -> RoutingTableEntry(Some(expectNothingProbe.ref), Some(childFiveProbe.ref), None, -1, -1))
          parentRightRoutingTable += (parentNumber+2 -> RoutingTableEntry(Some(expectNothingProbe.ref), Some(childSixProbe.ref), None, -1, -1))
          parentRightRoutingTable += (parentNumber+4 -> RoutingTableEntry(Some(expectNothingProbe.ref), Some(childSevenProbe.ref), None, -1, -1))
  
          val batonNode1 = system.actorOf(Props(new BatonNode(Some(parentProbe.ref), None)))
          parentProbe.expectMsg(500 millis, Join()) 
         
          parentProbe.reply(ParentForJoinFound(parentProbe.ref, isLeftChild, BatonNodeState(parentLevel, parentNumber, Some(expectNothingProbe.ref), None, 
		     Some(childFourProbe.ref), Some(parentsLeftAdjacentProbe.ref), Some(expectNothingProbe.ref),
		     parentLeftRoutingTable.toMap, parentRightRoutingTable.toMap)))
		
          expectNothingProbe.expectNoMsg(500 millis)
          
          batonNode1 ! GetState()

          expectMsgPF() {
            case BatonNodeState(level1: Int, number1: Int, parent1: Option[ActorRef], leftChild1: Option[ActorRef], 
            		rightChild1: Option[ActorRef], leftAdjacent1: Option[ActorRef], rightAdjacent1: Option[ActorRef],
            		leftRoutingTable1: Map[Int, RoutingTableEntry], rightRoutingTable1: Map[Int, RoutingTableEntry]) if 
              level==level1 && number==number1 && parent1.get==parentProbe.ref && leftChild1==None && 
             	rightChild1==None && leftAdjacent1.get==parentsLeftAdjacentProbe.ref && rightAdjacent1.get==parentProbe.ref &&
              leftRoutingTable1(number-1)==RoutingTableEntry(Some(childTwoProbe.ref), None, None, -1, -1) &&
              leftRoutingTable1(number-2)==RoutingTableEntry(Some(childOneProbe.ref), None, None, -1, -1) &&
              leftRoutingTable1(number-4)==RoutingTableEntry(Some(childThreeProbe.ref), None, None, -1, -1) &&
              rightRoutingTable1(number+1)==RoutingTableEntry(Some(childFourProbe.ref), None, None, -1, -1) &&
              rightRoutingTable1(number+2)==RoutingTableEntry(Some(childFiveProbe.ref), None, None, -1, -1) &&
              rightRoutingTable1(number+4)==RoutingTableEntry(Some(childSixProbe.ref), None, None, -1, -1) &&
              rightRoutingTable1(number+8)==RoutingTableEntry(Some(childSevenProbe.ref), None, None, -1, -1) =>
            
          }
          
          
		
          
        }
    }
    
    "join as rightChild of mid tree parent" in new AkkaTestkitSpecs2Support {
        within(3 second) {
          
          val parentProbe = TestProbe()
          val expectNothingProbe = TestProbe()
          val parentsRightAdjacentProbe = TestProbe()
              
          val childOneProbe = TestProbe()
          val childTwoProbe = TestProbe()
          val childThreeProbe = TestProbe()
          val childFourProbe = TestProbe()
          val childFiveProbe = TestProbe()
          val childSixProbe = TestProbe()
          val childSevenProbe = TestProbe()
          
          val isLeftChild = false
          val parentLevel = 3
          val parentNumber = 4
          val level = parentLevel + 1
          val number = isLeftChild match {
            case true => parentNumber*2-1
            case false => parentNumber*2
          }
          
          val parentLeftRoutingTable  = scala.collection.mutable.Map[Int, RoutingTableEntry]()
          val parentRightRoutingTable = scala.collection.mutable.Map[Int, RoutingTableEntry]()
          
          parentLeftRoutingTable += (parentNumber-1 -> RoutingTableEntry(Some(expectNothingProbe.ref), None, Some(childTwoProbe.ref), -1, -1))
          parentLeftRoutingTable += (parentNumber-2 -> RoutingTableEntry(Some(expectNothingProbe.ref), None, Some(childThreeProbe.ref), -1, -1))
          
          parentRightRoutingTable += (parentNumber+1 -> RoutingTableEntry(Some(expectNothingProbe.ref), Some(childFourProbe.ref), Some(childFiveProbe.ref), -1, -1))
          parentRightRoutingTable += (parentNumber+2 -> RoutingTableEntry(Some(expectNothingProbe.ref), None, Some(childSixProbe.ref), -1, -1))
          parentRightRoutingTable += (parentNumber+4 -> RoutingTableEntry(Some(expectNothingProbe.ref), None, Some(childSevenProbe.ref), -1, -1))
  
          val batonNode1 = system.actorOf(Props(new BatonNode(Some(parentProbe.ref), None)))
          parentProbe.expectMsg(500 millis, Join()) 
         
          parentProbe.reply(ParentForJoinFound(parentProbe.ref, isLeftChild, BatonNodeState(parentLevel, parentNumber, Some(expectNothingProbe.ref), Some(childOneProbe.ref), 
		     Some(expectNothingProbe.ref),  Some(expectNothingProbe.ref), Some(parentsRightAdjacentProbe.ref),
		     parentLeftRoutingTable.toMap, parentRightRoutingTable.toMap)))
		
          expectNothingProbe.expectNoMsg(500 millis)
          
          batonNode1 ! GetState()

          expectMsgPF() {
            case BatonNodeState(level1: Int, number1: Int, parent: Option[ActorRef], leftChild1: Option[ActorRef], 
            		rightChild1: Option[ActorRef], leftAdjacent1: Option[ActorRef], rightAdjacent1: Option[ActorRef],
            		leftRoutingTable1: Map[Int, RoutingTableEntry], rightRoutingTable1: Map[Int, RoutingTableEntry]) if
              level==level1 && number==number1 && parent.get==parentProbe.ref && leftChild1==None && 
             	rightChild1==None && leftAdjacent1.get==parentProbe.ref && rightAdjacent1.get==parentsRightAdjacentProbe.ref &&
              leftRoutingTable1(number-1)==RoutingTableEntry(Some(childOneProbe.ref), None, None, -1, -1) &&
              leftRoutingTable1(number-2)==RoutingTableEntry(Some(childTwoProbe.ref), None, None, -1, -1) &&
              leftRoutingTable1(number-4)==RoutingTableEntry(Some(childThreeProbe.ref), None, None, -1, -1) &&
              rightRoutingTable1(number+1)==RoutingTableEntry(Some(childFourProbe.ref), None, None, -1, -1) &&
              rightRoutingTable1(number+2)==RoutingTableEntry(Some(childFiveProbe.ref), None, None, -1, -1) &&
              rightRoutingTable1(number+4)==RoutingTableEntry(Some(childSixProbe.ref), None, None, -1, -1) &&
              rightRoutingTable1(number+8)==RoutingTableEntry(Some(childSevenProbe.ref), None, None, -1, -1) =>  
            
          }
          
          
        }
          
    }
 
    "join as leftChild of left corner parent" in new AkkaTestkitSpecs2Support {
        within(3 second) {
          
          val parentProbe = TestProbe()
          val expectNothingProbe = TestProbe()
          val parentsLeftAdjacentProbe = TestProbe()
              
          val childOneProbe = TestProbe()
          val childTwoProbe = TestProbe()
          val childThreeProbe = TestProbe()
          val childFourProbe = TestProbe()
          
          val isLeftChild = true
          val parentLevel = 3
          val parentNumber = 1
          val level = parentLevel + 1
          val number = isLeftChild match {
            case true => parentNumber*2-1
            case false => parentNumber*2
          }
          
          val parentLeftRoutingTable  = scala.collection.mutable.Map[Int, RoutingTableEntry]()
          val parentRightRoutingTable = scala.collection.mutable.Map[Int, RoutingTableEntry]()
          
          parentRightRoutingTable += (parentNumber+1 -> RoutingTableEntry(Some(expectNothingProbe.ref), Some(childTwoProbe.ref), None, -1, -1))
          parentRightRoutingTable += (parentNumber+2 -> RoutingTableEntry(Some(expectNothingProbe.ref), Some(childThreeProbe.ref), None, -1, -1))
          parentRightRoutingTable += (parentNumber+4 -> RoutingTableEntry(Some(expectNothingProbe.ref), Some(childFourProbe.ref), None, -1, -1))
  
          val batonNode1 = system.actorOf(Props(new BatonNode(Some(parentProbe.ref), None)))
          parentProbe.expectMsg(500 millis, Join()) 
         
          parentProbe.reply(ParentForJoinFound(parentProbe.ref, isLeftChild, BatonNodeState(parentLevel, parentNumber, Some(expectNothingProbe.ref), None, 
		     Some(childOneProbe.ref), Some(parentsLeftAdjacentProbe.ref), Some(expectNothingProbe.ref),
		     parentLeftRoutingTable.toMap, parentRightRoutingTable.toMap)))
		
          expectNothingProbe.expectNoMsg(500 millis)
          
          batonNode1 ! GetState()

          expectMsgPF() {
            case BatonNodeState(level1: Int, number1: Int, parent1: Option[ActorRef], parentsLeftChild1: Option[ActorRef], 
            		parentsRightChild1: Option[ActorRef], parentsLeftAdjacent1: Option[ActorRef], parentsRightAdjacent1: Option[ActorRef],
            		parentsLeftRoutingTable1: Map[Int, RoutingTableEntry], parentsRightRoutingTable1: Map[Int, RoutingTableEntry]) if 
            		     level==level1 && number==number1 && parent1.get==parentProbe.ref && parentsLeftChild1==None && 
             	parentsRightChild1==None && parentsLeftAdjacent1.get==parentsLeftAdjacentProbe.ref && parentsRightAdjacent1.get==parentProbe.ref &&
              parentsRightRoutingTable1(number+1)==RoutingTableEntry(Some(childOneProbe.ref), None, None, -1, -1) &&
              parentsRightRoutingTable1(number+2)==RoutingTableEntry(Some(childTwoProbe.ref), None, None, -1, -1) &&
              parentsRightRoutingTable1(number+4)==RoutingTableEntry(Some(childThreeProbe.ref), None, None, -1, -1) &&
              parentsRightRoutingTable1(number+8)==RoutingTableEntry(Some(childFourProbe.ref), None, None, -1, -1) =>
            
          }
        
        }
    }
     
    "join as rightChild of left corner parent" in new AkkaTestkitSpecs2Support {
        within(3 second) {
          
          val parentProbe = TestProbe()
          val expectNothingProbe = TestProbe()
          val parentsRightAdjacentProbe = TestProbe()
              
          val childOneProbe = TestProbe()
          val childTwoProbe = TestProbe()
          val childThreeProbe = TestProbe()
          val childFourProbe = TestProbe()
          
          val isLeftChild = false
          val parentLevel = 3
          val parentNumber = 1
          val level = parentLevel + 1
          val number = isLeftChild match {
            case true => parentNumber*2-1
            case false => parentNumber*2
          }
          
          val parentLeftRoutingTable  = scala.collection.mutable.Map[Int, RoutingTableEntry]()
          val parentRightRoutingTable = scala.collection.mutable.Map[Int, RoutingTableEntry]()
          
          parentRightRoutingTable += (parentNumber+1 -> RoutingTableEntry(Some(expectNothingProbe.ref), Some(childOneProbe.ref), Some(childTwoProbe.ref), -1, -1))
          parentRightRoutingTable += (parentNumber+2 -> RoutingTableEntry(Some(expectNothingProbe.ref), None, Some(childThreeProbe.ref), -1, -1))
          parentRightRoutingTable += (parentNumber+4 -> RoutingTableEntry(Some(expectNothingProbe.ref), None, Some(childFourProbe.ref), -1, -1))
  
          val batonNode1 = system.actorOf(Props(new BatonNode(Some(parentProbe.ref), None)))
          parentProbe.expectMsg(500 millis, Join()) 
         
          parentProbe.reply(ParentForJoinFound(parentProbe.ref, isLeftChild, BatonNodeState(parentLevel, parentNumber, Some(expectNothingProbe.ref), None, 
		     Some(expectNothingProbe.ref), Some(expectNothingProbe.ref), Some(parentsRightAdjacentProbe.ref),
		     parentLeftRoutingTable.toMap, parentRightRoutingTable.toMap)))
		
          expectNothingProbe.expectNoMsg(500 millis)
          
          batonNode1 ! GetState()

          expectMsgPF() {
            case BatonNodeState(level1: Int, number1: Int, parent1: Option[ActorRef], parentsLeftChild1: Option[ActorRef], 
            		parentsRightChild1: Option[ActorRef], parentsLeftAdjacent1: Option[ActorRef], parentsRightAdjacent1: Option[ActorRef],
            		parentsLeftRoutingTable1: Map[Int, RoutingTableEntry], parentsRightRoutingTable1: Map[Int, RoutingTableEntry]) if 
            		     level==level1 && number==number1 && parent1.get==parentProbe.ref && parentsLeftChild1==None && 
             	parentsRightChild1==None && parentsLeftAdjacent1.get==parentProbe.ref && parentsRightAdjacent1.get==parentsRightAdjacentProbe.ref &&
              parentsRightRoutingTable1(number+1)==RoutingTableEntry(Some(childOneProbe.ref), None, None, -1, -1) &&
              parentsRightRoutingTable1(number+2)==RoutingTableEntry(Some(childTwoProbe.ref), None, None, -1, -1) &&
              parentsRightRoutingTable1(number+4)==RoutingTableEntry(Some(childThreeProbe.ref), None, None, -1, -1) &&
              parentsRightRoutingTable1(number+8)==RoutingTableEntry(Some(childFourProbe.ref), None, None, -1, -1) =>
            
          }
          
          
        }
    }
 
    
  }
  
  
  
  
}