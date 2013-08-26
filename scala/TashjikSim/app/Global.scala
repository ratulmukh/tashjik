import play.api._
import play.libs.Akka
import akka.actor.Props
import models.NodeManager

object Global extends GlobalSettings {

  
  
  override def onStart(app: Application) {
     
    Logger.info("Application has started")
  }  
  
  override def onStop(app: Application) {
    Logger.info("Application shutdown...")
  }  
    
}

package object globals {
  lazy val nodeManager = Akka.system.actorOf(Props[NodeManager], name = "someActor")
}