using Wall_E.Domain;
using Xunit;

namespace Wall_E.Application.Tests;

public class ColorTableTests
{
    [Fact]
    public void Red_resolves_to_hex()
    {
        Assert.True(ColorTable.TryGetHex("red", out var hex));
        Assert.Equal("#FF0000", hex);
    }

    [Fact]
    public void Case_insensitive_lookup()
    {
        Assert.True(ColorTable.TryGetHex("DARKSLATEBLUE", out var hex));
        Assert.Equal("#483D8B", hex);
    }

    [Fact]
    public void Unknown_name_returns_false()
    {
        Assert.False(ColorTable.TryGetHex("notacolor", out _));
    }

    [Fact]
    public void Resolve_passes_through_hex()
    {
        Assert.Equal("#FF0000", ColorTable.Resolve("#FF0000"));
    }

    [Fact]
    public void Resolve_converts_name()
    {
        Assert.Equal("#FF0000", ColorTable.Resolve("red"));
    }

    [Fact]
    public void Dsl_accepts_css_color_name()
    {
        var p = DslRunner.Run("color coral; draw point(0,0);");
        Assert.Empty(p.Errors);
        Assert.Equal("#FF7F50", p.Scene.UtilizedColors.Peek());
    }

    [Fact]
    public void All_148_css_colors_present()
    {
        Assert.True(ColorTable.AllNames.Count >= 140);
    }
}
