using Wall_E.Domain;
using Xunit;

namespace Wall_E.Application.Tests;

public class RenderSceneSnapshotTests
{
    [Fact]
    public void Snapshot_is_isolated_from_later_mutations()
    {
        var scene = new RenderScene();
        scene.Add(MakeDraw("a"));

        var snapshot = scene.Snapshot();
        scene.Add(MakeDraw("b"));

        Assert.Single(snapshot);
        Assert.Equal(2, scene.DrawCount);
    }

    [Fact]
    public void DrawCount_reflects_added_objects()
    {
        var scene = new RenderScene();
        Assert.Equal(0, scene.DrawCount);
        scene.Add(MakeDraw("x"));
        scene.Add(MakeDraw("y"));
        Assert.Equal(2, scene.DrawCount);
    }

    [Fact]
    public void SnapshotRange_returns_only_objects_from_start()
    {
        var scene = new RenderScene();
        scene.Add(MakeDraw("a"));
        scene.Add(MakeDraw("b"));

        var tail = scene.SnapshotRange(1);
        var none = scene.SnapshotRange(5);

        Assert.Single(tail);
        Assert.Empty(none);
    }

    private static DrawObject MakeDraw(string tag) =>
        new(new Point(0, 0), tag, "black");
}
