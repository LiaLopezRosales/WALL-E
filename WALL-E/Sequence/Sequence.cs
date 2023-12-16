public class GenericSequence<T>:AbsSequence
{
  //Si count==-1 su valor es undefined
  public new long count{get;protected set;}
  public new IEnumerable<T>? Sequence {get;set;}
  public GenericSequence()
  {
    Sequence=new List<T>();
  }
  public GenericSequence(Sequence_Concatenation<T> concat)
  {
    Sequence=concat.Result;
  }
  public virtual T ReturnValue()
  {
    var enumerator= Sequence!.GetEnumerator();
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