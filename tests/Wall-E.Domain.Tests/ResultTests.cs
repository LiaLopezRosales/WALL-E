using Xunit;

namespace Wall_E.Domain.Tests;

public class ResultTests
{
    [Fact]
    public void Ok_creates_success()
    {
        var r = Result<string, string>.Ok("hello");
        Assert.True(r.IsSuccess);
        Assert.Equal("hello", r.Value);
    }

    [Fact]
    public void Fail_creates_error()
    {
        var r = Result<string, string>.Fail("boom");
        Assert.False(r.IsSuccess);
        Assert.True(r.IsError);
        Assert.Equal("boom", r.Error);
    }

    [Fact]
    public void Value_throws_on_error()
    {
        var r = Result<int, string>.Fail("err");
        Assert.Throws<InvalidOperationException>(() => r.Value);
    }

    [Fact]
    public void Error_throws_on_success()
    {
        var r = Result<int, string>.Ok(42);
        Assert.Throws<InvalidOperationException>(() => r.Error);
    }

    [Fact]
    public void Map_transforms_value()
    {
        var r = Result<int, string>.Ok(5);
        var mapped = r.Map(x => x * 2);
        Assert.True(mapped.IsSuccess);
        Assert.Equal(10, mapped.Value);
    }

    [Fact]
    public void Map_propagates_error()
    {
        var r = Result<int, string>.Fail("err");
        var mapped = r.Map(x => x * 2);
        Assert.False(mapped.IsSuccess);
    }

    [Fact]
    public void Bind_chains_operations()
    {
        var r = Result<int, string>.Ok(3);
        var bound = r.Bind(x => Result<string, string>.Ok($"result={x}"));
        Assert.True(bound.IsSuccess);
        Assert.Equal("result=3", bound.Value);
    }

    [Fact]
    public void Bind_propagates_error()
    {
        var r = Result<int, string>.Fail("err");
        var bound = r.Bind(x => Result<string, string>.Ok("ok"));
        Assert.False(bound.IsSuccess);
    }

    [Fact]
    public void ValueOr_returns_fallback_on_error()
    {
        var r = Result<int, string>.Fail("err");
        Assert.Equal(99, r.ValueOr(99));
    }

    [Fact]
    public void ValueOr_returns_value_on_success()
    {
        var r = Result<int, string>.Ok(42);
        Assert.Equal(42, r.ValueOr(0));
    }

    [Fact]
    public void Deconstruct_exposes_fields()
    {
        var r = Result<int, string>.Ok(7);
        var (success, value, error) = r;
        Assert.True(success);
        Assert.Equal(7, value);
        Assert.Null(error);
    }

    [Fact]
    public void Deconstruct_error_exposes_error()
    {
        var r = Result<int, string>.Fail("oops");
        var (success, value, error) = r;
        Assert.False(success);
        Assert.Equal(default(int), value);
        Assert.Equal("oops", error);
    }
}
