package follows

import cats.data.{NonEmptyList, _}
import cats.std.list._

object ParsePath {
  // TODO: these may stack-overflow. make them tail recursive.
  def intersperse[A](s: List[List[A]], c: A): List[A] = s match {
    case Nil => Nil
    case List(x) => x
    case x :: y :: xs => x ++ (c :: intersperse(y :: xs, c))
  }

  // split(s,c).intersperse(c) == s
  def split[A](s: List[A], c: A): NonEmptyList[List[A]] = s match {
    case Nil => NonEmptyList(Nil)
    case x :: xs =>
      if (x == c) NonEmptyList(Nil, split(xs, c).unwrap)
      else split(xs, c) match {
        case OneAnd(a, bs) => NonEmptyList(x :: a, bs)
      }
  }

  def parsePath(path: String, debug: Boolean = true): Option[List[String]] = {
    if (path.contains("\\\\|:|\\*|\\?|\"|<|>|\\|")) return None // forbidden symbols in file names in windows
    val parts = split(path.toList, '/')
    if (parts.head.nonEmpty) {
      if (debug) println(s"parsePath($path): paths must start with /")
      None
    } else if (parts.tail.isEmpty) {
      if (debug) println(s"parsePath($path): path must not be empty")
      None
    } else if (parts.tail.contains(Nil)) {
      if (debug) println(s"parsePath($path): path must not contain empty segments (//a)")
      None
    } else {
      Some(parts.tail.map(_.mkString))
    }
  }
}
