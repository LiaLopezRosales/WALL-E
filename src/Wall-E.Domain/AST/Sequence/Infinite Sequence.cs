namespace Wall_E.Domain;

/// <summary>An unbounded integer sequence exposed through the bounded MaxElements guard.</summary>
public class Infinite_Sequence : GenericSequence<long>
{
    //La secuencia infinita habitual debe ser entera pues, de manera similar a la acotada, avanza de uno en uno.
    /// <summary>The first value yielded by the sequence.</summary>
    public long StartsAd { get; set; }

    /// <summary>Creates an infinite integer sequence starting at the given value.</summary>
    public Infinite_Sequence(long start)
    {
        StartsAd = start;
        count = -1;
        Sequence = GenerateSequence(StartsAd).Take(MaxElements);
        enumerator = Sequence.GetEnumerator();
    }

    /// <summary>Creates an infinite integer sequence from the given enumerable.</summary>
    public Infinite_Sequence(IEnumerable<long> seq)
    {
        Sequence = seq.Take(MaxElements);
        count = -1;
        enumerator = Sequence.GetEnumerator();
    }

    private IEnumerable<long> GenerateSequence(long start)
    {
        long i = 0;
        while ((start + i) < long.MaxValue)
        {
            yield return start + i;
            i++;
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
    public override string ToString() => string.Format("Infinite Sequence of numbers of type long");
}

//Se definen dos clases alternativas que también son secuencias infinitas.
//Estas son resultado de funciones del lenguaje y no pueden ser declaradas manualmente.

/// <summary>An unbounded sequence of points produced by generator functions.</summary>
public class InfinitePointSequence : GenericSequence<Point>
{
    /// <summary>The first point yielded by the sequence.</summary>
    public Point StartsAd { get; set; }

    /// <summary>Creates an infinite point sequence starting at the given point.</summary>
    public InfinitePointSequence(Point start)
    {
        StartsAd = start;
        count = -1;
        Sequence = GenerateSequence(StartsAd).Take(MaxElements);
        enumerator = Sequence.GetEnumerator();
    }

    /// <summary>Creates an infinite point sequence from the given enumerable.</summary>
    public InfinitePointSequence(IEnumerable<Point> s)
    {
        StartsAd = new Point(1.3, 2.01);
        count = -1;
        Sequence = s.Take(MaxElements);
        enumerator = Sequence.GetEnumerator();
    }

    /// <summary>Creates an infinite point sequence from the given enumerable with an explicit starting point.</summary>
    public InfinitePointSequence(IEnumerable<Point> s, Point initial)
    {
        StartsAd = initial;
        count = -1;
        Sequence = s.Take(MaxElements);
        enumerator = Sequence.GetEnumerator();
    }

    private IEnumerable<Point> GenerateSequence(Point start)
    {
        long i = 0;
        while ((start.x + i) < long.MaxValue)
        {
            yield return new Point(start.x + i, start.y + i);
            i++;
        }
    }

    /// <summary>Returns the next value, or the default point once exhausted.</summary>
    public override Point ReturnValue()
    {
        if (enumerator.MoveNext())
        {
            return enumerator.Current;
        }
        else
        {
            return default(Point)!;
        }
    }

    /// <summary>Returns a human-readable description of the sequence.</summary>
    public override string ToString() => string.Format("Infinite Sequence of Points");
}

/// <summary>An unbounded sequence of doubles produced by generator functions.</summary>
public class InfiniteDoubleSequence : GenericSequence<double>
{
    /// <summary>The first value yielded by the sequence.</summary>
    public double StartsAd { get; set; }

    /// <summary>Creates an infinite double sequence from the given enumerable.</summary>
    public InfiniteDoubleSequence(IEnumerable<double> s)
    {
        StartsAd = 0.5;
        count = -1;
        Sequence = s.Take(MaxElements);
        enumerator = Sequence.GetEnumerator();
    }

    /// <summary>Creates an infinite double sequence starting at the given value.</summary>
    public InfiniteDoubleSequence(double start)
    {
        StartsAd = start;
        count = -1;
        Sequence = GenerateSequence(start).Take(MaxElements);
        enumerator = Sequence.GetEnumerator();
    }

    private IEnumerable<double> GenerateSequence(double start)
    {
        double i = 0;
        while ((start + i) < long.MaxValue)
        {
            yield return start + i;
            i++;
        }
    }

    /// <summary>Returns the next value, or long.MinValue once exhausted.</summary>
    public override double ReturnValue()
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
    public override string ToString() => string.Format("Infinite Sequence of Doubles");
}