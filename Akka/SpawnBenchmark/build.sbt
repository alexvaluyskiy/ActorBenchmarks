lazy val root = (project in file(".")).
  settings(
    name := "spawnbenchmark",
    organization := "com.akka.net",
    scalaVersion := "2.12.1",
    version := "0.1.0",
//    sbtVersion := "0.13.9",
    libraryDependencies ++= Seq(
      "com.typesafe.akka" %% "akka-actor" % "2.4.17"
    )
  )
