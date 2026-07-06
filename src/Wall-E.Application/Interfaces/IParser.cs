namespace Wall_E.Application.Interfaces;

public interface IParser
{
    List<Error> Errors { get; }
    Node Parse(List<Node> tokens);
}
