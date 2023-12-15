public abstract class Figure
{
   public abstract bool ContainPoint(Point p);
   public abstract GenericSequence<Point> FigurePoints();
   public abstract Finite_Sequence<Point> Intersect(Figure fig);
}