using Wall_E.Domain;

namespace Wall_E.Application.Interfaces;

/// <summary>Abstraction over the DSL evaluator, exposing cancellation and context wiring.</summary>
public interface IEvaluator
{
    /// <summary>Semantic errors accumulated while evaluating.</summary>
    IReadOnlyList<Error> Errors { get; }
    /// <summary>Cancellation token honored during long evaluation runs.</summary>
    CancellationToken CancellationToken { get; set; }
    /// <summary>Evaluates a single AST node to an EvaluationResult.</summary>
    EvaluationResult Evaluate(Node node);
    /// <summary>Sets the source line reported by subsequent diagnostics.</summary>
    void SetLine(string line);
    /// <summary>Sets the current scope used for variable/function resolution.</summary>
    void SetCurrentScope(Scope scope);
}
