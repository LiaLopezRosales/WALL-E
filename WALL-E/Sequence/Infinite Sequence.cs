public class Infinite_Sequence:GenericSequence<long>
{
    public long StartsAd{get;set;}
    public new IEnumerable<long> Sequence{ get;set;} 
    

    public Infinite_Sequence(long start)
    {
       StartsAd=start;
       count=-1;
       Sequence=GenerateSequence(StartsAd);
    }
    public Infinite_Sequence(IEnumerable<long> seq)
    {
        Sequence=seq;
        count=-1;
    }

    private IEnumerable<long> GenerateSequence(long start)
    {   
        long i = 0;
        while ((start+i)<long.MaxValue)
        {
            yield return start + i;
            i++;
        }
    }
     public override long ReturnValue()
    {
       var enumerator= Sequence.GetEnumerator();
       if (enumerator.MoveNext())
       {
         return enumerator.Current;
       }
       else
       {
        return long.MinValue;
       }
       
    }
}
public class InfinitePointSequence:GenericSequence<Point>
{
    public Point StartsAd{get;set;}
    public new IEnumerable<Point> Sequence{protected get;set;} 

    public InfinitePointSequence(Point start)
    {
       StartsAd=start;
       count=-1;
       Sequence=GenerateSequence(StartsAd);
    }
    public InfinitePointSequence(IEnumerable<Point> s)
    {
       StartsAd=new Point(1.3,2.01);
       count=-1;
       Sequence=s;
    }
    public InfinitePointSequence(IEnumerable<Point>s,Point initial)
    {
        StartsAd=initial;
        count=-1;
        Sequence=s;
    }

    private IEnumerable<Point> GenerateSequence(Point start)
    {   
        long i = 0;
        while ((start.x+i)<long.MaxValue)
        {
            yield return new Point(start.x + i,start.y+i);
            i++;
        }
    }
     public override Point ReturnValue()
    {
       var enumerator= Sequence.GetEnumerator();
       if (enumerator.MoveNext())
       {
         return enumerator.Current;
       }
       else
       {
        return default(Point)!;
       }
       
    }
   
}
 public class InfiniteDoubleSequence:GenericSequence<double>
    {
        public double StartsAd{get;set;}
        public new IEnumerable<double> Sequence{protected get;set;} 

    public InfiniteDoubleSequence(IEnumerable<double>s)
    {
       StartsAd=0.5;
       count=-1;
       Sequence=s;
    }

    private IEnumerable<double> GenerateSequence(double start)
    {   
        double i = 0;
        while ((start+i)<long.MaxValue)
        {
            yield return start + i;
            i++;
        }
    }
     public override double ReturnValue()
    {
       var enumerator= Sequence.GetEnumerator();
       if (enumerator.MoveNext())
       {
         return enumerator.Current;
       }
       else
       {
        return long.MinValue;
       }
       
    }
    }