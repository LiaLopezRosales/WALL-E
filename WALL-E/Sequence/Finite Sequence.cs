public class Finite_Sequence<T>:GenericSequence<T>
{
    public List<T> values{get;set;}
    public new IEnumerable<T> Sequence{get;set;}
    public SeqType type{get;set;}
    public enum SeqType{number,text,circle,line,point,segment,ray,arc,sequence,no_declared,other}

    public Finite_Sequence(List<T> items)
    {
      values=items;
      count=values.Count;
      Sequence=GenerateSequence(values);
      type=SeqType.no_declared;
    }

    private IEnumerable<T> GenerateSequence(List<T> items)
    {
        foreach (T item in items)
        {
            yield return item;
        }
    }
     public override T ReturnValue()
    {
       var enumerator= Sequence.GetEnumerator();
       if (enumerator.MoveNext())
       {
         return enumerator.Current;
       }
       else
       {
        return default(T)!;
       }
       
    }
}