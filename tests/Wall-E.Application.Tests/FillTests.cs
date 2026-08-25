using Wall_E.Domain;
using Xunit;

namespace Wall_E.Application.Tests;

public class FillTests
{
    [Fact]
    public void Fill_sets_filled_on_circle()
    {
        var p = DslRunner.Run("fill; draw circle(point(0,0), 5);");
        Assert.Empty(p.Errors);
        Assert.True(p.Scene.Snapshot()[0].IsFilled);
    }

    [Fact]
    public void Unfill_sets_outline_on_circle()
    {
        var p = DslRunner.Run("fill; unfill; draw circle(point(0,0), 5);");
        Assert.Empty(p.Errors);
        Assert.False(p.Scene.Snapshot()[0].IsFilled);
    }

    [Fact]
    public void Fill_sets_filled_on_polygon()
    {
        var p = DslRunner.Run("fill; draw polygon(point(0,0), 10, 6);");
        Assert.Empty(p.Errors);
        Assert.True(p.Scene.Snapshot()[0].IsFilled);
    }

    [Fact]
    public void Fill_sets_filled_on_ellipse()
    {
        var p = DslRunner.Run("fill; draw ellipse(point(0,0), 5, 10);");
        Assert.Empty(p.Errors);
        Assert.True(p.Scene.Snapshot()[0].IsFilled);
    }

    [Fact]
    public void Default_is_not_filled()
    {
        var p = DslRunner.Run("draw circle(point(0,0), 5);");
        Assert.Empty(p.Errors);
        Assert.False(p.Scene.Snapshot()[0].IsFilled);
    }

    [Fact]
    public void Fill_with_color_applies_to_figure()
    {
        var p = DslRunner.Run("color red; fill; draw polygon(point(0,0), 10, 3);");
        Assert.Empty(p.Errors);
        var d = p.Scene.Snapshot()[0];
        Assert.True(d.IsFilled);
    }
}
