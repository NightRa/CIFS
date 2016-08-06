package ipfs


import java.io.ByteArrayOutputStream
import java.net.{URI, URLEncoder}

import cats.data.Xor._
import io.circe.generic.auto._
import io.circe.parser._
import io.circe.{Decoder, Json}
import io.ipfs.api.Client
import org.apache.hc.client5.http.entity.mime.MultipartEntityBuilder
import org.apache.hc.client5.http.fluent.Request
import org.apache.hc.core5.http.HttpEntity
import scodec.bits.ByteVector
import serialization.Response
import serialization.Response.FType

object Files {

  case class IPFSStat(Blocks: Int, CumulativeSize: Long, Hash: String, Size: Long, Type: String) {
    def ftype: FType = Type match {
      case "directory" => Response.Folder
      case "file" => Response.File
      case _ => sys.error(s"stat(???); Invalid file type: $Type")
    }
  }
  case class Entry(Name: String, Type: Int, Size: Long, Hash: String) {
    def ftype: FType = Type match {
      case 0 => Response.File
      case 1 => Response.Folder
      case _ => sys.error(s"ls(???); Invalid file type: $Type")
    }
  }
  case class IPFSLs(Entries: Vector[Entry])

  def rootKey(): Response.RootKeyOK = {
    Response.RootKeyOK(new Client("localhost").id.ID)
  }

  def flush(): Response.FlushOK.type = {
    Request.Get("http://localhost:5001/api/v0/files/flush").execute()
    Response.FlushOK
  }

  def badPath(path: String): Boolean = path.contains("&|=|\\?") // TODO: This is not a proper security check. Need to do this sanitization properly. (Injection prone)

  def urlEncode(s: String): String = URLEncoder.encode(s, "UTF-8")

  def stat(path: String): Option[IPFSStat] = {
    if (badPath(path)) {
      println(s"stat($path): Bad path")
      None
    } else
      exec(s"stat($path)", Request.Get(new URI(s"http://localhost:5001/api/v0/files/stat?arg=${urlEncode(path)}"))).decode[IPFSStat]
  }

  def ls(path: String): Option[IPFSLs] = {
    if (badPath(path)) {
      println(s"ls($path): Bad path")
      None
    } else {
      val res = exec(s"ls($path)", Request.Get(new URI(s"http://localhost:5001/api/v0/files/ls?arg=${urlEncode(path)}&l=true")))
      res.json.flatMap(json => json.hcursor.downField("Entries").focus.fold(sys.error(s"ls($path): no Entries filed, json: $json")) { j =>
        if (j.isNull)
          Some(IPFSLs(Vector.empty))
        else
          res.decode[IPFSLs]
      })
    }
  }

  def read(path: String, offset: Long, count: Option[Long]): Option[ByteVector] = {
    // TODO: Report bug in IPFS; if offset = length, should be empty, not the whole message.
    stat(path).flatMap { stat =>
      if (offset == stat.Size) Some(ByteVector.empty)
      else {
        // The regular query
        if (badPath(path)) {
          println(s"read($path,offset=$offset,count=$count): Bad path")
          None
        } else {
          val res = exec(s"read($path,offset=$offset,count=$count)", Request.Get(new URI(s"http://localhost:5001/api/v0/files/read?arg=${urlEncode(path)}&offset=$offset${count.fold("")(c => s"&count=$c")}")))
          res.bytes
        }
      }
    }

  }

  def write(path: String, data: ByteVector, offset: Long, create: Boolean, truncate: Boolean, count: Long, flush: Boolean): Option[Unit] = {
    if (badPath(path)) {
      println(s"write($path,$data,offset=$offset,create=$create,truncate=$truncate,count=$count,flush=$flush): Bad path")
      None
    } else {
      val res = exec(s"write($path,$data,offset=$offset,create=$create,truncate=$truncate,count=$count,flush=$flush)",
        Request.Post(new URI(s"http://localhost:5001/api/v0/files/write?arg=${urlEncode(path)}&offset=$offset&create=$create&truncate=$truncate&count=$count"))
          .body(MultipartEntityBuilder.create()
            .addBinaryBody("data", data.toArray)
            .build())
      )
      res.string.map(_ => ()) // TODO: Report bad warning to intellij (unit return type in the argument of map)
    }
  }

  def mkdir(path: String, makeParents: Boolean, flush: Boolean): Option[Unit] = {
    if(badPath(path)) {
      println(s"mkdir($path,makeParents=$makeParents,flush=$flush)")
      None
    } else {
      val res = exec(s"mkdir($path,makeParents=$makeParents,flush=$flush)",
        Request.Post(new URI(s"http://localhost:5001/api/v0/files/mkdir?arg=${urlEncode(path)}&parents=$makeParents&flush=$flush")))
      res.string.map(_ => ())
    }
  }

  def rm(path: String, recursive: Boolean, flush: Boolean): Option[Unit] = {
    if(badPath(path)) {
      println(s"rm($path,recursive=$recursive,flush=$flush)")
      None
    } else {
      val res = exec(s"rm($path,recursive=$recursive,flush=$flush)",
        Request.Post(new URI(s"http://localhost:5001/api/v0/files/rm?arg=${urlEncode(path)}&recursive=$recursive&flush=$flush")))
      res.string.map(_ => ())
    }
  }

  def mv(src: String, dest: String, flush: Boolean):  Option[Unit] = {
    if(src == dest) Some(()) // TODO: Report bug to IPFS, if src == dest, it removes the file.
    else {
      if(badPath(src) || badPath(dest)) {
        println(s"mv($src,$dest,flush=$flush)")
        None
      } else {
        val res = exec(s"mv($src,$dest,flush=$flush)",
          Request.Post(new URI(s"http://localhost:5001/api/v0/files/mv?arg=${urlEncode(src)}&arg=${urlEncode(dest)}&flush=$flush")))
        res.string.map(_ => ())
      }
    }
  }

  /*val data1: Array[Byte] = "hello world test1!".getBytes
  println(write("/test1", ByteVector(data1), 0, create = true, truncate = false, data1.length, flush = true))
  println(stat("/test1"))

  val data2: Array[Byte] = "hello world test2!".getBytes
  println(write("/test2", ByteVector(data2), 0, create = true, truncate = false, data2.length, flush = true))
  println(stat("/test2"))
  val read1: Option[ByteVector] = read("/test2", 18, count = None)
  println(read1)
  println(read1.map(_.toArray).map(new String(_)))

  println("mkdir(/dir3/dir4) -r: " + mkdir("/dir3/dir4", makeParents = true, flush = true).toString)
  println("rm(/dir3/dir4) -r: " + rm("/dir3/dir4", recursive = true, flush = true))
  // println("rm(/dir3/dir4) -r: " + rm("/dir3/dir4", recursive = true, flush = true))

  println("mv(/test1, /test2): " + mv("/test1", "/test2", flush = true).toString)
  println("ls(/): " + ls("/"))

  val read2: Option[ByteVector] = read("/test2", 0, count = None)
  println("read(/test2): " + read2.map(_.toArray).map(new String(_)))*/

  // HTTP & fast and loose error handling TODO: Proper error handling. See IPFS-API.txt for deep analysis.

  case class HTTPResponse(statusCode: Int, resp: HttpEntity, description: String) {
    def data: Option[ByteArrayOutputStream] = {
      val out = new ByteArrayOutputStream
      resp.writeTo(out)

      if (statusCode == 400) {
        sys.error(s"Status code 400, Bad request: on $description, data: ${out.toString}")
      }
      else if (statusCode == 500) {
        println(s"Status code 500 on $description, data: ${out.toString}")
        None
      } else if (statusCode == 200) {
        Some(out)
      } else {
        sys.error(s"Unknown status code $statusCode, data: ${out.toString}")
      }
    }

    def bytes: Option[ByteVector] = {
      data.map(bs => ByteVector(bs.toByteArray))
    }

    def string: Option[String] = {
      data.map(_.toString)
    }

    def json: Option[Json] = {
      string.flatMap(s => parse(s) match {
        case Left(err) =>
          println(s"Json parsing error on $description: input = $s, error: $err")
          None
        case Right(json) => Some(json)
      })
    }

    def decode[A](implicit decoder: Decoder[A]): Option[A] = {
      string.flatMap(s => io.circe.parser.decode[A](s) match {
        case Left(err) =>
          println(s"Json Decoding error on $description, input: $s, error: $err")
          None
        case Right(res) =>
          Some(res)
      })
    }

  }

  def exec(description: String, request: Request): HTTPResponse = {
    val resp = request.execute().returnResponse()
    val statusCode = resp.getCode
    HTTPResponse(statusCode, resp.getEntity, description)
  }


}
