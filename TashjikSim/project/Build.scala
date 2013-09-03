import sbt._
import Keys._
import play.Project._
import com.typesafe.sbt.SbtAtmos.{ Atmos, atmosSett	ings }
import com.typesafe.sbt.SbtAtmos.AtmosKeys.{ traceable, sampling }
import com.typesafe.sbt.SbtAtmos.traceAkka

object ApplicationBuild extends Build {

  val appName         = "TashjikSim"
  val appVersion      = "1.0-SNAPSHOT"

  //resolvers += "Typesafe Repo" at "http://repo.typesafe.com/typesafe/releases/"
    
  val appDependencies = Seq(
    // Add your project dependencies here,
    jdbc,
    anorm,
	"commons-codec" % "commons-codec" % "1.8",
	"com.typesafe.atmos" % ("trace-akka-" + 2.2.1) % 2.1.4,
	"ch.qos.logback" % "logback-classic" % 1.0.13
	//"com.typesafe.akka" %% "akka-actor" % "2.2.1"
  )


  val main = play.Project(appName, appVersion, appDependencies).settings(
    // Add your own project settings here 
	scalaVersion := "2.10.2"	
	
  )
  
  object Dependencies {
 
  object V {
    val Akka    = "2.2.1"
    val Atmos   = "2.1.1"
    val Logback = "1.0.13"
  }
 
  val atmosTrace = "com.typesafe.atmos" % ("trace-akka-" + V.Akka) % V.Atmos
  val logback    = "ch.qos.logback"     % "logback-classic"        % V.Logback
 
  val tracedAkka = Seq(atmosTrace, logback)
}
  
  .configs(Atmos)
  .settings(atmosSettings: _*)
  
  traceAkka("2.2.0")
  
traceable in Atmos := Seq(
  "/user/someActor" -> true,  // trace this actor
  "/user/someActor/*"  -> true,  // trace all actors in this subtree
  "*"               -> false  // other actors are not traced
)

sampling in Atmos := Seq(
  "/user/someActor" -> 1,     // sample every trace for this actor
  "/user/someActor/*"  -> 1    // sample every 100th trace in this subtree
)

}

