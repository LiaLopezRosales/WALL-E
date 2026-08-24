using Wall_E.Domain;
using Xunit;
using static Wall_E.Application.Tests.DslRunner;

namespace Wall_E.Application.Tests;

public class GeometricFunctionCharacterizationTests
{
    [Fact]
    public void Point_function_creates_point_at_coordinates()
    {
        var pipeline = Run("p = point(1,2);");
        Assert.Empty(pipeline.Errors);
        var point = pipeline.Figures.ExistingPoints.Single();
        Assert.Equal(1.0, point.x);
        Assert.Equal(2.0, point.y);
    }

    [Fact]
    public void Circle_function_creates_circle_with_radius()
    {
        var pipeline = Run("c = circle(point(0,0), 5);");
        Assert.Empty(pipeline.Errors);
        var circle = pipeline.Figures.ExistingCircles.Single();
        Assert.Equal(5.0, circle.radio);
    }

    [Fact]
    public void Circle_with_measure_radius()
    {
        var pipeline = Run("c = circle(point(0,0), measure(point(0,0), point(5,0)));");
        Assert.Empty(pipeline.Errors);
        Assert.Equal(5.0, pipeline.Figures.ExistingCircles.Single().radio);
    }

    [Fact]
    public void Line_segment_ray_functions()
    {
        var pipeline = Run("l = line(point(0,0), point(3,4)); s = segment(point(0,0), point(3,4)); r = ray(point(0,0), point(3,4));");
        Assert.Empty(pipeline.Errors);
        Assert.Single(pipeline.Figures.ExistingLines);
        Assert.Single(pipeline.Figures.ExistingSegments);
        Assert.Single(pipeline.Figures.ExistingRays);
    }

    [Fact]
    public void Measure_function_returns_distance_value()
    {
        var pipeline = Run("m = measure(point(0,0), point(3,4));");
        Assert.Empty(pipeline.Errors);
        Assert.Equal(5.0, pipeline.Context.GlobalConstant["m"]);
    }

    [Fact]
    public void Invalid_circle_arguments_report_semantic_error()
    {
        var pipeline = Run("circle(5, 5);");
        Assert.NotEmpty(pipeline.Errors);
        Assert.Empty(pipeline.Figures.ExistingCircles);
    }

    [Fact]
    public void Arc_with_valid_arguments()
    {
        var pipeline = Run("a = arc(point(0,0), point(3,0), point(0,4), 90);");
        Assert.Empty(pipeline.Errors);
    }
}
