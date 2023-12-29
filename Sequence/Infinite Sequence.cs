public class Infinite_Sequence:GenericSequence<long>
{   //La secuencia infinita habitual debe ser entera pues de similar manera a la acotado avanza de uno en uno
    public long StartsAd{get;set;}
    public new IEnumerable<long> Sequence{ get;set;} 
     private IEnumerator<long> enumerator{get;set;}

    public Infinite_Sequence(long start)
    {
       StartsAd=start;
       count=-1;
       Sequence=GenerateSequence(StartsAd);
       enumerator =Sequence.GetEnumerator();
    }
    public Infinite_Sequence(IEnumerable<long> seq)
    {
        Sequence=seq;
        count=-1;
        enumerator =Sequence.GetEnumerator();
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
//Se definen dos clases alternativas que también son secuencias infinitas
//Estas son resultado de funciones del lenguaje y no pueden ser declaradas manualmente
public class InfinitePointSequence:GenericSequence<Point>
{
    public Point StartsAd{get;set;}
    public new IEnumerable<Point> Sequence{protected get;set;} 
    private IEnumerator<Point> enumerator{get;set;}

    public InfinitePointSequence(Point start)
    {
       StartsAd=start;
       count=-1;
       Sequence=GenerateSequence(StartsAd);
       enumerator =Sequence.GetEnumerator();
    }
    public InfinitePointSequence(IEnumerable<Point> s)
    {
       StartsAd=new Point(1.3,2.01);
       count=-1;
       Sequence=s;
       enumerator =Sequence.GetEnumerator();
    }
    public InfinitePointSequence(IEnumerable<Point>s,Point initial)
    {
        StartsAd=initial;
        count=-1;
        Sequence=s;
        enumerator =Sequence.GetEnumerator();
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
        private IEnumerator<double> enumerator{get;set;}

    public InfiniteDoubleSequence(IEnumerable<double>s)
    {
       StartsAd=0.5;
       count=-1;
       Sequence=s;
       enumerator =Sequence.GetEnumerator();
    }
    public InfiniteDoubleSequence(double start)
    {
       StartsAd=start;
       count=-1;
       Sequence=GenerateSequence(start);
       enumerator =Sequence.GetEnumerator();
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