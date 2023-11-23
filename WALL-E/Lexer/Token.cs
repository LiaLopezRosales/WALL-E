public  class Token
{
    public TokenType Type{get; set;}
    public string Value {get; set;}
    public enum TokenType{sin,cos,sqrt,exp,randoms,points,color_value,samples,log,PI,E,draw,sum,substraction,multiplication,division,power,module,keyword, conditional, symbol,left_bracket,right_bracket,left_key,right_key,low_hyphen,undefined, not, concatenate, Or,And,minor,major,equal_minor,equal_major,equal,diferent, identifier, text , number,point,line,sequence,segment,ray,circle,point_sequence,line_sequence,arc,measure,intersect,count,not_id, EOL,EOF}
    public Token(TokenType type,string value)
    {
      this.Type = type;
      this.Value=value;
    }

    public override string ToString() => string.Format("{0} [{1}]",Type,Value);
}