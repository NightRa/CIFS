package follows

import java.util.Scanner

import ipfs.{Files, Queries}
import FollowSerialization._
import scodec.bits.ByteVector
import serialization.Response

import scala.annotation.tailrec
import ResolvePath._
import ParsePath._

class FollowManager(var follows: Follows) {
  // Returns if could successfully add the follow.
  def addFollow(follow: Follow): Response.CloneFollow = {
    val path = follow.localPath
    parsePath(path) match {
      case None => Response.CloneFollowMalformedPath
      case Some(parsedPath) =>
        if(isFollow(path, follows)) Response.CloneFollowNameCollision
        else if (isUnderFollow(path, follows)) Response.CloneFollowParentDoesntExist
        else if (Files.stat(path).isDefined) Response.CloneFollowNameCollision
        else if (Files.stat(parsedPath.init.mkString("/", "/", "")).isEmpty) Response.CloneFollowParentDoesntExist
        else {
          follows = Follows(follow :: follows.follows)
          FollowManager.saveFollows(follows)
          Response.CloneFollowOK
        }
    }

  }

  // Have I succeeded deleting the follow
  def rmFollow(path: String): Boolean = {
    if(isFollow(path, follows)) {
      follows = Follows(follows.follows.filter(_.localPath != path))
      FollowManager.saveFollows(follows)
      true
    } else
      false
  }
}

object FollowManager {
  def fetchFollows(ipfsHash: String): Follows = {
    val pathOfFollows = ipfsHash + "/.follows"
    val serializedFollows = Queries.catStr(pathOfFollows)
    serializedFollows.flatMap(deserializeFollows).getOrElse(Follows(Nil))
  }

  def fetchFollowsIPNS(ipnsHash: String): Follows = {
    fetchFollows("/ipns/" + ipnsHash)
  }

  def fetchLocalFollows(): Follows = {
    println("Fetching .follows")
    Files.read("/", 0, None) match {
      case None =>
        println("No .follows file! Creating a new follows list.")
        createNewFollowsFile()
      case Some(byteVector) =>

        deserializeFollows(new String(byteVector.toArray)) match {

          case None =>
            println("Problem deserializing the .follows file!")
            val createNew = askDelete()
            if(createNew) createNewFollowsFile()
            else sys.error("Exiting, figure out how to fix the .follows file.")

          case Some(follows) =>
            follows
        }
    }
  }

  def createNewFollowsFile(): Follows = {
    val follows = Follows(Nil)
    saveFollows(follows)
    follows
  }

  @tailrec
  def askDelete(): Boolean = {
    print("Create a new follows file? (y/n)")
    val in = new Scanner(System.in)
    val ans = in.next()
    if(ans == "n") false
    else if (ans == "y") true
    else askDelete()
  }

  def saveFollows(follows: Follows): Unit = {
    val data = ByteVector(serializeFollows(follows).getBytes)
    Files.write("/.follows", data, 0, create = true, truncate = true, data.length, flush = true)
  }
}
