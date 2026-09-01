namespace Wall_E.Domain;
 ﻿using System;

/// <summary>Lexical scope holding variables and temporal functions during evaluation.</summary>
public class Scope
{
    public Scope? Parent { get; set; }
    public Dictionary<string, object> Variables { get; set; }
    public Dictionary<string, Function> TemporalFunctions { get; set; }
    public bool InFunction {get;set;}
    public Scope()
    {
        Variables = new Dictionary<string, object>();
        TemporalFunctions = new Dictionary<string, Function>();
        this.Parent = null;
        InFunction=false;
    }

    /// <summary>Creates a child scope that inherits this scope's variables and functions.</summary>
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
