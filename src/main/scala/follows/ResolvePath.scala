package follows

import ParsePath.parsePath

object ResolvePath {
  sealed trait FileLocation
  class Local(val _path: List[String]) extends FileLocation{
    def path: String = _path.mkString("/", "/", "")
  }
  case class Remote(remotePath: RemotePath) extends FileLocation

  object Local {
    def unapply(localPath: Local): Option[String] = Some(localPath.path)
  }

  // Jump through all follows
  def resolvePathL(path: List[String], localFollows: Follows, maxDepth: Int = 32): Option[FileLocation] = {
    if (maxDepth <= 0) {
      println(s"Hit recursion depth limit for resolve, path(deep inside) = $path")
      None
    } else
      localFollows.query(path.mkString("/", "/", "")) match {
        case None => Some(new Local(path))
        case Some(remotePath) =>
          for {
            location <- resolvePathL(remotePath.path, FollowManager.fetchFollowsIPNS(remotePath.root), maxDepth - 1)
          } yield location match {
            case l@Local(_) => Remote(RemotePath(remotePath.root, l._path))
            case remote => remote
          }
      }
  }

  def isUnderFollow(path: String, follows: Follows): Boolean = {
    follows.query(path) match {
      case None => false
      case Some(RemotePath(root, remotePath)) => true
    }
  }

  // Check if it's a follow exactly
  def isFollow(path: String, localFollows: Follows): Boolean = {
    localFollows.query(path) match {
      case None => false
      case Some(RemotePath(root, remotePath)) =>
        if(remotePath.isEmpty) true
        else false
    }
  }

  def resolvePath(path: String, localFollows: Follows): Option[FileLocation] =
    for {
      pathList <- parsePath(path)
      resolved <- resolvePathL(pathList, localFollows)
    } yield resolved
}
