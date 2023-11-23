public class Arc:IEquatable<Arc>
{
    public Point center{get;set;}
    public Point point_of_semirect1{get;set;}
    public Point point_of_semirect2{get;set;}

    public double measure{get;set;}

    public Arc(Point c,Point p1,Point p2,double m)
    {
        center=c;
        point_of_semirect1=p1;
        point_of_semirect2=p2;
        measure=m;
    }

    public bool Equals(Arc? arc)
    {
        if (center.Equals(arc!.center)&&point_of_semirect1.Equals(arc.point_of_semirect1)&&point_of_semirect2.Equals(arc.point_of_semirect2)&&measure==arc.measure)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}