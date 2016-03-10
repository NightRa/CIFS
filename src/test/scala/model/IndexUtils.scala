package model

import hash.SHA256.hash
import scalaz.IMap
import scalaz.std.string._

object IndexUtils {
  def folder[A,B](files: (String, Hash)*)(follows: (String, FollowLeaf)*)(folders: (String, Folder)*): Folder = Folder(IMap(files: _*), IMap(follows: _*), IMap(folders: _*))
  def leaf(value: String) = hash(value)

  def mutablePtr(value: String): MutablePtr = MutablePtr(hash(value))
  def follow(address: String, path: List[String]): FollowLeaf = FollowLeaf(RemotePath(mutablePtr(address), path))
}
