namespace Wall_E.Domain;

/// <summary>A finite sequence backed by an explicit list of elements.</summary>
public class Finite_Sequence<T> : GenericSequence<T>
{
    //Una secuencia finita se define a partir de una lista de objetos de un mismo tipo.
    /// <summary>The backing list of elements.</summary>
    public List<T> values { get; set; }
    /// <summary>The declared kind of the elements this sequence holds.</summary>
    public SeqType type { get; set; }

    /// <summary>Enumerates the possible element kinds of a sequence.</summary>
    public enum SeqType { number, text, circle, line, point, segment, ray, arc, sequence, no_declared, other }

    /// <summary>Creates a finite sequence from a list of (homogeneous) elements.</summary>
    public Finite_Sequence(List<T> items)
    {
        values = items;
        count = values.Count;
        Sequence = GenerateSequence(values);
        type = SeqType.no_declared;
        enumerator = Sequence.GetEnumerator();
    }

    /// <summary>Creates a finite sequence from an enumerable with a known count.</summary>
    public Finite_Sequence(IEnumerable<T> seq, long c)
    {
        values = new List<T>();
        Sequence = seq;
        count = c;
        enumerator = Sequence.GetEnumerator();
    }

    private IEnumerable<T> GenerateSequence(List<T> items)
    {
        foreach (T item in items)
        {
            yield return item;
        }
    }

    /// <summary>Returns the next value, or the default once the list is exhausted.</summary>
    public override T ReturnValue()
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

    /// <summary>Returns a human-readable description of the sequence.</summary>
    public override string ToString() => $"Finite Sequence of Type {type}";
}