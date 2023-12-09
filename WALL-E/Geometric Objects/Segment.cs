public class Segment:Figure,IEquatable<Segment>
{
    public Point StartIn{get;set;}
    public Point EndsIn{get;set;}

    public Segment(Point start,Point ends)
    {
        StartIn=start;
        EndsIn=ends;
    }
     
    public void RandomSegment(List<Segment>existingsegments,List<Point>points)
    {
       Random generator=new Random();
        if (existingsegments.Count==0)
        {
            StartIn=new Point(0,0);
            StartIn.RandomPoint(points);
            EndsIn=new Point(0,0);
            EndsIn.RandomPoint(points);
        }
        else
        {
            bool existing=true;
            while (existing)
            {
                existing=false;
                StartIn=existingsegments.Last().StartIn.IncrementCoordinate(generator.NextDouble(1,10),generator.NextDouble(1,10));
                EndsIn=existingsegments.Last().EndsIn.IncrementCoordinate(generator.NextDouble(1,10),generator.NextDouble(1,10));
                foreach (Segment seg in existingsegments)
                {
                    if ((StartIn.Equals(seg!.StartIn)&&EndsIn.Equals(seg!.EndsIn))||(StartIn.Equals(seg!.EndsIn)&&EndsIn.Equals(seg!.StartIn)))
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
    public bool Equals(Segment? s)
    {
        if ((StartIn.Equals(s!.StartIn)&&EndsIn.Equals(s!.EndsIn))||(StartIn.Equals(s!.EndsIn)&&EndsIn.Equals(s!.StartIn)))
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
        double m=(this.EndsIn.y-this.StartIn.y)/(this.EndsIn.x-this.StartIn.x);
            double b=this.StartIn.y-m*this.StartIn.x;
            if ((p.y-m*p.x)-b==0)
            {
                double minX=Math.Min(this.StartIn.x,this.EndsIn.x);
                double maxX=Math.Max(this.StartIn.x,this.EndsIn.x);
                double minY=Math.Min(this.StartIn.y,this.EndsIn.y);
                double maxY=Math.Max(this.StartIn.y,this.EndsIn.y);
                if (p.x>=minX&&p.x<=maxX&&p.y>=minY&&p.y<=maxY)
                {
                    return true;
                }
            }
            return false;
    }
}