public class Measure
{
    public Point point1{get;set;}
    public Point point2{get;set;}
    public double Value{get;protected set;}
     
    public Measure(Point p1,Point p2)
    {
       point1=p1;
       point2=p2;
       Value=Math.Sqrt(Math.Pow((point2.x - point1.x),2)+Math.Pow((point2.y - point1.y),2));
    }

    public Measure Sum(Measure m)
    {
        Point p2=m.point1;
        double distance=m.Value + this.Value;
        Point newp2=this.PointAtDistance(p2,distance);
        return new Measure(point1,newp2);
    }

    public Measure Rest(Measure m)
    {
        Point p2=m.point1;
        double distance=Math.Abs(m.Value - this.Value);
        Point newp2=this.PointAtDistance(p2,distance);
        return new Measure(point1,newp2);
    }

    public Measure Product(double n)
    {
        n=Math.Abs(Convert.ToInt64(n));
        Point newp2=this.PointAtDistance(this.point2,this.Value*n);
        return new Measure(point1,newp2);
    }

    public long Division(Measure m)
    {
       return Convert.ToInt64(this.Value/m.Value);
    }

    public static  bool Equals(Measure m1,Measure m2)
    {
        if (m1.Value==m2.Value)
        {
            return true;
        }
        else return false;
    }
    public static bool GreaterThen(Measure m1,Measure m2)
    {
        if (m1.Value>m2.Value)
        {
            return true;
        }
        else return false;
    }
    
    //Check this
    private Point PointAtDistance(Point p2,double wanteddistance)
    {
        double x=p2.x-point1.x;
        double y=p2.y-point1.y;
        double CurrentDistance=Math.Sqrt(Math.Pow(x,2)+Math.Pow(y,2));
        double factor=wanteddistance/CurrentDistance;
        double newX=point1.x+(p2.x-point1.x)*factor;
        double newY=point1.y+(p2.y-point1.y)*factor;
        return new Point(newX,newY);
    }
    public override string ToString() => string.Format("Measure is {0}",Value);
}