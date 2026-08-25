using Xunit;

namespace Wall_E.Domain.Tests;

public class ColorTableTests
{
    [Theory]
    [InlineData("red", "#FF0000")]
    [InlineData("blue", "#0000FF")]
    [InlineData("white", "#FFFFFF")]
    [InlineData("black", "#000000")]
    public void TryGetHex_returns_correct_hex(string name, string expected)
    {
        Assert.True(ColorTable.TryGetHex(name, out var hex));
        Assert.Equal(expected, hex);
    }

    [Theory]
    [InlineData("RED")]
    [InlineData("Red")]
    [InlineData("BLUE")]
    public void TryGetHex_is_case_insensitive(string name)
    {
        Assert.True(ColorTable.TryGetHex(name, out _));
    }

    [Fact]
    public void TryGetHex_returns_false_for_unknown()
    {
        Assert.False(ColorTable.TryGetHex("notacolor", out _));
    }

    [Fact]
    public void Resolve_returns_hex_for_known_name()
    {
        Assert.Equal("#FF0000", ColorTable.Resolve("red"));
    }

    [Fact]
    public void Resolve_passes_through_hex()
    {
        Assert.Equal("#00FF00", ColorTable.Resolve("#00FF00"));
    }

    [Fact]
    public void AllNames_is_not_empty()
    {
        Assert.True(ColorTable.AllNames.Count > 100);
    }

    [Fact]
    public void AllNames_are_lowercase()
    {
        foreach (var name in ColorTable.AllNames)
            Assert.Equal(name, name.ToLowerInvariant());
    }
}
