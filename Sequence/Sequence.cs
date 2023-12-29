public class GenericSequence<T>:AbsSequence
{ //Una secuencia genérica tiene además un iterador para obtener sus valores
  //Si count==-1 su valor es undefined
  public new long count{get;protected set;}
  public new IEnumerable<T>? Sequence {get;set;}
  private IEnumerator<T> enumerator{get;set;}
  public GenericSequence()
  {
    Sequence=new List<T>();
    enumerator =Sequence.GetEnumerator();
  }
  //Se puede hacer una secuencia recibiendo la concatenación de dos secuencias 
  public GenericSequence(Sequence_Concatenation<T> concat)
  {
    Sequence=concat.Result;
    enumerator =Sequence.GetEnumerator();
  }
  //Retorna el sigt valor de la secuencia si no existen más valores se devuelve un valor por defecto
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