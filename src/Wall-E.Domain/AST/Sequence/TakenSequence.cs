namespace Wall_E.Domain;
public class TakenSequence<T> : GenericSequence<T>
{
    public TakenSequence(GenericSequence<T> source, long count)
    {
        this.count = count;
        Sequence = source.Sequence.Take((int)Math.Min(count, int.MaxValue));
        enumerator = Sequence.GetEnumerator();
    }
}
