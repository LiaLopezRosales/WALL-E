using Wall_E.Domain;

namespace Wall_E.Application.Interfaces;

/// <summary>Contract for the end-to-end DSL execution pipeline.</summary>
public interface IPipeline
{
    /// <summary>Errors accumulated during pipeline execution.</summary>
    List<Error> Errors { get; }
    /// <summary>Executes the pipeline on the given source code and file name.</summary>
    void Execute(string source, string file);
}
