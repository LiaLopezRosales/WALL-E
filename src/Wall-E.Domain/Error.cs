namespace Wall_E.Domain;

/// <summary>Represents a diagnostic error raised by the lexer, parser, or evaluator.</summary>
public class Error
{
    /// <summary>The error code classifying the failure.</summary>
    public ErrorCode Code { get; set; }
    /// <summary>The offending token or message detail.</summary>
    public string Argument { get; set; }
    /// <summary>The source location where the error occurred.</summary>
    public Location location { get; set; }

    /// <summary>Enumerates the error severity codes.</summary>
    public enum ErrorCode { None, Expected, Invalid, Unknown }
    /// <summary>Enumerates the pipeline phase that produced the error.</summary>
    public enum TypeError { Lexical_Error, Syntactic_Error, Semantic_Error }

    /// <summary>Creates an error with the given phase, code, detail, and location.</summary>
    public Error(TypeError type, ErrorCode code, string argument, Location location)
    {
        //type is not persisted; see Code.
        this.Code = code;
        this.Argument = argument;
        this.location = location;
    }

    /// <summary>Formats the error as a human-readable string.</summary>
    public override string ToString()
    {
        return String.Format("{0}, {1}, {2},", Code, Argument, location);
    }
}