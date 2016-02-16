package hash

import java.security.MessageDigest

import model.Hash
import scodec.bits.BitVector

object SHA256 {
  // TODO: It's temporary
  def hash(text: String): Hash = {
    val md = MessageDigest.getInstance("SHA-256")
    md.update(text.getBytes("UTF-8"))
    val digestArr = md.digest()
    Hash(BitVector(digestArr))
  }
}
