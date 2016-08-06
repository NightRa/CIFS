import java.net.{InetAddress, InetSocketAddress}
import java.nio.ByteBuffer
import java.nio.channels.{ServerSocketChannel, SocketChannel}

import io.ipfs.api.Client
import ipfs.Files
import scodec.Attempt.{Failure, Successful}
import scodec.bits.{BitVector, _}
import scodec.{Attempt, DecodeResult}
import serialization.{CommunicationProtocol, Requests, Response}
import server.Republisher.republisher

object ServerDemo extends App {

  val all = InetAddress.getByName("0.0.0.0")
  val port = 8008
  val server = ServerSocketChannel.open().bind(new InetSocketAddress(all, port))

  println("Opened server")

  while (true) {
    val socket: SocketChannel = server.accept()
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

  server.close()

  println("Closed server")
  def handleRequest(request: Requests.Request): Response.Response = request match {
    case Requests.RootKey => Response.RootKeyOK(Files.rootKey().rootKey)
    case Requests.Flush =>
      Files.flush()
      republisher ! true
      Response.FlushOK
    case Requests.Stat(path) =>
      if (path == "/") Response.StatOK(Response.Folder, Response.ReadWrite, 0) // TODO: Probably remote this. (The frontend sends alot of these)
      else Files.stat(path).fold[Response.Stat](Response.StatNoSuchFile)(stat => Response.StatOK(stat.ftype, Response.ReadWrite, stat.Size))
    case Requests.Ls(path) => Files.ls(path).fold[Response.Ls](Response.LsNoSuchFolder)(ls => Response.LsOK(ls.Entries.map(e => Response.FEntry(e.ftype, Response.ReadWrite, e.Size, e.Name))))
    case Requests.CreateFile(path) => Files.write(path, ByteVector.empty, 0, create = true, truncate = false, count = 0, flush = true).fold[Response.CreateFile](Response.CreateFileNameCollision)(_ => Response.CreateFileOK)
    case Requests.Mkdir(path) => Files.mkdir(path, makeParents = false, flush = true).fold[Response.Mkdir](Response.MkdirNameCollision)(_ => Response.MkdirOK)
    case Requests.Rm(path) => Files.rm(path, recursive = true, flush = true).fold[Response.Rm](Response.RmPathDoesntExist)(_ => Response.RmOK)
    case Requests.Mv(src, dest) => Files.mv(src, dest, flush = true).fold[Response.Mv](Response.MvSrcDoesntExist)(_ => Response.MvOK)
    case Requests.Read(path, offset, count) => Files.read(path, offset, Some(count)).fold[Response.Read](Response.ReadNoSuchFile)(bytes => Response.ReadOK(bytes))
    case Requests.Write(path, offset, buf) => Files.write(path, buf, offset, create = false, truncate = false, buf.length, flush = true).fold[Response.Write](Response.WriteFileDoesntExist)(_ => Response.WriteOK)

    case Requests.CloneFollow(Requests.Clone, localPath, remotePath) => Response.CloneFollowMalformedPath
    case Requests.CloneFollow(Requests.Follow, localPath, remotePath) => Response.CloneFollowNameCollision
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
