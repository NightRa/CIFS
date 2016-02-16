package model

import hash.SHA256._
import model.Index.{TravelFailure, TravelFollow, TravelSubtree}
import org.scalatest.{Matchers, FreeSpec}

import scala.collection.immutable.TreeMap

class TravelTest extends FreeSpec with Matchers {
  "travelLocal" - {
    /*
     * root-Folder:
     *  - Leaf: 0
     *  - Folder: 1
     *    - Leaf: 2
     *    - Folder: 3
     *      - Leaf: 4
     *    - Follow: 5, path: /6/7
    */

    // ---------------------- The Tree ----------------------------------------
    val leaf0: Index = HashLeaf(hash("0"))

    val leaf2: Index = HashLeaf(hash("2"))

    val leaf4: Index = HashLeaf(hash("4"))

    val folder3: Index = Folder(TreeMap(
      "5" -> leaf4
    ))

    val remotePath5 = RemotePath(MutablePtr(hash("public key hash")),
      List("6", "7"))

    val follow5: Index = FollowLeaf(remotePath5)

    val folder1: Index = Folder(TreeMap(
      "2" -> leaf2,
      "3" -> folder3,
      "5" -> follow5
    ))

    val root: Index = Folder(TreeMap(
      "0" -> leaf0,
      "1" -> folder1
    ))

    // ---------------------- End of Tree -------------------------------------

    "Valid travel paths to subtrees " - {
      "empty path => root" in {
        Index.travelLocal(root, Nil) shouldBe TravelSubtree(root)
      }
      "leaf path" in {
        Index.travelLocal(root, List("0")) shouldBe TravelSubtree(leaf0)
      }
      "folder path" in {
        Index.travelLocal(root, List("1")) shouldBe TravelSubtree(folder1)
      }
      "leaf in folder" in {
        Index.travelLocal(root, List("1", "2")) shouldBe TravelSubtree(leaf2)
      }
    }

    "Paths to Follows" - {
      "path to follow w/o further path" in {
        Index.travelLocal(root, List("1", "5")) shouldBe TravelFollow(List("1", "5"), Nil, remotePath5)
      }
      "path to follow w/ further path" in {
        val remoteRest = RemotePath(MutablePtr(hash("public key hash")), List("6", "7", "x"))
        Index.travelLocal(root, List("1", "5", "x")) shouldBe TravelFollow(List("1", "5"), List("x"), remoteRest)
      }
    }

    "Invalid Paths Failures" - {
      "no child from root" in {
        Index.travelLocal(root, List("a")) shouldBe TravelFailure(Nil, List("a"))
      }
      "path from leaf" in {
        Index.travelLocal(root, List("0", "x")) shouldBe TravelFailure(List("0"), List("x"))
      }
      "no such child in folder" in {
        Index.travelLocal(root, List("1", "3", "b", "c")) shouldBe TravelFailure(List("1", "3"), List("b", "c"))
      }
    }
  }
}
