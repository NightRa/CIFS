package serialization

import model._
import scodec.Codec

import scodec.codecs._

import scala.collection.immutable.TreeMap

object Model {
  val hashSha256Codec: Codec[Hash] = "Hash-SHA256" | bits(256).as[Hash]
  val mutablePtrCodec: Codec[MutablePtr] = "Mutable Pointer" | hashSha256Codec.as[MutablePtr]
  val remotePathCodec: Codec[RemotePath] = "Remote Path" | (mutablePtrCodec :: listOfN(int32, utf8_32)).as[RemotePath]

  lazy val folderCodec: Codec[Folder] = "Index Folder" | listOfN(int32, utf8_32 ~ lazily(indexCodec)).
    xmap[TreeMap[String, Index]](l => TreeMap(l: _*), _.toList).
    as[Folder]

  implicit lazy val indexCodec: Codec[Index] =
    "Index" | discriminated[Index].
      by(uint4).
      typecase(0, lazily(folderCodec)).
      typecase(1, hashSha256Codec.as[HashLeaf]).
      typecase(2, remotePathCodec.as[FollowLeaf])

}
