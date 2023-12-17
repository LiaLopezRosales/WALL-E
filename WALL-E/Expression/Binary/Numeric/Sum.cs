using System.Globalization;
public class Sum:Binary
{
    public Sum()
    {}
    public override ExpressionType Type { get => base.Type; set => base.Type = value; }

    public override object? Value { get => base.Value; set => base.Value = value; }
    public override void Evaluate(object left,object right)
    {
        if(left is double && right is double)
        {
        Value = Convert.ToDouble(left,CultureInfo.InvariantCulture) + Convert.ToDouble(right,CultureInfo.InvariantCulture);
        }
        else if (left is string && right is string)
        {
            Value=left.ToString() + right.ToString();
        }
        else if (left is Measure && right is Measure)
        {
            Value=((Measure)left).Sum((Measure)right);
        }
        else if (left is AbsSequence && right is AbsSequence)
        {
            Sequence_Concatenation<object> sum=new Sequence_Concatenation<object>((AbsSequence)left,(AbsSequence)right);
            if (sum.count<0)
            {
                Value=new Infinite_Sequence((IEnumerable<long>)sum.Result);
            }
            else
            {
                Value=new Finite_Sequence<object>(sum.Result,sum.count);
            }
            
        }
        else if (left is AbsSequence && right is string && (((string)right)=="undefined"))
        {
            Sequence_Concatenation<object> sum=new Sequence_Concatenation<object>((AbsSequence)left,(string)right);
            if (sum.count<0)
            {
                Value=new Infinite_Sequence((IEnumerable<long>)sum.Result);
            }
            else
            {
                Value=new Finite_Sequence<object>(sum.Result,sum.count);
            }
        }
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