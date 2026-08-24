using Wall_E.Domain;
using Xunit;
using static Wall_E.Application.Tests.DslRunner;

namespace Wall_E.Application.Tests;

// Characterization of CURRENT sequence behavior (visitor still uses legacy fallback).
//
// KNOWN BUGS captured here on purpose (to be fixed by sequence-migration lots,
// after which these assertions are replaced with correct typed behavior):
//  - Finite_Sequence<T>.ToString() has a malformed format string ("Type {}") and throws
//    FormatException whenever WrapResult stringifies a finite/empty sequence.
//  - The fallback bridge wraps any legacy sequence as StringResult(ToString()), so
//    count() cannot see real sequence types through the bridge.
public class SequenceCharacterizationTests
{
    [Fact]
    public void Finite_sequence_as_statement_crashes_on_ToString_KNOWN_BUG()
    {
        Assert.Throws<FormatException>(() => Run("{1,2,3};"));
    }

    [Fact]
    public void Count_of_finite_sequence_crashes_KNOWN_BUG()
    {
        Assert.Throws<FormatException>(() => Run("count({1,2,3});"));
    }

    [Fact]
    public void Count_of_empty_sequence_crashes_KNOWN_BUG()
    {
        Assert.Throws<FormatException>(() => Run("count({});"));
    }

    [Fact]
    public void Infinite_sequence_statement_reports_error()
    {
        var pipeline = Run("{1...};");
        Assert.NotEmpty(pipeline.Errors);
    }

    [Fact]
    public void Count_of_infinite_sequence_is_not_supported_through_bridge()
    {
        var pipeline = Run("count({1...100});");
        Assert.NotEmpty(pipeline.Errors);
        Assert.IsType<VoidResult>(pipeline.Context.Results[0]);
    }
}
