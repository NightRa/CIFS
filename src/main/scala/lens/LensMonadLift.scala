package lens

import monocle.Lens

import scala.language.higherKinds
import scalaz.Applicative
import scalaz.syntax.applicative._

object LensMonadLift {
  def liftApplicative[M[_]: Applicative, S, A](lens: Lens[S,A]): Lens[M[S], M[A]] =
    //               M[A]
    Lens[M[S], M[A]](_.map(lens.get))(
      ma => ms =>
        // map2(ma,ms)(lens.set)
        Applicative[M].apply2(ma, ms)((a,s) => lens.set(a)(s))
    )

  // Lens[S,A], M[_]: Applicative, Lens[M[S], M[A]]
  // get: S => A, set: (S,A) => S
  // get: M[S] => M[A], set: (M[S], M[A]) => M[S]
}
