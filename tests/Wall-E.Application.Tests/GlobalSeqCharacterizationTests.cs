using Wall_E.Domain;
using Xunit;
using static Wall_E.Application.Tests.DslRunner;

// Characterization of multiple assignment from sequences (T4, DEBT_SPRINT.md).
// Semantics verified against the running pipeline:
//   - every target except the last consumes ONE element ("undefined" when exhausted)
//   - the LAST target receives ALL remaining elements as a finite sequence
//     ("{}" when exhausted); '_' discards one element without storing
//   - this is why the trailing '_' exists: a,b,_ = {1,2,3} yields plain numbers
public class GlobalSeqCharacterizationTests
{
    [Fact]
    public void Last_target_receives_the_rest_as_a_sequence()
    {
        var pipeline = Run("a,b = {5,6};");
        Assert.Empty(pipeline.Errors);
        Assert.Equal(5.0, pipeline.Context.GlobalConstant["a"]);
        var rest = Assert.IsType<Finite_Sequence<object>>(pipeline.Context.GlobalConstant["b"]);
        Assert.Equal(1, rest.count);
    }

    [Fact]
    public void Low_hyphen_discards_one_element()
    {
        var pipeline = Run("a,_ = {1,2,3};");
        Assert.Empty(pipeline.Errors);
        Assert.Equal(1.0, pipeline.Context.GlobalConstant["a"]);
        Assert.False(pipeline.Context.GlobalConstant.ContainsKey("_"));
    }

    [Fact]
    public void Rest_collects_all_remaining_elements()
    {
        var pipeline = Run("x,y = {7,8,9};");
        Assert.Empty(pipeline.Errors);
        Assert.Equal(7.0, pipeline.Context.GlobalConstant["x"]);
        var rest = Assert.IsType<Finite_Sequence<object>>(pipeline.Context.GlobalConstant["y"]);
        Assert.Equal(2, rest.count);
    }

    [Fact]
    public void Exhausted_sequence_stores_undefined_and_empty_braces()
    {
        var pipeline = Run("p,q = {};");
        Assert.Empty(pipeline.Errors);
        Assert.Equal("undefined", pipeline.Context.GlobalConstant["p"]);
        Assert.Equal("{}", pipeline.Context.GlobalConstant["q"]);
    }

    [Fact]
    public void Non_sequence_rhs_reports_semantic_error()
    {
        // Deliberate divergence from legacy (which silently ignored it), decided in Lot 5.
        var pipeline = Run("a,b = 5;");
        Assert.NotEmpty(pipeline.Errors);
    }

    [Fact]
    public void Infinite_rhs_is_drained_up_to_MaxElements_safely()
    {
        // n consumes 1 element; the last target drains the Take(MaxElements)-capped
        // remainder (10000 - 1). Guards the infinite-sequence safety invariant.
        var pipeline = Run("n,x = {1...};");
        Assert.Empty(pipeline.Errors);
        Assert.Equal(1.0, pipeline.Context.GlobalConstant["n"]);
        var rest = Assert.IsType<Finite_Sequence<object>>(pipeline.Context.GlobalConstant["x"]);
        Assert.False(rest.IsInfinite);
        Assert.Equal((long)(AbsSequence.DefaultMaxElements - 1), rest.count);
    }

    [Fact]
    public void Mixed_skip_and_rest()
    {
        var pipeline = Run("m,_,k = {1,2,3,4};");
        Assert.Empty(pipeline.Errors);
        Assert.Equal(1.0, pipeline.Context.GlobalConstant["m"]);
        var rest = Assert.IsType<Finite_Sequence<object>>(pipeline.Context.GlobalConstant["k"]);
        Assert.Equal(2, rest.count);
    }
}
