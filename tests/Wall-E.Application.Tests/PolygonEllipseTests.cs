using Wall_E.Domain;
using Xunit;

namespace Wall_E.Application.Tests;

public class PolygonEllipseTests
{
    [Fact]
    public void Polygon_creates_figure_with_correct_vertices()
    {
        var p = DslRunner.Run("draw polygon(point(0,0), 10, 4);");
        Assert.Empty(p.Errors);
        var d = Assert.IsType<DrawObject>(p.Scene.Snapshot()[0]);
        var poly = Assert.IsType<Polygon>(d.Figures);
        Assert.Equal(4, poly.Sides);
        Assert.Equal(10.0, poly.Radius);
        Assert.Equal(0.0, poly.Center.x);
        Assert.Equal(0.0, poly.Center.y);
    }

    [Fact]
    public void Polygon_has_correct_vertex_count()
    {
        var p = DslRunner.Run("draw polygon(point(0,0), 10, 6);");
        Assert.Empty(p.Errors);
        var d = Assert.IsType<DrawObject>(p.Scene.Snapshot()[0]);
        var poly = Assert.IsType<Polygon>(d.Figures);
        Assert.Equal(6, poly.Vertices().Count);
    }

    [Fact]
    public void Triangle_first_vertex_at_top()
    {
        var p = DslRunner.Run("draw polygon(point(0,0), 10, 3);");
        var d = Assert.IsType<DrawObject>(p.Scene.Snapshot()[0]);
        var poly = Assert.IsType<Polygon>(d.Figures);
        var verts = poly.Vertices();
        Assert.InRange(verts[0].x, -0.01, 0.01);
        Assert.InRange(verts[0].y, -10.01, -9.99);
    }

    [Fact]
    public void Ellipse_creates_figure_with_correct_params()
    {
        var p = DslRunner.Run("draw ellipse(point(5,5), 10, 20);");
        Assert.Empty(p.Errors);
        var d = Assert.IsType<DrawObject>(p.Scene.Snapshot()[0]);
        var ell = Assert.IsType<Ellipse>(d.Figures);
        Assert.Equal(5.0, ell.Center.x);
        Assert.Equal(5.0, ell.Center.y);
        Assert.Equal(10.0, ell.Rx);
        Assert.Equal(20.0, ell.Ry);
    }

    [Fact]
    public void Ellipse_contains_center()
    {
        var p = DslRunner.Run("draw ellipse(point(0,0), 10, 20);");
        var d = Assert.IsType<DrawObject>(p.Scene.Snapshot()[0]);
        var ell = Assert.IsType<Ellipse>(d.Figures);
        Assert.True(ell.ContainPoint(new Point(0, 0)));
    }

    [Fact]
    public void Polygon_contains_center()
    {
        var p = DslRunner.Run("draw polygon(point(0,0), 10, 6);");
        var d = Assert.IsType<DrawObject>(p.Scene.Snapshot()[0]);
        var poly = Assert.IsType<Polygon>(d.Figures);
        Assert.True(poly.ContainPoint(new Point(0, 0)));
    }

    [Fact]
    public void Ellipse_with_expressions()
    {
        var p = DslRunner.Run("draw ellipse(point(0,0), 5+5, 10*2);");
        Assert.Empty(p.Errors);
        var d = Assert.IsType<DrawObject>(p.Scene.Snapshot()[0]);
        var ell = Assert.IsType<Ellipse>(d.Figures);
        Assert.Equal(10.0, ell.Rx);
        Assert.Equal(20.0, ell.Ry);
    }

    [Fact]
    public void Polygon_with_expressions()
    {
        var p = DslRunner.Run("draw polygon(point(1,2), 3*4, 8);");
        Assert.Empty(p.Errors);
        var d = Assert.IsType<DrawObject>(p.Scene.Snapshot()[0]);
        var poly = Assert.IsType<Polygon>(d.Figures);
        Assert.Equal(12.0, poly.Radius);
        Assert.Equal(8, poly.Sides);
    }
}
