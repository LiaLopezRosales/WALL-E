public class Finite_Sequence<T>:GenericSequence<T>
{
    public List<T> values{get;set;}
    public new IEnumerable<T> Sequence{get;set;}

    public Finite_Sequence(List<T> items)
    {
      values=items;
      count=values.Count;
      Sequence=GenerateSequence(values);
    }

    private IEnumerable<T> GenerateSequence(List<T> items)
    {
        foreach (T item in items)
        {
            yield return item;
        }
    }
}