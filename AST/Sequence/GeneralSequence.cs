public abstract class AbsSequence
{
    public const long DefaultMaxElements = 10000;
    public long count { get; protected set; }
    public IEnumerable<object>? Sequence { get; set; }
    public long MaxElements { get; set; } = DefaultMaxElements;
    public bool IsInfinite => count < 0;
    public bool IsExhausted { get; protected set; }
}