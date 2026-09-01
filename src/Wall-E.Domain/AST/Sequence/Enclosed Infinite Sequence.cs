namespace Wall_E.Domain;

/// <summary>A bounded integer sequence advancing by one from a start to an end value (inclusive).</summary>
public class Enclosed_Infinite_Sequence : GenericSequence<long>
{
    /// <summary>The inclusive first value of the sequence.</summary>
    public long StartsAd { get; set; }
    /// <summary>The inclusive last value of the sequence.</summary>
    public long EndsAd { get; set; }

    /// <summary>Creates a sequence counting from start to end (inclusive).</summary>
    public Enclosed_Infinite_Sequence(long start, long end)
    {
        StartsAd = start;
        EndsAd = end;
        count = end - start + 1;
        Sequence = GenerateSequence(StartsAd, EndsAd);
        enumerator = Sequence.GetEnumerator();
    }

    private IEnumerable<long> GenerateSequence(long start, long end)
    {
        //Hay que incluir los valores de inicio y final a la hora de devolver.
        for (long i = start; i <= end; i++)
        {
            yield return i;
        }
    }

    /// <summary>Returns the next value, or long.MinValue once exhausted.</summary>
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

    /// <summary>Returns a human-readable description of the sequence.</summary>
    public override string ToString() => string.Format("Enclosed Sequence");
}