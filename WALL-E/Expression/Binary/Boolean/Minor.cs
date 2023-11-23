using System.Globalization;
public class Minor:Binary
{
    public Minor()
    {}
    public override ExpressionType Type { get => Type=ExpressionType.Bool; set => Type=ExpressionType.Bool; }

    public override object? Value { get => base.Value; set => base.Value = value; }
    public override void Evaluate(object left,object right)
    {
        if (Convert.ToDouble(left,CultureInfo.InvariantCulture) < Convert.ToDouble(right,CultureInfo.InvariantCulture))
        {
          Value=1;
        }
        else
        {
            Value=0;
        }
    }
    public override string ToString()
    {
        if (Value==null)
        {
            return String.Format("({0}<{1})",Left,Right);
        }
        return Value.ToString()!;
    }
}