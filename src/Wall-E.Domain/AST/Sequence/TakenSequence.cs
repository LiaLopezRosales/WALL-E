namespace Wall_E.Domain;

/// <summary>A sequence truncated to at most the requested number of elements.</summary>
public class TakenSequence<T> : GenericSequence<T>
{
    /// <summary>Creates a bounded view of the source sequence limited to the given count.</summary>
    public TakenSequence(GenericSequence<T> source, long count)
    {
        this.count = count;
        Sequence = source.Sequence.Take((int)Math.Min(count, int.MaxValue));
        enumerator = Sequence.GetEnumerator();
    }
}
