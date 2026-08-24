using Wall_E.Application.Pipeline;
using Wall_E.Domain;
using Xunit;

namespace Wall_E.Application.Tests;

public class HexColorCharacterizationTests
{
    [Fact]
    public void Six_digit_hex_is_a_valid_color_statement()
    {
        var pipeline = DslRunner.Run("""
            color #FF8800;
            draw point(0,0);
            """);

        Assert.Empty(pipeline.Errors);
        var draw = Assert.IsType<DrawObject>(pipeline.Scene.Snapshot()[0]);
        Assert.Equal("#FF8800", draw.UsedColor);
    }

    [Fact]
    public void Three_digit_hex_is_a_valid_color_statement()
    {
        var pipeline = DslRunner.Run("""
            color #f80;
            draw point(0,0);
            """);

        Assert.Empty(pipeline.Errors);
        var draw = Assert.IsType<DrawObject>(pipeline.Scene.Snapshot()[0]);
        Assert.Equal("#f80", draw.UsedColor);
    }

    [Fact]
    public void Mixed_case_hex_is_accepted()
    {
        var pipeline = DslRunner.Run("""
            color #AbC123;
            draw point(0,0);
            """);

        Assert.Empty(pipeline.Errors);
        var draw = Assert.IsType<DrawObject>(pipeline.Scene.Snapshot()[0]);
        Assert.Equal("#AbC123", draw.UsedColor);
    }

    [Fact]
    public void Invalid_hex_after_color_is_rejected()
    {
        var pipeline = DslRunner.Run("""
            color #GGHHII;
            draw point(0,0);
            """);

        // "#GGHHII" lexes as an identifier (the '#' is skipped), so the
        // parser reports a missing valid color and the program never runs.
        Assert.NotEmpty(pipeline.Errors);
        Assert.Equal(0, pipeline.Scene.DrawCount);
    }

    [Fact]
    public void Restore_pops_hex_back_to_previous_ink()
    {
        var pipeline = DslRunner.Run("""
            color red;
            color #00FF00;
            draw point(0,0);
            restore;
            draw point(10,10);
            """);

        Assert.Empty(pipeline.Errors);
        var snapshot = pipeline.Scene.Snapshot();
        Assert.Equal(2, snapshot.Count);
        var first = Assert.IsType<DrawObject>(snapshot[0]);
        var second = Assert.IsType<DrawObject>(snapshot[1]);
        Assert.Equal("#00FF00", first.UsedColor);
        Assert.Equal("red", second.UsedColor);
    }

    [Fact]
    public void Hex_outside_color_statement_is_still_not_an_identifier()
    {
        // `#FF0000` alone is not usable as an expression/identifier.
        var pipeline = DslRunner.Run("draw point(#FF0000, 0);");

        Assert.NotEmpty(pipeline.Errors);
    }
}
