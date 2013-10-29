package controllers

import play.api._
import play.api.mvc._
import models._
//import com.tindr.pusher.Pusher
import java.io.IOException
import play.libs.WS
import play.libs.WS.WSRequestHolder
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
// good samaritan - 4301  2-8  main entrance -> go to first floor
//  bindu
  def startSim = Action { implicit request =>
    Logger.info("Received request body = " +request.body)
    
    // Default channel and events created by Pusher for a new app
		val channel = "test_channel"
		val event = "my_event"
		val message = "Hello from the Play2Pusher sample app!"

		// Send message through Pusher
		//val promise = 
/*		  Pusher().trigger(channel, event, message)
pusher.appId = 56513
pusher.key = 1ddc5a4134a76a2eaba6
pusher.secret = ff836dc41286e3e30575
*/
	try {	  
//val pusher = Pusher("56513", "1ddc5a4134a76a2eaba6", "ff836dc41286e3e30575")
/*val promise = *///pusher.trigger(channel, event, message)
	}
    catch 
    {
      case ex: IOException => {
           Logger.info("Error")  
         }
      case _ => {Logger.info("Error")}  

    }
  
    /*val appId = "3"
    val secret = "7ad3773142a6692b25b8"
    val auth_key = "278d425bdf160c739803"
    val auth_timestamp = "1353088179"
    val auth_version="1.0"
    */
   
    val appId =  "56513"
    val secret =  "ff836dc41286e3e30575"
    val auth_key =  "1ddc5a4134a76a2eaba6"
    val auth_timestamp = (System.currentTimeMillis / 1000).toString()
    val auth_version="1.0"
 
    //val post_data = """{"name":"foo","channels":["project-3"],"data":"{\"some\":\"data\"}"}""" 
    //val post_data =  """{"name":"my_event","channels":["test_channel"],"data":"{\"some\":\"data\"}"}"""
    val post_data = "{\"message\":\"hello world\"}"
    //val body_md5 = DigestUtils.md5Hex(MessageDigest.getInstance("MD5").digest(post_data.getBytes))
    val body_md5 = (new HexBinaryAdapter()).marshal(MessageDigest.getInstance("MD5").digest(post_data.getBytes())).toLowerCase()
    Logger.info("body_md5 = " + body_md5)
    
    //POST\n/apps/3/events\nauth_key=278d425bdf160c739803&auth_timestamp=1353088179&auth_version=1.0&body_md5=ec365a775a4cd0599faeb73354201b6f
    
    val toBeSigned = "POST\n/apps/" + appId + "/channels/test_channel/events\nauth_key=" + auth_key + """&auth_timestamp=""" + auth_timestamp + """&auth_version=1.0&body_md5=""" + body_md5 + """&name=my_event"""
    Logger.info("toBeSigned = " + toBeSigned)
 
 val charSet = Charset.forName("UTF8");
 val sha256_HMAC = Mac.getInstance("HmacSHA256")
 val secret_key = new SecretKeySpec(charSet.encode(secret).array(), "HmacSHA256")
 sha256_HMAC.init(secret_key);


  Logger.info("auth_signature = " +  Hex.encodeHexString(sha256_HMAC.doFinal(toBeSigned.getBytes("ISO-8859-1"))))
  
    
val auth_signature = (new HexBinaryAdapter()).marshal(sha256_HMAC.doFinal(toBeSigned.getBytes())).toLowerCase()
Logger.info("auth_signature = " + auth_signature)
    

    
    val api = "http://api.pusherapp.com/apps/" + appId + "/channels/test_channel/events?auth_key=" + auth_key + "&auth_timestamp=" + auth_timestamp + "&auth_version=1.0&body_md5=" + body_md5 +  "&auth_signature=" + auth_signature + "&name=my_event"
    //val api = "http://api.pusherapp.com/apps/56513/channels/test_channel/events?name=my_event&body_md5=2212ef73cae278cd891952624ab1bfc5&auth_version=1.0&auth_key=1ddc5a4134a76a2eaba6&auth_timestamp=1382428181&auth_signature=ae13c399407e709e5d392293f541bad81009e1342fb18be9b1b9139398a570a2&"
    Logger.info("api = " + api)
    
    //val feedUrl = """http://www.google.com"""
 //   Async {
//      val request = WS.url("""http://api.pusherapp.com/apps/""" + appId + """/events?auth_key=""" + auth_key + """&auth_timestamp=""" + auth_timestamp + """&auth_version=1.0&body_md5=""" + body_md5 +  """&auth_signature=""" + auth_signature)..setHeader("Content-Type", "application/soap+xml")
  //    request...setHeader("content-type","application/json")
      val ws = new WSRequestHolder(api) //WS.url(api)
      ws.setContentType("""application/json""")
      val p = ws.post(post_data)
      val response = p.get()
      val jsonData =  response.getBody()
      Logger.info("Response = " + jsonData)
      Ok("0")
      
      //Ok("howdy")
      /*ws.post(post_data).map { response =>
        Logger.info("!!!!howdy!!!!! code = " + response.status + "respons = " + response.body.toString())  
        Ok("howdy") //Feed title: " + (response.json \ "title").as[String])
      }*/
 //   }  
    
    
    //val nodeCount = request.body.asFormUrlEncoded.get("nodeCount")(0).toInt
    //_root_.globals.nodeManager ! StartSimulation(nodeCount)
    
    //Ok("Simulation request received for " + nodeCount + " node")
  }
}

