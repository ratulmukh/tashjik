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
 
  "A TestKit" should {
    /* for every case where you would normally use "in", use 
       "in new AkkaTestkitSpecs2Support" to create a new 'context'. */
    "work properly with Specs2 unit tests" in new AkkaTestkitSpecs2Support {
        within(1 second) {
          val probe1 = TestProbe()
          val act = system.actorOf(Props(new Actor {
            var dest1: ActorRef = _
            
            def receive = { 
              case (d1: ActorRef) => dest1 = d1
              case x => sender ! x;  dest1 ! x 
            }
          })) 
 
          act ! ((probe1.ref))
          act ! "hallo"
          probe1.expectMsg(500 millis, "hallo")
          
          expectMsgType[String] must be equalTo "hallo"
        }
      }
  }
  
  
}