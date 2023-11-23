public class Circle:IEquatable<Circle>
{
    public Point center{get;set;}
    public int radio{get;set;}

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
            radio=generator.Next(1,25);
        }
        else
        {
            bool existing=true;
            while (existing)
            {
                existing=false;
                //Buscar otra idea para balancear estas coordenadas
                center=existingcircles.Last().center.IncrementCoordinate(generator.Next(1,10),generator.Next(1,10));
                radio=generator.Next(existingcircles.Last().radio+1,existingcircles.Last().radio+21);
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
}