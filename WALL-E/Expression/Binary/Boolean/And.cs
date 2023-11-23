public class And:Binary
{
    public And()
    {}
    public override ExpressionType Type { get => base.Type; set => base.Type = value; }

    public override object? Value { get => base.Value; set => base.Value = value; }
    public override void Evaluate(object left,object right)
    {
        if ((bool)left&&(bool)right)
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
            return String.Format("({0}&{1})",Left,Right);
        }
        return Value.ToString()!;
    }
}