public abstract class Expression:Node
{
    public Expression()
    {}
    public abstract ExpressionType Type {get;set;}

    public abstract object? Value{get;set;}

    public abstract void Evaluate(object left,object right);
    public abstract void Evaluate(object condition,object If,object Else);

    public enum ExpressionType{Bool,Number,Text,Other,Conditional,Let_In}
}