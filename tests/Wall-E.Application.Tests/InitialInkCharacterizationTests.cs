using Wall_E.Application.Pipeline;
using Wall_E.Domain;
using Xunit;

namespace Wall_E.Application.Tests;

public class InitialInkCharacterizationTests
{
    [Fact]
    public void Default_initial_ink_is_black()
    {
        var pipeline = DslRunner.Run("draw point(0,0);");

        Assert.Empty(pipeline.Errors);
        var draw = Assert.IsType<DrawObject>(pipeline.Scene.Snapshot()[0]);
        Assert.Equal("black", draw.UsedColor);
    }

    [Fact]
    public void InitialInk_colors_draws_without_explicit_color()
    {
        var orchestrator = new PipelineOrchestrator { InitialInk = "red" };
        var pipeline = DslRunner.Run("draw point(0,0);", orchestrator);

        Assert.Empty(pipeline.Errors);
        var draw = Assert.IsType<DrawObject>(pipeline.Scene.Snapshot()[0]);
        Assert.Equal("red", draw.UsedColor);
    }

    [Fact]
    public void Explicit_color_statement_wins_over_InitialInk()
    {
        var orchestrator = new PipelineOrchestrator { InitialInk = "red" };
        var pipeline = DslRunner.Run("""
            color blue;
            draw point(0,0);
            """, orchestrator);

        Assert.Empty(pipeline.Errors);
        var draw = Assert.IsType<DrawObject>(pipeline.Scene.Snapshot()[0]);
        Assert.Equal("#0000FF", draw.UsedColor);
    }

    [Fact]
    public void Restore_after_InitialInk_returns_to_base()
    {
        var orchestrator = new PipelineOrchestrator { InitialInk = "red" };
        var pipeline = DslRunner.Run("""
            color blue;
            draw point(0,0);
            restore;
            draw point(10,10);
            """, orchestrator);

        Assert.Empty(pipeline.Errors);
        var snapshot = pipeline.Scene.Snapshot();
        Assert.Equal(2, snapshot.Count);
        // After `restore`, the stack pops back to the initial ink (not black).
        var second = Assert.IsType<DrawObject>(snapshot[1]);
        Assert.Equal("red", second.UsedColor);
    }

    [Fact]
    public void InitialInk_accepts_hex_literals()
    {
        var orchestrator = new PipelineOrchestrator { InitialInk = "#FF8800" };
        var pipeline = DslRunner.Run("draw point(0,0);", orchestrator);

        Assert.Empty(pipeline.Errors);
        var draw = Assert.IsType<DrawObject>(pipeline.Scene.Snapshot()[0]);
        Assert.Equal("#FF8800", draw.UsedColor);
    }

    [Fact]
    public void Blank_InitialInk_is_ignored()
    {
        var orchestrator = new PipelineOrchestrator { InitialInk = "   " };
        var pipeline = DslRunner.Run("draw point(0,0);", orchestrator);

        Assert.Empty(pipeline.Errors);
        var draw = Assert.IsType<DrawObject>(pipeline.Scene.Snapshot()[0]);
        Assert.Equal("black", draw.UsedColor);
    }
}
