package instance

import java.io.File
import java.nio.file.{Files, Path, Paths}

import model.{Folder, Hash}
import org.ipfs.api.{IPFS, MerkleNode, Multihash, NamedStreamable}
import org.ipfs.api.NamedStreamable._
import scodec.Attempt
import scodec.bits.BitVector
import serialization.FileIO
import serialization.Model._

abstract class ICIFSInstance {
  def setIndex(index: Folder)

  def getIndex(): Folder
}

class CIFSInstance(ipfs: IPFS, initialTree: Folder) extends ICIFSInstance {
  // TODO: MutableVar
  var index: Folder = initialTree

  override def setIndex(newIndex: Folder): Unit = {
    index = newIndex
  }

  override def getIndex(): Folder = {
    index
  }

  // IO[Hash]
  def addFile(file: File, fileName: String, path: List[String]): Hash = {
    // Todo: use Task, ipfs.add is a blocking call.
    val ipfsResp = ipfs.add(fileStream(file))
    val multiHash: Multihash = ipfsResp.hash
    val hash = Hash(BitVector(multiHash.toBytes))

    setIndex(index.writeFile(fileName, path, hash))
    hash
  }

}

object CIFS {

  def fresh(ipfs: IPFS): CIFSInstance =
    new CIFSInstance(ipfs, Folder.emptyFolder)

  def load(indexFile: Path, ipfs: IPFS): Attempt[CIFSInstance] = {
    FileIO.readBinaryFile[Folder](indexFile).unsafePerformSync.map {
      index =>
        new CIFSInstance(ipfs, index)
    }
  }

  def save(indexFile: Path, instance: ICIFSInstance): Unit = {
    FileIO.writeBinaryFile(indexFile, instance.getIndex())
  }

  def main(args: Array[String]): Unit = {
    val ipfs: IPFS = new IPFS("127.0.0.1", 5001)
    // Path of index
    val indexPath = Paths.get(System.getProperty("user.home"), ".CIFS/index.dat")

    if (!Files.exists(indexPath)) {
      // create fresh
    } else {
      // load
    }
  }

}
