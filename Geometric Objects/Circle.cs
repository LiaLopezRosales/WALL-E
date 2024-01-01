public class Circle:Figure,IEquatable<Circle>
{
    public Point center{get;set;}
    public double radio{get;set;}

    public Circle(Point p,double r)
    {
        center=p;
        radio=r;
    }

    public void RandomCircle(List<Circle>existingcircles,List<Point>points)
    {
       Random generator=new Random();
        if (existingcircles.Count==0)
        {
            center=new Point(0,0);
            center.RandomPoint(points);
            radio=generator.NextDouble(20,50);
        }
        else
        {
            bool existing=true;
            while (existing)
            {
                existing=false;
                center=existingcircles.Last().center.IncrementCoordinate(generator.NextDouble(1,10),generator.NextDouble(1,10));
                radio=generator.NextDouble(existingcircles.Last().radio+1,existingcircles.Last().radio+20);
                foreach (Circle circle in existingcircles)
                {
                    if (center.Equals(circle!.center)&&radio==circle.radio)
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
    public bool Equals(Circle? c)
    {
        if (center.Equals(c!.center)&&radio==c.radio)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public Point PointInsideFigure(List<Point> existingpoints)
    {
       Random random=new Random();
       double angle=0;
       double distance=0;
       if (existingpoints.Count==0)
        {
            angle=random.NextDouble(0,2*Math.PI);
            distance=random.NextDouble(0,radio);
            return new Point(center.x+distance*Math.Cos(angle),center.y+distance*Math.Sin(angle));
        }
        else
        {
            bool existing=true;
            Point alternative_point=new Point(0,0);
            while (existing)
            {
                alternative_point=PointInsideFigure(new List<Point>());
                existing=false;
                
                foreach (Point point in existingpoints)
                {
                    if (point.Equals(alternative_point))
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
            return alternative_point;
        }
    }

    public override bool ContainPoint(Point p)
    {
        double distance=Math.Sqrt(Math.Pow(p.x-this.center.x,2)+Math.Pow(p.y-this.center.y,2));
            if (distance==this.radio)
            {
                return true;
            }
            else return false;
    }
    public override GenericSequence<Point> FigurePoints()
    {
        IEnumerable<Point> Circle_PointsSeq()
        {
            while (true)
            {
                Random random = new Random();
                double x = RandomExtensions.NextDouble(random,center.x-radio-1,center.x+radio+1);
                int one_or_two=random.Next(2);
                double y = FindYofXinCircle(x)[one_or_two];
                yield return new Point(x,y);
            }

        }
        IEnumerable<Point> seq=Circle_PointsSeq();
        return new InfinitePointSequence(seq);
    }
    public List<double> FindYofXinCircle(double x)
    {
        double atdistance1=(Math.Sqrt(Math.Pow(this.radio,2)-Math.Pow(x-this.center.x,2)))+this.center.y;
        double atdistance2=(-Math.Sqrt(Math.Pow(this.radio,2)-Math.Pow(x-this.center.x,2)))+this.center.y;
        return new List<double>(){atdistance1,atdistance2};
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
            return ((Ray)fig).Intersect(this);
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
    private Finite_Sequence<Point> IntersectCircle(Circle c)
    {
        if (this.Equals(c))
        {
            return null!;
        }
        double distance=GeometricTools.PointsDistance(this.center,c.center);
        double radiosum=this.radio+c.radio;
        if (distance>radiosum||distance<Math.Abs(this.radio-c.radio))
        {
            Finite_Sequence<Point> temp1=new Finite_Sequence<Point>(new List<Point>());
        temp1.type=Finite_Sequence<Point>.SeqType.point;
            return temp1;
        }
        double a=(Math.Pow(this.radio,2)-Math.Pow(c.radio,2)+Math.Pow(distance,2))/(distance*2);
        double h=Math.Sqrt(Math.Pow(this.radio,2)-Math.Pow(a,2));
        double x3=this.center.x+a*(c.center.x-this.center.x)/distance;
        double y3=this.center.y+a*(c.center.y-this.center.y)/distance;
        double x1=(x3+h*(c.center.y-this.center.y))/distance;
        double y1=(y3-h*(c.center.x-this.center.x))/distance;
        double x2=(x3-h*(c.center.y-this.center.y))/distance;
        double y2=(y3+h*(c.center.x-this.center.x))/distance;
        Point first=new Point(x1,y1);
        Point second=new Point(x2,y2);
        Finite_Sequence<Point> temp=new Finite_Sequence<Point>(new List<Point>(){first,second});
        temp.type=Finite_Sequence<Point>.SeqType.point;
        return temp;
    }
    private Finite_Sequence<Point> IntersectArc(Arc arc)
    {
        if (arc.center.Equals(this.center)&&arc.measure==this.radio)
        {
            return null!;
        }
        List<Point> points=new List<Point>();
        Circle relativecircle=new Circle(arc.center,arc.measure);
        var intersect=this.IntersectCircle(relativecircle);
        if (intersect.count>0)
        {
            foreach (var item in intersect.Sequence)
            {
                var pointarcintersect=item.Intersect(arc);
                if (pointarcintersect.count>0)
                {
                    points.Add(item);
                }
            }
            
        }
        Finite_Sequence<Point> temp=new Finite_Sequence<Point>(points);
        temp.type=Finite_Sequence<Point>.SeqType.point;
        return temp;
    }
    public override string ToString() => string.Format("Circle with Center in {0} and radio {1} ",center,radio);
}