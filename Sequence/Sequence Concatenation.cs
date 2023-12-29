public class Sequence_Concatenation<T>
{   //Concatenar secuencias no es más que recorrer la secuencia de la primera y cuando acabe(si acaba) recorrer la segunda 
    public AbsSequence right { get; set; }
    public AbsSequence left { get; set; }
    public long count { get; protected set; }
    public IEnumerable<T> Result { get; protected set; }

    public Sequence_Concatenation(AbsSequence r, AbsSequence l)
    {
        right = r;
        left = l;
        Result = GenerateNewSequence((GenericSequence<T>)right, (GenericSequence<T>)left);
        if (left.count < 0 || right.count < 0)
        {
            count = -1;
        }
        else
        {
            count = left.count + right.count;
        }
    }
    //Sobrecarga para cuando se suma una secuencia con undefined
    public Sequence_Concatenation(AbsSequence l, string undefined)
    {
        left = l;
        right = new Finite_Sequence<object>(new List<object>());
        Result = GenerateNewSequence((GenericSequence<T>)right, (GenericSequence<T>)left);
        if (left.count < 0 || right.count < 0)
        {
            count = -1;
        }
        else
        {
            count = left.count + right.count;
        }
    }

    private IEnumerable<T> GenerateNewSequence(GenericSequence<T> r, GenericSequence<T> l)
    {
        if (r is Finite_Sequence<T>)
        {
            foreach (T item in ((Finite_Sequence<T>)r).Sequence!)
            {
                yield return item;
            }
        }
        else
        {
            foreach (T item in r.Sequence!)
            {
                yield return item;
            }
        }
        if (l is Finite_Sequence<T>)
        {
            foreach (T item in ((Finite_Sequence<T>)l).Sequence!)
            {
                yield return item;
            }
        }
        else
        {
            foreach (T item in l.Sequence!)
            {
                yield return item;
            }
        }
    }
}