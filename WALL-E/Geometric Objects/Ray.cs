public class Ray:Figure,IEquatable<Ray>
{   //Clase que describe un rayo
    public Point StartIn{get;set;}
    public Point PassFor{get;set;}

    public Ray(Point start,Point pass)
    {
        StartIn=start;
        PassFor=pass;
    }
     //Genera un rayo aleatorio
    public void RandomRay(List<Ray>existingrays,List<Point>points)
    {
       Random generator=new Random();
        if (existingrays.Count==0)
        {
            StartIn=new Point(0,0);
            StartIn.RandomPoint(points);
            PassFor=new Point(0,0);
            PassFor.RandomPoint(points);
        }
        else
        {
            bool existing=true;
            while (existing)
            {
                existing=false;
                //Revisar pq pueden generarse rayos coincidentes
                StartIn=existingrays.Last().StartIn.IncrementCoordinate(generator.NextDouble(1,10),generator.NextDouble(1,10));
                PassFor=existingrays.Last().PassFor.IncrementCoordinate(generator.NextDouble(1,10),generator.NextDouble(1,10));
                foreach (Ray ray in existingrays)
                {
                    if ((StartIn.Equals(ray!.StartIn)&&PassFor.Equals(ray!.PassFor))||(StartIn.Equals(ray!.PassFor)&&PassFor.Equals(ray!.StartIn)))
                    {
                        existing=true;
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
    public bool Equals(Ray? r)
    {
        if (StartIn.Equals(r!.StartIn)&&PassFor.Equals(r!.PassFor))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //Determina si el punto pertenece al rayo
    public override bool ContainPoint(Point p)
    {
       double m=(this.PassFor.y-this.StartIn.y)/(this.PassFor.x-this.StartIn.x);
            double b=this.StartIn.y-m*this.StartIn.x;
            if ((p.y-m*p.x)-b==0)
            {
                if (this.StartIn.x<=this.PassFor.x)
                {
                    if (this.StartIn.y<=this.PassFor.y)
                    {
                        if (p.x>=this.StartIn.x&&p.y>=this.PassFor.y)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (p.x>=this.StartIn.x&&p.y<=this.PassFor.y)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    if (this.StartIn.y<=this.PassFor.y)
                    {
                        if (p.x<=this.StartIn.x&&p.y>=this.PassFor.y)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (p.x<=this.StartIn.x&&p.y<=this.PassFor.y)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
    }
    public Point CreateRelativeEnd()
    {
        double x_of_new_end=0;
        double y_of_new_end=0;
        if (this.StartIn.x<=this.PassFor.x)
        {
            x_of_new_end=this.PassFor.x+5000;
        }
        else
        {
            x_of_new_end=this.PassFor.x-5000;
        }
        double m=GeometricTools.Pendient(this.StartIn,this.PassFor);
        y_of_new_end=GeometricTools.FindY(m,GeometricTools.N_of_Equation(this.StartIn,this.PassFor),x_of_new_end);
        if (double.IsInfinity(m))
        {
            x_of_new_end=this.StartIn.x;
            if (this.StartIn.y<=this.PassFor.y)
            {
                y_of_new_end=this.PassFor.y+5000;
            }
            else y_of_new_end=this.PassFor.y-5000;
        }
        return new Point(x_of_new_end,y_of_new_end);
    }
    public override GenericSequence<Point> FigurePoints()
    {
        IEnumerable<Point> Line_PointsSeq()
        {
            while (true)
            {
                Random random = new Random();
                double x = RandomExtensions.NextDouble(random, Math.Min(StartIn.x, PassFor.x), Math.Max(StartIn.x, PassFor.x));
                while (!GeometricTools.BelongToSegment(StartIn, PassFor, x))
                {
                    x = RandomExtensions.NextDouble(random, Math.Min(StartIn.x, PassFor.x), Math.Max(StartIn.x, PassFor.x));
                }
                double y = GeometricTools.FindY(GeometricTools.Pendient(StartIn, PassFor), GeometricTools.N_of_Equation(StartIn, PassFor), x);
                yield return new Point(x,y);
            }

        }
        IEnumerable<Point> seq=Line_PointsSeq();
        return new InfinitePointSequence(seq);
    }
    public override Finite_Sequence<Point> Intersect(Figure fig)
    {
        if (fig is Point)
        {
            return ((Point)fig).Intersect(this);
        }
        else if (fig is Line)
        {
            return ((Line)fig).Intersect(this);
        }
        else if (fig is Segment)
        {
            return ((Segment)fig).Intersect(this);
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
    private Finite_Sequence<Point> IntersectRay(Ray r)
    {
        Segment s1=new Segment(this.StartIn,this.CreateRelativeEnd());
        Segment s2=new Segment(r.StartIn,r.CreateRelativeEnd());
        return s1.Intersect(s2);
    }
    private Finite_Sequence<Point> IntersectCircle(Circle c)
    {
        Segment relativesegment=new Segment(this.StartIn,this.CreateRelativeEnd());
        return relativesegment.Intersect(c);
    }
    private Finite_Sequence<Point> IntersectArc(Arc arc)
    {
        Segment relativesegment=new Segment(this.StartIn,this.CreateRelativeEnd());
        return relativesegment.Intersect(arc);
    }
}