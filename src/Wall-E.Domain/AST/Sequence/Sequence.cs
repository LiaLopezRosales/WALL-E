namespace Wall_E.Domain;

/// <summary>A generic sequence holding an enumerator over its elements.</summary>
public class GenericSequence<T> : AbsSequence
{
    //Una secuencia genérica tiene además un iterador para obtener sus valores.
    //Si count==-1 su valor es undefined.
    /// <summary>The number of elements (-1 for infinite sequences).</summary>
    public override long count { get; protected set; }
    /// <summary>The underlying enumerable of sequence elements.</summary>
    public IEnumerable<T>? Sequence { get; set; }
    /// <summary>Enumerator used to stream sequence values one at a time.</summary>
    protected IEnumerator<T> enumerator { get; set; }

    /// <summary>Creates an empty generic sequence.</summary>
    public GenericSequence()
    {
        Sequence = new List<T>();
        enumerator = Sequence.GetEnumerator();
    }

    /// <summary>Creates a sequence from the result of concatenating two sequences.</summary>
    public GenericSequence(Sequence_Concatenation<T> concat)
    {
        Sequence = concat.Result;
        enumerator = Sequence.GetEnumerator();
    }

    /// <summary>Returns the next value in the sequence, or the default value once exhausted.</summary>
    public virtual T ReturnValue()
    {
        if (enumerator.MoveNext())
        {
            return enumerator.Current;
        }
        else
        {
            return default(T)!;
        }
    }

    /// <summary>Boxed ReturnValue() — avoids DLR dynamic dispatch.</summary>
    public override object? ReturnValueBoxed() => ReturnValue();
}