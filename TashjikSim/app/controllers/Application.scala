package controllers

import play.api._
import play.api.mvc._
import tashjik.chord._
import play.api.libs.concurrent.Execution.Implicits._
import akka.actor.actorRef2Scala
import play.api.libs.iteratee.Enumerator
import play.api.libs.iteratee.Iteratee
import play.api.libs.concurrent.Promise

object Application extends Controller {
  
  def index = Action {
    val version = Play.current.configuration.getString("tashjik.version")
    Ok(views.html.index(20, "Logs"))
    //Ok(views.html.main("Tashjik", version))
  }

  def startSim = Action { implicit request =>
    Logger.info("Received request body = " +request.body)
    
      
    
    val nodeCount = request.body.asFormUrlEncoded.get("nodeCount")(0).toInt
    val dataObjectsCount = request.body.asFormUrlEncoded.get("dataObjectsCount")(0).toInt
    _root_.globals.nodeManager ! StartSimulation(nodeCount, dataObjectsCount)
    
    Ok("Simulation request received for " + nodeCount + " node")
  }
  
  
  def dynamicCount = Action {
    Ok.chunked(Enumerator("2"))
  }
  
  def websocket = WebSocket.using[String] { request => 
  
  	// Just consume and ignore the input
  	val in = Iteratee.consume[String]()
  
  	// Send a single 'Hello!' message and close
  	//val out = Enumerator("Hello!") >>> Enumerator.eof
    val out = Enumerator.generateM(Promise.timeout(Some("Hello!"), 500)) // >>> Enumerator.eof
  	(in, out)
  }
}

