namespace Wall_E.Domain;

/// <summary>Abstract base for all geometric figures.</summary>
public abstract class Figure
{  //Todas las figuras deben definir sus puntos y su intercepción con otras

   /// <summary>Returns true if the given point lies on this figure.</summary>
   public abstract bool ContainPoint(Point p);

   /// <summary>Returns a sequence of sample points belonging to this figure.</summary>
   public abstract GenericSequence<Point> FigurePoints();

   /// <summary>Computes the intersection points with another figure.</summary>
   public abstract Finite_Sequence<Point> Intersect(Figure fig);
}