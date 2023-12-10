using System.Globalization;
public class Division:Binary
{
    public Division()
    {}
    public override ExpressionType Type { get => base.Type; set => base.Type = value; }

    public override object? Value { get => base.Value; set => base.Value = value; }
    public override void Evaluate(object left,object right)
    {
        if(left is double && right is double)
        {
        Value = Convert.ToDouble(left,CultureInfo.InvariantCulture) / Convert.ToDouble(right,CultureInfo.InvariantCulture);
        }
        else if (left is Measure && right is Measure)
        {
            Value=((Measure)left).Division((Measure)right);
        }
    }
    public override string ToString()
    {
        if (Value==null)
        {
            return String.Format("({0}/{1})",Left,Right);
        }
        return Value.ToString()!;
    }
}