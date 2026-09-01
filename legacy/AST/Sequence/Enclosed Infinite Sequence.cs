public class Enclosed_Infinite_Sequence:GenericSequence<long>
{   //Una secuencia acotada tiene que ser entera pues avanza de uno en uno desde un valor inicial auno final
    public long StartsAd {get;set;}
    public long EndsAd {get;set;}
    public new IEnumerable<long> Sequence{ get;set;}
    private IEnumerator<long> enumerator{get;set;}
    public Enclosed_Infinite_Sequence(long start,long end)
    {
        StartsAd=start;
        EndsAd=end;
        count=end-start+1;
        Sequence=GenerateSequence(StartsAd,EndsAd);
        enumerator =Sequence.GetEnumerator();
    }

    private IEnumerable<long> GenerateSequence(long start,long end)
    {
        //Hay que incluir los valores de inicio y final a la hora de devolver?
        for (long i = start; i <= end; i++)
        {
            yield return i;
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
    public override string ToString() => string.Format("Enclosed Sequence");
}