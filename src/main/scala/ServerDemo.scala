import java.net.{InetAddress, InetSocketAddress}
import java.nio.ByteBuffer
import java.nio.channels.{ServerSocketChannel, SocketChannel}

import follows.FollowSerialization._
import follows.ParsePath._
import follows.ResolvePath._
import follows.{Follow, FollowManager, RemotePath}
import io.ipfs.api.Client
import ipfs.Files
import ipfs.Queries.localize
import scodec.Attempt.{Failure, Successful}
import scodec.bits.{BitVector, _}
import scodec.{Attempt, DecodeResult}
import serialization.Response.FEntry
import serialization.{CommunicationProtocol, Requests, Response}

object ServerDemo extends App {
  val client = new Client("localhost")
  var followManager = new FollowManager(FollowManager.fetchLocalFollows())
  def follows = followManager.follows // mutable

  import server.Republisher.republisher

  val all = InetAddress.getByName("0.0.0.0")
  val port = 8008
  val serverListener = ServerSocketChannel.open().bind(new InetSocketAddress(all, port))

  println("Opened server")

  while (true) {
    val socket: SocketChannel = serverListener.accept()
    socket.configureBlocking(true)

    // println("Got connection")

    val input: BitVector = BitVector.fromChannel(socket, 1024 * 4, direct = true)

    // val startTime = System.currentTimeMillis()

    // println(s"Start time: ${System.currentTimeMillis() - startTime}")

    val req: Attempt[DecodeResult[Requests.Request]] = CommunicationProtocol.request.decode(input)

    // println(s"Decoded time: ${System.currentTimeMillis() - startTime}")

    req match {
      case Failure(err) => println(err)
      case Successful(DecodeResult(value, rest)) =>
        println("Request: " + value)
        val response = handleRequest(value)
        println("Response: " + response)
        // println(s"Handled request: ${System.currentTimeMillis() - startTime}")


        val encoded: Attempt[BitVector] = CommunicationProtocol.response.encode(response)

        // println(s"Encoded response: ${System.currentTimeMillis() - startTime}")

        encoded match {
          case Failure(err) => println(s"Encoding error: $err")
          case Successful(bitsToSend) =>
            val chunkSize = 1024 * 1000 * 16
            val buf: ByteBuffer = bitsToSend.toByteBuffer
            // println(s"Converted to byte buffer: ${System.currentTimeMillis() - startTime}")

            while (buf.remaining() > 0) {
              socket.write(buf)
              // println(s"Wrote response: ${System.currentTimeMillis() - startTime}")
            }
            socket.shutdownOutput()
        }
    }

    socket.shutdownOutput()

    // println("Finished getting data")

    socket.close()

    // println("Closed connection")
  }

  serverListener.close()

  println("Closed server")
  def handleRequest(request: Requests.Request): Response.Response = request match {
    case Requests.RootKey => Response.RootKeyOK(Files.rootKey().rootKey)
    case Requests.Flush =>
      Files.flush()
      republisher ! true
      Response.FlushOK
    case Requests.Stat(path) =>
      if (path == "/") Response.StatOK(Response.Folder, Response.ReadWrite, 0) // TODO: Probably remote this. (The frontend sends alot of these)
      else {
        localize(path, follows)(Files.stat)
          .fold[Response.Stat](Response.StatNoSuchFile)(stat => Response.StatOK(stat.ftype, Response.ReadWrite, stat.Size))
      }
    case Requests.Ls(path) =>
      val dir =
        if (path.last != '/') path + '/'
        else path

      localize(path, follows)(Files.ls)
        .fold[Response.Ls](Response.LsNoSuchFolder)(ls =>
        Response.LsOK(ls.Entries.map(e => Response.FEntry(e.ftype, Response.ReadWrite, e.Size, e.Name)) ++
          follows.followsUnder(path).map { name =>
            localize(dir + name, follows)(Files.stat).fold[Option[Response.FEntry]](None)(stat => Some(FEntry(stat.ftype, Response.ReadWrite, 0, name)))
          }.collect { case Some(x) => x }
        ))
    case Requests.Read(path, offset, count) =>
      localize(path, follows)(Files.read(_, offset, Some(count)))
        .fold[Response.Read](Response.ReadNoSuchFile)(bytes => Response.ReadOK(bytes))
    case Requests.CreateFile(path) =>
      resolvePath(path, follows).fold[Response.CreateFile](Response.CreateFileInvalidName) {
        case Local(path) =>
          Files.write(path, ByteVector.empty, 0, create = true, truncate = false, count = 0, flush = true)
            .fold[Response.CreateFile](Response.CreateFileNameCollision)(_ => Response.CreateFileOK)
        case Remote(_) => Response.CreateFileFolderIsReadOnly
      }
    case Requests.Mkdir(path) =>
      resolvePath(path, follows).fold[Response.Mkdir](Response.MkdirInvalidName) {
        case Local(path) =>
          Files.mkdir(path, makeParents = false, flush = true)
            .fold[Response.Mkdir](Response.MkdirNameCollision)(_ => Response.MkdirOK)
        case Remote(_) => Response.MkdirFolderReadOnly
      }
    case Requests.Rm(path) =>
      if (followManager.rmFollow(path)) Response.RmOK
      else {
        resolvePath(path, follows).fold[Response.Rm](Response.RmPathDoesntExist) {
          case Local(path) =>
            Files.rm(path, recursive = true, flush = true)
              .fold[Response.Rm](Response.RmPathDoesntExist)(_ => Response.RmOK)
          case Remote(_) => Response.RmReadOnly
        }
      }
    case Requests.Mv(src, dest) =>
      resolvePath(dest, follows).fold[Response.Mv](Response.MvSrcDoesntExist /*problematic, can't resolve dest*/) {
        case Local(destPath) =>
          localize(src, follows)(srcPath => Files.mv(srcPath, destPath, flush = true))
            .fold[Response.Mv](Response.MvSrcDoesntExist)(_ => Response.MvOK)
        case Remote(_) => Response.MvReadOnly
      }
    case Requests.Write(path, offset, buf) =>
      resolvePath(path, follows).fold[Response.Write](Response.WriteFileDoesntExist) {
        case Local(path) =>
          Files.write(path, buf, offset, create = false, truncate = false, buf.length, flush = true)
            .fold[Response.Write](Response.WriteFileDoesntExist)(_ => Response.WriteOK)
        case Remote(_) => Response.WriteFileReadOnly
      }

    case Requests.CloneFollow(Requests.Clone, localPath, remotePathStr) =>

      (for {
        localPathList <- parsePath(localPath)
        remote <- deserializeRemotePath(remotePathStr)
      } yield (localPathList, remote)) match {
        case None => Response.CloneFollowMalformedPath
        case Some((localPathList, RemotePath(root, remotePath))) =>
          if (Files.ls(localPathList.init.mkString("/", "/", "")).isEmpty)
            Response.CloneFollowParentDoesntExist
          else
            Files.cp(s"/ipns/$root/${remotePath.mkString("/")}", localPath, flush = true)
              .fold[Response.CloneFollow](Response.CloneFollowRootNotFound)(_ => Response.CloneFollowOK)
      }


    case Requests.CloneFollow(Requests.Follow, localPath, remotePath) =>
      deserializeRemotePath(remotePath) match {
        case None => Response.CloneFollowMalformedPath
        case Some(remote) => followManager.addFollow(Follow(localPath, remote))
      }
  }

  /*var amountRead: Int = 0
  while ({
    amountRead = socket.read(buffer)
    amountRead != -1
  }) {
    val data = new Array[Byte](amountRead)
    System.arraycopy(buffer.array, 0, data, 0, amountRead)
    System.out.print(new String(data))

    buffer.flip()
    socket.write(buffer)
    buffer.clear()
  }*/


}


object ClientDemo extends App {
  // while(true) {
  val socket: SocketChannel = SocketChannel.open(new InetSocketAddress("localhost", 8008))

  val message: Requests.Request = Requests.Flush

  val bytes = CommunicationProtocol.request.encode(message).require.toByteBuffer

  socket.write(bytes)

  socket.shutdownOutput()

  val input: BitVector = BitVector.fromChannel(socket, 1024 * 4, direct = true)

  val startTime = System.currentTimeMillis()

  println(s"Start time: ${System.currentTimeMillis() - startTime}")

  val answer: Attempt[DecodeResult[Response.Response]] = CommunicationProtocol.response.decode(input)

  println(s"Decoded time: ${System.currentTimeMillis() - startTime}")

  println(answer)

  println("Finished sending data")

  socket.close()

  println("Closed connection (Client)")
  // }
}

object IPFSDemo extends App {
  new Client("localhost").fileLs("/")
}
