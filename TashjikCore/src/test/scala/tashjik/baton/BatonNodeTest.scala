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
 
  "Baton network" should {
    "have correct state for 1st node" in new AkkaTestkitSpecs2Support {
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
  }
  
  
}