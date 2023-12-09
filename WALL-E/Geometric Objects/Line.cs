public class Line:Figure,IEquatable<Line>
{
    public Point generalpoint1{get;set;}
    public Point generalpoint2{get;set;}

    public Line(Point p1,Point p2)
    {
        generalpoint1=p1;
        generalpoint2=p2;
    }
     
    public void RandomLine(List<Line>existinglines,List<Point>points)
    {
       Random generator=new Random();
        if (existinglines.Count==0)
        {
            generalpoint1=new Point(0,0);
            generalpoint1.RandomPoint(points);
            generalpoint2=new Point(0,0);
            generalpoint2.RandomPoint(points);
        }
        else
        {
            bool existing=true;
            while (existing)
            {
                existing=false;
                generalpoint1=existinglines.Last().generalpoint1.IncrementCoordinate(generator.NextDouble(1,10),generator.NextDouble(1,10));
                generalpoint2=existinglines.Last().generalpoint2.IncrementCoordinate(generator.NextDouble(1,10),generator.NextDouble(1,10));
                foreach (Line line in existinglines)
                {
                    if ((generalpoint1.Equals(line!.generalpoint1)&&generalpoint2.Equals(line!.generalpoint2))||(generalpoint1.Equals(line!.generalpoint2)&&generalpoint2.Equals(line!.generalpoint1)))
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
    public bool Equals(Line?l)
    {
        if ((generalpoint1.Equals(l!.generalpoint1)&&generalpoint2.Equals(l!.generalpoint2))||(generalpoint1.Equals(l!.generalpoint2)&&generalpoint2.Equals(l!.generalpoint1)))
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
       double m=(this.generalpoint2.y-this.generalpoint1.y)/(this.generalpoint2.x-this.generalpoint1.x);
            double b=this.generalpoint1.y-m*this.generalpoint1.x;
            if ((p.y-m*p.x)-b==0)
            {
                return true;
            }
            else return false;
    }
}