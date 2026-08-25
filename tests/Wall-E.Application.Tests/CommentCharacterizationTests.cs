using Xunit;

namespace Wall_E.Application.Tests;

public class CommentCharacterizationTests
{
    [Fact]
    public void Full_line_comment_produces_no_statements()
    {
        var p = DslRunner.Run("// this is a comment");
        Assert.Empty(p.Errors);
        Assert.Equal(0, p.Scene.DrawCount);
    }

    [Fact]
    public void Trailing_comment_is_stripped()
    {
        var p = DslRunner.Run("draw point(0,0); // draw at origin");
        Assert.Empty(p.Errors);
        Assert.Equal(1, p.Scene.DrawCount);
    }

    [Fact]
    public void Comment_after_statement_separator_is_ignored()
    {
        var p = DslRunner.Run("draw point(0,0);\n// rest");
        Assert.Empty(p.Errors);
        Assert.Equal(1, p.Scene.DrawCount);
    }

    [Fact]
    public void Slashes_inside_string_are_not_comment()
    {
        var p = DslRunner.Run("draw point(0,0) \"http://example.com\";");
        Assert.Empty(p.Errors);
        Assert.Equal(1, p.Scene.DrawCount);
    }

    [Fact]
    public void Multiple_comments_and_statements_mix()
    {
        var p = DslRunner.Run("// setup\np1 = point(0,0);\n// draw\ndraw p1; // done");
        Assert.Empty(p.Errors);
        Assert.Equal(1, p.Scene.DrawCount);
    }
}
