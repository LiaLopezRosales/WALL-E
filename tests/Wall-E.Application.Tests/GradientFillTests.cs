using Wall_E.Domain;
using Xunit;

namespace Wall_E.Application.Tests;

public class GradientFillTests
{
    [Fact]
    public void Linear_gradient_on_circle()
    {
        var p = DslRunner.Run("fill linear(red, blue); draw circle(point(0,0), 5);");
        Assert.Empty(p.Errors);
        var d = p.Scene.Snapshot()[0];
        Assert.Equal(FillType.LinearGradient, d.FillType);
        Assert.Equal("#FF0000", d.GradientColor1);
        Assert.Equal("#0000FF", d.GradientColor2);
    }

    [Fact]
    public void Radial_gradient_on_polygon()
    {
        var p = DslRunner.Run("fill radial(yellow, green); draw polygon(point(0,0), 10, 6);");
        Assert.Empty(p.Errors);
        var d = p.Scene.Snapshot()[0];
        Assert.Equal(FillType.RadialGradient, d.FillType);
        Assert.Equal("#FFFF00", d.GradientColor1);
        Assert.Equal("#008000", d.GradientColor2);
    }

    [Fact]
    public void Gradient_with_hex_colors()
    {
        var p = DslRunner.Run("fill linear(#FF0000, #00FF00); draw circle(point(0,0), 3);");
        Assert.Empty(p.Errors);
        var d = p.Scene.Snapshot()[0];
        Assert.Equal(FillType.LinearGradient, d.FillType);
        Assert.Equal("#FF0000", d.GradientColor1);
        Assert.Equal("#00FF00", d.GradientColor2);
    }

    [Fact]
    public void Unfill_after_gradient_clears_gradient()
    {
        var p = DslRunner.Run("fill linear(red, blue); unfill; draw circle(point(0,0), 5);");
        Assert.Empty(p.Errors);
        var d = p.Scene.Snapshot()[0];
        Assert.Equal(FillType.None, d.FillType);
        Assert.False(d.IsFilled);
    }

    [Fact]
    public void Solid_fill_after_gradient_overrides()
    {
        var p = DslRunner.Run("fill linear(red, blue); fill; draw circle(point(0,0), 5);");
        Assert.Empty(p.Errors);
        var d = p.Scene.Snapshot()[0];
        Assert.Equal(FillType.Solid, d.FillType);
        Assert.True(d.IsFilled);
    }

    [Fact]
    public void Gradient_default_is_not_gradient()
    {
        var p = DslRunner.Run("draw circle(point(0,0), 5);");
        Assert.Empty(p.Errors);
        Assert.Equal(FillType.None, p.Scene.Snapshot()[0].FillType);
    }

    [Fact]
    public void Gradient_resolves_css_name_colors()
    {
        var p = DslRunner.Run("fill linear(aqua, fuchsia); draw circle(point(0,0), 3);");
        Assert.Empty(p.Errors);
        var d = p.Scene.Snapshot()[0];
        Assert.Equal(FillType.LinearGradient, d.FillType);
        Assert.Equal("#00FFFF", d.GradientColor1);
        Assert.Equal("#FF00FF", d.GradientColor2);
    }
}
