package follows

import ParsePath.parsePath

object ResolvePath {
  sealed trait FileLocation
  case class Local(path: List[String]) extends FileLocation
  case class Remote(remotePath: RemotePath) extends FileLocation

  def resolvePathL(path: List[String], localFollows: Follows, maxDepth: Int = 32): Option[FileLocation] = {
    if (maxDepth <= 0) None
    else
      localFollows.query(path.mkString("/")) match {
        case None => Some(Local(path))
        case Some(remotePath) =>
          for {
            location <- resolvePathL(remotePath.path, FetchFollows.fetchFollowsIPNS(remotePath.root), maxDepth - 1)
          } yield location match {
            case Local(localPath) => Remote(RemotePath(remotePath.root, localPath))
            case remote => remote
          }
      }
  }

  def resolvePath(path: String, localFollows: Follows): Option[FileLocation] =
    for {
      pathList <- parsePath(path)
      resolvePathL(pathL, localFollows)
    }
}
