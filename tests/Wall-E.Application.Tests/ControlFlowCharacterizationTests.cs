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

    // KNOWN LIMITATION: let-in currently reports a syntax error in this form.
    // The Let_In parser loop expects statements terminated before 'in' but the
    // evaluation path is broken (NRE with ';' variant). Revisit during Lot 1/4 migration.
    [Fact]
    public void Let_in_reports_syntax_error_in_current_state()
    {
        var pipeline = Run("let x = 5 in x + 1;");
        Assert.NotEmpty(pipeline.Errors);
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
