using Wall_E.Domain;
using Xunit;

namespace Wall_E.Application.Tests;

public class LabelTests
{
    [Fact]
    public void Label_adds_to_scene()
    {
        var p = DslRunner.Run("label(point(0,0), \"hello\", 14);");
        Assert.Empty(p.Errors);
        Assert.Single(p.Scene.Labels);
        var lbl = p.Scene.Labels[0];
        Assert.Equal("hello", lbl.Text);
        Assert.Equal(14.0, lbl.FontSize);
    }

    [Fact]
    public void Label_uses_current_color()
    {
        var p = DslRunner.Run("rgb(255, 0, 0); label(point(1,2), \"red\", 12);");
        Assert.Empty(p.Errors);
        var lbl = p.Scene.Labels[0];
        Assert.Equal("#FF0000", lbl.Color);
    }

    [Fact]
    public void Label_with_draw()
    {
        var p = DslRunner.Run("draw point(5,5); label(point(10,10), \"A\", 16);");
        Assert.Empty(p.Errors);
        Assert.Single(p.Scene.Snapshot());
        Assert.Single(p.Scene.Labels);
    }
}
