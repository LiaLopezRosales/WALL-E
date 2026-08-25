using Wall_E.Domain;
using Xunit;

namespace Wall_E.Application.Tests;

public class RgbColorTests
{
    [Fact]
    public void Rgb_sets_color_correctly()
    {
        var p = DslRunner.Run("rgb(255, 0, 0);");
        Assert.Empty(p.Errors);
        Assert.Equal("#FF0000", p.Scene.CurrentColor);
    }

    [Fact]
    public void Rgb_with_expressions()
    {
        var p = DslRunner.Run("rgb(128+127, 10*10, 255/2);");
        Assert.Empty(p.Errors);
        Assert.Equal("#FF6480", p.Scene.CurrentColor);
    }

    [Fact]
    public void Rgba_sets_color_with_alpha()
    {
        var p = DslRunner.Run("rgba(0, 255, 0, 0.5);");
        Assert.Empty(p.Errors);
        Assert.Equal("#00FF0080", p.Scene.CurrentColor);
    }

    [Fact]
    public void Rgba_full_opacity()
    {
        var p = DslRunner.Run("rgba(255, 0, 255, 1.0);");
        Assert.Empty(p.Errors);
        Assert.Equal("#FF00FFFF", p.Scene.CurrentColor);
    }

    [Fact]
    public void Rgb_clamps_values()
    {
        var p = DslRunner.Run("rgb(300, -10, 128);");
        Assert.Empty(p.Errors);
        Assert.Equal("#FF0080", p.Scene.CurrentColor);
    }

    [Fact]
    public void Rgb_draw_with_color()
    {
        var p = DslRunner.Run("rgb(0, 0, 255); draw point(0,0);");
        Assert.Empty(p.Errors);
        Assert.Equal("#0000FF", p.Scene.CurrentColor);
    }
}
