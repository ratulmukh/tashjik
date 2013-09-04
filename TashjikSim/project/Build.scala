import sbt._
import Keys._
import play.Project._
import com.typesafe.sbt.SbtAtmosPlay.atmosPlaySettings

object ApplicationBuild extends Build {

  val appName         = "TashjikSim"
  val appVersion      = "1.0-SNAPSHOT"

  val appDependencies = Seq(
    // Add your project dependencies here,
    jdbc,
    anorm,
	"commons-codec" % "commons-codec" % "1.8"
	
  )


  val main = play.Project(appName, appVersion, appDependencies).settings(
    // Add your own project settings here 
	
	
	atmosPlaySettings: _*
	
  )
 }
 