public class Error
{
    public ErrorCode Code{get;set;}
    public string Argument{get;set;}
    //public TypeError type{get;set;}
    public Location location{get;set;}
    public enum ErrorCode{None,Expected,Invalid,Unknown}
    public enum TypeError{Lexical_Error,Syntactic_Error,Semantic_Error}
    public Error(TypeError type,ErrorCode code,string argument,Location location)
    {
        //this.type=type;
        this.Code =code;
        this.Argument=argument;
        this.location=location;
    }

    public override string ToString()
    {
        return String.Format("{0}, {1}, {2},",Code,Argument,location);
    }
}