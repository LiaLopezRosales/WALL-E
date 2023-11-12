public class Error
{
    public ErrorCode Code{get;set;}
    public string Argument{get;set;}
    public TypeError type{get;set;}
    public enum ErrorCode{None,Expected,Invalid,Unknown}
    public enum TypeError{Lexical_Error,Syntactic_Error,Semantic_Error}
    public Error(TypeError type,ErrorCode code,string argument)
    {
        this.type=type;
        this.Code =code;
        this.Argument=argument;
    }

    public override string ToString()
    {
        return String.Format("!{0}: {1} {2}",type,Code,Argument);
    }
}