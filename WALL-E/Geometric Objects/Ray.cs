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
}