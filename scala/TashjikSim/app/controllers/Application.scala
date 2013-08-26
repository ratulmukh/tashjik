package controllers

import play.api._
import play.api.mvc._
import models.NodeManager

object Application extends Controller {
  
  def index = Action {
    
    
    Ok(views.html.index())
  }

  def startSim = Action {
    _root_.globals.nodeManager ! "test"
    Logger.info("Node Manager spun up")
    Ok("Sim still being built")
  }
}

