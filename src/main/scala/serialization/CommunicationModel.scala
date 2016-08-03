package serialization

import java.nio.charset.Charset

import model.MutablePtr
import scodec.Codec
import scodec.bits.ByteVector
import scodec.codecs._

import scala.language.higherKinds

object Requests {
  type Path = String
  sealed trait Request
  case object RootKey extends Request
  case object Flush extends Request
  case class Stat(path: Path) extends Request
  case class Ls(path: Path) extends Request
  case class Read(path: Path, offset: Long, count: Long /*uint32*/) extends Request
  case class Write(path: Path, offset: Long, data: ByteVector) extends Request
  case class CreateFile(path: Path) extends Request
  case class Mkdir(path: Path) extends Request
  case class Rm(path: Path) extends Request
  case class Mv(src: Path, dest: Path) extends Request
}

object Response {
  sealed trait FType
  case object File extends FType
  case object Folder extends FType

  sealed trait RW
  case object ReadOnly extends RW
  case object ReadWrite extends RW

  case class FEntry(fType: FType, rW: RW, size: Long, name: String)

  sealed trait Response

  case class RootKeyOK(hash: MutablePtr) extends Response
  case object FlushOK extends Response

  sealed trait Stat extends Response
  case object StatNoSuchFile extends Stat
  case class StatOK(ftype: FType, rw: RW, size: Long) extends Stat

  sealed trait Ls extends Response
  case object LsNoSuchFolder extends Ls
  case class LsOK(entries: Vector[FEntry]) extends Ls

  sealed trait Read extends Response
  case object ReadNoSuchFile extends Read
  case class ReadOK(data: ByteVector) extends Read

  sealed trait Write extends Response
  case object WriteOK extends Write
  case object WriteFileReadOnly extends Write
  case object WriteFileDoesntExist extends Write

  sealed trait CreateFile extends Response
  case object CreateFileOK extends CreateFile
  case object CreateFileFolderIsReadOnly extends CreateFile
  case object CreateFileNameCollision extends CreateFile
  case object CreateFileParentDoesntExist extends CreateFile

  sealed trait Mkdir extends Response
  case object MkdirOK extends Mkdir
  case object MkdirFolderReadOnly extends Mkdir
  case object MkdirNameCollision extends Mkdir

  sealed trait Rm extends Response
  case object RmOK extends Rm
  case object RmReadOnly extends Rm
  case object RmPathDoesntExist extends Rm

  sealed trait Mv extends Response
  case object MvOK extends Mv
  case object MvReadOnly extends Mv
  case object MvSrcDoesntExist extends Mv
}

