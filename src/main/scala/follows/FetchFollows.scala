package follows

import FollowSerialization._
import ipfs.Queries

object FetchFollows {
  def fetchIndex(ipfsHash: String): Follows = {
    val pathOfFollows = ipfsHash + "/.follows"
    val serializedFollows = Queries.catStr(pathOfFollows)
    serializedFollows.flatMap(deserializeFollows).getOrElse(Follows(Nil))
  }
}
