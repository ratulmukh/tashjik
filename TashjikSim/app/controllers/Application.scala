package controllers

import play.api._
import play.api.mvc._
import models._

object Application extends Controller {
  
  def index = Action {
    
    
    Ok(views.html.index())
  }

  def startSim = Action { implicit request =>
    Logger.info("Received request body = " +request.body)
    val nodeCount = request.body.asFormUrlEncoded.get("nodeCount")(0).toInt
    _root_.globals.nodeManager ! StartSimulation(nodeCount)
    
    Ok("Simulation request received for " + nodeCount + " node")
  }
}

