using System.IO;
using Wall_E.Domain;
using Xunit;

namespace Wall_E.Application.Tests;

public class AnimateTests
{
    [Fact]
    public void Animate_produces_bounded_frames()
    {
        var p = DslRunner.Run("animate(t from 0 to 1) { draw point(t, t); }");
        Assert.Empty(p.Errors);
        Assert.Equal(EvaluatorVisitor.AnimateFrames, p.Frames.Count);
    }

    [Fact]
    public void Each_frame_is_isolated()
    {
        var p = DslRunner.Run("animate(t from 0 to 1) { draw point(t, t); }");
        Assert.Empty(p.Errors);
        foreach (var frame in p.Frames)
            Assert.Single(frame.Snapshot());
    }

    [Fact]
    public void Parameter_varies_across_frames()
    {
        var p = DslRunner.Run("animate(t from 0 to 1) { draw point(t, 0); }");
        Assert.Empty(p.Errors);

        var first = (Point)p.Frames[0].Snapshot()[0].Figures;
        var last = (Point)p.Frames[^1].Snapshot()[0].Figures;
        Assert.Equal(0.0, first.x, 3);
        Assert.Equal(1.0, last.x, 3);
    }

    [Fact]
    public void Frames_do_not_pollute_outer_scene()
    {
        var p = DslRunner.Run("animate(t from 0 to 1) { draw point(t, t); }");
        Assert.Empty(p.Errors);
        // Animation draws go to frames, not the base scene.
        Assert.Empty(p.Scene.Snapshot());
    }

    [Fact]
    public void Color_state_is_per_frame()
    {
        // The color statement runs fresh inside each frame's scratch scene,
        // so every frame draws with the color it sets. `color blue` resolves
        // to its hex equivalent.
        var p = DslRunner.Run("animate(t from 0 to 1) { color blue; draw point(t, t); }");
        Assert.Empty(p.Errors);
        foreach (var frame in p.Frames)
            Assert.Equal("#0000FF", frame.Snapshot()[0].UsedColor);
    }

    [Fact]
    public void Animate_without_animate_yields_no_frames()
    {
        var p = DslRunner.Run("draw point(0, 0);");
        Assert.Empty(p.Frames);
    }

    [Fact]
    public void Multiple_animate_blocks_append_all_frames()
    {
        // Frames from every animate block must accumulate (not just the last
        // block's) so the full animation replays end to end.
        var p = DslRunner.Run(
            "animate(t from 0 to 1) { draw point(t, t); }\n" +
            "animate(t from 0 to 1) { draw circle(point(0,0), t); }");
        Assert.Empty(p.Errors);
        Assert.Equal(2 * EvaluatorVisitor.AnimateFrames, p.Frames.Count);
        // First block's frame carries the point; second block's a circle.
        Assert.IsType<Point>(p.Frames[0].Snapshot()[0].Figures);
        Assert.IsType<Circle>(p.Frames[^1].Snapshot()[0].Figures);
    }

    [Fact]
    public void Full_checked_in_animate_example_runs_end_to_end()
    {
        // Regression guard for the bundled example programs/08-animate.geo
        // (colours, grosor, cos/sin, two animate blocks).
        var source = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Examples", "08-animate.geo"));
        var p = DslRunner.Run(source);
        Assert.Empty(p.Errors);
        Assert.Equal(2 * EvaluatorVisitor.AnimateFrames, p.Frames.Count);

        // First block draws a point and a growing circle in every frame.
        var first = p.Frames[0].Snapshot();
        Assert.Equal(2, first.Count);
        Assert.Single(first, f => f.Figures is Point);
        Assert.Single(first, f => f.Figures is Circle);

        // Second block draws two orbiting points in distinct colours.
        var last = p.Frames[^1].Snapshot();
        Assert.Equal(2, last.Count);
        Assert.All(last, f => Assert.IsType<Point>(f.Figures));
        Assert.Equal(2, last.Select(f => f.UsedColor).Distinct().Count());

        // The point sweeps across the parabola between first and last frame.
        var leadingPoint = (Point)last[0].Figures;
        var firstPoint = (Point)first.First(f => f.Figures is Point).Figures;
        Assert.True(leadingPoint.x > firstPoint.x);
    }
}
