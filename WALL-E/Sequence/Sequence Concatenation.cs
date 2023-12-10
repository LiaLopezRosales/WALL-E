public class Sequence_Concatenation<T>
{
   public GenericSequence<T> right{get;set;}
   public GenericSequence<T> left{get;set;}
   public IEnumerable<T> Result{protected get;set;}

   public Sequence_Concatenation(GenericSequence<T> r,GenericSequence<T> l)
   {
      right=r;
      left=l;
      Result=GenerateNewSequence(right,left);
   }

   private IEnumerable<T> GenerateNewSequence(GenericSequence<T> r,GenericSequence<T> l)
   {
      foreach (T item in r.Sequence!)
      {
        yield return item;
      }
      foreach (T item in l.Sequence!)
      {
        yield return item;
      }
   }
}