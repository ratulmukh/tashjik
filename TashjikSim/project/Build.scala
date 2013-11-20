import sbt._
import Keys._
import play.Project._
//import com.typesafe.sbt.SbtAtmosPlay.atmosPlaySettings

object ApplicationBuild extends Build {

  val appName         = "TashjikSim"
  val appVersion      = "1.0-SNAPSHOT"

    
  val appDependencies = Seq(
    // Add your project dependencies here,
    jdbc,
    anorm,
	"commons-codec" % "commons-codec" % "1.8",
	"junit" % "junit" % "4.8.1" % "test",
	"com.typesafe.akka" %% "akka-actor"    % "2.2.3",
	"com.typesafe.akka" %% "akka-testkit"  % "2.2.3" % "test",
	"org.scalatest" % "scalatest_2.10" % "2.0" % "test"
	//"tindr" % "play2pusher_2.9.1" % "1.0.1" // exclude("play", "play_2.9.1")
	
  )


  //val main = play.Project(appName, appVersion, appDependencies).settings(atmosPlaySettings: _*).settings(
    val main = play.Project(appName, appVersion, appDependencies).settings(
    // Add your own project settings here 
	
	resolvers += Resolver.url("Tindr's Play module repository",
	    url("http://tindr.github.com/releases/"))
	    (Resolver.ivyStylePatterns)
	    
        
  )
 }
 