public abstract class GenericSequence<T>
{
  //Si count==-1 su valor es undefined
  public long count{get;protected set;}
  public IEnumerable<T>? Sequence {get;set;}
  public abstract T ReturnValue();
}