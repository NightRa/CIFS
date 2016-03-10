package serialization

import model._
import scodec.Codec
import scodec.codecs._

import scalaz.{IMap, Order}
import scalaz.std.string._

object Model {
  val hashSha256Codec: Codec[Hash] = "Hash-SHA256" | bits(256).as[Hash]
  val mutablePtrCodec: Codec[MutablePtr] = "Mutable Pointer" | hashSha256Codec.as[MutablePtr]
  val remotePathCodec: Codec[RemotePath] = "Remote Path" | (mutablePtrCodec :: listOfN(int32, utf8_32)).as[RemotePath]

  def imapCodec[A: Order, B](codecA: Codec[A], codecB: Codec[B]): Codec[IMap[A, B]] = listOfN(int32, codecA ~ codecB).
    xmap[IMap[A, B]](l => IMap(l: _*), _.toList)

  implicit lazy val folderCodec: Codec[Folder] = "Index Folder" |
    (imapCodec(utf8_32, hashSha256Codec) ~
      imapCodec(utf8_32, remotePathCodec.as[FollowLeaf]) ~
      imapCodec(utf8_32, lazily(folderCodec)))
      .xmap[Folder]({
      case (((files, follows), folders)) => Folder(files, follows, folders)
    }, folder => Folder.unapply(folder).get match {
      case (files, follows, folders) => ((files, follows), folders)
    })
  // Todo: understand why .as[Folder] didn't work as before.
}
