package model

import hash.SHA256._
import model.IndexCompanion.{TravelFailure, TravelFollow, TravelSubtree}
import model.IndexUtils._
import org.scalatest.{FreeSpec, Matchers}

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
    val leaf0 = leaf("0")

    val leaf2 = leaf("2")

    val leaf4 = leaf("4")

    val folder3 = folder("5" -> leaf4)()()

    val remotePath5 = RemotePath(MutablePtr(hash("public key hash")), List("6", "7"))

    val follow5 = FollowLeaf(remotePath5)

    val folder1: Folder = folder("2" -> leaf2)("5" -> follow5)("3" -> folder3)


    val root: Folder = folder("0" -> leaf0)()("1" -> folder1)


    // ---------------------- End of Tree -------------------------------------

    "Valid travel paths to subtrees " - {
      "empty path => root" in {
        IndexCompanion.travelToLocalFolder(root, Nil) shouldBe TravelSubtree(root)
      }
      "leaf path" ignore {
        // Doesn't compile anymore, paths only of folders.
        // IndexCompanion.travelToLocalFolder(root, List("0")) shouldBe TravelSubtree(leaf0)
      }
      "folder path" in {
        IndexCompanion.travelToLocalFolder(root, List("1")) shouldBe TravelSubtree(folder1)
      }
      "leaf in folder" ignore {
        // Doesn't compile anymore, paths only of folders.
        // IndexCompanion.travelToLocalFolder(root, List("1", "2")) shouldBe TravelSubtree(leaf2)
      }
    }

    "Paths to Follows" - {
      "path to follow w/o further path" in {
        IndexCompanion.travelToLocalFolder(root, List("1", "5")) shouldBe TravelFollow(List("1", "5"), Nil, remotePath5)
      }
      "path to follow w/ further path" in {
        val remoteRest = RemotePath(MutablePtr(hash("public key hash")), List("6", "7", "x"))
        IndexCompanion.travelToLocalFolder(root, List("1", "5", "x")) shouldBe TravelFollow(List("1", "5"), List("x"), remoteRest)
      }
    }

    "Invalid Paths Failures" - {
      "no child from root" in {
        IndexCompanion.travelToLocalFolder(root, List("a")) shouldBe TravelFailure(Nil, List("a"))
      }
      "path from leaf" in {
        // Doesn't compile anymore, paths only of folders.
        // IndexCompanion.travelToLocalFolder(root, List("0", "x")) shouldBe TravelFailure(List("0"), List("x"))
      }
      "no such child in folder" in {
        IndexCompanion.travelToLocalFolder(root, List("1", "3", "b", "c")) shouldBe TravelFailure(List("1", "3"), List("b", "c"))
      }
    }
  }
}
