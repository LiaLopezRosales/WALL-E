using Wall_E.Domain;
using Xunit;

namespace Wall_E.Application.Tests;

public class HideShowTests
{
    [Fact]
    public void Hide_sets_label_hidden()
    {
        var p = DslRunner.Run("hide myLabel;");
        Assert.Empty(p.Errors);
        Assert.Contains("myLabel", p.Scene.HiddenLabels);
    }

    [Fact]
    public void Show_removes_label_from_hidden()
    {
        var p = DslRunner.Run("hide myLabel; show myLabel;");
        Assert.Empty(p.Errors);
        Assert.DoesNotContain("myLabel", p.Scene.HiddenLabels);
    }

    [Fact]
    public void Hide_multiple_labels()
    {
        var p = DslRunner.Run("hide a; hide b; hide c;");
        Assert.Empty(p.Errors);
        Assert.Equal(3, p.Scene.HiddenLabels.Count);
        Assert.Contains("a", p.Scene.HiddenLabels);
        Assert.Contains("b", p.Scene.HiddenLabels);
        Assert.Contains("c", p.Scene.HiddenLabels);
    }
}
