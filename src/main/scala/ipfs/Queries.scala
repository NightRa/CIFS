package ipfs

import java.net.URI

import follows.{Follows, RemotePath}
import follows.ResolvePath.{FileLocation, Local, Remote}
import ipfs.Files.{badPath, exec, urlEncode}
import org.apache.hc.client5.http.fluent.Request
import follows.ResolvePath.resolvePath

object Queries {
  var tmpCreated = false

  def catStr(path: String, debug: Boolean = true): Option[String] = {
    if (badPath(path)) {
      println(s"cat($path): Bad path")
      None
    } else {
      val res = exec(s"cat($path)", Request.Get(new URI(s"http://localhost:5001/api/v0/cat?arg=${urlEncode(path)}")), debug = false)
      res.string
    }
  }

  def localize[A](complexPath: String, follows: Follows)(handlePath: String => Option[A]): Option[A] =
    resolvePath(complexPath, follows).flatMap(loc => localizeLocation(loc)(handlePath))

  def localizeLocation[A](location: FileLocation)(handlePath: String => A): A = location match {
    case Local(path) => handlePath(path)
    case Remote(RemotePath(root, remotePath)) =>
      createTmp()
      val fileName = s"/tmp/$root/${remotePath.mkString("/")}"
      Files.cp(s"/ipns/$root/${remotePath.mkString("/")}", fileName, flush = false)
      val res = handlePath(fileName)
      Files.rm(fileName, recursive = true, flush = true)
      res
  }

  def createTmp(): Unit = {
    if(!tmpCreated){
      Files.mkdir("/.tmp", makeParents = false, flush = true)
      tmpCreated = true
    }
  }
}
