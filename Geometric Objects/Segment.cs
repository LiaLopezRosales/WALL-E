public class Segment:Figure,IEquatable<Segment>
{   //Clase que define un segmento
    public Point StartIn{get;set;}
    public Point EndsIn{get;set;}

    public Segment(Point start,Point ends)
    {
        StartIn=start;
        EndsIn=ends;
    }
     //Genera un segmento aleatorio(funciona igual que el generador de pntos y de lineas)
    public void RandomSegment(List<Segment>existingsegments,List<Point>points)
    {
       Random generator=new Random();
        if (existingsegments.Count==0)
        {
            StartIn=new Point(0,0);
            StartIn.RandomPoint(points);
            EndsIn=new Point(0,0);
            EndsIn.RandomPoint(points);
        }
        else
        {
            bool existing=true;
            while (existing)
            {
                existing=false;
                StartIn=existingsegments.Last().StartIn.IncrementCoordinate(generator.NextDouble(1,10),generator.NextDouble(1,10));
                EndsIn=existingsegments.Last().EndsIn.IncrementCoordinate(generator.NextDouble(1,10),generator.NextDouble(1,10));
                foreach (Segment seg in existingsegments)
                {
                    if ((StartIn.Equals(seg!.StartIn)&&EndsIn.Equals(seg!.EndsIn))||(StartIn.Equals(seg!.EndsIn)&&EndsIn.Equals(seg!.StartIn)))
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
    //Igualdad de segmentos
    public bool Equals(Segment? s)
    {
        if ((StartIn.Equals(s!.StartIn)&&EndsIn.Equals(s!.EndsIn))||(StartIn.Equals(s!.EndsIn)&&EndsIn.Equals(s!.StartIn)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //Punto pertenece a segmento
    public override bool ContainPoint(Point p)
    {
        double m=(this.EndsIn.y-this.StartIn.y)/(this.EndsIn.x-this.StartIn.x);
            double b=this.StartIn.y-m*this.StartIn.x;
            if ((p.y-m*p.x)-b==0)
            {
                double minX=Math.Min(this.StartIn.x,this.EndsIn.x);
                double maxX=Math.Max(this.StartIn.x,this.EndsIn.x);
                double minY=Math.Min(this.StartIn.y,this.EndsIn.y);
                double maxY=Math.Max(this.StartIn.y,this.EndsIn.y);
                if (p.x>=minX&&p.x<=maxX&&p.y>=minY&&p.y<=maxY)
                {
                    return true;
                }
            }
            return false;
    }
    //Puntos del segmento
    public override GenericSequence<Point> FigurePoints()
    {
        IEnumerable<Point> Line_PointsSeq()
        {
            while (true)
            {
                Random random = new Random();
                double x = RandomExtensions.NextDouble(random, Math.Min(StartIn.x, EndsIn.x), Math.Max(StartIn.x, EndsIn.x));
                while (!GeometricTools.BelongToSegment(StartIn, EndsIn, x))
                {
                    x = RandomExtensions.NextDouble(random, Math.Min(StartIn.x, EndsIn.x), Math.Max(StartIn.x, EndsIn.x));
                }
                double y = GeometricTools.FindY(GeometricTools.Pendient(StartIn, EndsIn), GeometricTools.N_of_Equation(StartIn, EndsIn), x);
                yield return new Point(x,y);
            }

        }
        IEnumerable<Point> seq=Line_PointsSeq();
        return new InfinitePointSequence(seq);
    }
    //Intersección del segmento con otra figura
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
    private Finite_Sequence<Point> IntersectSegment(Segment s)
    {
        Line l1=new(this.StartIn,this.EndsIn);
        Line l2=new(s.StartIn,s.EndsIn);
        var intersect=l1.Intersect(l2);
        if (intersect.count>0)
        {
            List<Point> points=new List<Point>();
            foreach (var item in intersect.Sequence)
            {
                if (GeometricTools.BelongToSegment(this.StartIn,this.EndsIn,item.x)&&GeometricTools.BelongToSegment(s.StartIn,s.EndsIn,item.x))
                {
                    points.Add(item);
                }
            }
            Finite_Sequence<Point> temp=new Finite_Sequence<Point>(points);
        temp.type=Finite_Sequence<Point>.SeqType.point;
        return temp;
        }
        Finite_Sequence<Point> temp2=new Finite_Sequence<Point>(new List<Point>());
        temp2.type=Finite_Sequence<Point>.SeqType.point;
        return temp2;

    }
    private Finite_Sequence<Point> IntersectRay(Ray r)
    {
        Segment relativesegment=new Segment(r.StartIn,r.CreateRelativeEnd());
        return this.IntersectSegment(relativesegment);
    }
    private Finite_Sequence<Point> IntersectCircle(Circle c)
    {
        List<Point> points=new List<Point>();
        Line relativeline=new Line(this.StartIn,this.EndsIn);
        var intersect=relativeline.Intersect(c);
        if (intersect.count>0)
        {
            foreach (var item in intersect.Sequence)
            {
                if (GeometricTools.BelongToSegment(this.StartIn,this.EndsIn,item.x))
                {
                    points.Add(item);
                }
            }
        }
        Finite_Sequence<Point> temp=new Finite_Sequence<Point>(points);
        temp.type=Finite_Sequence<Point>.SeqType.point;
        return temp;
    }
    private Finite_Sequence<Point> IntersectArc(Arc arc)
    {
        List<Point> points=new List<Point>();
        Line relativeline=new Line(this.StartIn,this.EndsIn);
        var intersect=relativeline.Intersect(arc);
        if (intersect.count>0)
        {
            foreach (var item in intersect.Sequence)
            {
                if (GeometricTools.BelongToSegment(this.StartIn,this.EndsIn,item.x))
                {
                    points.Add(item);
                }
            }
        }
        Finite_Sequence<Point> temp=new Finite_Sequence<Point>(points);
        temp.type=Finite_Sequence<Point>.SeqType.point;
        return temp;
    }
}