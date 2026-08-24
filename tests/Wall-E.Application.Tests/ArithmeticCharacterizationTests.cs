using Wall_E.Domain;
using Xunit;
using static Wall_E.Application.Tests.DslRunner;

namespace Wall_E.Application.Tests;

public class ArithmeticCharacterizationTests
{
    [Fact]
    public void Sum_of_two_numbers()
    {
        var pipeline = Run("5 + 3;");
        Assert.Empty(pipeline.Errors);
        var result = Assert.IsType<NumberResult>(pipeline.Context.Results[0]);
        Assert.Equal(8.0, result.Value);
    }

    [Fact]
    public void Multiplication_binds_before_sum()
    {
        var pipeline = Run("2 + 3 * 4;");
        Assert.Empty(pipeline.Errors);
        var result = Assert.IsType<NumberResult>(pipeline.Context.Results[0]);
        Assert.Equal(14.0, result.Value);
    }

    [Fact]
    public void Parentheses_override_precedence()
    {
        var pipeline = Run("(2 + 3) * 4;");
        Assert.Empty(pipeline.Errors);
        var result = Assert.IsType<NumberResult>(pipeline.Context.Results[0]);
        Assert.Equal(20.0, result.Value);
    }

    [Fact]
    public void Division_produces_double()
    {
        var pipeline = Run("10 / 4;");
        Assert.Empty(pipeline.Errors);
        var result = Assert.IsType<NumberResult>(pipeline.Context.Results[0]);
        Assert.Equal(2.5, result.Value);
    }

    [Fact]
    void Sin_of_zero_is_zero()
    {
        var pipeline = Run("sin(0);");
        Assert.Empty(pipeline.Errors);
        var result = Assert.IsType<NumberResult>(pipeline.Context.Results[0]);
        Assert.Equal(0.0, result.Value);
    }

    [Fact]
    void Cos_of_zero_is_one()
    {
        var pipeline = Run("cos(0);");
        Assert.Empty(pipeline.Errors);
        var result = Assert.IsType<NumberResult>(pipeline.Context.Results[0]);
        Assert.Equal(1.0, result.Value);
    }

    [Fact]
    void Sqrt_of_nine_is_three()
    {
        var pipeline = Run("sqrt(9);");
        Assert.Empty(pipeline.Errors);
        var result = Assert.IsType<NumberResult>(pipeline.Context.Results[0]);
        Assert.Equal(3.0, result.Value);
    }

    [Fact]
    void PI_constant()
    {
        var pipeline = Run("PI;");
        Assert.Empty(pipeline.Errors);
        var result = Assert.IsType<NumberResult>(pipeline.Context.Results[0]);
        Assert.Equal(Math.PI, result.Value);
    }
}
