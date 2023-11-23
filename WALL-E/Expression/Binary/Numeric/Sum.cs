using System.Globalization;
public class Sum:Binary
{
    public Sum()
    {}
    public override ExpressionType Type { get => base.Type; set => base.Type = value; }

    public override object? Value { get => base.Value; set => base.Value = value; }
    public override void Evaluate(object left,object right)
    {
        Value = Convert.ToDouble(left,CultureInfo.InvariantCulture) + Convert.ToDouble(right,CultureInfo.InvariantCulture);
    }
    public override string ToString()
    {
        if (Value==null)
        {
            return String.Format("({0}+{1})",Left,Right);
        }
        return Value.ToString()!;
    }
}