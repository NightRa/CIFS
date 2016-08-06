package follows

case class RemotePath(root: String, path: List[String])
case class Follow(localPath: List[String], remotePath: RemotePath)
