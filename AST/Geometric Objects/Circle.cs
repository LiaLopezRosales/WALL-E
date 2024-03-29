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
    private Finite_Sequence<Point> IntersectCircleB(Circle c)
    {
        // if (this.Equals(c))
        // {
        //     return null!;
        // }
        // double distance=GeometricTools.PointsDistance(this.center,c.center);
        // double radiosum=this.radio+c.radio;
        // if (distance>radiosum||distance<Math.Abs(this.radio-c.radio))
        // {
        //     Finite_Sequence<Point> temp1=new Finite_Sequence<Point>(new List<Point>());
        // temp1.type=Finite_Sequence<Point>.SeqType.point;
        //     return temp1;
        // }
        // double a=(Math.Pow(this.radio,2)-Math.Pow(c.radio,2)+Math.Pow(distance,2))/(distance*2);
        // double h=Math.Sqrt(Math.Pow(this.radio,2)-Math.Pow(a,2));
        // double x3=this.center.x+a*(c.center.x-this.center.x)/distance;
        // double y3=this.center.y+a*(c.center.y-this.center.y)/distance;
        // double x1=(x3+h*(c.center.y-this.center.y))/distance;
        // double y1=(y3-h*(c.center.x-this.center.x))/distance;
        // double x2=(x3-h*(c.center.y-this.center.y))/distance;
        // double y2=(y3+h*(c.center.x-this.center.x))/distance;
        // Point first=new Point(x1,y1);
        // Point second=new Point(x2,y2);
        // Finite_Sequence<Point> temp=new Finite_Sequence<Point>(new List<Point>(){first,second});
        // temp.type=Finite_Sequence<Point>.SeqType.point;
        // return temp;
        double d=Math.Sqrt(Math.Pow(c.center.x-this.center.x,2)+Math.Pow(c.center.y-this.center.y,2));
       if (d>this.radio+c.radio || d<Math.Abs(this.radio-c.radio))
       {
        Finite_Sequence<Point> temp=new Finite_Sequence<Point>(new List<Point>());
        return temp;
       }
       else if (d==0)
       {
        Finite_Sequence<Point> temp=new Finite_Sequence<Point>(new List<Point>());
        return temp;
       }
       else
       {
         double a=(Math.Pow(this.radio,2)-Math.Pow(c.radio,2)+Math.Pow(d,2))/2*d;
         double h=Math.Sqrt(Math.Pow(this.radio,2)-Math.Pow(a,2));
         double x1=this.center.x+a*(c.center.x-this.center.x)/d;
         double y1=this.center.y+a*(c.center.y-this.center.y)/d;
         double x2=x1+h*(c.center.y-this.center.y)/d;
         double y2=y1-h*(c.center.x-this.center.x)/d;
         double x3=x1-h*(c.center.y-this.center.y)/d;
         double y3=y1+h*(c.center.x-this.center.x)/d;
         
         Point first=new Point(x3,y3);
         Point second=new Point(x2,y2);
         Finite_Sequence<Point> temp=new Finite_Sequence<Point>(new List<Point>(){first,second});
         temp.type=Finite_Sequence<Point>.SeqType.point;
         return temp;
       }
    }
     private Finite_Sequence<Point> IntersectCircleL(Circle c)
    {
        if (this.Equals(c))
        {
            return null!;
        }
        double distance=GeometricTools.PointsDistance(this.center,c.center);
        double radiosum=this.radio+c.radio;
        if (distance>radiosum||distance<Math.Abs(this.radio-c.radio)|| (distance==0 && this.radio==c.radio))
        {
            Finite_Sequence<Point> temp1=new Finite_Sequence<Point>(new List<Point>());
            temp1.type=Finite_Sequence<Point>.SeqType.point;
            return temp1;
        }
        double a=(this.radio*this.radio - c.radio*c.radio)/(2*distance);
        double h=Math.Sqrt(this.radio*this.radio-a*a);
        double distance_x=c.center.x-this.center.x;
        double distance_y=c.center.y-this.center.y;
        double x2=this.center.x+(distance_x*a/distance);
        double y2=this.center.y+(distance_y*a/distance);
        double intersection_x1=x2+h*(distance_y/distance);
        double intersection_y1=y2-h*(distance_x/distance);
        double intersection_x2=x2-h*(distance_y/distance);
        double intersection_y2=y2+h*(distance_x/distance);
        Point first=new Point(intersection_x1,intersection_y1);
        Point second=new Point(intersection_x2,intersection_y2);
        Finite_Sequence<Point> temp=new Finite_Sequence<Point>(new List<Point>(){first,second});
        temp.type=Finite_Sequence<Point>.SeqType.point;
        return temp;
    }

    private Finite_Sequence<Point> IntersectCircle(Circle cir)
    {
        if (this.Equals(cir))
        {
            return null!;
        }

        Point c1 = this.center;
        Point c2 = cir.center;
        double r1 = this.radio;
        double r2 = cir.radio;

        double d = GeometricTools.PointsDistance(c1, c2);
        if (d > r1 + r2 || d < Math.Abs(r1 - r2) || (d == 0 && r1 == r2))
        {
            Finite_Sequence<Point> temp1 = new(new List<Point>());
            temp1.type = Finite_Sequence<Point>.SeqType.point;
            return temp1;
        }

        double a, h, x2, x3, x4, y2, y3, y4;

        a = (Math.Pow(r1, 2) - Math.Pow(r2, 2) + Math.Pow(d, 2)) / (2 * d);
        h = Math.Sqrt(Math.Pow(r1, 2) - Math.Pow(a, 2));

        x2 = c1.x + a * (c2.x - c1.x) / d;
        y2 = c1.y + a * (c2.y - c1.y) / d;

        x3 = x2 + h * (c2.y - c1.y) / d;
        y3 = y2 - h * (c2.x - c1.x) / d;

        x4 = x2 - h * (c2.y - c1.y) / d;
        y4 = y2 + h * (c2.x - c1.x) / d;

        Finite_Sequence<Point> temp = new (new List<Point>() { new Point(x3, y3), new Point(x4, y4) });
        temp.type = Finite_Sequence<Point>.SeqType.point;
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