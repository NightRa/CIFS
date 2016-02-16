package resolution

import model.Index.{TravelFailure, TravelFollow, TravelResult, TravelSubtree}
import model._

import scala.collection.mutable
import scala.language.higherKinds
import scala.languageFeature.higherKinds
import scalaz._
import scalaz.std.map._
import scalaz.std.string._
import scalaz.syntax.monad._

private sealed class Resolve[F[_]](
                                    val mainRoot: MutablePtr,
                                    val index: Index,
                                    val resolveAndFetch: MutablePtr => F[Index])
                                  (implicit val F: Monad[F]) {
  // Just a manual closure for convenience for the computation only.
  // Creation isn't free - to be created only when resolving, thus private & sealed.

  val mainIndex = Resolve.toResolutionIndex(index)
  val forest = mutable.HashMap(mainRoot -> mainIndex)
  var errors: Vector[PathBreakageError] = Vector.empty

  def compose[G1[_], G2[_]](G1: Applicative[G1], G2: Applicative[G2]): Applicative[({type H[A] = G1[G2[A]]})#H] = G1.compose(G2)

  type ResolutionValidation[A] = ValidationNel[ResolutionError, A]
  type FValidation[A] = F[ResolutionValidation[A]]
  val FValidation: Applicative[FValidation] =
    compose[F, ResolutionValidation](
      F, Validation.ValidationApplicative[NonEmptyList[ResolutionError]]
    )


  type Res = FValidation[Unit]

  val success: Res = F.point(Validation.success(()))

  // Given a pointer to a root,
  //   return it if it's already in the forest,
  //   or fetch it, and put it in the forest.
  def getRoot(ptr: MutablePtr): F[ResolutionIndex] = {
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
  def resolveFollow(remote: RemotePath): F[BreakagePath \/ (ResolutionIndex, MutablePtr)] = {
    getRoot(remote.root).flatMap {
      // Gets index, puts it in the forest.
      followRoot =>
        val travel: TravelResult[ResolutionIndex] = Index.travelLocal(followRoot, remote.path)
        travel match {
          case fail@TravelFailure(_,_) => F.point(-\/(fail.toHere)) // Invalid path => generate error in F. Here error, because it's in this tree (when we travelled here)
          case TravelSubtree(tree) => // tree is either Hash or Folder.
            F.point(\/-((tree, remote.root))) // Finished resolve successfully!

          case TravelFollow(pathToFollow, pathFromFollow, remotePath) => // zipper: to/from follow, continueFollow: RemotePath
            resolveFollow(remotePath).flatMap[BreakagePath \/ (ResolutionIndex, MutablePtr)] {
              case -\/(breakageFromFollow) => F.point(-\/(NotMyFault(pathToFollow, pathFromFollow, breakageFromFollow)))
              case \/-(success) => F.point(\/-(success))
            }
        }
    }
  }


  /*
  * resolveSubtree(index, owner): F[ValidationNel[ResolutionError, Unit]] = {
  *   if(index.seen){
  *     success
  *   } else {
  *     index.seen = true
  *     index match {
  *       case Hash(_) => success
  *       case Folder
  **/

  private def resolveSubtree(index: ResolutionIndex, owner: MutablePtr): Res = {
    // Don't visit a node twice
    if (index.seen) {
      success
    } else {
      // Unseen
      // Mark as seen, visited
      index.seen = true
      index match {
        case ResolutionHash(hash, _) => success
        case ResolutionFolder(children, _) =>
          mapInstance.traverse_(children)(index => resolveSubtree(index, owner))(FValidation)
        case follow@ResolutionFollow(remote, _, _) =>
          val resolved: F[BreakagePath \/ (ResolutionIndex, MutablePtr)] =
            resolveFollow(remote)

          resolved.flatMap[ValidationNel[ResolutionError, Unit]] {
            case -\/(breakagePath) =>
              F.point(Validation.failureNel(
                PathBreakageError(breakagePath, owner))
              )
            case \/-((remoteIndex, remoteRoot)) =>
              resolveSubtree(remoteIndex, remoteRoot)
          }
      }
    }
  }
}

object Resolve {

  private def toResolutionIndex(index: Index): ResolutionIndex = index match {
    case HashLeaf(hash) => ResolutionHash(hash, seen = false)
    case FollowLeaf(remotePtr) => ResolutionFollow(remotePtr, seen = false, resolutionResult = None)
    case Folder(children) => ResolutionFolder(children.mapValues(toResolutionIndex), seen = false)
  }

  private def toIndex(resIndex: ResolutionIndex): Index = resIndex match {
    case ResolutionHash(hash, _) => HashLeaf(hash)
    case ResolutionFollow(remotePtr, _, _) => FollowLeaf(remotePtr)
    case ResolutionFolder(children, _) => Folder(children.mapValues(toIndex))
  }

  def resolve[F[_] : Monad](resolveAndFetchFunc: MutablePtr => F[Index], rootIndex: RootIndex): F[ValidationNel[ResolutionError, Map[MutablePtr, Index]]] = {
    val resolveContext = new Resolve[F](rootIndex.ptr, rootIndex.index, resolveAndFetchFunc)

    val subtree: F[ValidationNel[ResolutionError, Unit]] =
      resolveContext.resolveSubtree(resolveContext.mainIndex, resolveContext.mainRoot)

    //      F    Validation
    subtree.map(_.map(_ => resolveContext.forest.mapValues(toIndex).toMap))
  }
}
