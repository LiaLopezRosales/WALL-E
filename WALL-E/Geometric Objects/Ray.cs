public class Ray:Figure,IEquatable<Ray>
{
    public Point StartIn{get;set;}
    public Point PassFor{get;set;}

    public Ray(Point start,Point pass)
    {
        StartIn=start;
        PassFor=pass;
    }
     
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
}