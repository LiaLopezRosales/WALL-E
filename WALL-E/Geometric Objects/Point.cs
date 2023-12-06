public class Point:IEquatable<Point>
{
    public double x {get;set;}
    public double y {get;set;}

    public Point(double x1,double y1)
    {
        x=x1;
        y=y1;
    }
    public void RandomPoint(List<Point> existingpoints)
    {
        Random generator=new Random();
        if (existingpoints.Count==0)
        {
            x=generator.Next();
            y=generator.Next();
        }
        else
        {
            bool existing=true;
            while (existing)
            {
                existing=false;
                x=generator.NextDouble(existingpoints.Last().x+1,existingpoints.Last().x+13);
                y=generator.NextDouble(existingpoints.Last().y+1,existingpoints.Last().y+13);
                foreach (Point point in existingpoints)
                {
                    if (point.x==x && point.y==y)
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
    public Point IncrementCoordinate(double x1,double y1)
    {
        double x2=x+x1;
        double y2=y+y1;
        return new Point(x2,y2);
        
    }
    public bool Equals(Point? p)
    {
        if (x.Equals(p!.x)&&y.Equals(p!.y))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    
}