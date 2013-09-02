import sbt._
import Keys._
import play.Project._

object ApplicationBuild extends Build {

  val appName         = "TashjikSim"
  val appVersion      = "1.0-SNAPSHOT"

  //resolvers += "Typesafe Repo" at "http://repo.typesafe.com/typesafe/releases/"
    
  val appDependencies = Seq(
    // Add your project dependencies here,
    jdbc,
    anorm,
	"commons-codec" % "commons-codec" % "1.8"
	//"com.typesafe.akka" %% "akka-actor" % "2.2.1"
  )


  val main = play.Project(appName, appVersion, appDependencies).settings(
    // Add your own project settings here 
	scalaVersion := "2.10.2"	
  )

}
