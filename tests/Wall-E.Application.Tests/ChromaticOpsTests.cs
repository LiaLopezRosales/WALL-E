using Wall_E.Domain;
using Xunit;

namespace Wall_E.Application.Tests;

public class ChromaticOpsTests
{
    [Fact]
    public void Lighten_increases_lightness()
    {
        var p = DslRunner.Run("color red; lighten(20); draw point(0,0);");
        Assert.Empty(p.Errors);
        string result = p.Scene.UtilizedColors.Peek();
        HslConverter.RgbToHsl(255, 0, 0, out _, out _, out double origL);
        var (r2, g2, b2) = ParseHex(result);
        HslConverter.RgbToHsl(r2, g2, b2, out _, out _, out double newL);
        Assert.True(newL > origL);
    }

    [Fact]
    public void Darken_decreases_lightness()
    {
        var p = DslRunner.Run("color white; darken(30); draw point(0,0);");
        Assert.Empty(p.Errors);
        string result = p.Scene.UtilizedColors.Peek();
        HslConverter.RgbToHsl(255, 255, 255, out _, out _, out double origL);
        var (r2, g2, b2) = ParseHex(result);
        HslConverter.RgbToHsl(r2, g2, b2, out _, out _, out double newL);
        Assert.True(newL < origL);
    }

    [Fact]
    public void Complement_flips_hue()
    {
        var p = DslRunner.Run("color red; complement(); draw point(0,0);");
        Assert.Empty(p.Errors);
        string result = p.Scene.UtilizedColors.Peek();
        Assert.StartsWith("#", result);
        HslConverter.RgbToHsl(255, 0, 0, out double h1, out _, out _);
        var (r2, g2, b2) = ParseHex(result);
        HslConverter.RgbToHsl(r2, g2, b2, out double h2, out _, out _);
        Assert.InRange(h2, 170, 190);
    }

    [Fact]
    public void Mix_two_named_colors()
    {
        var p = DslRunner.Run("color red; mix(blue, 0.5); draw point(0,0);");
        Assert.Empty(p.Errors);
        string result = p.Scene.UtilizedColors.Peek();
        Assert.Equal("#800080", result);
    }

    [Fact]
    public void Mix_defaults_to_half()
    {
        var p = DslRunner.Run("color red; mix(blue); draw point(0,0);");
        Assert.Empty(p.Errors);
        Assert.Equal(1, p.Scene.Snapshot().Count);
        string result = p.Scene.UtilizedColors.Peek();
        Assert.Equal("#800080", result);
    }

    [Fact]
    public void Lighten_then_draw_uses_new_color()
    {
        var p = DslRunner.Run("color red; lighten(20); draw point(0,0);");
        Assert.Empty(p.Errors);
        var d = Assert.IsType<DrawObject>(p.Scene.Snapshot()[0]);
        var (r, g, b) = ParseHex(d.UsedColor);
        Assert.Equal(255, r);
        Assert.True(g > 0);
    }

    private static (int r, int g, int b) ParseHex(string hex)
    {
        string h = hex.TrimStart('#');
        if (h.Length == 3) h = $"{h[0]}{h[0]}{h[1]}{h[1]}{h[2]}{h[2]}";
        return (Convert.ToInt32(h[..2], 16), Convert.ToInt32(h[2..4], 16), Convert.ToInt32(h[4..6], 16));
    }
}
