package serialization

import scodec.bits._
import scodec.codecs._
import scodec.{Attempt, Codec, DecodeResult, Err, SizeBound}
import serialization.Model.mutablePtrCodec
import serialization.Response.RootKeyOK

object CommunicationProtocol {
  // Some primitives
  val path = utf8_32
  val byte: Codec[Short] = ushort8
  val uint64: Codec[Long] = constant(BitVector.zero) ~> ulong(63)

  // The codecs

  val ftype: Codec[Response.FType] = "File/Folder type" | discriminated[Response.FType].by(byte)
    .typecase(0, provide(Response.File))
    .typecase(1, provide(Response.Folder))

  val rw: Codec[Response.RW] = "Read/Write" | discriminated[Response.RW].by(byte)
    .typecase(0, provide(Response.ReadOnly))
    .typecase(1, provide(Response.ReadWrite))

  // 0 - root key
  val rootKeyRequest = "Root Key Request" | provide(Requests.RootKey)
  val rootKeyResponse: Codec[Response.RootKeyOK] = "Root Key Response" | utf8_32.as[RootKeyOK]

  // 1 - flush
  val flushRequest = "Flush Request" | provide(Requests.Flush)
  val flushResponse = "Flush Response" | provide(Response.FlushOK)

  // 2 - stat
  val statRequest: Codec[Requests.Stat] = "Stat Request" | path.as[Requests.Stat]
  val statResponse: Codec[Response.Stat] = "Stat Response" | discriminated[Response.Stat].by(byte)
    .typecase(1, provide(Response.StatNoSuchFile))
    .typecase(0, (ftype :: rw :: uint64).as[Response.StatOK])

  // 3 - ls
  val fentry: Codec[Response.FEntry] = "FEntry" | (ftype :: rw :: uint64 :: utf8_32).as[Response.FEntry]
  val lsRequest: Codec[Requests.Ls] = "Ls Request" | path.as[Requests.Ls]
  val lsResponse: Codec[Response.Ls] = "Ls Response" | discriminated[Response.Ls].by(byte)
    .typecase(1, provide(Response.LsNoSuchFolder))
    .typecase(0, vectorOfN(int32, fentry).as[Response.LsOK])

  // 4 - read
  val readRequest: Codec[Requests.Read] = "Read Request" | (path :: uint64 :: uint32).as[Requests.Read]
  val readResponse: Codec[Response.Read] = "Read Response" | discriminated[Response.Read].by(byte)
    .typecase(1, provide(Response.ReadNoSuchFile))
    .typecase(0, data32.as[Response.ReadOK])

  // 5 - write
  val writeRequest: Codec[Requests.Write] = "Write Request" | (path :: uint64 :: data32).as[Requests.Write]
  val writeResponse: Codec[Response.Write] = "Write Response" | discriminated[Response.Write].by(byte)
    .typecase(0, provide(Response.WriteOK))
    .typecase(1, provide(Response.WriteFileReadOnly))
    .typecase(2, provide(Response.WriteFileDoesntExist))

  // 6 - create file
  val createFileRequest: Codec[Requests.CreateFile] = "Create File Request" | path.as[Requests.CreateFile]
  val createFileResponse: Codec[Response.CreateFile] = "Create File Response" | discriminated[Response.CreateFile].by(byte)
    .typecase(0, provide(Response.CreateFileOK))
    .typecase(1, provide(Response.CreateFileFolderIsReadOnly))
    .typecase(2, provide(Response.CreateFileNameCollision))
    .typecase(3, provide(Response.CreateFileParentDoesntExist))
    .typecase(4, provide(Response.CreateFileInvalidName))

  // 7 - mkdir
  val mkdirRequest: Codec[Requests.Mkdir] = "Mkdir Request" | path.as[Requests.Mkdir]
  val mkdirResponse: Codec[Response.Mkdir] = "Mkdir Response" | discriminated[Response.Mkdir].by(byte)
    .typecase(0, provide(Response.MkdirOK))
    .typecase(1, provide(Response.MkdirFolderReadOnly))
    .typecase(2, provide(Response.MkdirNameCollision))
    .typecase(3, provide(Response.MkdirParentDoesntExist))
    .typecase(4, provide(Response.MkdirInvalidName))

  // 8 - rm
  val rmRequest: Codec[Requests.Rm] = "Rm Request" | path.as[Requests.Rm]
  val rmResponse: Codec[Response.Rm] = "Rm Response" | discriminated[Response.Rm].by(byte)
    .typecase(0, provide(Response.RmOK))
    .typecase(1, provide(Response.RmReadOnly))
    .typecase(2, provide(Response.RmPathDoesntExist))

  // 9 - mv
  val mvRequest: Codec[Requests.Mv] = "Mv Request" | (path :: path).as[Requests.Mv]
  val mvResponse: Codec[Response.Mv] = "Mv Response" | discriminated[Response.Mv].by(byte)
    .typecase(0, provide(Response.MvOK))
    .typecase(1, provide(Response.MvReadOnly))
    .typecase(2, provide(Response.MvSrcDoesntExist))

  // 10 - mv
  val cloneOrFollow: Codec[Requests.CloneOrFollow] = "Clone or Follow" | discriminated[Requests.CloneOrFollow].by(byte)
    .typecase(0, provide(Requests.Clone))
    .typecase(1, provide(Requests.Follow))
  val cloneFollowRequest: Codec[Requests.CloneFollow] = "Clone-Follow Request" | (cloneOrFollow :: path :: path).as[Requests.CloneFollow]
  val cloneFollowResponse: Codec[Response.CloneFollow] = "Clone-Follow Response" | discriminated[Response.CloneFollow].by(byte)
    .typecase(0, provide(Response.CloneFollowOK))
    .typecase(1, provide(Response.CloneFollowFolderIsReadOnly))
    .typecase(2, provide(Response.CloneFollowNameCollision))
    .typecase(3, provide(Response.CloneFollowParentDoesntExist))
    .typecase(4, provide(Response.CloneFollowMalformedPath))
    .typecase(5, provide(Response.CloneFollowRootNotFound))
    .typecase(6, provide(Response.CloneFollowRemotePathBroken))

  // Request codec
  val request = discriminated[Requests.Request].by(byte)
    .typecase(0, rootKeyRequest)
    .typecase(1, flushRequest)
    .typecase(2, statRequest)
    .typecase(3, lsRequest)
    .typecase(4, readRequest)
    .typecase(5, writeRequest)
    .typecase(6, createFileRequest)
    .typecase(7, mkdirRequest)
    .typecase(8, rmRequest)
    .typecase(9, mvRequest)
    .typecase(10, cloneFollowRequest)

  // Response codec
  val response = discriminated[Response.Response].by(byte)
    .typecase(0, rootKeyResponse)
    .typecase(1, flushResponse)
    .typecase(2, statResponse)
    .typecase(3, lsResponse)
    .typecase(4, readResponse)
    .typecase(5, writeResponse)
    .typecase(6, createFileResponse)
    .typecase(7, mkdirResponse)
    .typecase(8, rmResponse)
    .typecase(9, mvResponse)
    .typecase(10, cloneFollowResponse)

  // Some primitives

  def fixedBytes(size: Long): Codec[ByteVector] = new Codec[ByteVector] {

    object BitVectorCodec extends Codec[BitVector] {
      override def sizeBound = SizeBound.unknown
      override def encode(buffer: BitVector) = Attempt.successful(buffer)
      override def decode(buffer: BitVector) = Attempt.successful(DecodeResult(buffer, BitVector.empty))
      override def toString = "bits"
    }

    private val codec = fixedSizeBytes(size, BitVectorCodec).xmap[ByteVector](_.toByteVector, _.toBitVector)
    def sizeBound = SizeBound.exact(size * 8L)
    def encode(b: ByteVector) = codec.encode(b)
    def decode(b: BitVector) = codec.decode(b)
    override def toString = s"bytes($size)"
  }

  lazy val data32: Codec[ByteVector] = {
    uint32.flatZip(count => fixedBytes(count)).
      narrow[ByteVector]({ case (cnt, xs) =>
      if (xs.size == cnt) Attempt.successful(xs)
      else Attempt.failure(Err(s"Insufficient number of elements: decoded ${xs.size} but should have decoded $cnt"))
    }, xs => (xs.size, xs)).withToString(s"data32")
  }

}
