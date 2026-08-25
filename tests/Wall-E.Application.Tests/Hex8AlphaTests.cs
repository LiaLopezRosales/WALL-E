using Wall_E.Application.Pipeline;
using Wall_E.Domain;
using Xunit;

namespace Wall_E.Application.Tests;

public class Hex8AlphaTests
{
    [Fact]
    public void Eight_digit_hex_is_accepted()
    {
        var p = DslRunner.Run("color #FF000080; draw point(0,0);");
        Assert.Empty(p.Errors);
        var draw = Assert.IsType<DrawObject>(p.Scene.Snapshot()[0]);
        Assert.Equal("#FF000080", draw.UsedColor);
    }

    [Fact]
    public void Four_digit_hex_is_accepted()
    {
        var p = DslRunner.Run("color #F00F; draw point(0,0);");
        Assert.Empty(p.Errors);
        var draw = Assert.IsType<DrawObject>(p.Scene.Snapshot()[0]);
        Assert.Equal("#F00F", draw.UsedColor);
    }

    [Fact]
    public void Eight_digit_hex_preserves_alpha()
    {
        var p = DslRunner.Run("color #FF000080; draw point(0,0);");
        Assert.Empty(p.Errors);
        Assert.EndsWith("80", p.Scene.UtilizedColors.Peek());
    }
}
