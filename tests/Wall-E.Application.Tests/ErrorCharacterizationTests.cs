using Xunit;
using static Wall_E.Application.Tests.DslRunner;

namespace Wall_E.Application.Tests;

public class ErrorCharacterizationTests
{
    [Fact]
    public void Division_by_zero_is_reported()
    {
        var pipeline = Run("5 / 0;");
        Assert.NotEmpty(pipeline.Errors);
    }

    [Fact]
    public void Reassigning_variable_reports_semantic_error()
    {
        var pipeline = Run("x = 5; x = 6;");
        Assert.NotEmpty(pipeline.Errors);
    }

    [Fact]
    public void Unknown_variable_is_reported()
    {
        var pipeline = Run("y;");
        Assert.NotEmpty(pipeline.Errors);
    }

    [Fact]
    public void Count_of_non_sequence_is_reported()
    {
        var pipeline = Run("count(5);");
        Assert.NotEmpty(pipeline.Errors);
    }
}
