package model

import scodec.bits.BitVector

case class Hash(hash: BitVector) extends AnyVal
case class MutablePtr(mutablePtr: Hash)
case class RemotePath(root: MutablePtr, path: List[String])
