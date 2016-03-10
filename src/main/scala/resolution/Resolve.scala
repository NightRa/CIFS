
package resolution

import model.IndexCompanion.{TravelFailure, TravelFollow, TravelResult, TravelSubtree}
import model._

import scala.collection.mutable
import scala.language.higherKinds
import scala.languageFeature.higherKinds
import scalaz._
import scalaz.std.string._
import scalaz.syntax.monad._
import scalaz.IMap._
import scala.language.higherKinds

private sealed class Resolve[F[_]](
                                    val mainRoot: MutablePtr,
                                    val index: Folder,
                                    val resolveAndFetch: MutablePtr => F[Folder])
                                  (implicit val F: Monad[F]) {
  // Just a manual closure for convenience for the computation only.
  // Creation isn't free - to be created only when resolving, thus private & sealed.

  val mainIndex = Resolve.toResolutionIndex(index)
  val forest = mutable.HashMap(mainRoot -> mainIndex)
  var errors: Vector[PathBreakageError] = Vector.empty

  val success: F[DList[ResolutionError]] = F.point(DList())

  // Given a pointer to a root,
  //   return it if it's already in the forest,
  //   or fetch it, and put it in the forest.
  def getRoot(ptr: MutablePtr): F[ResolutionFolder] = {
    if (forest.contains(ptr)) {
      F.point(forest(ptr))
    } else {
      F.map(resolveAndFetch(ptr)) // F[Index]
      {
        index =>
          val freshResolvedIndex = Resolve.toResolutionIndex(index)
          forest(ptr) = freshResolvedIndex // Add to the forest
          freshResolvedIndex
        // F.map(F.point(forest(ptr) = freshResolvedIndex)) (_ => freshResolvedIndex) // If I want to wrap all mutations in F.
      }
    }
  }

  // The subtree and the owner of the subtree
  // should keep the original owner while resolving the follow link.
  // Travels down the tree, resolving recursively follow links.

  // Returns either a path breakage error
  // Or the resolved index & the owner of the link or the tree???
  def resolveFollow(remote: RemotePath): F[BreakagePath \/ (ResolutionFolder, MutablePtr)] = {
    getRoot(remote.root).flatMap {
      // Gets index, puts it in the forest.
      followRoot =>
        val travel: TravelResult[ResolutionFolder] = IndexCompanion.travelToLocalFolder(followRoot, remote.path)
        travel match {
          case fail@TravelFailure(_, _) => F.point(-\/(fail.toHere)) // Invalid path => generate error in F. Here error, because it's in this tree (when we travelled here)
          case TravelSubtree(tree) => // tree is either Hash or Folder.
            F.point(\/-((tree, remote.root))) // Finished resolve successfully!

          case TravelFollow(pathToFollow, pathFromFollow, remotePath) => // zipper: to/from follow, continueFollow: RemotePath
            resolveFollow(remotePath).flatMap[BreakagePath \/ (ResolutionFolder, MutablePtr)] {
              case -\/(breakageFromFollow) => F.point(-\/(NotMyFault(pathToFollow, pathFromFollow, breakageFromFollow)))
              case \/-(success) => F.point(\/-(success))
            }
        }
    }
  }

  // resolveFollow for all the follows in the subtree.
  private def resolveSubtree(index: ResolutionFolder, owner: MutablePtr): F[DList[ResolutionError]] = {
    // Don't visit a node twice
    if (index.seen) {
      success
    } else {
      // Unseen
      // Mark as seen, visited
      index.seen = true
      index.follows.map { follow =>
        val resolved: F[BreakagePath \/ (ResolutionFolder, MutablePtr)] =
          resolveFollow(follow.remotePath)

        resolved.flatMap[DList[ResolutionError]] {
          case -\/(breakagePath) =>
            F.point(
              DList(PathBreakageError(breakagePath, owner))
            )
          case \/-((remoteIndex, remoteRoot)) =>
            resolveSubtree(remoteIndex, remoteRoot)
        }
      }
      index.folders.map(index => resolveSubtree(index, owner)).
        fold(success)(
          (x, v1, v2) => F.apply2(v1, v2)(_ ++ _))
      // Applicative[F], Monoid[A] => Monoid[F[A]]

    }
  }

}

object Resolve {

  private def toResolutionIndex(index: Folder): ResolutionFolder =
    ResolutionFolder(index.files, index.follows.map(toResolutionFollow), index.folders.map(toResolutionIndex), seen = false)

  private def toResolutionFollow(follow: FollowLeaf): ResolutionFollow =
    ResolutionFollow(follow.remotePath, seen = false, resolutionResult = None)

  private def toFollowLeaf(follow: ResolutionFollow): FollowLeaf =
    FollowLeaf(follow.remotePath)

  private def toIndex(resIndex: ResolutionFolder): Folder =
    Folder(resIndex.files, resIndex.follows.map(toFollowLeaf), resIndex.folders.map(toIndex))

  def resolve[F[_] : Monad](resolveAndFetchFunc: MutablePtr => F[Folder], rootIndex: RootIndex): F[ValidationNel[ResolutionError, Map[MutablePtr, Folder]]] = {
    val resolveContext = new Resolve[F](rootIndex.ptr, rootIndex.root, resolveAndFetchFunc)

    val subtree: F[DList[ResolutionError]] =
      resolveContext.resolveSubtree(resolveContext.mainIndex, resolveContext.mainRoot)

    def preprareResult(in: DList[ResolutionError]): ValidationNel[ResolutionError, Map[MutablePtr, Folder]] = {
      val errors = in.toList
      errors match {
        case Nil => Validation.success(resolveContext.forest.mapValues(toIndex).toMap)
        case err1 :: errRest => Validation.failure(NonEmptyList(err1, errRest: _*))
      }
    }

    subtree.map(preprareResult)
  }
}
