package follows

import cats.std.list._
import follows.ParsePath._
import org.scalacheck.Gen._
import org.scalacheck.Prop.{forAll, _}
import org.scalacheck.{Arbitrary, Gen, Properties}
import serialization.FollowSerialization
import Arbitrary.arbitrary
import FollowSerialization._

object FollowSerializationSpecification extends Properties("FollowSerialization") {
  implicit val chars: Arbitrary[Char] =
    Arbitrary(oneOf((32 to 126).map(_.toChar).filter(!FollowSerialization.specialSymbols.contains(_))))

  // segments must not be empty
  implicit val segment: Arbitrary[String] =
    Arbitrary(Gen.nonEmptyListOf(chars.arbitrary).map(_.mkString))

  // paths must not be emtpy
  implicit val path: Arbitrary[List[String]] =
    Arbitrary(Gen.nonEmptyListOf(segment.arbitrary))

  implicit val arbRemotePath = Arbitrary(for {
    root <- arbitrary[String]
    path <- path.arbitrary
  } yield RemotePath(root, path))

  implicit val arbFollow = Arbitrary(for {
    localPath <- path.arbitrary
    remotePath <- arbitrary[RemotePath]
  } yield Follow(localPath, remotePath))

  property("serialize-deserialize RemotePath") = forAll { (remote: RemotePath) =>
    deserializeRemotePath(serializeRemotePath(remote)) == Some(remote)
  }

  property("serialize-deserialize Follow") = forAll { (follow: Follow) =>
    deserializeFollow(serializeFollow(follow)) == Some(follow)
  }

}
