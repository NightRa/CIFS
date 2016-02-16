package demos

import java.nio.file.{Path, Paths}
import java.security.MessageDigest

import model._
import scodec.Attempt
import scodec.bits.BitVector
import serialization.FileIO

import scala.collection.immutable.TreeMap

object DemoBinary extends App {

  // ------------------------ Hash Function ----------------------------------

  import hash.SHA256._

  // ------------------------- Sample Tree -----------------------------------

  val remoteRoot = RemotePath(MutablePtr(hash("public key hash")),
    List("4", "5"))

  val uniNotesFolder: Index = Folder(TreeMap(
    "0" -> HashLeaf(hash("0")),
    "1" -> Folder(TreeMap(
      "2" -> HashLeaf(hash("2"))
    )),
    "3" -> FollowLeaf(remoteRoot)
  ))

  println(serialization.Model.indexCodec.encode(uniNotesFolder).require.toHex)

  // ------------------------- Serialization ---------------------------------

  import serialization.Model._

  val indexFile: Path = Paths.get("index.dat")

  FileIO.writeBinaryFile(indexFile, uniNotesFolder).unsafePerformSync

  println {
    FileIO.readBinaryFile[Index](indexFile).unsafePerformSync ==
      Attempt.Successful(uniNotesFolder)
  }

}
