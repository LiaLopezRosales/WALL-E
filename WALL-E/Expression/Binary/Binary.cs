public class Binary:Expression
{   
    public Node? Left{get;set;}
    public Node? Right{get;set;}
    public Binary()
    {}

    public override ExpressionType Type{get;set;}
    public override object? Value{get;set;}
    public override void Evaluate(object left,object right)
    {}
    public override void Evaluate(object condition, object If, object Else)
    {
        throw new NotImplementedException();
    }

}