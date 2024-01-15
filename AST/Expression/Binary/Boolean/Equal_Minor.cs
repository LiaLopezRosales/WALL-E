using System.Globalization;
public class Equal_Minor:Binary
{
    public Equal_Minor()
    {}
    public override ExpressionType Type { get => Type=ExpressionType.Bool; set => Type=ExpressionType.Bool; }

    public override object? Value { get => base.Value; set => base.Value = value; }
    public override void Evaluate(object left,object right)
    {if (left is double && right is double)
        {
            if (Convert.ToDouble(left, CultureInfo.InvariantCulture) <= Convert.ToDouble(right, CultureInfo.InvariantCulture))
            {
                Value = 1;
            }
            else
            {
                Value = 0;
            }
        }
        else if (left is Measure && right is Measure)
        {
            if (Measure.Equals((Measure)left, (Measure)right) || !(Measure.GreaterThen((Measure)left, (Measure)right)))
            {
                Value = 1;
            }
            else
            {
                Value = 0;
            }
        }
        else if (left is long && right is long)
        {
            if (Convert.ToDouble(left, CultureInfo.InvariantCulture) <= Convert.ToDouble(right, CultureInfo.InvariantCulture))
            {
                Value = 1;
            }
            else
            {
                Value = 0;
            }
        }
    }
    public override string ToString()
    {
        if (Value==null)
        {
            return String.Format("({0}<={1})",Left,Right);
        }
        return Value.ToString()!;
    }
}