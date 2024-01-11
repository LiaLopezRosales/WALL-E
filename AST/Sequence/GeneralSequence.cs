public abstract class AbsSequence
{   //Todas las secuencias tienen que definir su cuenta y su valor
    public long count{get;protected set;}
    public IEnumerable<object>? Sequence{get;set;}
}