public class Enclosed_Infinite_Sequence:GenericSequence<long>
{
    public long StartsAd {get;set;}
    public long EndsAd {get;set;}
    public new IEnumerable<long> Sequence{protected get;set;}

    public Enclosed_Infinite_Sequence(long start,long end)
    {
        StartsAd=start;
        EndsAd=end;
        count=end-start+1;
        Sequence=GenerateSequence(StartsAd,EndsAd);
    }

    private IEnumerable<long> GenerateSequence(long start,long end)
    {
        //Hay que incluir los valores de inicio y final a la hora de devolver?
        for (long i = start; i <= end; i++)
        {
            yield return i;
        }
    }
}