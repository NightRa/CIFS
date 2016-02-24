import sbt.Keys._

lazy val CIFSSettings = Seq(
  name := "CIFS",
  version := "1.0",
  scalaVersion := "2.11.7",
  libraryDependencies := Seq(
    "org.scalaz" %% "scalaz-core" % "7.2.0",
    "org.scalaz" %% "scalaz-concurrent" % "7.2.0",
    "org.scodec" %% "scodec-core" % "1.8.3",
    "org.scodec" %% "scodec-scalaz" % "1.1.0",
    "org.scalatest" %% "scalatest" % "2.2.6" % "test",
    // "com.googlecode.concurrent-trees" % "concurrent-trees" % "2.5.0",
    "com.github.dokan-dev.dokan-java" % "dokan-java" % "0.1-SNAPSHOT",

    // Temporary for dokany sample
    "net.sf.trove4j" % "trove4j" % "2.0.2",
    "commons-io" % "commons-io" % "1.4",
    "org.slf4j" % "slf4j-api" % "1.6.14"
  ),
  resolvers += "Sonatype Snapshots" at "https://oss.sonatype.org/content/repositories/snapshots/"
)

lazy val concurrentTrees = project.in(file("concurrent-trees/code"))
  .settings(name := "concurrent-trees")
  .settings(scalaVersion := "2.11.7")
  .settings(libraryDependencies := Seq(
    "org.scala-lang" % "scala-library" % "2.11.7",
    "junit" % "junit" % "4.8.2" % "test"))

lazy val CIFS = project.in(file("."))
  .settings(CIFSSettings)
  .aggregate(concurrentTrees)
  .dependsOn(concurrentTrees)
