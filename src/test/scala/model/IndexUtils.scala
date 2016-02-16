package model

import scala.collection.immutable.TreeMap
import hash.SHA256.hash

object IndexUtils {
  def folder[A,B](elems: (String, Index)*): Index = Folder(TreeMap(elems: _*))
  def leaf(value: String): Index = HashLeaf(hash(value))

  def mutablePtr(value: String): MutablePtr = MutablePtr(hash(value))
  def follow(address: String, path: List[String]): Index = FollowLeaf(RemotePath(mutablePtr(address), path))
}
