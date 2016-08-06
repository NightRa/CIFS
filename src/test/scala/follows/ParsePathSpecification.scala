package follows

import cats.std.list._
import follows.ParsePath._
import org.scalacheck.Gen._
import org.scalacheck.Prop.{forAll, _}
import org.scalacheck.{Arbitrary, Properties}

object ParsePathSpecification extends Properties("ParsePath") {
  /*implicit val chars: Arbitrary[Char] =
    Arbitrary(oneOf((32 to 126).map(_.toChar).filter(!FollowSerialization.specialSymbols.tail.contains(_))))*/

  implicit val chars: Arbitrary[Char] =
    Arbitrary(oneOf((47 to 57).map(_.toChar).filter(!FollowSerialization.specialSymbols.tail.contains(_))))

  property("split-intersperse inverse") = forAll { (s: List[Char], c: Char) =>
    intersperse(split(s, '/').unwrap, '/') == s
  }

  property("parsePath(path), path must start with /") = forAll { (s: String) =>
    val path = parsePath(s, debug = false)
    if(s.startsWith("/")) path.isDefined
    else path.isEmpty
  }

}
