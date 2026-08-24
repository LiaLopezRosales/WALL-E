namespace Wall_E.Domain;
public abstract class AbsSequence
{
    public const int DefaultMaxElements = 10000;
    // Single source of truth for element count (-1 = infinite). Declared abstract so
    // every read through AbsSequence-typed references is polymorphically correct;
    // GenericSequence<T> provides the only implementation/storage.
    public abstract long count { get; protected set; }
    public int MaxElements { get; set; } = DefaultMaxElements;
    public bool IsInfinite => count < 0;
}