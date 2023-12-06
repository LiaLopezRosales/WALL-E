public class Scope
{
    public Scope? Parent { get; set; }
    public Dictionary<string, Node> Variables { get; set; }
    public Dictionary<string,Fuction> TemporalFunctions{get;set;}

    public Scope()
    {
        Variables = new Dictionary<string, Node>();
        TemporalFunctions=new Dictionary<string, Fuction>();
        this.Parent = null;
    }

    public Scope Child()
    {
        Scope child = new Scope();
        child.Parent = this;
        return child;
    }
}