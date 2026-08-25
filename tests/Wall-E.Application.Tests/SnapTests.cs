using Wall_E.Domain;
using Xunit;

namespace Wall_E.Application.Tests;

public class SnapTests
{
    [Fact]
    public void Snap_sets_snap_value()
    {
        var p = DslRunner.Run("snap 0.5;");
        Assert.Empty(p.Errors);
        Assert.Equal(0.5, p.Scene.SnapValue);
    }

    [Fact]
    public void Snap_default_is_zero()
    {
        var p = DslRunner.Run("draw point(0,0);");
        Assert.Empty(p.Errors);
        Assert.Equal(0, p.Scene.SnapValue);
    }

    [Fact]
    public void Snap_can_be_overridden()
    {
        var p = DslRunner.Run("snap 1; snap 0.25;");
        Assert.Empty(p.Errors);
        Assert.Equal(0.25, p.Scene.SnapValue);
    }
}
