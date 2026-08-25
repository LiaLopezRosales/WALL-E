using Wall_E.Domain;

namespace Wall_E.Application.Interfaces;

/// <summary>Contract for lexical analysis of DSL source code.</summary>
public interface ILexer
{
    /// <summary>Lexical errors produced during analysis.</summary>
    List<Error> Errors { get; }
    /// <summary>Performs lexical analysis on the source and returns the resulting token/AST list.</summary>
    List<Node> Lex(string source, string file);
}
