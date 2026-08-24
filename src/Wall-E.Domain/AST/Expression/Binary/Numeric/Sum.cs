namespace Wall_E.Domain;
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
        else if(left is long && right is long)
        {
        Value = Convert.ToInt64(left,CultureInfo.InvariantCulture) + Convert.ToInt64(right,CultureInfo.InvariantCulture);
        }
        else if (left is Measure && right is Measure)
        {
            Value=((Measure)left).Sum((Measure)right);
        }
            else if (left is AbsSequence && right is AbsSequence)
            {
             if (((left is Enclosed_Infinite_Sequence || left is Infinite_Sequence)) || (right is Enclosed_Infinite_Sequence || right is Infinite_Sequence))
            {
                // GenericSequence<T> hides AbsSequence.count with 'new'; read counts
                // through GenericSequence references or base values return 0.
                IEnumerable<object> Generate(AbsSequence r,AbsSequence l)
                {
                    var gr = (GenericSequence<object>)r;
                    var gl = (GenericSequence<object>)l;
                    long limit = gr.count < 0 ? gr.MaxElements : gr.count;
                    long taken = 0;
                    foreach (object item in gr.Sequence!)
                        if (taken++ >= limit) break; else yield return item;
                    taken = 0;
                    limit = gl.count < 0 ? gl.MaxElements : gl.count;
                    foreach (object item in gl.Sequence!)
                        if (taken++ >= limit) break; else yield return item;
                }
                IEnumerable<object> sum1=Generate((AbsSequence)left,(AbsSequence)right);
                IEnumerable<long> sum=sum1.OfType<long>();
                Value=new Infinite_Sequence((IEnumerable<long>)sum);
            }
            else
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