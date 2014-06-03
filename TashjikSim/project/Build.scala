import sbt._
import Keys._
//import play.Project._
import play.Play.autoImport._
import PlayKeys._
//import com.typesafe.sbt.SbtAtmosPlay.atmosPlaySettings

object ApplicationBuild extends Build {

  val appName         = "TashjikSim"
  val appVersion      = System.getProperty("version")

  val playCommonSettings = Seq(
    organization := "tashjik",
    crossPaths := false,
    version := System.getProperty("version"),
    scalaVersion := "2.11.1")
    
    
  val appDependencies = Seq(
    // Add your project dependencies here,
	"commons-codec" % "commons-codec" % "1.8",
	"junit" % "junit" % "4.8.1" % "test",
	"org.scalatest" % "scalatest_2.11" % "2.1.7" % "test"
	//"tashjik" %% "tashjikcore" % System.getProperty("version")
	//"tindr" % "play2pusher_2.9.1" % "1.0.1" // exclude("play", "play_2.9.1")
	
  )


  //val main = play.Project(appName, appVersion, appDependencies).settings(atmosPlaySettings: _*).settings(
    val main = Project(appName, file(".")).enablePlugins(play.PlayScala).settings(playCommonSettings: _*).settings(
    // Add your own project settings here 
	
        version := appVersion,
        libraryDependencies ++= appDependencies
   
   
	    
        
  )
 }
 