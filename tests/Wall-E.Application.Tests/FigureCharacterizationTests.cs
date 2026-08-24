using Wall_E.Domain;
using Xunit;
using static Wall_E.Application.Tests.DslRunner;

namespace Wall_E.Application.Tests;

public class FigureCharacterizationTests
{
    [Fact]
    public void Point_declaration_creates_point_in_repository()
    {
        var pipeline = Run("point p1;");
        Assert.Empty(pipeline.Errors);
        Assert.Single(pipeline.Figures.ExistingPoints);
    }

    [Fact]
    public void Circle_declaration_creates_circle_in_repository()
    {
        var pipeline = Run("circle c;");
        Assert.Empty(pipeline.Errors);
        Assert.Single(pipeline.Figures.ExistingCircles);
    }

    [Fact]
    public void Line_segment_ray_declarations()
    {
        var pipeline = Run("line l; segment s; ray r;");
        Assert.Empty(pipeline.Errors);
        Assert.Single(pipeline.Figures.ExistingLines);
        Assert.Single(pipeline.Figures.ExistingSegments);
        Assert.Single(pipeline.Figures.ExistingRays);
    }

    [Fact]
    public void Draw_point_adds_to_scene()
    {
        var pipeline = Run("draw point(0,0);");
        Assert.Empty(pipeline.Errors);
        Assert.Single(pipeline.Scene.ToDraw);
    }

    [Fact]
    public void Default_color_is_black()
    {
        var pipeline = Run("point p1;");
        Assert.Equal("black", pipeline.Scene.UtilizedColors.Peek());
    }
}
