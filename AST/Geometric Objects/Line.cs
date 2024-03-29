public class Line : Figure, IEquatable<Line>
{ //Clase que define una recta
    //Contiene dos puntos cualesquiera por donde pasa la recta 
    public Point generalpoint1 { get; set; }
    public Point generalpoint2 { get; set; }

    public Line(Point p1, Point p2)
    {
        generalpoint1 = p1;
        generalpoint2 = p2;
    }
    //Genera una recta aleatoria(combinación de dos puntos aleatorios única,funciona de forma análoga a la de los puntos)
    public void RandomLine(List<Line> existinglines, List<Point> points)
    {
        Random generator = new Random();
        if (existinglines.Count == 0)
        {
            generalpoint1 = new Point(0, 0);
            generalpoint1.RandomPoint(points);
            generalpoint2 = new Point(0, 0);
            generalpoint2.RandomPoint(points);
        }
        else
        {
            bool existing = true;
            while (existing)
            {
                existing = false;
                generalpoint1 = existinglines.Last().generalpoint1.IncrementCoordinate(generator.NextDouble(1, 10), generator.NextDouble(1, 10));
                generalpoint2 = existinglines.Last().generalpoint2.IncrementCoordinate(generator.NextDouble(1, 10), generator.NextDouble(1, 10));
                foreach (Line line in existinglines)
                {
                    if ((generalpoint1.Equals(line!.generalpoint1) && generalpoint2.Equals(line!.generalpoint2)) || (generalpoint1.Equals(line!.generalpoint2) && generalpoint2.Equals(line!.generalpoint1)))
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
    //Definición de igualdad de rectas(no del todo absoluta)
    public bool Equals(Line? l)
    {
        if ((generalpoint1.Equals(l!.generalpoint1) && generalpoint2.Equals(l!.generalpoint2)) || (generalpoint1.Equals(l!.generalpoint2) && generalpoint2.Equals(l!.generalpoint1)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //Determina si un punto pertenece a la recta
    public override bool ContainPoint(Point p)
    {
        double m = (this.generalpoint2.y - this.generalpoint1.y) / (this.generalpoint2.x - this.generalpoint1.x);
        double b = this.generalpoint1.y - m * this.generalpoint1.x;
        if ((p.y - m * p.x) - b == 0)
        {
            return true;
        }
        else return false;
    }
    //Genera puntos pertenecientes a la recta(debería) 
    public override GenericSequence<Point> FigurePoints()
    {
        IEnumerable<Point> Line_PointsSeq()
        {
            while (true)
            {
                Random random = new Random();
                double x = RandomExtensions.NextDouble(random, Math.Min(generalpoint1.x, generalpoint2.x), Math.Max(generalpoint1.x, generalpoint2.x));
                while (!GeometricTools.BelongToSegment(generalpoint1, generalpoint2, x))
                {
                    x = RandomExtensions.NextDouble(random, Math.Min(generalpoint1.x, generalpoint2.x), Math.Max(generalpoint1.x, generalpoint2.x));
                }
                double y = GeometricTools.FindY(GeometricTools.Pendient(generalpoint1, generalpoint2), GeometricTools.N_of_Equation(generalpoint1, generalpoint2), x);
                yield return new Point(x,y);
            }

        }
        IEnumerable<Point> seq=Line_PointsSeq();
        return new InfinitePointSequence(seq);
    }
    //Busca el tipo de intersección
    public override Finite_Sequence<Point> Intersect(Figure fig)
    {
        if (fig is Point)
        {
            return ((Point)fig).Intersect(this);
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
        Finite_Sequence<Point> temp=new Finite_Sequence<Point>(new List<Point>());
        temp.type=Finite_Sequence<Point>.SeqType.point;
        return temp;
        
    }
    //Métodos de intersección auxiliares(no hay con un punto pq este método ya está contenido en clase Point)
    private Finite_Sequence<Point> IntersectLineB(Line l)
    {
        if (this.Equals(l))
        {
            return null!;
        }
        double m1=GeometricTools.Pendient(generalpoint1, generalpoint2);
        double m2=GeometricTools.Pendient(l.generalpoint1, l.generalpoint2);
        double n1=GeometricTools.N_of_Equation(generalpoint1, generalpoint2);
        double n2=GeometricTools.N_of_Equation(l.generalpoint1, l.generalpoint2);
        if (m1==m2||double.IsInfinity(m1)||double.IsInfinity(m2))
        {
            Finite_Sequence<Point> temp1=new Finite_Sequence<Point>(new List<Point>());
        temp1.type=Finite_Sequence<Point>.SeqType.point;
        return temp1;
        }
        double m=m1;
        double n=n1;
        double x=(n2-n1)/(m1-m2);
        if (double.IsNaN(x))
        {
            if (double.IsInfinity(m))
            {
                m=m2;
                n=n2;
                x=this.generalpoint1.x;
            }
            else x=l.generalpoint1.x;
        }
        double y=GeometricTools.FindY(m,n,x);
        Finite_Sequence<Point> temp=new Finite_Sequence<Point>(new List<Point>{new Point(x,y)});
        temp.type=Finite_Sequence<Point>.SeqType.point;
        return temp;
        
    }
    private Finite_Sequence<Point> IntersectLine(Line l)
    {
        if (this.Equals(l))
        {
            return null!;
        }
        double den=(this.generalpoint1.x-this.generalpoint2.x)*(l.generalpoint1.y-l.generalpoint2.y)-(this.generalpoint1.y-this.generalpoint2.y)*(l.generalpoint1.x-l.generalpoint2.x);
        if (den==0)
        {
            return new Finite_Sequence<Point>(new List<Point>());
        }
        double t=((this.generalpoint1.x-l.generalpoint1.x)*(l.generalpoint1.y-l.generalpoint2.y)-(this.generalpoint1.y-l.generalpoint1.y)*(l.generalpoint1.x-l.generalpoint2.x))/den;
        double u=-((this.generalpoint1.x-this.generalpoint2.x)*(this.generalpoint1.y-l.generalpoint1.y)-(this.generalpoint1.y-this.generalpoint2.y)*(this.generalpoint1.x-l.generalpoint1.x))/den;
        if (t>=0 && t<=1 && u>=0 && u<=1)
        {
            return new Finite_Sequence<Point>(new List<Point>(){new Point(this.generalpoint1.x+t*(this.generalpoint2.x-this.generalpoint1.x),this.generalpoint1.y+t*(this.generalpoint2.y-this.generalpoint1.y))});
        }
        else
        {
           return new Finite_Sequence<Point>(new List<Point>());
        }
        
    }
    private Finite_Sequence<Point> IntersectSegment(Segment s)
    {
      Line relativeline=new Line(s.StartIn,s.EndsIn);
      var intersect=this.IntersectLine(relativeline);
      if (intersect is null)
      {
        return null!;
      }
      if (intersect.count>0 && intersect is Finite_Sequence<Point> seq)
      {
        var point=seq.ReturnValue();
        if (GeometricTools.BelongToSegment(s.StartIn,s.EndsIn,point.x))
        {
            Finite_Sequence<Point> temp1=new Finite_Sequence<Point>(new List<Point>(){point});
        temp1.type=Finite_Sequence<Point>.SeqType.point;
        return temp1;
        }
      }
      Finite_Sequence<Point> temp=new Finite_Sequence<Point>(new List<Point>());
        temp.type=Finite_Sequence<Point>.SeqType.point;
        return temp;
    }
    private Finite_Sequence<Point> IntersectRay(Ray r)
    {
        Segment relativesegment=new Segment(r.StartIn,r.CreateRelativeEnd());
        return this.IntersectSegment(relativesegment);
    }
    private Finite_Sequence<Point> IntersectCircleL(Circle cir)
    {
        double distance=GeometricTools.Point_LineDistance(cir.center,this);
        if (distance>cir.radio)
        {
            Finite_Sequence<Point> temp1=new Finite_Sequence<Point>(new List<Point>());
        temp1.type=Finite_Sequence<Point>.SeqType.point;
        return temp1;
        }
        double m=(this.generalpoint2.y-this.generalpoint1.y)/(this.generalpoint2.x-this.generalpoint1.x);
        double b=this.generalpoint1.y-m*this.generalpoint1.x;
        double a =cir.center.x;
        double b1=-cir.center.y * a;
        double c=a*a+cir.center.y*cir.center.y-cir.radio*cir.radio;
        double x1=(-b1 + Math.Sqrt(b1*b1-4*a*c))/2*a;
        double y1=m*x1+b;
        double x2=(-b1 - Math.Sqrt(b1*b1-4*a*c))/2*a;
        double y2=m*x2+b;
        Finite_Sequence<Point> temp=new Finite_Sequence<Point>(new List<Point>(){new Point(x1,y1),new Point(x2,y2)});
        return temp;
    }

    private Finite_Sequence<Point> IntersectCircle(Circle cir)
    {
        Point p1 = this.generalpoint1;
        Point p2 = this.generalpoint2;
        Point c = cir.center;
        double r = cir.radio;

        Point intersect1, intersect2;

        double dx, dy, A, B, C, det, t;

        dx = p2.x - p1.x;
        dy = p2.y - p1.y;

        A = dx * dx + dy * dy;
        B = 2 * (dx * (p1.x - c.x) + dy * (p1.y - c.y));
        C = (p1.x - c.x) * (p1.x - c.x) + (p1.y - c.y) * (p1.y - c.y) - r * r;

        det = B * B - 4 * A * C;

        if ((A <= 0.0000001) || (det < 0))
        {
            return new Finite_Sequence<Point>(new List<Point>());
        }
        else if (det == 0)
        {
            // One real solution
            t = -B / (2 * A);
            intersect1 = new Point(p1.x + t * dx, p1.y + t * dy);
            return new Finite_Sequence<Point>(new List<Point>() { intersect1 });
        }
        else
        {
            // Two real solutions
            t = (double)((-B + Math.Sqrt(det)) / (2 * A));
            intersect1 = new Point(p1.x + t * dx, p1.y + t * dy);
            t = (double)((-B - Math.Sqrt(det)) / (2 * A));
            intersect2 = new Point(p1.x + t * dx, p1.y + t * dy);
            return new Finite_Sequence<Point>(new List<Point>() { intersect1, intersect2 });
        }

    }
    private Finite_Sequence<Point> IntersectCircleB(Circle cir)
    {
        double dx=this.generalpoint2.x-this.generalpoint1.x;
        double dy=this.generalpoint2.y-this.generalpoint1.y;
        double A=dx*dx+dy*dy;
        double B=2*(dx*(this.generalpoint1.x-cir.center.x)+dy*(this.generalpoint1.y-cir.center.y));
        double C=(this.generalpoint1.x-cir.center.x)*(this.generalpoint1.x-cir.center.x)+(this.generalpoint1.y-cir.center.y)*(this.generalpoint1.y-cir.center.y)-cir.radio*cir.radio;
        double det=B*B-4*A*C;
        if (A<=0.0000001||det<0)
        {
            return new Finite_Sequence<Point>(new List<Point>());
        }
        else if (det==0)
        {
            double t=-B/(2*A);
            return new Finite_Sequence<Point>(new List<Point>(){new Point(this.generalpoint1.x+t*dx,this.generalpoint1.y+t*dy)});
        }
        else
        {
            double t=(-B+Math.Sqrt(det))/(2*A);
            Point p1=new Point(this.generalpoint1.x+t*dx,this.generalpoint1.y+t*dy);
            double k=(-B-Math.Sqrt(det))/(2*A);
            Point s = new Point(this.generalpoint1.x + k * dx, this.generalpoint1.y + k * dy);
            Point p2=new Point(this.generalpoint1.x-k*dx,this.generalpoint1.y-k*dy);
            return new Finite_Sequence<Point>(new List<Point>(){p1,p2});
            
        }

    }
    private Finite_Sequence<Point> IntersectArc(Arc arc)
    {
        List<Point> points=new List<Point>();
        Circle relativecircle=new Circle(arc.center,arc.measure);
        var intersect=this.IntersectCircle(relativecircle);
        if (intersect.count>0)
        {
            foreach (var item in intersect.Sequence)
            {
                var PointInsideArc=item.Intersect(arc);
                if (PointInsideArc.count>0)
                {
                    points.Add(item);
                }
            }
        }
        Finite_Sequence<Point> temp=new Finite_Sequence<Point>(points);
        temp.type=Finite_Sequence<Point>.SeqType.point;
        return temp;
    }
    public override string ToString() => string.Format("Line with reference points at {0} , {1} ",generalpoint1,generalpoint2);
}