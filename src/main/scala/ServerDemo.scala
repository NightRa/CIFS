import java.net.{InetAddress, InetSocketAddress}
import java.nio.ByteBuffer
import java.nio.channels.{ServerSocketChannel, SocketChannel}

import io.ipfs.api.Client
import scodec.Attempt.{Failure, Successful}
import scodec.bits.{BitVector, _}
import scodec.{Attempt, DecodeResult}
import serialization.{CommunicationProtocol, Requests, Response}

object ServerDemo extends App {
  val client = new Client("localhost")

  val all = InetAddress.getByName("0.0.0.0")
  val port = 8008
  val server = ServerSocketChannel.open().bind(new InetSocketAddress(all, port))

  println("Opened server")

  while (true) {
    val socket: SocketChannel = server.accept()
    socket.configureBlocking(true)

    println("Got connection")

    val input: BitVector = BitVector.fromChannel(socket, 1024 * 4, direct = true)

    val startTime = System.currentTimeMillis()

    println(s"Start time: ${System.currentTimeMillis() - startTime}")

    val req: Attempt[DecodeResult[Requests.Request]] = CommunicationProtocol.request.decode(input)

    println(s"Decoded time: ${System.currentTimeMillis() - startTime}")

    req match {
      case Failure(err) => println(err)
      case Successful(DecodeResult(value, rest)) =>
        println(value)
        val response = handleRequest(value, client)
        println(s"Handled request: ${System.currentTimeMillis() - startTime}")

        val encoded: Attempt[BitVector] = CommunicationProtocol.response.encode(response)

        println(s"Encoded response: ${System.currentTimeMillis() - startTime}")

        encoded match {
          case Failure(err) => println(s"Encoding error: $err")
          case Successful(bitsToSend) =>
            val chunkSize = 1024 * 1000 * 16
            val buf: ByteBuffer = bitsToSend.toByteBuffer
            println(s"Converted to byte buffer: ${System.currentTimeMillis() - startTime}")

            while (buf.remaining() > 0) {
              socket.write(buf)
              println(s"Wrote response: ${System.currentTimeMillis() - startTime}")
            }
            socket.shutdownOutput()
        }
    }

    socket.shutdownOutput()

    println("Finished getting data")

    socket.close()

    println("Closed connection")
  }

  server.close()

  println("Closed server")

  def handleRequest(request: Requests.Request, client: Client): Response.Response = request match {
    case Requests.RootKey => Response.RootKeyOK(client.id.ID)
    case Requests.Flush => Response.FlushOK
    case Requests.Stat(path) => Response.StatOK(Response.File, Response.ReadWrite, 20)
    case Requests.Rm(path) => Response.RmReadOnly
    case Requests.Mkdir(path) => Response.MkdirParentDoesntExist
    case Requests.CreateFile(path) => Response.CreateFileParentDoesntExist
    case Requests.Ls(path) => Response.LsOK(Vector(Response.FEntry(Response.File, Response.ReadOnly, 666L, "אאאaaa")))
    case Requests.Mv(src, dest) => Response.MvReadOnly
    case Requests.Read(path, offset, count) => Response.ReadOK(ByteVector(1, 7, 9))
    case Requests.Write(path, offset, buf) => Response.WriteOK
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