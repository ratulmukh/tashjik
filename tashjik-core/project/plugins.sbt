// Comment to get more information during initialization
logLevel := Level.Warn

// The Typesafe repository 
resolvers += "Typesafe repository" at "http://repo.typesafe.com/typesafe/releases/"

//resolvers += Classpaths.typesafeResolver

resolvers += Classpaths.sbtPluginReleases

addSbtPlugin("com.typesafe.play" % "sbt-plugin" % "2.2.3")

addSbtPlugin("org.scoverage" %% "sbt-scoverage" % "0.99.7.1")

