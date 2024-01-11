public class Finite_Sequence<T>:GenericSequence<T>
{   //Una secuencia finita se define a partir de una lista de objetos(todos los objetos deben ser del mismo tipo)
    public List<T> values{get;set;}
    public new IEnumerable<T> Sequence{get;set;}
    //Tipo de los objetos
    public SeqType type{get;set;}
    private IEnumerator<T> enumerator{get;set;}
    
    public enum SeqType{number,text,circle,line,point,segment,ray,arc,sequence,no_declared,other}

    public Finite_Sequence(List<T> items)
    {
      values=items;
      count=values.Count;
      Sequence=GenerateSequence(values);
      type=SeqType.no_declared;
      enumerator=Sequence.GetEnumerator();
    }
    //También se puede definir una secuencia finita a partir de un IEnumerable finito
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
    public override string ToString() => string.Format("Finite Sequence of Type {}",T);
}