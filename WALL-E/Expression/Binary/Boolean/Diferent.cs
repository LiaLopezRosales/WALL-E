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
            if (Convert.ToDouble(left,CultureInfo.InvariantCulture) != Convert.ToDouble(right,CultureInfo.InvariantCulture))
        {
          Value=1;
        }
        else
        {
            Value=0;
        }
        }
        //Modificar este para "undefined"
        if(left is string && right is string)
        {
            Value = left.ToString() != right.ToString();
        }
        //Modificar esto para valor {}
        if (left is bool && right is bool)
        {
            Value = (bool)left!=(bool)right;
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