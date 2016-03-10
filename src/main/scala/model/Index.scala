package model

import datatypes.{Zipper, ZipperMore}
import monocle.Lens
import monocle.macros.{GenIso, Lenses}
import monocle.std.imap._
import resolution.{BreakagePath, Here}

import scalaz.IMap
import Folder._
import scalaz.std.string._

// --------------------------------------------------------------------

// Index typeclass
abstract class IndexCompanion[T] {
  def file(fileName: String): Lens[T, Option[Hash]]

  def folder(folderName: String): Lens[T, Option[T]]

  def follow(followName: String): Lens[T, Option[FollowLeaf]]
}

// Just for nice syntax
abstract class Index[T <: Index[T]](val companion: IndexCompanion[T]) {
  self: T =>

  def getFile(fileName: String): Option[Hash] =
    companion.file(fileName).get(this)

  def setFile(fileName: String, value: Option[Hash]): T =
    companion.file(fileName).set(value)(this)

  def getFolder(folderName: String): Option[T] =
    companion.folder(folderName).get(this)

  def setFolder(folderName: String, value: Option[T]): T =
    companion.folder(folderName).set(value)(this)

  def getFollow(followName: String): Option[FollowLeaf] =
    companion.follow(followName).get(this)

  def setFollow(followName: String, value: Option[FollowLeaf]): T =
    companion.follow(followName).set(value)(this)

}

case class FollowLeaf(remotePath: RemotePath)

object FollowLeaf {
  def iso = GenIso[FollowLeaf, RemotePath]
}

/**
  * Invariant: A follow may not be of the same name of another file or a folder in the same directory.
  * Files and folders can have colliding names.
  */
@Lenses("_")
case class Folder(files: IMap[String, Hash], follows: IMap[String, FollowLeaf], folders: IMap[String, Folder]) extends Index[Folder](Folder) {

  // Creates the folders on the path too.
  // overrides if exists
  def writeFile(fileName: String, path: List[String], hash: Hash): Folder = path match {
    case Nil =>
      this.setFile(fileName, Some(hash))

    case childFolderName :: restPath =>
      // Folder doesn't exist, create it.
      val childFolder = folders.lookup(childFolderName).getOrElse(emptyFolder)
      val insertedFolder = childFolder.writeFile(fileName, restPath, hash)

      this.setFolder(childFolderName, Some(insertedFolder))

  }
}

object Folder extends IndexCompanion[Folder] {
  val emptyFolder: Folder = Folder(IMap.empty, IMap.empty, IMap.empty)

  def file(fileName: String): Lens[Folder, Option[Hash]] =
    _files ^|-> atIMap[String, Hash].at(fileName)

  def folder(folderName: String): Lens[Folder, Option[Folder]] =
    _folders ^|-> atIMap[String, Folder].at(folderName)

  def follow(followName: String): Lens[Folder, Option[FollowLeaf]] =
    _follows ^|-> atIMap[String, FollowLeaf].at(followName)

}

case class RootIndex(root: Folder, ptr: MutablePtr)

// --------------------------------------------------------------------

object IndexCompanion {
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
  sealed abstract class TravelResult[T <: Index[T]]
  case class TravelFailure[T <: Index[T]](validPrefixPath: List[String], remainingPath: List[String]) extends TravelResult[T] {
    def toHere: BreakagePath = Here(validPrefixPath, remainingPath)
  }
  case class TravelSubtree[T <: Index[T]](tree: T) extends TravelResult[T]
  case class TravelFollow[T <: Index[T]](pathToFollow: List[String], pathFromFollow: List[String], remotePath: RemotePath) extends TravelResult[T]

  // Failure / Subtree / Follow leaf + what remains
  def travelToLocalFolder[T <: Index[T]](index: T, folderPath: List[String]): TravelResult[T] = {
    // folder1/folder2/x
    def go(index: T, zipper: Zipper[String]): TravelResult[T] = zipper match {
      // When pattern matching, the head is pushed automatically to the left in tail.
      case ZipperMore(folderName, rest) =>
        index.getFolder(folderName) match {
          case Some(folder) => go(folder, rest)
          case None =>
            // no such folder child in the folder
            index.getFollow(folderName) match {
              case Some(FollowLeaf(remotePath)) => TravelFollow(rest.getLeft, rest.right, RemotePath(remotePath.root, remotePath.path ++ rest.right))
              case None => TravelFailure(zipper.getLeft, zipper.right)
            }
        }
      case _ => TravelSubtree(index)

    }

    go(index, Zipper.fromList(folderPath))
  }

}
