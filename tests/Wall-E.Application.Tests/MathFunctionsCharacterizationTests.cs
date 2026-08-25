using Wall_E.Domain;
using Xunit;

namespace Wall_E.Application.Tests;

public class MathFunctionsCharacterizationTests
{
    [Fact]
    public void Tan_returns_correct_value()
    {
        var p = DslRunner.Run("draw point(tan(PI/4), 0);");
        Assert.Empty(p.Errors);
        var d = Assert.IsType<DrawObject>(p.Scene.Snapshot()[0]);
        var pt = Assert.IsType<Point>(d.Figures);
        Assert.InRange(pt.x, 0.99, 1.01);
    }

    [Fact]
    public void Atan_returns_correct_value()
    {
        var p = DslRunner.Run("draw point(atan(1), 0);");
        Assert.Empty(p.Errors);
        var d = Assert.IsType<DrawObject>(p.Scene.Snapshot()[0]);
        var pt = Assert.IsType<Point>(d.Figures);
        Assert.InRange(pt.x, 0.78, 0.79);
    }

    [Fact]
    public void Abs_of_negative_is_positive()
    {
        var p = DslRunner.Run("draw point(abs(-42), 0);");
        Assert.Empty(p.Errors);
        var d = Assert.IsType<DrawObject>(p.Scene.Snapshot()[0]);
        var pt = Assert.IsType<Point>(d.Figures);
        Assert.Equal(42.0, pt.x);
    }

    [Fact]
    public void Floor_rounds_down()
    {
        var p = DslRunner.Run("draw point(floor(3.7), 0);");
        Assert.Empty(p.Errors);
        var d = Assert.IsType<DrawObject>(p.Scene.Snapshot()[0]);
        var pt = Assert.IsType<Point>(d.Figures);
        Assert.Equal(3.0, pt.x);
    }

    [Fact]
    public void Ceil_rounds_up()
    {
        var p = DslRunner.Run("draw point(ceil(3.2), 0);");
        Assert.Empty(p.Errors);
        var d = Assert.IsType<DrawObject>(p.Scene.Snapshot()[0]);
        var pt = Assert.IsType<Point>(d.Figures);
        Assert.Equal(4.0, pt.x);
    }

    [Fact]
    public void Phi_is_the_golden_ratio()
    {
        var p = DslRunner.Run("draw point(phi, 0);");
        Assert.Empty(p.Errors);
        var d = Assert.IsType<DrawObject>(p.Scene.Snapshot()[0]);
        var pt = Assert.IsType<Point>(d.Figures);
        Assert.InRange(pt.x, 1.618, 1.619);
    }

    [Fact]
    public void Sqrt2_is_root_two()
    {
        var p = DslRunner.Run("draw point(sqrt2, 0);");
        Assert.Empty(p.Errors);
        var d = Assert.IsType<DrawObject>(p.Scene.Snapshot()[0]);
        var pt = Assert.IsType<Point>(d.Figures);
        Assert.InRange(pt.x, 1.414, 1.415);
    }
}
