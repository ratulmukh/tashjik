package controllers

import play.api._
import play.api.mvc._
import models._
//import com.tindr.pusher.Pusher
import java.io.IOException
import play.api.libs.concurrent.Execution.Implicits._
import java.security.Security
import java.security.MessageDigest
import javax.crypto.Mac
import javax.crypto.spec.SecretKeySpec
import org.apache.commons.codec.binary.Base64
import org.apache.commons.codec.digest.DigestUtils
import javax.xml.bind.annotation.adapters.HexBinaryAdapter
import org.apache.commons.codec.binary.Hex
import java.nio.charset.Charset

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

