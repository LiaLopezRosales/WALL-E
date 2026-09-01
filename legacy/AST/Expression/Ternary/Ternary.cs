public class Ternary : Expression
{
    public Node? Condition { get; set; }
    public Node? If_True { get; set; }
    public Node? If_False { get; set; }

    public Ternary()
    { }

    public override ExpressionType Type { get => Type = ExpressionType.Conditional; set => Type = ExpressionType.Conditional; }
    public override object? Value { get; set; }
    public override void Evaluate(object condition, object If, object Else)
    {
        if (CheckTrueORFalse.Check(condition))
        {
            Value = If;
        }
        else
        {
            Value = Else;
        }
    }
    public override void Evaluate(object left, object right)
    {
        throw new NotImplementedException();

    }
}