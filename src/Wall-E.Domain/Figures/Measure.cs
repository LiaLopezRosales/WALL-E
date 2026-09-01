namespace Wall_E.Domain;

/// <summary>Encapsulates the distance between two points and arithmetic over it.</summary>
public class Measure
{
    //Medida entre dos puntos.
    /// <summary>The first endpoint of the measured segment.</summary>
    public Point point1 { get; set; }
    /// <summary>The second endpoint of the measured segment.</summary>
    public Point point2 { get; set; }
    /// <summary>The computed distance between the endpoints.</summary>
    public double Value { get; protected set; }

    /// <summary>Creates a measure from the distance between two points.</summary>
    public Measure(Point p1, Point p2)
    {
        point1 = p1;
        point2 = p2;
        Value = Math.Sqrt(Math.Pow((point2.x - point1.x), 2) + Math.Pow((point2.y - point1.y), 2));
    }
    //Se crea una medida que es la suma de los valores de dos medidas.
    /// <summary>Returns a measure whose value is the sum of this measure and another.</summary>
    public Measure Sum(Measure m)
    {
        Point p2 = m.point1;
        double distance = m.Value + this.Value;
        Point newp2 = this.PointAtDistance(p2, distance);
        return new Measure(point1, newp2);
    }
    //Medida que es la resta de los valores de dos medidas.
    /// <summary>Returns a measure whose value is the absolute difference with another.</summary>
    public Measure Rest(Measure m)
    {
        Point p2 = m.point1;
        double distance = Math.Abs(m.Value - this.Value);
        Point newp2 = this.PointAtDistance(p2, distance);
        return new Measure(point1, newp2);
    }
    //n veces una medida.
    /// <summary>Returns a measure scaled by the given factor.</summary>
    public Measure Product(double n)
    {
        n = Math.Abs(Convert.ToInt64(n));
        Point newp2 = this.PointAtDistance(this.point2, this.Value * n);
        return new Measure(point1, newp2);
    }
    //Cuantas veces cabe una medida dentro de otra.
    /// <summary>Returns how many times the other measure fits into this one.</summary>
    public long Division(Measure m)
    {
        return Convert.ToInt64(this.Value / m.Value);
    }

    /// <summary>Returns true when both measures have the same value.</summary>
    public static bool Equals(Measure m1, Measure m2)
    {
        if (m1.Value == m2.Value)
        {
            return true;
        }
        else return false;
    }

    /// <summary>Returns true when the first measure exceeds the second.</summary>
    public static bool GreaterThen(Measure m1, Measure m2)
    {
        if (m1.Value > m2.Value)
        {
            return true;
        }
        else return false;
    }

    //Check this.
    private Point PointAtDistance(Point p2, double wanteddistance)
    {
        double x = p2.x - point1.x;
        double y = p2.y - point1.y;
        double CurrentDistance = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        double factor = wanteddistance / CurrentDistance;
        double newX = point1.x + (p2.x - point1.x) * factor;
        double newY = point1.y + (p2.y - point1.y) * factor;
        return new Point(newX, newY);
    }

    /// <summary>Returns a human-readable description of the measure.</summary>
    public override string ToString() => string.Format("Measure is {0}", Value);
}