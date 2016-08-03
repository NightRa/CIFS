import sbt.Keys._

lazy val publishDir = settingKey[String]("Publishing directory")
lazy val publishIpfs = taskKey[Unit]("Publish to IPFS")

organization := "io.ipfs"
version := "1.0.0-SNAPSHOT"
scalaVersion := "2.11.8"

name := "scala-ipfs-api"
libraryDependencies ++=
  Seq(
    "com.fasterxml.jackson.core" % "jackson-databind" % "2.7.4",
    "com.fasterxml.jackson.module" %% "jackson-module-scala" % "2.7.4",
    "org.specs2" %% "specs2-core" % "3.6.4" % "test"
  )

publishDir := {
  import java.nio.file.Files
  Files.createTempDirectory("sbt-ipfs-").toString
}

publishTo := Some(Resolver.file("ipfs", file(publishDir.value)))

publishIpfs := {
  publish.value
  ("ipfs add -r " + publishDir.value) !
}
