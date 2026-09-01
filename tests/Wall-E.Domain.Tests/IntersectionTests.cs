using Xunit;

namespace Wall_E.Domain.Tests;

public class IntersectionTests
{
    private static readonly Point P1 = new(5, 5);
    private static readonly Line L1 = new(new Point(0, 0), new Point(10, 10));
    private static readonly Line L2 = new(new Point(0, 10), new Point(10, 0));
    private static readonly Segment S1 = new(new Point(0, 10), new Point(10, 0));
    private static readonly Ray R1 = new(new Point(0, 0), new Point(10, 0));
    private static readonly Circle C1 = new(new Point(0, 0), 10);
    private static readonly Arc A1 = new(new Point(0, 0), new Point(10, 0), new Point(0, 10), 90);
    private static readonly Polygon Poly1 = new(new Point(0, 0), 10, 5);
    private static readonly Ellipse E1 = new(new Point(0, 0), 10, 5);

    private static readonly Figure[] AllFigures =
    {
        P1, L1, S1, R1, C1, A1, Poly1, E1,
    };

    [Fact]
    public void All_ordered_figure_pairs_intersect_to_finite_point_sequence()
    {
        foreach (var a in AllFigures)
        {
            foreach (var b in AllFigures)
            {
                if (ReferenceEquals(a, b)) continue;

                var result = a.Intersect(b);

                Assert.NotNull(result);
                Assert.True(result!.count >= 0);
                foreach (var item in result.Sequence!)
                    Assert.IsType<Point>(item);
            }
        }
    }

    [Fact]
    public void Line_intersects_Line_in_single_point()
    {
        var result = L1.Intersect(L2);

        Assert.Equal(1, result!.count);
        var p = Assert.Single(result.Sequence!);
        Assert.InRange(p.x, 4.9, 5.1);
        Assert.InRange(p.y, 4.9, 5.1);
    }

    [Fact]
    public void Segment_intersects_Segment_in_single_point()
    {
        var other = new Segment(new Point(0, 0), new Point(10, 10));
        var result = S1.Intersect(other);

        Assert.Equal(1, result!.count);
        var p = Assert.Single(result.Sequence!);
        Assert.InRange(p.x, 4.9, 5.1);
        Assert.InRange(p.y, 4.9, 5.1);
    }

    [Fact]
    public void Line_through_circle_center_yields_two_points()
    {
        var horizontal = new Line(new Point(-10, 0), new Point(10, 0));
        var result = C1.Intersect(horizontal);

        Assert.Equal(2, result!.count);
        Assert.All(result.Sequence!, p => Assert.IsType<Point>(p));
    }

    [Fact]
    public void Two_intersecting_circles_yield_two_points()
    {
        var other = new Circle(new Point(15, 0), 10);
        var result = C1.Intersect(other);

        Assert.Equal(2, result!.count);
        Assert.All(result.Sequence!, p => Assert.IsType<Point>(p));
    }

    [Fact]
    public void Point_on_circle_is_detected()
    {
        var onCircle = new Point(10, 0);
        var result = onCircle.Intersect(C1);

        Assert.Equal(1, result!.count);
    }

    [Fact]
    public void Point_off_circle_is_empty()
    {
        var offCircle = new Point(30, 5);
        var result = offCircle.Intersect(C1);

        Assert.Equal(0, result!.count);
    }
}