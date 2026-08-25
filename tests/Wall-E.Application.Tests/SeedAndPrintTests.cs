using Wall_E.Domain;
using Xunit;

namespace Wall_E.Application.Tests;

public class SeedAndPrintTests
{
    [Fact]
    public void Seed_executes_without_error()
    {
        var p = DslRunner.Run("seed(42);");
        Assert.Empty(p.Errors);
    }

    [Fact]
    public void Seed_does_not_affect_point_draw()
    {
        var p = DslRunner.Run("seed(42); draw point(1, 2);");
        Assert.Empty(p.Errors);
        var d = Assert.IsType<DrawObject>(p.Scene.Snapshot()[0]);
        var pt = Assert.IsType<Point>(d.Figures);
        Assert.Equal(1.0, pt.x);
        Assert.Equal(2.0, pt.y);
    }

    [Fact]
    public void Print_outputs_string_value()
    {
        var p = DslRunner.Run("print(42);");
        Assert.Empty(p.Errors);
        Assert.Single(p.Context.PrintOutput);
        Assert.Equal("42", p.Context.PrintOutput[0]);
    }

    [Fact]
    public void Print_outputs_expression_result()
    {
        var p = DslRunner.Run("print(2 + 3);");
        Assert.Empty(p.Errors);
        Assert.Single(p.Context.PrintOutput);
        Assert.Equal("5", p.Context.PrintOutput[0]);
    }

    [Fact]
    public void Print_can_output_pi()
    {
        var p = DslRunner.Run("print(PI);");
        Assert.Empty(p.Errors);
        Assert.Single(p.Context.PrintOutput);
        Assert.Contains("3.14", p.Context.PrintOutput[0]);
    }
}
