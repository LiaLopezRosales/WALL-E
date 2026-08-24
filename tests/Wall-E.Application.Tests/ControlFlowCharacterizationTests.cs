using Wall_E.Domain;
using Xunit;
using static Wall_E.Application.Tests.DslRunner;

namespace Wall_E.Application.Tests;

public class ControlFlowCharacterizationTests
{
    [Fact]
    public void Variable_assignment_then_read()
    {
        var pipeline = Run("x = 5; x;");
        Assert.Empty(pipeline.Errors);
        var second = Assert.IsType<NumberResult>(pipeline.Context.Results[1]);
        Assert.Equal(5.0, second.Value);
    }

    [Fact]
    public void If_then_else_takes_then_branch()
    {
        var pipeline = Run("if 1 < 2 then 10 else 20;");
        Assert.Empty(pipeline.Errors);
        var result = Assert.IsType<NumberResult>(pipeline.Context.Results[0]);
        Assert.Equal(10.0, result.Value);
    }

    [Fact]
    public void If_then_else_takes_else_branch()
    {
        var pipeline = Run("if 2 < 1 then 10 else 20;");
        Assert.Empty(pipeline.Errors);
        var result = Assert.IsType<NumberResult>(pipeline.Context.Results[0]);
        Assert.Equal(20.0, result.Value);
    }

    // T2 fix (DEBT_SPRINT.md): let-in previously always failed to parse.
    // Justification for replacing the KNOWN_BUG assertion: the legacy grammar
    // never produced a working let-in, so there was no legacy behavior to
    // preserve — the semantics below are a deliberate design decision:
    //   - body statements separated by ';', closed by 'in <expr>'
    //   - ';' before 'in' is optional (GlobalVar no longer eats it)
    //   - scope variables shadow global constants inside the body
    [Fact]
    public void Let_in_without_semicolon_before_in_evaluates_body()
    {
        var pipeline = Run("let x = 5 in x + 1;");
        Assert.Empty(pipeline.Errors);
        var result = Assert.IsType<NumberResult>(pipeline.Context.Results[0]);
        Assert.Equal(6.0, result.Value);
    }

    [Fact]
    public void Let_in_with_semicolon_before_in_evaluates_body()
    {
        var pipeline = Run("let x = 5; in x + 1;");
        Assert.Empty(pipeline.Errors);
        Assert.Equal(6.0, Assert.IsType<NumberResult>(pipeline.Context.Results[0]).Value);
    }

    [Fact]
    public void Let_in_scope_shadows_global_constant()
    {
        var pipeline = Run("x = 1; let x = 5 in x + 1; x;");
        Assert.Empty(pipeline.Errors);
        Assert.Equal(6.0, Assert.IsType<NumberResult>(pipeline.Context.Results[1]).Value);
        // outer value is preserved after the let scope exits
        Assert.Equal(1.0, Assert.IsType<NumberResult>(pipeline.Context.Results[2]).Value);
    }

    [Fact]
    public void Nested_let_in_expressions()
    {
        var pipeline = Run("let a = (let b = 2 in b) in a * 3;");
        Assert.Empty(pipeline.Errors);
        Assert.Equal(6.0, Assert.IsType<NumberResult>(pipeline.Context.Results[0]).Value);
    }

    [Fact]
    public void Function_definition_and_call()
    {
        var pipeline = Run("f(x) = x + 1; f(5);");
        Assert.Empty(pipeline.Errors);
        var result = Assert.IsType<NumberResult>(pipeline.Context.Results[1]);
        Assert.Equal(6.0, result.Value);
    }
}
