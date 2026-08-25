using Wall_E.Application.DSL;
using Xunit;

namespace Wall_E.Application.Tests;

public class LexerTests
{
    private static Lexer MakeLexer() => new("test.geo", "1");

    [Fact]
    public void Tokenize_simple_assignment()
    {
        var tokens = MakeLexer().Tokens("A = 5;");
        Assert.Equal(Token.TokenType.identifier, tokens[0].Type);
        Assert.Equal("A", tokens[0].Value);
        Assert.Equal("=", tokens[1].Value);
        Assert.Equal("5", tokens[2].Value);
    }

    [Fact]
    public void Tokenize_point_keyword()
    {
        var tokens = MakeLexer().Tokens("point A = (0, 0);");
        Assert.Equal(Token.TokenType.point, tokens[0].Type);
        Assert.Equal(Token.TokenType.identifier, tokens[1].Type);
    }

    [Fact]
    public void Tokenize_circle_keyword()
    {
        var tokens = MakeLexer().Tokens("circle c = point(0,0), 5;");
        Assert.Equal(Token.TokenType.circle, tokens[0].Type);
        Assert.Equal(Token.TokenType.identifier, tokens[1].Type);
    }

    [Fact]
    public void Tokenize_draw_keyword()
    {
        var tokens = MakeLexer().Tokens("draw A;");
        Assert.Equal(Token.TokenType.draw, tokens[0].Type);
        Assert.Equal(Token.TokenType.identifier, tokens[1].Type);
    }

    [Fact]
    public void Tokenize_color_keyword()
    {
        var tokens = MakeLexer().Tokens("color red;");
        Assert.Equal(Token.TokenType.keyword, tokens[0].Type);
        Assert.Equal("color", tokens[0].Value);
        Assert.Equal(Token.TokenType.color_value, tokens[1].Type);
        Assert.Equal("red", tokens[1].Value);
    }

    [Fact]
    public void Tokenize_rgb_function()
    {
        var tokens = MakeLexer().Tokens("rgb(255, 0, 128);");
        Assert.Equal(Token.TokenType.rgb, tokens[0].Type);
    }

    [Fact]
    public void Tokenize_hsl_function()
    {
        var tokens = MakeLexer().Tokens("hsl(180, 50, 50);");
        Assert.Equal(Token.TokenType.hsl, tokens[0].Type);
    }

    [Fact]
    public void Tokenize_line_styles()
    {
        Assert.Equal(Token.TokenType.dashed, MakeLexer().Tokens("dashed;")[0].Type);
        Assert.Equal(Token.TokenType.dotted, MakeLexer().Tokens("dotted;")[0].Type);
        Assert.Equal(Token.TokenType.dashdot, MakeLexer().Tokens("dashdot;")[0].Type);
        Assert.Equal(Token.TokenType.solid_k, MakeLexer().Tokens("solid;")[0].Type);
    }

    [Fact]
    public void Tokenize_fill_unfill()
    {
        Assert.Equal(Token.TokenType.fill, MakeLexer().Tokens("fill;")[0].Type);
        Assert.Equal(Token.TokenType.unfill, MakeLexer().Tokens("unfill;")[0].Type);
    }

    [Fact]
    public void Tokenize_gradient_keywords()
    {
        Assert.Equal(Token.TokenType.linear, MakeLexer().Tokens("linear;")[0].Type);
        Assert.Equal(Token.TokenType.radial, MakeLexer().Tokens("radial;")[0].Type);
    }

    [Fact]
    public void Tokenize_layer_snap_hide_show()
    {
        Assert.Equal(Token.TokenType.layer, MakeLexer().Tokens("layer 1;")[0].Type);
        Assert.Equal(Token.TokenType.snap, MakeLexer().Tokens("snap 0.5;")[0].Type);
        Assert.Equal(Token.TokenType.hide, MakeLexer().Tokens("hide a;")[0].Type);
        Assert.Equal(Token.TokenType.show, MakeLexer().Tokens("show a;")[0].Type);
    }

    [Fact]
    public void Tokenize_arithmetic_operators()
    {
        var tokens = MakeLexer().Tokens("a + b - c * d / e ^ f % g");
        Assert.Equal(Token.TokenType.sum, tokens[1].Type);
        Assert.Equal(Token.TokenType.substraction, tokens[3].Type);
        Assert.Equal(Token.TokenType.multiplication, tokens[5].Type);
        Assert.Equal(Token.TokenType.division, tokens[7].Type);
        Assert.Equal(Token.TokenType.power, tokens[9].Type);
        Assert.Equal(Token.TokenType.module, tokens[11].Type);
    }

    [Fact]
    public void Tokenize_comparison_operators()
    {
        var tokens = MakeLexer().Tokens("a < b > c <= d >= e == f != g");
        Assert.Equal(Token.TokenType.minor, tokens[1].Type);
        Assert.Equal(Token.TokenType.major, tokens[3].Type);
        Assert.Equal(Token.TokenType.equal_minor, tokens[5].Type);
        Assert.Equal(Token.TokenType.equal_major, tokens[7].Type);
        Assert.Equal(Token.TokenType.equal, tokens[9].Type);
        Assert.Equal(Token.TokenType.diferent, tokens[11].Type);
    }

    [Fact]
    public void Tokenize_sequence_keywords()
    {
        Assert.Equal(Token.TokenType.randoms, MakeLexer().Tokens("randoms;")[0].Type);
        Assert.Equal(Token.TokenType.samples, MakeLexer().Tokens("samples;")[0].Type);
        Assert.Equal(Token.TokenType.points, MakeLexer().Tokens("points;")[0].Type);
    }

    [Fact]
    public void Tokenize_math_constants()
    {
        Assert.Equal(Token.TokenType.phi, MakeLexer().Tokens("phi;")[0].Type);
        Assert.Equal(Token.TokenType.sqrt2, MakeLexer().Tokens("sqrt2;")[0].Type);
    }

    [Fact]
    public void Tokenize_function_keywords()
    {
        Assert.Equal(Token.TokenType.polygon, MakeLexer().Tokens("polygon;")[0].Type);
        Assert.Equal(Token.TokenType.ellipse, MakeLexer().Tokens("ellipse;")[0].Type);
        Assert.Equal(Token.TokenType.label, MakeLexer().Tokens("label;")[0].Type);
        Assert.Equal(Token.TokenType.grosor, MakeLexer().Tokens("grosor;")[0].Type);
    }

    [Fact]
    public void Tokenize_seed_and_print()
    {
        Assert.Equal(Token.TokenType.seed, MakeLexer().Tokens("seed;")[0].Type);
        Assert.Equal(Token.TokenType.print, MakeLexer().Tokens("print;")[0].Type);
    }

    [Fact]
    public void Tokenize_string_literal()
    {
        var tokens = MakeLexer().Tokens("\"hello world\"");
        Assert.Equal(Token.TokenType.text, tokens[0].Type);
    }

    [Fact]
    public void Tokenize_negative_number()
    {
        var tokens = MakeLexer().Tokens("-42;");
        Assert.Equal(Token.TokenType.number, tokens[0].Type);
    }

    [Fact]
    public void Tokenize_hex_color()
    {
        var tokens = MakeLexer().Tokens("#FF00AA;");
        Assert.Equal(Token.TokenType.color_value, tokens[0].Type);
        Assert.Equal("#FF00AA", tokens[0].Value);
    }

    [Fact]
    public void Tokenize_repeat_and_for_loop()
    {
        Assert.Equal(Token.TokenType.repeat, MakeLexer().Tokens("repeat")[0].Type);
        Assert.Equal(Token.TokenType.for_token, MakeLexer().Tokens("for")[0].Type);
    }

    [Fact]
    public void Tokenize_chromatic_operations()
    {
        Assert.Equal(Token.TokenType.lighten, MakeLexer().Tokens("lighten;")[0].Type);
        Assert.Equal(Token.TokenType.darken, MakeLexer().Tokens("darken;")[0].Type);
        Assert.Equal(Token.TokenType.mix, MakeLexer().Tokens("mix;")[0].Type);
        Assert.Equal(Token.TokenType.complement, MakeLexer().Tokens("complement;")[0].Type);
    }

    [Fact]
    public void Tokenize_intersect_and_count()
    {
        Assert.Equal(Token.TokenType.intersect, MakeLexer().Tokens("intersect;")[0].Type);
        Assert.Equal(Token.TokenType.count, MakeLexer().Tokens("count;")[0].Type);
    }

    [Fact]
    public void Tokenize_logic_operators()
    {
        var tokens = MakeLexer().Tokens("a and b or c");
        Assert.Equal(Token.TokenType.And, tokens[1].Type);
        Assert.Equal(Token.TokenType.Or, tokens[3].Type);
    }

    [Fact]
    public void Tokenize_let_in_statement()
    {
        var tokens = MakeLexer().Tokens("let x = 5 in x + 1");
        Assert.Equal(Token.TokenType.keyword, tokens[0].Type);
        Assert.Equal("let", tokens[0].Value);
    }

    [Fact]
    public void Tokenize_multiple_tokens()
    {
        var tokens = MakeLexer().Tokens("draw circle(point(0,0), 5);");
        Assert.True(tokens.Count > 3);
        Assert.Equal(Token.TokenType.draw, tokens[0].Type);
        Assert.Equal(Token.TokenType.circle, tokens[1].Type);
    }

    [Fact]
    public void Tokenize_left_bracket_after_identifier()
    {
        var tokens = MakeLexer().Tokens("point(1, 2);");
        Assert.Equal(Token.TokenType.point, tokens[0].Type);
        Assert.Equal(Token.TokenType.left_bracket, tokens[1].Type);
    }
}
