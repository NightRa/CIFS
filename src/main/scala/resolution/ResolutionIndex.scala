package resolution

import model._
import monocle.Lens
import monocle.macros.Lenses
import monocle.std.imap._
import lens.LensMonadLift._
import resolution.ResolutionFollow._followLeaf

import scalaz._
import scalaz.std.string._
import scalaz.std.option._
// Mutable structures for the process of resolution
// Needed to avoid cycles (seen bit in a graph)

// Property: seen = true => children (including under the followed tree).seen = true

// Invariant: seen = true => resolutionResult != None.
@Lenses("_")
case class ResolutionFollow(remotePath: RemotePath, var seen: Boolean, var resolutionResult: Option[ResolutionResult])

object ResolutionFollow {
  def _followLeaf: Lens[ResolutionFollow, FollowLeaf] = _remotePath ^<-> FollowLeaf.iso.reverse
}

@Lenses("_")
case class ResolutionFolder(files: IMap[String, Hash], follows: IMap[String, ResolutionFollow], folders: IMap[String, ResolutionFolder], var seen: Boolean) extends Index[ResolutionFolder](ResolutionFolder)

object ResolutionFolder extends IndexCompanion[ResolutionFolder]{
  def file(fileName: String): Lens[ResolutionFolder, Option[Hash]] =
    _files ^|-> atIMap[String, Hash].at(fileName)

  def folder(folderName: String): Lens[ResolutionFolder, Option[ResolutionFolder]] =
    _folders ^|-> atIMap[String, ResolutionFolder].at(folderName)

  def follow(followName: String): Lens[ResolutionFolder, Option[FollowLeaf]] =
    _follows ^|-> atIMap[String, ResolutionFollow].at(followName) ^|-> liftApplicative[Option, ResolutionFollow, FollowLeaf](_followLeaf)
}

// case class ResolvedForest(mainRoot: MutablePtr, mainIndex: ResolutionIndex, forest: HashMap[MutablePtr, ResolutionIndex])

