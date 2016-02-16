package datatypes

case class Zipper[+A](backPath: List[A], right: List[A]) {
  // more verbose to emphasize it's not free.
  def getLeft: List[A] = backPath.reverse
}

// When pattern matching, the head is pushed automatically to the left in tail.
object ZipperMore {
  def unapply[A](zipper: Zipper[A]): Option[(A, Zipper[A])] = zipper.right match {
    case Nil => None
    case x :: xs => Some((x, Zipper(x :: zipper.backPath, xs)))
  }
}

object Zipper {
  def fromList[A](list: List[A]): Zipper[A] = Zipper(Nil, list)
}
