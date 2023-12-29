public class Point : Figure, IEquatable<Point>
{   //Clase que describe un punto en el plano
    public double x { get; set; }
    public double y { get; set; }
    //Crea un punto con las coordenadas indicadas
    public Point(double x1, double y1)
    {
        x = x1;
        y = y1;
    }
    //Método que genera un punto aleatorio dependiendo del último punto generado
    public void RandomPoint(List<Point> existingpoints)
    {
        Random generator = new Random();
        //Si es el primer punto a generar sus coordenadas serán enteras
        if (existingpoints.Count == 0)
        {
            x = generator.Next(1,20);
            y = generator.Next(1,20);
        }
        else
        {
            bool existing = true;
            //Sino se crea un ciclo donde se generan nuevas coordenadas en un radio determinado de las del último punto
            while (existing)
            {
                existing = false;
                x = generator.NextDouble(existingpoints.Last().x + 1, existingpoints.Last().x + 13);
                y = generator.NextDouble(existingpoints.Last().y + 1, existingpoints.Last().y + 13);
                //Se comprueba si estas coordenadas nuevas coinciden con un punto existente
                foreach (Point point in existingpoints)
                {   //Si coinciden se continua en el ciclo hasta generar coordenadas únicas 
                    if (point.x == x && point.y == y)
                    {
                        existing = true;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }

            }
        }
    }
    //Método auxiliar para crear un punto a una distancia de coordenadas determinadas de uno existente
    public Point IncrementCoordinate(double x1, double y1)
    {
        double x2 = x + x1;
        double y2 = y + y1;
        return new Point(x2, y2);

    }
    //Se define la igualdad entre puntos
    public bool Equals(Point? p)
    {
        if (x.Equals(p!.x) && y.Equals(p!.y))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //Método de la clase Figura(todas las figuras deben poder definir si contienen a un punto indicado)
    public override bool ContainPoint(Point p)
    {
        if (this.Equals(p)) return true;
        else return false;
    }
    //Debería ser un generador de puntos de la figura en cuestión(heredado de Figure no cumple mucho objetivo en Point)
    public override GenericSequence<Point> FigurePoints()
    {
        List<Point> points = new List<Point>() { this };
        Finite_Sequence<Point> temp = new Finite_Sequence<Point>(points);
        temp.type = Finite_Sequence<Point>.SeqType.point;
        return temp;
    }
    //Método que dado una figura indica la intersección con esta
    public override Finite_Sequence<Point> Intersect(Figure fig)
    {
        if (fig is Point)
        {
            return this.IntersectPoint((Point)fig);
        }
        else if (fig is Line)
        {
            return this.IntersectLine((Line)fig);
        }
        else if (fig is Segment)
        {
            return this.IntersectSegment((Segment)fig);
        }
        else if (fig is Ray)
        {
            return this.IntersectRay((Ray)fig);
        }
        else if (fig is Circle)
        {
            return this.IntersectCircle((Circle)fig);
        }
        else if (fig is Arc)
        {
            return this.IntersectArc((Arc)fig);
        }
        Finite_Sequence<Point> temp = new Finite_Sequence<Point>(new List<Point>());
        temp.type = Finite_Sequence<Point>.SeqType.point;
        return temp;

    }
    //Métodos auxiliares se define la intersección con cada tipo de figura existente("funciona"?)
    private Finite_Sequence<Point> IntersectPoint(Point p)
    {
        if (this.ContainPoint(p))
        {
            Finite_Sequence<Point> temp = new Finite_Sequence<Point>(new List<Point>() { p });
            temp.type = Finite_Sequence<Point>.SeqType.point;
            return temp;
        }
        else
        {
            Finite_Sequence<Point> temp = new Finite_Sequence<Point>(new List<Point>());
            temp.type = Finite_Sequence<Point>.SeqType.point;
            return temp;
        }
    }
    private Finite_Sequence<Point> IntersectLine(Line l)
    {
        var new_y = GeometricTools.FindY(GeometricTools.Pendient(l.generalpoint1, l.generalpoint2), GeometricTools.N_of_Equation(l.generalpoint1, l.generalpoint2), this.x);
        var original_y = this.y;
        double aprox_range = 0.3;
        if (Math.Abs(new_y - original_y) <= aprox_range)
        {
            Finite_Sequence<Point> temp = new Finite_Sequence<Point>(new List<Point>() { this });
            temp.type = Finite_Sequence<Point>.SeqType.point;
            return temp;
        }
        else
        {
            Finite_Sequence<Point> temp = new Finite_Sequence<Point>(new List<Point>());
            temp.type = Finite_Sequence<Point>.SeqType.point;
            return temp;
        }
    }
    private Finite_Sequence<Point> IntersectSegment(Segment s)
    {
        var new_y = GeometricTools.FindY(GeometricTools.Pendient(s.StartIn, s.EndsIn), GeometricTools.N_of_Equation(s.StartIn, s.EndsIn), this.x);
        var original_y = this.y;
        double aprox_range = 0.3;
        if (Math.Abs(new_y - original_y) <= aprox_range && GeometricTools.BelongToSegment(s.StartIn, s.EndsIn, this.x))
        {
            Finite_Sequence<Point> temp = new Finite_Sequence<Point>(new List<Point>() { this });
            temp.type = Finite_Sequence<Point>.SeqType.point;
            return temp;
        }
        else
        {
            Finite_Sequence<Point> temp = new Finite_Sequence<Point>(new List<Point>());
            temp.type = Finite_Sequence<Point>.SeqType.point;
            return temp;
        }
    }
    private Finite_Sequence<Point> IntersectRay(Ray r)
    {
        double x_of_new_end = 0;
        double y_of_new_end = 0;
        if (r.StartIn.x <= r.PassFor.x)
        {
            x_of_new_end = r.PassFor.x + 5000;
        }
        else
        {
            x_of_new_end = r.PassFor.x - 5000;
        }
        double m = GeometricTools.Pendient(r.StartIn, r.PassFor);
        y_of_new_end = GeometricTools.FindY(m, GeometricTools.N_of_Equation(r.StartIn, r.PassFor), x_of_new_end);
        if (double.IsInfinity(m))
        {
            x_of_new_end = r.StartIn.x;
            if (r.StartIn.y <= r.PassFor.y)
            {
                y_of_new_end = r.PassFor.y + 5000;
            }
            else y_of_new_end = r.PassFor.y - 5000;
        }

        Segment segment_of_r = new Segment(r.StartIn, new Point(x_of_new_end, y_of_new_end));
        return this.IntersectSegment(segment_of_r);
    }
    private Finite_Sequence<Point> IntersectCircle(Circle c)
    {
        double distance = GeometricTools.PointsDistance(this, c.center);
        double aprox_range = 0.3;
        if (Math.Abs(distance - c.radio) <= aprox_range)
        {
            Finite_Sequence<Point> temp = new Finite_Sequence<Point>(new List<Point>() { this });
            temp.type = Finite_Sequence<Point>.SeqType.point;
            return temp;
        }
        else
        {
            Finite_Sequence<Point> temp = new Finite_Sequence<Point>(new List<Point>());
            temp.type = Finite_Sequence<Point>.SeqType.point;
            return temp;
        }
    }
    private Finite_Sequence<Point> IntersectArc(Arc arc)
    {
        double distance = GeometricTools.PointsDistance(this, arc.center);
        double radio = arc.measure;
        double aprox_range = 0.3;
        if (Math.Abs(distance - radio) > aprox_range)
        {
            Finite_Sequence<Point> temp = new Finite_Sequence<Point>(new List<Point>());
            temp.type = Finite_Sequence<Point>.SeqType.point;
            return temp;
        }
        Segment fromcenter = new Segment(arc.center, this);
        Segment cord = new Segment(arc.CircleRay1_Intersection, arc.CircleRay2Intersection);
        bool intersect = cord.Intersect(fromcenter).count > 0;
        if (Math.Abs(arc.SweepAngle) >= 180 && !intersect)
        {
            Finite_Sequence<Point> temp = new Finite_Sequence<Point>(new List<Point>() { this });
            temp.type = Finite_Sequence<Point>.SeqType.point;
            return temp;
        }
        if (Math.Abs(arc.SweepAngle) < 180 && intersect)
        {
            Finite_Sequence<Point> temp = new Finite_Sequence<Point>(new List<Point>() { this });
            temp.type = Finite_Sequence<Point>.SeqType.point;
            return temp;
        }
        Finite_Sequence<Point> temp1 = new Finite_Sequence<Point>(new List<Point>());
        temp1.type = Finite_Sequence<Point>.SeqType.point;
        return temp1;
    }
    public override string ToString() => string.Format("Point in {0} {1} ",x,y);

}