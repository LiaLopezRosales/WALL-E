using Wall_E.Application.DSL;
using Wall_E.Domain;
using Xunit;

namespace Wall_E.Application.Tests;

public class ParserTests
{
    private static (Node ast, List<Error> errors) Parse(string code)
    {
        var lexer = new Lexer("test.geo", "1");
        var tokens = lexer.Tokens(code);
        var parser = new Parser(tokens);
        var ast = parser.Parse();
        return (ast, parser.Syntactic_Errors());
    }

    [Fact]
    public void Parse_simple_assignment()
    {
        var (ast, errors) = Parse("A = 5;");
        Assert.Empty(errors);
        Assert.Equal(Node.NodeType.GlobalVar, ast.Type);
    }

    [Fact]
    public void Parse_draw_statement()
    {
        var (ast, errors) = Parse("draw A;");
        Assert.Empty(errors);
        Assert.Equal(Node.NodeType.Draw, ast.Type);
    }

    [Fact]
    public void Parse_draw_with_tag()
    {
        var (ast, errors) = Parse("draw A \"hello\";");
        Assert.Empty(errors);
        Assert.Equal(Node.NodeType.Draw, ast.Type);
    }

    [Fact]
    public void Parse_color_statement()
    {
        var (ast, errors) = Parse("color red;");
        Assert.Empty(errors);
        Assert.Equal(Node.NodeType.Color, ast.Type);
    }

    [Fact]
    public void Parse_seed_statement()
    {
        var (ast, errors) = Parse("seed(42);");
        Assert.Empty(errors);
        Assert.Equal(Node.NodeType.Seed, ast.Type);
    }

    [Fact]
    public void Parse_print_statement()
    {
        var (ast, errors) = Parse("print(A);");
        Assert.Empty(errors);
        Assert.Equal(Node.NodeType.Print, ast.Type);
    }

    [Fact]
    public void Parse_grosor_statement()
    {
        var (ast, errors) = Parse("grosor(3);");
        Assert.Empty(errors);
        Assert.Equal(Node.NodeType.GrosorStmt, ast.Type);
    }

    [Fact]
    public void Parse_fill_statement()
    {
        var (ast, errors) = Parse("fill;");
        Assert.Empty(errors);
        Assert.Equal(Node.NodeType.FillStmt, ast.Type);
    }

    [Fact]
    public void Parse_layer_statement()
    {
        var (ast, errors) = Parse("layer 2;");
        Assert.Empty(errors);
        Assert.Equal(Node.NodeType.LayerStmt, ast.Type);
    }

    [Fact]
    public void Parse_snap_statement()
    {
        var (ast, errors) = Parse("snap 0.5;");
        Assert.Empty(errors);
        Assert.Equal(Node.NodeType.SnapStmt, ast.Type);
    }

    [Fact]
    public void Parse_line_style_statements()
    {
        var (a, ea) = Parse("dashed;");
        Assert.Empty(ea);
        Assert.Equal(Node.NodeType.LineStyleStmt, a.Type);

        var (b, eb) = Parse("dotted;");
        Assert.Empty(eb);
        Assert.Equal(Node.NodeType.LineStyleStmt, b.Type);

        var (c, ec) = Parse("dashdot;");
        Assert.Empty(ec);
        Assert.Equal(Node.NodeType.LineStyleStmt, c.Type);
    }

    [Fact]
    public void Parse_arithmetic_expression()
    {
        var (ast, errors) = Parse("A = 2 + 3 * 4;");
        Assert.Empty(errors);
        Assert.Equal(Node.NodeType.GlobalVar, ast.Type);
    }

    [Fact]
    public void Parse_conditional_expression()
    {
        var (ast, errors) = Parse("A = 5 > 3;");
        Assert.Empty(errors);
        Assert.Equal(Node.NodeType.GlobalVar, ast.Type);
    }

    [Fact]
    public void Parse_label_statement()
    {
        var (ast, errors) = Parse("label(point(0,0), \"hi\", 12);");
        Assert.Empty(errors);
        Assert.Equal(Node.NodeType.Label, ast.Type);
    }

    [Fact]
    public void Parse_intersect_expression()
    {
        var (ast, errors) = Parse("A = intersect(l, c);");
        Assert.Empty(errors);
        Assert.Equal(Node.NodeType.GlobalVar, ast.Type);
    }

    [Fact]
    public void Parse_count_expression()
    {
        var (ast, errors) = Parse("A = count(s);");
        Assert.Empty(errors);
        Assert.Equal(Node.NodeType.GlobalVar, ast.Type);
    }

    [Fact]
    public void Parse_let_in_statement()
    {
        var (ast, errors) = Parse("let x = 5 in x + 1");
        Assert.Empty(errors);
        Assert.Equal(Node.NodeType.Let_exp, ast.Type);
    }

    [Fact]
    public void Parse_function_declaration()
    {
        var (ast, errors) = Parse("f(x) = x + 1;");
        Assert.Empty(errors);
        Assert.Equal(Node.NodeType.Function, ast.Type);
    }

    [Fact]
    public void Parse_number_literal()
    {
        var (ast, errors) = Parse("A = 42.5;");
        Assert.Empty(errors);
        Assert.Equal(Node.NodeType.GlobalVar, ast.Type);
    }

    [Fact]
    public void Parse_string_literal()
    {
        var (ast, errors) = Parse("A = \"hello\";");
        Assert.Empty(errors);
        Assert.Equal(Node.NodeType.GlobalVar, ast.Type);
    }

    [Fact]
    public void Parse_boolean_operators()
    {
        var (ast, errors) = Parse("A = true and false or not true;");
        Assert.Empty(errors);
        Assert.Equal(Node.NodeType.GlobalVar, ast.Type);
    }

    [Fact]
    public void Parse_rgb_color()
    {
        var (ast, errors) = Parse("rgb(255, 0, 128);");
        Assert.Empty(errors);
        Assert.Equal(Node.NodeType.ColorRgb, ast.Type);
    }

    [Fact]
    public void Parse_hsl_color()
    {
        var (ast, errors) = Parse("hsl(180, 50, 50);");
        Assert.Empty(errors);
        Assert.Equal(Node.NodeType.ColorHsl, ast.Type);
    }
}
