namespace Wall_E.Application.Interfaces;

public interface ILexer
{
    List<Error> Errors { get; }
    List<Node> Lex(string source, string file);
}
