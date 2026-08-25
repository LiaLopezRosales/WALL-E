using Xunit;

namespace Wall_E.Domain.Tests;

public class HslConverterTests
{
    [Theory]
    [InlineData(0, 100, 50, "#FF0000")]
    [InlineData(120, 100, 50, "#00FF00")]
    [InlineData(240, 100, 50, "#0000FF")]
    [InlineData(0, 0, 0, "#000000")]
    [InlineData(0, 0, 100, "#FFFFFF")]
    [InlineData(60, 100, 50, "#FFFF00")]
    public void HslToRgb_known_colors(double h, double s, double l, string expected)
    {
        var (r, g, b) = HslConverter.HslToRgb(h, s, l);
        string hex = $"#{r:X2}{g:X2}{b:X2}";
        Assert.Equal(expected, hex);
    }

    [Theory]
    [InlineData(255, 0, 0, 0, 100, 50)]
    [InlineData(0, 255, 0, 120, 100, 50)]
    [InlineData(0, 0, 255, 240, 100, 50)]
    public void RgbToHsl_known_colors(byte r, byte g, byte b, double expectedH, double expectedS, double expectedL)
    {
        HslConverter.RgbToHsl(r, g, b, out double h, out double s, out double l);
        Assert.Equal(expectedH, h, 0);
        Assert.Equal(expectedS / 100, s, 2);
        Assert.Equal(expectedL / 100, l, 2);
    }

    [Fact]
    public void HslToRgb_roundtrip()
    {
        var (r1, g1, b1) = HslConverter.HslToRgb(200, 80, 60);
        HslConverter.RgbToHsl(r1, g1, b1, out double h, out double s, out double l);
        var (r2, g2, b2) = HslConverter.HslToRgb(h, s * 100, l * 100);
        Assert.Equal(r1, r2);
        Assert.Equal(g1, g2);
        Assert.Equal(b1, b2);
    }

    [Theory]
    [InlineData("#FF0000")]
    [InlineData("#000000")]
    public void Lighten_changes_color(string input)
    {
        string result = HslConverter.Lighten(input, 20);
        Assert.StartsWith("#", result);
        Assert.NotEqual(input, result);
    }

    [Theory]
    [InlineData("#FFFFFF")]
    [InlineData("#FF0000")]
    public void Darken_changes_color(string input)
    {
        string result = HslConverter.Darken(input, 20);
        Assert.StartsWith("#", result);
        Assert.NotEqual(input, result);
    }

    [Fact]
    public void Complement_flips_hue()
    {
        string result = HslConverter.Complement("#FF0000");
        Assert.Equal("#00FFFF", result);
    }

    [Fact]
    public void Mix_at_0_returns_first()
    {
        string result = HslConverter.Mix("#FF0000", "#0000FF", 0);
        Assert.Equal("#FF0000", result);
    }

    [Fact]
    public void Mix_at_1_returns_second()
    {
        string result = HslConverter.Mix("#FF0000", "#0000FF", 1);
        Assert.Equal("#0000FF", result);
    }

    [Fact]
    public void Mix_at_half_blends()
    {
        string result = HslConverter.Mix("#FF0000", "#0000FF", 0.5);
        Assert.Equal("#800080", result);
    }

    [Fact]
    public void ToHex_returns_valid_hex()
    {
        string result = HslConverter.ToHex(0, 100, 50);
        Assert.Equal("#FF0000", result);
    }
}
