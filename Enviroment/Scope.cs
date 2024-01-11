using System;

public class Scope
{
    public Scope? Parent { get; set; }
    public Dictionary<string, object> Variables { get; set; }
    public Dictionary<string, Fuction> TemporalFunctions { get; set; }
    public bool InFunction {get;set;}
    public Scope()
    {
        Variables = new Dictionary<string, object>();
        TemporalFunctions = new Dictionary<string, Fuction>();
        this.Parent = null;
        InFunction=false;
    }

    public Scope Child()
    {
        Scope child = new Scope();
        child.Parent = this;
        foreach (var variable in this.Variables)
        {
            child.Variables.Add(variable.Key, variable.Value);
        }
        foreach (var func in this.TemporalFunctions)
        {
            child.TemporalFunctions.Add(func.Key, func.Value);
        }
        return child;
    }
}
