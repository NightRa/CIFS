package demos

import java.nio.file.{Path, Paths}

import model.IndexUtils._
import model._
import scodec.{Attempt, Encoder}
import serialization.FileIO
import serialization.Model._

object DemoBinary extends App {

  // ------------------------ Hash Function ----------------------------------

  import hash.SHA256._

  // ------------------------- Sample Tree -----------------------------------

  val remoteRoot = RemotePath(MutablePtr(hash("public key hash")),
    List("4", "5"))

  val uniNotesFolder: Folder = folder("0" -> hash("0"))("3" -> FollowLeaf(remoteRoot))(
    "1" ->
      folder("2" -> hash("2"))()()
  )

  /*
  Folder(IMap("0" -> hash("0")),
    IMap("1" -> Folder(IMap(
      "2" -> hash("2")
    ), IMap.empty, IMap.empty)),
    IMap("3" -> FollowLeaf(remoteRoot))
  )
  */

  println(serialization.Model.folderCodec.encode(uniNotesFolder).require.toHex)

  // ------------------------- Serialization ---------------------------------

  val indexFile: Path = Paths.get("BinaryDemoIndex.dat")

  FileIO.writeBinaryFile(indexFile, uniNotesFolder).unsafePerformSync

  println {
    FileIO.readBinaryFile[Folder](indexFile).unsafePerformSync ==
      Attempt.Successful(uniNotesFolder)
  }

}
