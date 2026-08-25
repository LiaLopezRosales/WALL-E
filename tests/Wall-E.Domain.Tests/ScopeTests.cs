using Xunit;

namespace Wall_E.Domain.Tests;

public class ScopeTests
{
    [Fact]
    public void Root_scope_has_no_parent()
    {
        var scope = new Scope();
        Assert.Null(scope.Parent);
    }

    [Fact]
    public void Child_has_parent_reference()
    {
        var root = new Scope();
        var child = root.Child();
        Assert.Same(root, child.Parent);
    }

    [Fact]
    public void Child_inherits_variables()
    {
        var root = new Scope();
        root.Variables["x"] = 42.0;
        var child = root.Child();
        Assert.Equal(42.0, child.Variables["x"]);
    }

    [Fact]
    public void Child_modification_does_not_affect_parent()
    {
        var root = new Scope();
        root.Variables["x"] = 1.0;
        var child = root.Child();
        child.Variables["x"] = 2.0;
        Assert.Equal(1.0, root.Variables["x"]);
    }

    [Fact]
    public void New_variable_in_child_does_not_affect_parent()
    {
        var root = new Scope();
        var child = root.Child();
        child.Variables["y"] = 99.0;
        Assert.False(root.Variables.ContainsKey("y"));
    }
}
