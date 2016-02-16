package resolution

import model.IndexUtils._
import model._
import org.scalatest.{FreeSpec, Matchers}

import scalaz.Id._
import scalaz.{Validation, ValidationNel}

class ResolutionTest extends FreeSpec with Matchers {
  // TODO: More tests: Failure cases and more regular cases.

  def success(elems: (MutablePtr, Index)*): ValidationNel[ResolutionError, Map[MutablePtr, Index]] =
    Validation.success(Map(elems: _*))

  def resolveFail() = fail("Resolve tried to fetch an unexpected root")

  "Resolution" - {
    "Tree with no follows should resolve to itself" in {
      val root = folder(
        "a" -> leaf("a"),
        "b" -> folder(
          "c" -> leaf("c")
        )
      )

      val mainPtr = mutablePtr("main root")
      val rootIndex = RootIndex(root, mainPtr)
      Resolve.resolve[Id](_ => fail("Resolve tried to fetch remote root"), rootIndex) shouldEqual
        success(mainPtr -> root)
    }

    "Follow a single other root" in {
      val root = folder(
        "friend" -> follow("friend", Nil)
      )
      val friendRoot = folder(
        "x" -> leaf("x")
      )

      val rootPtr = mutablePtr("main root")
      val friendPtr = mutablePtr("friend")
      val rootIndex = RootIndex(root, rootPtr)
      Resolve.resolve[Id](
        ptr =>
          if (ptr == friendPtr) friendRoot
          else resolveFail(),
        rootIndex
      ) shouldEqual
        success(
          rootPtr -> root,
          friendPtr -> friendRoot
        )

    }

    "Follow inside a follow subtree" in {
      val (rootPtr, root) = (mutablePtr("root"), folder(
        "friend1" -> follow("friend1", Nil)
      ))

      val (friend1Ptr, friend1Root) = (mutablePtr("friend1"), folder(
        "friend2" -> follow("friend2", Nil)
      ))

      val (friend2Ptr, friend2Root) = (mutablePtr("friend2"), folder(
        "x" -> leaf("x")
      ))

      Resolve.resolve[Id](
        ptr =>
          if (ptr == friend1Ptr) friend1Root
          else if (ptr == friend2Ptr) friend2Root
          else resolveFail(),
        RootIndex(root, rootPtr)
      ) shouldEqual
        success(
          rootPtr -> root,
          friend1Ptr -> friend1Root,
          friend2Ptr -> friend2Root
        )
    }

    "Follow inside a different follow subtree" in {
      val (rootPtr, root) = (mutablePtr("root"), folder(
        "friend1" -> follow("friend1", List("folder"))
      ))

      val (friend1Ptr, friend1Root) = (mutablePtr("friend1"), folder(
        "friend2" -> follow("friend2", Nil),
        "folder" -> folder(
          "y" -> leaf("y")
        )
      ))

      val (friend2Ptr, friend2Root) = (mutablePtr("friend2"), folder(
        "x" -> leaf("x")
      ))

      Resolve.resolve[Id](
        ptr =>
          if (ptr == friend1Ptr) friend1Root
          else if (ptr == friend2Ptr) friend2Root
          else resolveFail(),
        RootIndex(root, rootPtr)
      ) shouldEqual
        success(
          rootPtr -> root,
          friend1Ptr -> friend1Root
        )
    }

    "Follow while opening path" in {
      val (rootPtr, root) = (mutablePtr("root"), folder(
        "friend1" -> follow("friend1", List("friend2", "folder"))
      ))

      val (friend1Ptr, friend1Root) = (mutablePtr("friend1"), folder(
        "friend2" -> follow("friend2", Nil)
      ))

      val (friend2Ptr, friend2Root) = (mutablePtr("friend2"), folder(
        "folder" -> folder(
          "y" -> leaf("y")
        )
      ))

      Resolve.resolve[Id](
        ptr =>
          if (ptr == friend1Ptr) friend1Root
          else if (ptr == friend2Ptr) friend2Root
          else resolveFail(),
        RootIndex(root, rootPtr)
      ) shouldEqual
        success(
          rootPtr -> root,
          friend1Ptr -> friend1Root,
          friend2Ptr -> friend2Root
        )
    }
  }
}
