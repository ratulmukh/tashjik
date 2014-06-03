package controllers

import play.api._
import play.api.mvc._
import tashjik.chord._
import play.api.libs.concurrent.Execution.Implicits._
import akka.actor.actorRef2Scala
import play.api.libs.iteratee.Enumerator
import play.api.libs.iteratee.Iteratee
import play.api.libs.concurrent.Promise
import play.api.libs.iteratee.Concurrent
import akka.actor._
import akka.pattern.ask
import akka.util.Timeout
import scala.concurrent.duration._
import play.libs.Akka

object Application extends Controller {
  
    
  def index = Action {
    val version = Play.current.configuration.getString("tashjik.version")
    Ok(views.html.index(20, "Logs"))
     
    //Ok(views.html.main("Tashjik", version))
  }

/*  def startSim = Action { implicit request =>
    Logger.info("Received request body = " +request.body)
    
      
      
    val nodeCount = request.body.asFormUrlEncoded.get("nodeCount")(0).toInt
    val dataObjectsCount = request.body.asFormUrlEncoded.get("dataObjectsCount")(0).toInt
    //_root_.globals.nodeManager ! StartSimulation(nodeCount, dataObjectsCount)
    
    Ok(views.html.index(20, "Logs"))
  }
*/ 
  
  def dynamicCount = Action {
    Ok.chunked(Enumerator("2"))
  }

 /* 
  def websocket = WebSocket.using[String] { request => 
  
  	// Just consume and ignore the input
  	//val in = Iteratee.consume[String]()
  
  	// Send a single 'Hello!' message and close
  	//val out = Enumerator("""{"result":true,"count":1}""") // >>> Enumerator.eof
  	//			<circle cx="290" cy="290" r="225" fill="purple" />
		//	<circle cx="290" cy="290" r="221" fill="white" />
			//<!--  -->circle cx="483.12" cy="401.5" r="10" fill="red" --/>
  	
  	//val out = Enumerator("""{"svgType":"circle", "cx":290, "cy": 290, "r":225, "fill": "purple"}""")
  	
  	//out >>> Enumerator("""{"svgType":"rectangle", "cx":290, "cy": 290, "r":225, "fill": "purple"}""")
  	
  	//enume = out
    //val out = Enumerator.generateM(Promise.timeout(Some("Hello!"), 500)) // >>> Enumerator.eof
    
    //Concurrent.broadcast returns (Enumerator, Concurrent.Channel)
    val (out,channel) = Concurrent.broadcast[String]
    //log the message to stdout and send response back to client
    val in = Iteratee.foreach[String] {
      msg => println(msg)
             //the Enumerator returned by Concurrent.broadcast subscribes to the channel and will 
             //receive the pushed messages
             channel push("RESPONSE: " + msg)
    }
  	(in, out)
  }
*/
  
  
  def websocket = WebSocket.async[String] { request =>
	Logger.info("Inside of websocket controller")
    implicit val timeout = Timeout(1 second)
    
    val nodeManager = Akka.system.actorOf(Props[NodeManager]/*, name = "someActor"*/)
    
    //val future = (nodeManager ?  StartSimulation(1, 1)).mapTo
    
	(nodeManager ?  GetIterateeAndEnumerator) map {
	  case IterateeAndEnumerator(in, out) =>
		Logger.info("Got a websocket response to initialize websocket: " + in.toString + " " + out.toString)
		(in, out)
	}
  }
}

