namespace Wall_E.Application.Interfaces;

/// <summary>
/// Resolves DSL library names to source code. Implemented in Infrastructure
/// (GeoLibraryLoader); injected into the pipeline so Domain's evaluator can
/// process imports without referencing Infrastructure.
/// </summary>
public interface IGeoLibrarySource
{
    /// <summary>Returns the content of the .geo file for the given library name, or null if not found.</summary>
    string? Resolve(string libraryName);
}
