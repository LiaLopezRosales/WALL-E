namespace Wall_E.Application.Interfaces;

public interface IPipeline
{
    List<Error> Errors { get; }
    void Execute(string source, string file);
}
