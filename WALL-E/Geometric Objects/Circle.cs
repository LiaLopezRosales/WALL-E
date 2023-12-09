public class Circle:Figure,IEquatable<Circle>
{
    public Point center{get;set;}
    public double radio{get;set;}

    public Circle(Point p,int r)
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
            radio=generator.NextDouble(1,25);
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
}