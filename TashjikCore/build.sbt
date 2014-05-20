name := "TashjikCore"

version := System.getProperty("version")

scalaVersion := "2.10.3"

crossPaths := false

organization := "tashjik"

libraryDependencies ++= Seq(
  "commons-codec" % "commons-codec" % "1.8",
  "junit" % "junit" % "4.8.1" % "test",
  "com.typesafe.akka" %% "akka-actor" % "2.3.2",
  "com.typesafe.akka" %% "akka-contrib" % "2.3.2",
  "com.typesafe.akka" %% "akka-testkit" % "2.3.2" % "test",
  "org.scalatest" % "scalatest_2.10" % "2.0" % "test")
  
