package follows

import cats.std.list._
import cats.std.option._
import cats.syntax.traverse._
import com.googlecode.concurrenttrees.radix.ConcurrentRadixTree
import com.googlecode.concurrenttrees.radix.node.concrete.SmartArrayBasedNodeFactory
import follows.FollowSerialization._
import follows.ParsePath._
import prefixSubstitution.PatternTrie

case class RemotePath(root: String /*IPNS address*/, path: List[String]) {
  override def toString = serializeRemotePath(this)
}
case class Follow(localPath: List[String], remotePath: RemotePath) {
  override def toString = serializeFollow(this)
}
case class Follows(follows: List[Follow]) {
  override def toString = serializeFollows(this)

  lazy val trie: ConcurrentRadixTree[RemotePath] = {
    val rules = new ConcurrentRadixTree[RemotePath](new SmartArrayBasedNodeFactory)
    follows.foreach(follow => rules.put(follow.localPath.mkString("/"), follow.remotePath))
    rules
  }

  // given a path (may be local or non local), returns a remote path (starting with a root hash), if the path hits a follow
  def query(path: String): Option[RemotePath] = {
    for {
      rule <- PatternTrie.getPrefix(trie, path)
      remaining <- parsePath(path.drop(rule.pattern.length))
    } yield RemotePath(rule.replacement.root, rule.replacement.path ++ remaining)
  }
}

object FollowSerialization {
  val specialSymbols = List('/', '\\', ':', '*', '?', '"', '<', '>', '|')

  def serializeRemotePath(remotePath: RemotePath): String = remotePath.root + "/" + remotePath.path.mkString("/")
  def serializeFollow(follow: Follow): String = "/" + follow.localPath.mkString("/") + ">" + serializeRemotePath(follow.remotePath)
  def serializeFollows(follows: Follows): String = follows.follows.map(serializeFollow).mkString("\n")

  def deserializeRemotePath(s: String): Option[RemotePath] = split(s.toList, '/').unwrap.map(_.mkString) match {
    case root :: after =>
      for {
        path <- parsePath(after.mkString("/", "/", ""))
      } yield RemotePath(root, path)
    case _ => None
  }

  def deserializeFollow(s: String): Option[Follow] = s.split(">") match {
    case Array(local, remote) =>
      for {
        localPath <- parsePath(local)
        remotePath <- deserializeRemotePath(remote)
      } yield Follow(localPath, remotePath)
    case _ => None
  }

  def deserializeFollows(s: String): Option[Follows] = split(s.toList, '\n').unwrap.map(_.mkString).filter(_.nonEmpty).traverse(deserializeFollow).map(Follows)
}
