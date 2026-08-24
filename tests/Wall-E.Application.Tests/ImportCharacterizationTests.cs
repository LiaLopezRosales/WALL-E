using Wall_E.Application.Interfaces;
using Wall_E.Application.Pipeline;
using Wall_E.Domain;
using Xunit;
using static Wall_E.Application.Tests.DslRunner;

namespace Wall_E.Application.Tests;

// Import wiring (T3, DEBT_SPRINT.md). The library source is injected; the fake
// keeps tests independent of Infrastructure. Deliberate choices under test:
// libraries share the main visitor/context, and their internal results do not
// leak into Context.Results.
public class ImportCharacterizationTests
{
    private class FakeGeoLibrarySource : IGeoLibrarySource
    {
        private readonly Dictionary<string, string> _libraries;
        public FakeGeoLibrarySource(Dictionary<string, string> libraries) => _libraries = libraries;
        public string? Resolve(string libraryName) =>
            _libraries.TryGetValue(libraryName, out string? content) ? content : null;
    }

    [Fact]
    public void Import_makes_library_definitions_available()
    {
        var source = new FakeGeoLibrarySource(new Dictionary<string, string>
        {
            ["mylib"] = "f(x) = x * 2; g = 7;"
        });
        var pipeline = Run("import \"mylib.geo\"; f(g);", new PipelineOrchestrator(source));
        Assert.Empty(pipeline.Errors);
        var result = Assert.IsType<NumberResult>(pipeline.Context.Results[1]);
        Assert.Equal(14.0, result.Value);
    }

    [Fact]
    public void Library_internal_results_do_not_leak_into_main_results()
    {
        var source = new FakeGeoLibrarySource(new Dictionary<string, string>
        {
            ["mylib"] = "5 + 5; g = 3;"
        });
        var pipeline = Run("import \"mylib.geo\"; g;", new PipelineOrchestrator(source));
        Assert.Empty(pipeline.Errors);
        // The library's internal "5 + 5" must NOT appear: results are the import
        // statement itself (VoidResult) followed by the main program's statements.
        Assert.Equal(2, pipeline.Context.Results.Count);
        Assert.IsType<VoidResult>(pipeline.Context.Results[0]);
        Assert.Equal(3.0, Assert.IsType<NumberResult>(pipeline.Context.Results[1]).Value);
        Assert.DoesNotContain(pipeline.Context.Results, r => r is NumberResult n && n.Value == 10.0);
    }

    [Fact]
    public void Unknown_library_reports_semantic_error()
    {
        var source = new FakeGeoLibrarySource(new Dictionary<string, string>());
        var pipeline = Run("import \"nope.geo\"; 1;", new PipelineOrchestrator(source));
        Assert.NotEmpty(pipeline.Errors);
        Assert.Contains(pipeline.Errors, e => e.Argument!.Contains("'nope'"));
    }

    [Fact]
    public void Circular_imports_are_detected_and_terminate()
    {
        var source = new FakeGeoLibrarySource(new Dictionary<string, string>
        {
            ["a"] = "import \"b.geo\";",
            ["b"] = "import \"a.geo\";"
        });
        var pipeline = Run("import \"a.geo\"; 2;", new PipelineOrchestrator(source));
        Assert.NotEmpty(pipeline.Errors);
        Assert.Contains(pipeline.Errors, e => e.Argument!.Contains("circular"));
    }

    [Fact]
    public void Without_source_import_keeps_semantic_error()
    {
        var pipeline = Run("import \"mylib.geo\";");
        Assert.NotEmpty(pipeline.Errors);
        Assert.Contains(pipeline.Errors, e => e.Argument == "import requires UI/Infrastructure layer");
    }
}
