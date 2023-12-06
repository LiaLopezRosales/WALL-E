public class Infinite_Sequence:GenericSequence<long>
{
    public long StartsAd{get;set;}
    public new IEnumerable<long> Sequence{protected get;set;} 

    public Infinite_Sequence(long start)
    {
       StartsAd=start;
       count=-1;
       Sequence=GenerateSequence(StartsAd);
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
}