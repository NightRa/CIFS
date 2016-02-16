package model

import datatypes.{Zipper, ZipperMore}
import resolution.{BreakagePath, Here}

import scala.collection.immutable.SortedMap

trait TreeExt[T <: TreeExt[T]] {
  def cataExt[B](caseHash: Hash => B, caseFollow: RemotePath => B, caseFolder: SortedMap[String, T] => B, caseElse: () => B): B
}

trait Tree[T <: Tree[T]] extends TreeExt[T] {
  def cata[B](caseHash: Hash => B, caseFollow: RemotePath => B, caseFolder: SortedMap[String, T] => B): B

  def cataExt[B](caseHash: Hash => B, caseFollow: RemotePath => B, caseFolder: SortedMap[String, T] => B, caseElse: () => B): B =
    cata(caseHash, caseFollow, caseFolder)
}

// --------------------------------------------------------------------

sealed trait Index extends Tree[Index]

case class HashLeaf(hash: Hash) extends Index {
  def cata[B](caseHash: Hash => B, caseFollow: RemotePath => B, caseFolder: SortedMap[String, Index] => B): B =
    caseHash(hash)
}

case class FollowLeaf(remotePath: RemotePath) extends Index {
  def cata[B](caseHash: Hash => B, caseFollow: RemotePath => B, caseFolder: SortedMap[String, Index] => B): B =
    caseFollow(remotePath)
}

case class Folder(children: SortedMap[String, Index]) extends Index {
  def cata[B](caseHash: Hash => B, caseFollow: RemotePath => B, caseFolder: SortedMap[String, Index] => B): B =
    caseFolder(children)
}

case class RootIndex(index: Index, ptr: MutablePtr)

// --------------------------------------------------------------------

object Index {
  // Go down the given path in the given tree
  // Until:
  // 1. Invalid Path: non empty path at a leaf -> Zipper, None
  // 2. Invalid Path: no such child -> Zipper, None
  // 3. Empty path: Zipper, Some(Left(subtree)) - Legitimate Real tree.
  // 4. Follow pointer: Zipper, Some(Right(ptr, ptr-path ++ remaining-path))

  // Remote Ptr Leaf: A/b => B/x
  // If queried: A/b/y => B/x/y
  // Then ptr = B, ptr-path = x, remaining-path = y
  // TODO: Change the pair of lists!!! Make a datatype for the error.

  // Invariant: If we return a tree, then it's either a Hash or a Folder! Can't be a follow.
  // Follow (leaf or non leaf) => RemotePath
  // Hash Leaf                 => Hash
  // Folder Leaf               => Folder

  // TODO: Redundant list construction & reverse in the good case when the path is valid.
  // Zipper: Returns path until stop and path from stop
  // If got to a leaf, path from stop = []
  // In the case of a follow, path to follow & path from follow.

  // TODO: Deal with infinite path expansion
  sealed abstract class TravelResult[T <: Tree[T]]
  case class TravelFailure[T <: Tree[T]](validPrefixPath: List[String], remainingPath: List[String]) extends TravelResult[T] {
    def toHere: BreakagePath = Here(validPrefixPath, remainingPath)
  }
  case class TravelSubtree[T <: Tree[T]](tree: T) extends TravelResult[T]
  case class TravelFollow[T <: Tree[T]](pathToFollow: List[String], pathFromFollow: List[String], remotePath: RemotePath) extends TravelResult[T]

  def travelLocal[T <: Tree[T]](index: T, path: List[String]): TravelResult[T] = {

    def go(index: T, zipper: Zipper[String]): TravelResult[T] = {
      index.cata(
        caseHash = _ => zipper match {
          case ZipperMore(x, xs) => TravelFailure(zipper.getLeft, zipper.right) // Leaf, non empty path => Invalid Path
          case _ => TravelSubtree(index) // Leaf, empty path => Valid path
        },

        caseFollow = remotePath =>
          TravelFollow(zipper.getLeft, zipper.right, RemotePath(remotePath.root, remotePath.path ++ zipper.right)),

        caseFolder = children => zipper match {
          case ZipperMore(pathHead, tail) => children.get(pathHead) match {
            // When pattern matching, the head is pushed automatically to the left in tail.
            case None => TravelFailure(zipper.getLeft, zipper.right) // no such child in the folder
            case Some(child) => go(child, tail) // Non tail recursive
          }
          case _ => TravelSubtree(index) // Folder, empty path => Valid path, point to the folder subtree.
        }

      )

    }

    go(index, Zipper.fromList(path))
  }
}
