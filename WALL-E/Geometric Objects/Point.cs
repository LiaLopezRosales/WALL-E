public class Point:IEquatable<Point>
{
    public int x {get;set;}
    public int y {get;set;}

    public Point(int x1,int y1)
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
                //Buscar otra idea para balancear estas coordenadas
                x=generator.Next(existingpoints.Last().x+1,existingpoints.Last().x+13);
                y=generator.Next(existingpoints.Last().y+1,existingpoints.Last().y+13);
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
    public Point IncrementCoordinate(int x1,int y1)
    {
        int x2=x+x1;
        int y2=y+y1;
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