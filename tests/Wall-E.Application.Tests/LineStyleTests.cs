using Wall_E.Domain;
using Xunit;

namespace Wall_E.Application.Tests;

public class LineStyleTests
{
    [Fact]
    public void Dashed_sets_line_style()
    {
        var p = DslRunner.Run("dashed; draw point(0,0);");
        Assert.Empty(p.Errors);
        Assert.Equal(LineStyle.Dashed, p.Scene.Snapshot()[0].LineStyle);
    }

    [Fact]
    public void Dotted_sets_line_style()
    {
        var p = DslRunner.Run("dotted; draw point(0,0);");
        Assert.Empty(p.Errors);
        Assert.Equal(LineStyle.Dotted, p.Scene.Snapshot()[0].LineStyle);
    }

    [Fact]
    public void Dashdot_sets_line_style()
    {
        var p = DslRunner.Run("dashdot; draw point(0,0);");
        Assert.Empty(p.Errors);
        Assert.Equal(LineStyle.DashDot, p.Scene.Snapshot()[0].LineStyle);
    }

    [Fact]
    public void Solid_resets_to_default()
    {
        var p = DslRunner.Run("dashed; solid; draw point(0,0);");
        Assert.Empty(p.Errors);
        Assert.Equal(LineStyle.Solid, p.Scene.Snapshot()[0].LineStyle);
    }

    [Fact]
    public void Grosor_sets_stroke_width()
    {
        var p = DslRunner.Run("grosor(3); draw point(0,0);");
        Assert.Empty(p.Errors);
        Assert.Equal(3.0, p.Scene.Snapshot()[0].StrokeWidth);
    }

    [Fact]
    public void Grosor_and_dashed_combine()
    {
        var p = DslRunner.Run("dashed; grosor(2.5); draw point(0,0);");
        Assert.Empty(p.Errors);
        Assert.Equal(LineStyle.Dashed, p.Scene.Snapshot()[0].LineStyle);
        Assert.Equal(2.5, p.Scene.Snapshot()[0].StrokeWidth);
    }

    [Fact]
    public void Default_style_is_solid_width_1()
    {
        var p = DslRunner.Run("draw point(0,0);");
        Assert.Empty(p.Errors);
        Assert.Equal(LineStyle.Solid, p.Scene.Snapshot()[0].LineStyle);
        Assert.Equal(1.0, p.Scene.Snapshot()[0].StrokeWidth);
    }
}
