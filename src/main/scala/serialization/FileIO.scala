package serialization

import java.nio.file.{Files, Path}

import scodec.bits.BitVector
import scodec.{Decoder, Attempt, Encoder}

import scala.languageFeature.higherKinds
import scalaz.concurrent.Task
import scalaz.syntax.traverse._
import scalaz.{Applicative, Traverse}

object FileIO {
  def writeFile(file: Path, data: BitVector): Task[BitVector] = {
    Task.delay {
      Files.write(file, data.toByteArray)
    }.as(data)
  }

  def readFile(file: Path): Task[BitVector] = {
    Task.delay {
      BitVector(Files.readAllBytes(file))
    }
  }

  def writeBinaryFile[A](file: Path, data: A)(implicit encoder: Encoder[A]): Task[Attempt[BitVector]] =
    encoder.encode(data).
      traverseU(binaryData => writeFile(file, binaryData))


  def readBinaryFile[A](file: Path)(implicit decoder: Decoder[A]): Task[Attempt[A]] =
    readFile(file).
      map(decoder.decodeValue)




























  implicit val attemptTraverse: Traverse[Attempt] = new Traverse[Attempt] {
    def traverseImpl[G[_], A, B](fa: Attempt[A])(f: A => G[B])(implicit appG: Applicative[G]): G[Attempt[B]] =
      fa match {
        case Attempt.Successful(value) => f(value).map(Attempt.Successful(_))
        case fail@Attempt.Failure(err) => appG.point(fail)
      }
  }
}
