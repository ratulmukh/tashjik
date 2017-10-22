package tashjik.baton

sealed trait ChildPosition

object ChildPosition {
  case object LeftChild extends ChildPosition;
  case object RightChild extends ChildPosition; 
}