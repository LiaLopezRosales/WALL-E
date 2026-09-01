namespace Wall_E.Domain;

/// <summary>Abstract base for all sequences, enforcing the bounded-consumption safety
/// invariant: any read through an AbsSequence-typed reference honors MaxElements.</summary>
public abstract class AbsSequence
{
    /// <summary>Upper bound applied to any unbounded sequence consumption.</summary>
    public const int DefaultMaxElements = 10000;
    /// <summary>Single source of truth for element count (-1 = infinite).</summary>
    public abstract long count { get; protected set; }
    /// <summary>Maximum elements this sequence exposes to consumers.</summary>
    public int MaxElements { get; set; } = DefaultMaxElements;
    /// <summary>Returns true when the sequence has no known finite bound.</summary>
    public bool IsInfinite => count < 0;

    /// <summary>Boxed ReturnValue() — avoids DLR dynamic dispatch.</summary>
    public abstract object? ReturnValueBoxed();
}