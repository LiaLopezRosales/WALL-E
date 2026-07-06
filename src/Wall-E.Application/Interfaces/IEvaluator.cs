using Wall_E.Domain;

namespace Wall_E.Application.Interfaces;

public interface IEvaluator
{
    IReadOnlyList<Error> Errors { get; }
    CancellationToken CancellationToken { get; set; }
    EvaluationResult Evaluate(Node node);
    void SetLine(string line);
    void SetCurrentScope(Scope scope);
}
