using System.Globalization;
public class Diferent:Binary
{
    public Diferent()
    {}
    public override ExpressionType Type { get => Type=ExpressionType.Bool; set => Type=ExpressionType.Bool; }

    public override object? Value { get => base.Value; set => base.Value = value; }
    public override void Evaluate(object left,object right)
    {
        if (left is double && right is double)
        {
            if (Convert.ToDouble(left, CultureInfo.InvariantCulture) != Convert.ToDouble(right, CultureInfo.InvariantCulture))
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
            if (!Measure.Equals((Measure)left,(Measure)right))
            {
                Value = 1;
            }
            else
            {
                Value = 0;
            }
        }
         else if(left is string && right is string)
        {
            if(left.ToString() != right.ToString())
            {
                Value=1;
            }
            else Value=0;
        }
        else
        {
            if (!left.Equals(right))
            {
                Value=1;
            }
            else
            {
                Value=0;
            }
        }
         
    }
    public override string ToString()
    {
        if (Value==null)
        {
            return String.Format("({0}!={1})",Left,Right);
        }
        return Value.ToString()!;
    }
}