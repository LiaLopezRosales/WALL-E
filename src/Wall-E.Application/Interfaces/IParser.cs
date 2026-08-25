using Wall_E.Domain;

namespace Wall_E.Application.Interfaces;

/// <summary>Contract for syntactic analysis of parsed tokens into an AST.</summary>
public interface IParser
{
    /// <summary>Syntactic errors produced during parsing.</summary>
    List<Error> Errors { get; }
    /// <summary>Parses the given input nodes and returns the root AST node.</summary>
    Node Parse(List<Node> tokens);
}
