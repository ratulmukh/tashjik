package controllers

import play.api._
import play.api.mvc._
import tashjik.chord._
import play.api.libs.concurrent.Execution.Implicits._
import akka.actor.actorRef2Scala

object Application extends Controller {
  
  def index = Action {
    val version = Play.current.configuration.getString("tashjik.version")
    Ok(views.html.index())
   // Ok(views.html.main("Tashjik", version))
  }

  def startSim = Action { implicit request =>
    Logger.info("Received request body = " +request.body)
    
      
    
    val nodeCount = request.body.asFormUrlEncoded.get("nodeCount")(0).toInt
    _root_.globals.nodeManager ! StartSimulation(nodeCount)
    
    Ok("Simulation request received for " + nodeCount + " node")
  }
}

