package model

import scala.collection.immutable.TreeMap

sealed trait Index
case class HashLeaf(hash: Hash) extends Index
case class FollowLeaf(follow: RemotePath) extends Index
case class Folder(children: TreeMap[String, Index]) extends Index
