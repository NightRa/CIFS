package resolution

import model.{Hash, MutablePtr, RemotePath, Tree}

import scala.collection.immutable.{HashMap, SortedMap}

// Mutable structures for the process of resolution
// Needed to avoid cycles (seen bit in a graph)

// Property: seen = true => children (including under the followed tree).seen = true
sealed trait ResolutionIndex extends Tree[ResolutionIndex] {
  var seen: Boolean
}

case class ResolutionHash(hash: Hash, var seen: Boolean) extends ResolutionIndex {
  def cata[B](caseHash: Hash => B, caseFollow: RemotePath => B, caseFolder: SortedMap[String, ResolutionIndex] => B): B =
    caseHash(hash)
}

// Invariant: seen = true => resolutionResult != None.
case class ResolutionFollow(ptr: RemotePath, var seen: Boolean, var resolutionResult: Option[ResolutionResult]) extends ResolutionIndex {
  def cata[B](caseHash: Hash => B, caseFollow: RemotePath => B, caseFolder: SortedMap[String, ResolutionIndex] => B): B =
    caseFollow(ptr)
}

case class ResolutionFolder(children: SortedMap[String, ResolutionIndex], var seen: Boolean) extends ResolutionIndex {
  def cata[B](caseHash: Hash => B, caseFollow: RemotePath => B, caseFolder: SortedMap[String, ResolutionIndex] => B): B =
    caseFolder(children)
}

case class ResolvedForest(mainRoot: MutablePtr, mainIndex: ResolutionIndex, forest: HashMap[MutablePtr, ResolutionIndex])

