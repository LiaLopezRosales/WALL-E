using Wall_E.Domain;
using Xunit;
using static Wall_E.Application.Tests.DslRunner;

namespace Wall_E.Application.Tests;

// Sequence behavior after the Lot 4 migration (typed SequenceResult everywhere).
// The pre-migration KNOWN_BUG tests (ToString FormatException crash, count-blind
// bridge) were replaced by these assertions once the bugs were fixed.
public class SequenceCharacterizationTests
{
    [Fact]
    public void Finite_sequence_statement_returns_typed_sequence()
    {
        var pipeline = Run("{1,2,3};");
        Assert.Empty(pipeline.Errors);
        var result = Assert.IsType<SequenceResult>(pipeline.Context.Results[0]);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void Concat_result_exposes_correct_count_through_WrapResult()
    {
        // Regression for the property-shadowing debt: WrapResult used to read
        // AbsSequence.count (never assigned, always 0) when wrapping sequences
        // produced by expression nodes like Sum.
        var pipeline = Run("{1,2} + undefined;");
        Assert.Empty(pipeline.Errors);
        var result = Assert.IsType<SequenceResult>(pipeline.Context.Results[0]);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Count_of_finite_sequence()
    {
        var pipeline = Run("count({1,2,3});");
        Assert.Empty(pipeline.Errors);
        var result = Assert.IsType<NumberResult>(pipeline.Context.Results[0]);
        Assert.Equal(3.0, result.Value);
    }

    [Fact]
    public void Count_of_empty_sequence_is_zero()
    {
        var pipeline = Run("count({});");
        Assert.Empty(pipeline.Errors);
        Assert.Equal(0.0, Assert.IsType<NumberResult>(pipeline.Context.Results[0]).Value);
    }

    [Fact]
    public void Infinite_sequence_statement_returns_sequence()
    {
        // Deliberate fix vs legacy: integral doubles are accepted as start value
        // (the lexer only produces doubles; legacy required long and always errored).
        var pipeline = Run("{1...};");
        Assert.Empty(pipeline.Errors);
        Assert.IsType<SequenceResult>(pipeline.Context.Results[0]);
    }

    [Fact]
    public void Infinite_sequence_is_capped_at_MaxElements()
    {
        var pipeline = Run("{1...};");
        var result = Assert.IsType<SequenceResult>(pipeline.Context.Results[0]);
        var seq = Assert.IsAssignableFrom<AbsSequence>(result.Value);
        Assert.Equal(AbsSequence.DefaultMaxElements, seq.MaxElements);
    }

    [Fact]
    public void Enclosed_infinite_sequence_count()
    {
        var pipeline = Run("count({1...100});");
        Assert.Empty(pipeline.Errors);
        Assert.Equal(100.0, Assert.IsType<NumberResult>(pipeline.Context.Results[0]).Value);
    }

    [Fact]
    public void Non_integral_bounds_report_error()
    {
        var pipeline = Run("count({1.5...100});");
        Assert.NotEmpty(pipeline.Errors);
    }

    [Fact]
    public void Heterogeneous_finite_sequence_reports_error()
    {
        var pipeline = Run("count({1, \"a\"});");
        Assert.NotEmpty(pipeline.Errors);
    }

    [Fact]
    public void Concat_two_finite_sequences_counts_elements()
    {
        var pipeline = Run("count({1,2} + {3,4});");
        Assert.Empty(pipeline.Errors);
        Assert.Equal(4.0, Assert.IsType<NumberResult>(pipeline.Context.Results[0]).Value);
    }

    [Fact]
    public void Concat_with_undefined_returns_first_sequence()
    {
        // Documented DSL quirk preserved: {seq} + undefined returns seq unchanged.
        var pipeline = Run("count({1,2} + undefined);");
        Assert.Empty(pipeline.Errors);
        Assert.Equal(2.0, Assert.IsType<NumberResult>(pipeline.Context.Results[0]).Value);
    }

    [Fact]
    public void Stored_intersect_result_can_be_counted()
    {
        var pipeline = Run("l = line(point(-5,0), point(5,0)); c = circle(point(0,0),3); i = intersect(c, l); count(i);");
        Assert.Empty(pipeline.Errors);
        Assert.Equal(2.0, Assert.IsType<NumberResult>(pipeline.Context.Results[3]).Value);
    }

    [Fact]
    public void Randoms_samples_points_return_sequences()
    {
        var pipeline = Run("randoms(); samples(); points(circle(point(0,0),5));");
        Assert.Empty(pipeline.Errors);
        Assert.All(pipeline.Context.Results, r => Assert.IsType<SequenceResult>(r));
    }

    [Fact]
    public void Points_function_requires_circle_argument()
    {
        var pipeline = Run("points(5);");
        Assert.NotEmpty(pipeline.Errors);
    }

    [Fact]
    public void Point_sequence_declaration_creates_random_points()
    {
        var pipeline = Run("point sequence ps;");
        Assert.Empty(pipeline.Errors);
        Assert.InRange(pipeline.Figures.ExistingPoints.Count, 1, 30);
    }
}
