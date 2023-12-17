public class GenericSequence<T>:AbsSequence
{
  //Si count==-1 su valor es undefined
  public new long count{get;protected set;}
  public new IEnumerable<T>? Sequence {get;set;}
  private IEnumerator<T> enumerator{get;set;}
  public GenericSequence()
  {
    Sequence=new List<T>();
    enumerator =Sequence.GetEnumerator();
  }
  public GenericSequence(Sequence_Concatenation<T> concat)
  {
    Sequence=concat.Result;
    enumerator =Sequence.GetEnumerator();
  }
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
}