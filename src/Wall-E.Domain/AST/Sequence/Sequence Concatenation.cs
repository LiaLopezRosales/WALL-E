namespace Wall_E.Domain;
public class Sequence_Concatenation<T>
{   //Concatenar secuencias no es más que recorrer la secuencia de la primera y cuando acabe(si acaba) recorrer la segunda 
    public AbsSequence right { get; set; }
    public AbsSequence left { get; set; }
    public long count { get; protected set; }
    public IEnumerable<T> Result { get; protected set; }

    public Sequence_Concatenation(AbsSequence r, AbsSequence l)
    {
        // GenericSequence<T> hides AbsSequence.count with 'new'; reading through
        // AbsSequence-typed references returns the never-assigned base value (0).
        var gr = (GenericSequence<T>)r;
        var gl = (GenericSequence<T>)l;
        right = gr;
        left = gl;
        Result = GenerateNewSequence(gr, gl);
        if (gl.count < 0 || gr.count < 0)
        {
            count = -1;
        }
        else
        {
            count = gl.count + gr.count;
        }
    }
    //Sobrecarga para cuando se suma una secuencia con undefined
    public Sequence_Concatenation(AbsSequence l, string undefined)
    {
        var gl = (GenericSequence<T>)l;
        left = gl;
        right = new Finite_Sequence<object>(new List<object>());
        Result = GenerateNewSequence((GenericSequence<T>)right, gl);
        if (gl.count < 0 || ((GenericSequence<T>)right).count < 0)
        {
            count = -1;
        }
        else
        {
            count = gl.count + ((GenericSequence<T>)right).count;
        }
    }

    private IEnumerable<T> GenerateNewSequence(GenericSequence<T> r, GenericSequence<T> l)
    {
        long limit = r.count < 0 ? r.MaxElements : r.count;
        long taken = 0;
        foreach (T item in r.Sequence!)
            if (taken++ >= limit) break; else yield return item;
        taken = 0;
        limit = l.count < 0 ? l.MaxElements : l.count;
        foreach (T item in l.Sequence!)
            if (taken++ >= limit) break; else yield return item;
    }
}