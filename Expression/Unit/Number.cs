public class Number: Unit
{
    public override object? Value { get; set; }
    public Number(double value)
    {
       this.Value=value;
    }
    public override ExpressionType Type 
    { get => ExpressionType.Number; 
     set{}
    }
}