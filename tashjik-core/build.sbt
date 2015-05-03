name := "tashjik-core"

version := sys.props.getOrElse("version", default = "v0.0.1-SNAPSHOT")

scalaVersion := "2.11.6"

crossPaths := true

organization := "tashjik"

instrumentSettings

libraryDependencies ++= Seq(
  "commons-codec" % "commons-codec" % "1.8",
  "junit" % "junit" % "4.8.1" % "test",
  "com.typesafe.akka" % "akka-actor_2.11" % "2.3.3",
  "com.typesafe.akka" % "akka-contrib_2.11" % "2.3.3",
  "com.typesafe.akka" % "akka-testkit_2.11" % "2.3.3" % "test",
  "com.typesafe.play" %% "play" % "2.3.0",
  "org.scalatest" %% "scalatest" % "2.1.7" % "test",
  "org.specs2" %% "specs2" % "2.4" % "test")
 
  scalacOptions in Test ++= Seq("-Yrangepos")
 
  resolvers += "Typesafe Maven Repository" at "http://repo.typesafe.com/typesafe/maven-releases/"

  resolvers ++= Seq("snapshots", "releases").map(Resolver.sonatypeRepo)

  
