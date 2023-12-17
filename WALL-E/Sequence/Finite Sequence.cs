public class Finite_Sequence<T>:GenericSequence<T>
{
    public List<T> values{get;set;}
    public new IEnumerable<T> Sequence{get;set;}
    public SeqType type{get;set;}
    private IEnumerator<T> enumerator{get;set;}
    //var enumerator= Sequence.GetEnumerator();
    
    public enum SeqType{number,text,circle,line,point,segment,ray,arc,sequence,no_declared,other}

    public Finite_Sequence(List<T> items)
    {
      values=items;
      count=values.Count;
      Sequence=GenerateSequence(values);
      type=SeqType.no_declared;
      enumerator=Sequence.GetEnumerator();
    }
    public Finite_Sequence(IEnumerable<T> seq,long c)
    {
      values=new List<T>();
      Sequence=seq;
      count=c;
      enumerator=Sequence.GetEnumerator();
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