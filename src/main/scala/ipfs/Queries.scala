package ipfs

import java.net.URI

import ipfs.Files.{badPath, exec, urlEncode}
import org.apache.hc.client5.http.fluent.Request

object Queries {
  def catStr(path: String, debug: Boolean = true): Option[String] = {
    if (badPath(path)) {
      println(s"cat($path): Bad path")
      None
    } else {
      val res = exec(s"cat($path)", Request.Get(new URI(s"http://localhost:5001/api/v0/cat?arg=${urlEncode(path)}")), debug = false)
      res.string
    }
  }
}
