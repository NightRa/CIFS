package follows

import ParsePath._

case class RemotePath(root: String, path: List[String])
case class Follow(localPath: List[String], remotePath: RemotePath)

object FollowSerialization {
  val specialSymbols = List('/', '\\', ':', '*', '?', '"', '<', '>', '|')

  def serializeRemotePath(remotePath: RemotePath): String = remotePath.root + ":" + "/" + remotePath.path.mkString("/")
  def serializeFollow(follow: Follow): String = "/" + follow.localPath.mkString("/") + ">" + serializeRemotePath(follow.remotePath)
  def deserializeRemotePath(s: String): Option[RemotePath] = s.split(":") match {
    case Array(root, after) =>
      for {
        path <- parsePath(after)
      } yield RemotePath(root, path)
    case _ => None
  }

  def deserializeFollow(s: String): Option[Follow] = s.split(">") match {
    case Array(local, remote) =>
      for {
        localPath <- parsePath(local)
        remotePath <- deserializeRemotePath(remote)
      } yield Follow(localPath, remotePath)
  }
}
