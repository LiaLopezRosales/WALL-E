public abstract class Figure
{  //Todas las figuras deben definir sus puntos y su intercepción con otras
   public abstract bool ContainPoint(Point p);
   public abstract GenericSequence<Point> FigurePoints();
   public abstract Finite_Sequence<Point> Intersect(Figure fig);
}