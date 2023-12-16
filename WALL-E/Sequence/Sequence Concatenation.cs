public class Sequence_Concatenation<T>
{
   public AbsSequence right{get;set;}
   public AbsSequence left{get;set;}
   public long count{get;protected set;}
   public IEnumerable<T> Result{ get;protected set;}

   public Sequence_Concatenation(GenericSequence<T> r,GenericSequence<T> l)
   {
      right=r;
      left=l;
      Result=GenerateNewSequence(right,left);
      if (left.count<0 || right.count<0)
      {
         count=-1;
      }
      else
      {
         count=left.count+right.count;
      }
   }

   private IEnumerable<T> GenerateNewSequence(AbsSequence r,AbsSequence l)
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