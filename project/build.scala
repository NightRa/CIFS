import sbt.Keys._
import sbt._

object build extends Build {
  val monocleVersion = "1.2.0" // or "1.3.0-SNAPSHOT"

  lazy val CIFSSettings = Seq(
    name := "CIFS",
    version := "1.0",
    scalaVersion := "2.11.8",
    resolvers ++= Seq(
      "Sonatype Snapshots" at "https://oss.sonatype.org/content/repositories/snapshots/"
      // "scala-ipfs-api" at "https://ipfs.io/ipfs/QmbWUQEuTtFwNNg94nbpVY25b5PAyPQTd7HhkDsGhRG8Ur/"
    ),
    libraryDependencies := Seq(
      "org.scalaz" %% "scalaz-core" % "7.2.4",
      "org.scalaz" %% "scalaz-concurrent" % "7.2.4",
      "com.github.julien-truffaut" %% "monocle-core" % monocleVersion,
      "com.github.julien-truffaut" %% "monocle-macro" % monocleVersion,
      "co.fs2" %% "fs2-core" % "0.9.0-M6",
      "co.fs2" %% "fs2-io" % "0.9.0-M6",
      "org.scodec" %% "scodec-core" % "1.10.2",
      "org.scodec" %% "scodec-scalaz" % "1.3.0a",
      "org.scodec" %% "scodec-stream" % "1.0.0-M6",
      "org.scalatest" %% "scalatest" % "2.2.6" % "test",
      "org.apache.httpcomponents.client5" % "httpclient5-fluent" % "5.0-alpha1"
      // "io.ipfs" % "scala-ipfs-api_2.10" % "1.0.0-SNAPSHOT"
      // "com.googlecode.concurrent-trees" % "concurrent-trees" % "2.5.0",
      // "com.github.dokan-dev.dokan-java" % "dokan-java" % "0.1-SNAPSHOT",

      // Temporary for dokany sample
      // "net.sf.trove4j" % "trove4j" % "2.0.2",
      // "commons-io" % "commons-io" % "1.4",
      // "org.slf4j" % "slf4j-api" % "1.6.14"
    ).map(_.exclude("org.scala-lang.modules","scala-xml_2.11")),

    // for @Lenses macro support
    addCompilerPlugin("org.scalamacros" %% "paradise" % "2.1.0" cross CrossVersion.full)
  )

  lazy val junit = Seq("junit" % "junit" % "4.8.2" % "test")

  lazy val concurrentTrees = project.in(file("concurrent-trees/code"))
    .settings(name := "concurrent-trees")
    .settings(scalaVersion := "2.11.8")
    .settings(libraryDependencies := junit)

  /*lazy val IPFS = project.in(file("java-ipfs-api"))
    .settings(name := "java-ipfs-api")
    .settings(scalaVersion := "2.11.8")
    .settings(libraryDependencies := junit)*/

  /*lazy val Scala_IPFS = project.in(file("scala-ipfs-api"))

  lazy val CIFS = project.in(file("."))
    .settings(CIFSSettings)
    .aggregate(concurrentTrees, Scala_IPFS/*, IPFS*/)
    .dependsOn(concurrentTrees, Scala_IPFS/*, IPFS*/)*/


  lazy val root =
    Project(
      id = "CIFS",
      base = file("."),
      settings = CIFSSettings,
      aggregate = Seq(concurrentTrees, scala_ipfs_api)
    ).dependsOn(concurrentTrees, scala_ipfs_api)

  lazy val scala_ipfs_api =
    ProjectRef(file("scala-ipfs-api"), "scala-ipfs-api")
}
