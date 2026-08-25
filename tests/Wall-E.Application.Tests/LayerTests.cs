using Wall_E.Domain;
using Xunit;

namespace Wall_E.Application.Tests;

public class LayerTests
{
    [Fact]
    public void Layer_sets_layer_on_drawn_object()
    {
        var p = DslRunner.Run("layer 2; draw point(0,0);");
        Assert.Empty(p.Errors);
        Assert.Equal(2, p.Scene.Snapshot()[0].Layer);
    }

    [Fact]
    public void Default_layer_is_zero()
    {
        var p = DslRunner.Run("draw point(0,0);");
        Assert.Empty(p.Errors);
        Assert.Equal(0, p.Scene.Snapshot()[0].Layer);
    }

    [Fact]
    public void Multiple_layers()
    {
        var p = DslRunner.Run("layer 1; draw point(0,0); layer 3; draw point(1,1);");
        Assert.Empty(p.Errors);
        Assert.Equal(1, p.Scene.Snapshot()[0].Layer);
        Assert.Equal(3, p.Scene.Snapshot()[1].Layer);
    }

    [Fact]
    public void Layer_resets_previous_draws()
    {
        var p = DslRunner.Run("layer 5; draw circle(point(0,0),3); layer 0; draw point(0,0);");
        Assert.Empty(p.Errors);
        Assert.Equal(5, p.Scene.Snapshot()[0].Layer);
        Assert.Equal(0, p.Scene.Snapshot()[1].Layer);
    }
}
