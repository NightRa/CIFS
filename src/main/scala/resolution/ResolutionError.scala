package resolution

import model.{Hash, MutablePtr}
import scodec.bits.BitVector

// The whole path error stack: where from and why.
sealed trait BreakagePath {
  def path: List[String]
}

case class Here(validPrefix: List[String], remainingPath: List[String]) extends BreakagePath {
  def path = validPrefix ++ remainingPath
}

case class NotMyFault(pathToFollow: List[String], pathFromFollow: List[String], errorFromFollow: BreakagePath) extends BreakagePath {
  def path = pathToFollow ++ pathFromFollow
}


sealed trait ResolutionResult
case object ResolutionSuccess extends ResolutionResult

// Errors of the whole resolution process
sealed trait ResolutionError extends ResolutionResult

// System errors
case class ReferenceResolutionError(mutablePtr: MutablePtr) extends ResolutionError
case class FetchError(hash: Hash) extends ResolutionError
// May be used outside for non resolution errors.
case class IndexDeserializationError(mutablePtr: MutablePtr, hash: Hash, data: BitVector) extends ResolutionError
// User errors
case class PathBreakageError(breakagePath: BreakagePath, owner: MutablePtr) extends ResolutionError

