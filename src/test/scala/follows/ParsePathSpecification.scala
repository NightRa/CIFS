package follows

import cats.std.list._
import follows.ParsePath._
import org.scalacheck.Gen._
import org.scalacheck.Prop.{forAll, _}
import org.scalacheck.{Arbitrary, Properties}

object ParsePathSpecification extends Properties("Split") {
  implicit val chars = Arbitrary(choose(47, 50).map(_.toChar))
  property("split-intersperse inverse") = forAll { (s: List[Char], c: Char) =>
    intersperse(split(s, c).unwrap, c) == s
  }

  property("parsePath") = forAll { (s: String) =>
    val path = parsePath(s, debug = false)
    if(s.startsWith("/")) path.isDefined
    else path.isEmpty
  }

}
