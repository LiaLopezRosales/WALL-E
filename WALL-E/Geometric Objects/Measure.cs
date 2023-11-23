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
}