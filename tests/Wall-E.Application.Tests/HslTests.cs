using Wall_E.Domain;
using Xunit;

namespace Wall_E.Application.Tests;

public class HslTests
{
    [Fact]
    public void Hsl_red_is_correct()
    {
        var (r, g, b) = HslConverter.HslToRgb(0, 100, 50);
        Assert.Equal(255, r);
        Assert.Equal(0, g);
        Assert.Equal(0, b);
    }

    [Fact]
    public void Hsl_green_is_correct()
    {
        var (r, g, b) = HslConverter.HslToRgb(120, 100, 50);
        Assert.Equal(0, r);
        Assert.Equal(255, g);
        Assert.Equal(0, b);
    }

    [Fact]
    public void Hsl_blue_is_correct()
    {
        var (r, g, b) = HslConverter.HslToRgb(240, 100, 50);
        Assert.Equal(0, r);
        Assert.Equal(0, g);
        Assert.Equal(255, b);
    }

    [Fact]
    public void Hsl_white()
    {
        var (r, g, b) = HslConverter.HslToRgb(0, 0, 100);
        Assert.Equal(255, r);
        Assert.Equal(255, g);
        Assert.Equal(255, b);
    }

    [Fact]
    public void Hsl_black()
    {
        var (r, g, b) = HslConverter.HslToRgb(0, 0, 0);
        Assert.Equal(0, r);
        Assert.Equal(0, g);
        Assert.Equal(0, b);
    }

    [Fact]
    public void Hsl_to_hex_coral()
    {
        Assert.Equal("#FF8052", HslConverter.ToHex(16, 100, 66));
    }

    [Fact]
    public void Dsl_accepts_hsl_statement()
    {
        var p = DslRunner.Run("hsl(200, 80, 50); draw point(0,0);");
        Assert.Empty(p.Errors);
        var hex = p.Scene.UtilizedColors.Peek();
        Assert.StartsWith("#", hex);
    }

    [Fact]
    public void Hsl_roundtrip()
    {
        HslConverter.RgbToHsl(255, 127, 80, out double h, out double s, out double l);
        var (r2, g2, b2) = HslConverter.HslToRgb(h, s * 100, l * 100);
        Assert.Equal(255, r2);
        Assert.InRange(g2, 125, 129);
        Assert.InRange(b2, 78, 82);
    }
}
