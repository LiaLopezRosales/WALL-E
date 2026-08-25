using Wall_E.Domain;
using Xunit;

namespace Wall_E.Application.Tests;

public class LoopTests
{
    [Fact]
    public void Repeat_draws_correct_number_of_times()
    {
        var p = DslRunner.Run("repeat(3) { draw point(0, 0); }");
        Assert.Empty(p.Errors);
        Assert.Equal(3, p.Scene.Snapshot().Count);
    }

    [Fact]
    public void Repeat_zero_times_draws_nothing()
    {
        var p = DslRunner.Run("repeat(0) { draw point(0, 0); }");
        Assert.Empty(p.Errors);
        Assert.Empty(p.Scene.Snapshot());
    }

    [Fact]
    public void Repeat_with_expression_count()
    {
        var p = DslRunner.Run("repeat(1+1) { draw point(1, 1); }");
        Assert.Empty(p.Errors);
        Assert.Equal(2, p.Scene.Snapshot().Count);
    }

    [Fact]
    public void Repeat_can_use_print()
    {
        var p = DslRunner.Run("repeat(2) { print(42); }");
        Assert.Empty(p.Errors);
        Assert.Equal(2, p.Context.PrintOutput.Count);
    }

    [Fact]
    public void Repeat_multiple_statements_in_body()
    {
        var p = DslRunner.Run("repeat(2) { draw point(0, 0); draw point(1, 1); }");
        Assert.Empty(p.Errors);
        Assert.Equal(4, p.Scene.Snapshot().Count);
    }

    [Fact]
    public void For_draws_over_finite_sequence()
    {
        var p = DslRunner.Run("for p in {1, 2, 3} { print(p); }");
        Assert.Empty(p.Errors);
        Assert.Equal(3, p.Context.PrintOutput.Count);
    }

    [Fact]
    public void For_repeat_nested()
    {
        var p = DslRunner.Run("repeat(2) { print(42); }");
        Assert.Empty(p.Errors);
        Assert.Equal(2, p.Context.PrintOutput.Count);
    }
}
