name := "TashjikCore"

version := "0.0.0"

scalaVersion := "2.10.3"

artifactName := { (sv: ScalaVersion, module: ModuleID, artifact: Artifact) =>
  artifact.name + "_" + System.getProperty("version")"." + artifact.extension
}

libraryDependencies ++= Seq(
  "commons-codec" % "commons-codec" % "1.8",
  "junit" % "junit" % "4.8.1" % "test",
  "com.typesafe.akka" %% "akka-actor" % "2.3.2",
  "com.typesafe.akka" %% "akka-contrib" % "2.3.0",
  "com.typesafe.akka" %% "akka-testkit" % "2.3.0",
  "org.scalatest" % "scalatest_2.10" % "2.0" % "test")
  
