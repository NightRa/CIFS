package follows

import follows.FollowSerialization._
import org.scalacheck.Arbitrary.arbitrary
import org.scalacheck.Gen._
import org.scalacheck.Prop.{forAll, _}
import org.scalacheck.{Arbitrary, Gen, Properties}

object FollowSerializationSpecification extends Properties("FollowSerialization") {
/*
  implicit val chars: Arbitrary[Char] =
    Arbitrary(oneOf((32 to 126).map(_.toChar).filter(!FollowSerialization.specialSymbols.contains(_))))
*/

  implicit val chars: Arbitrary[Char] =
    Arbitrary(oneOf((47 to 57).map(_.toChar).filter(!FollowSerialization.specialSymbols.contains(_))))


  // segments must not be empty
  implicit val segment: Arbitrary[String] =
    Arbitrary(Gen.nonEmptyListOf(chars.arbitrary).map(_.mkString))

  // paths must not be emtpy
  implicit val path: Arbitrary[List[String]] =
    Arbitrary(Gen.nonEmptyListOf(segment.arbitrary).map(_.filter(_.nonEmpty)))

  implicit val arbRemotePath: Arbitrary[RemotePath] = Arbitrary(for {
    root <- arbitrary[String]
    path <- arbitrary[List[String]]
  } yield RemotePath(root, path))

  implicit val arbFollow: Arbitrary[Follow] = Arbitrary(for {
    localPath <- path.arbitrary
    remotePath <- arbitrary[RemotePath]
  } yield Follow(localPath.mkString("/", "/", ""), remotePath))

  implicit val arbFollows: Arbitrary[Follows] = Arbitrary(resize(10, Gen.listOf(arbFollow.arbitrary).map(Follows)))

  property("serialize-deserialize RemotePath") = forAll { (remote: RemotePath) =>
    deserializeRemotePath(serializeRemotePath(remote)) == Some(remote)
  }

  property("serialize-deserialize Follow") = forAll { (follow: Follow) =>
    deserializeFollow(serializeFollow(follow)) == Some(follow)
  }

  property("serialize-deserialize Follows") = forAll { (follows: Follows) =>
    deserializeFollows(serializeFollows(follows)) == Some(follows)
  }
}
