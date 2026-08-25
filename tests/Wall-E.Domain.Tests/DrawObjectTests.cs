using Xunit;

namespace Wall_E.Domain.Tests;

public class DrawObjectTests
{
    [Fact]
    public void Default_constructor_sets_defaults()
    {
        var pt = new Point(1, 2);
        var d = new DrawObject(pt, "tag", "red");
        Assert.Equal(LineStyle.Solid, d.LineStyle);
        Assert.Equal(1.0, d.StrokeWidth);
        Assert.Equal(FillType.None, d.FillType);
        Assert.Equal(0, d.Layer);
        Assert.False(d.IsFilled);
    }

    [Fact]
    public void Full_constructor_sets_all()
    {
        var pt = new Point(0, 0);
        var d = new DrawObject(pt, "", "#FF0000", LineStyle.Dashed, 2.5, FillType.Solid, "#FF0000", "#0000FF", 5);
        Assert.Equal(LineStyle.Dashed, d.LineStyle);
        Assert.Equal(2.5, d.StrokeWidth);
        Assert.Equal(FillType.Solid, d.FillType);
        Assert.Equal("#FF0000", d.GradientColor1);
        Assert.Equal("#0000FF", d.GradientColor2);
        Assert.Equal(5, d.Layer);
        Assert.True(d.IsFilled);
    }

    [Fact]
    public void CheckValidType_returns_true_for_point()
    {
        var d = new DrawObject(new Point(0, 0), "", "red");
        Assert.True(d.CheckValidType());
    }

    [Fact]
    public void CheckValidType_returns_true_for_circle()
    {
        var c = new Circle(new Point(0, 0), 5);
        var d = new DrawObject(c, "", "red");
        Assert.True(d.CheckValidType());
    }

    [Fact]
    public void CheckValidType_returns_false_for_string()
    {
        var d = new DrawObject("invalid", "", "red");
        Assert.False(d.CheckValidType());
    }

    [Fact]
    public void CheckValidType_returns_true_for_line()
    {
        var l = new Line(new Point(0, 0), new Point(1, 1));
        var d = new DrawObject(l, "", "red");
        Assert.True(d.CheckValidType());
    }

    [Fact]
    public void CheckValidType_returns_true_for_segment()
    {
        var s = new Segment(new Point(0, 0), new Point(1, 1));
        var d = new DrawObject(s, "", "red");
        Assert.True(d.CheckValidType());
    }

    [Fact]
    public void CheckValidType_returns_true_for_polygon()
    {
        var poly = new Polygon(new Point(0, 0), 5.0, 6);
        var d = new DrawObject(poly, "", "red");
        Assert.True(d.CheckValidType());
    }

    [Fact]
    public void CheckValidType_returns_true_for_arc()
    {
        var a = new Arc(new Point(0, 0), new Point(5, 0), new Point(0, 5), 5.0);
        var d = new DrawObject(a, "", "red");
        Assert.True(d.CheckValidType());
    }

    [Fact]
    public void CheckValidType_returns_true_for_point_sequence()
    {
        var seq = new Finite_Sequence<Point>(new List<Point> { new(0, 0), new(1, 1) });
        var d = new DrawObject(seq, "", "red");
        Assert.True(d.CheckValidType());
    }
}
