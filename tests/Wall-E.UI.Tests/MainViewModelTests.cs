using Avalonia.Headless.XUnit;
using Wall_E.UI.Avalonia.ViewModels;

namespace Wall_E.UI.Tests;

public class MainViewModelTests
{
    private const string TwoShapesProgram =
        "p1 = point(100, 100);\ndraw p1;\nc = circle(point(200, 200), 50);\ndraw c;\n";

    private const string ErrorProgram = "draw q;\n";

    private const string AnimateProgram =
        "b = point(100, 100);\ne = point(300, 100);\nanimate(t from 0 to 1) { p = point(100 + t * 200, 100); draw p; }\n";

    private static async Task<bool> WaitUntilAsync(Func<bool> condition, int timeoutMs = 15000)
    {
        var deadline = Environment.TickCount64 + timeoutMs;
        while (Environment.TickCount64 < deadline)
        {
            if (condition()) return true;
            await Task.Delay(25);
        }
        return condition();
    }

    [AvaloniaFact]
    public async Task Process_simple_program_populates_scene()
    {
        var vm = new MainViewModel { Code = TwoShapesProgram };
        vm.ProcessCommand.Execute(null);
        Assert.True(await WaitUntilAsync(() => !vm.IsProcessing));

        Assert.False(vm.HasErrors);
        Assert.True(vm.Scene is not null);
        Assert.Equal(2, vm.Scene!.ToDraw.Count);
        Assert.Contains("OK", vm.StatusMessage);
    }

    [AvaloniaFact]
    public async Task Process_with_semantic_error_reports_errors()
    {
        var vm = new MainViewModel { Code = ErrorProgram };
        vm.ProcessCommand.Execute(null);
        Assert.True(await WaitUntilAsync(() => !vm.IsProcessing));

        Assert.True(vm.HasErrors);
        Assert.True(vm.ErrorCount > 0);
        Assert.True(vm.StatusIsError);
    }

    [AvaloniaFact]
    public async Task Animate_program_loads_frames_and_playback_toggles()
    {
        var vm = new MainViewModel { Code = AnimateProgram };
        vm.ProcessCommand.Execute(null);
        Assert.True(await WaitUntilAsync(() => !vm.IsProcessing));

        Assert.True(vm.HasAnimation);
        Assert.True(vm.FrameCount > 0);
        Assert.True(vm.PlayPauseCommand.CanExecute(null));

        vm.PlayPauseCommand.Execute(null);
        Assert.True(vm.IsPlaying);

        vm.PlayPauseCommand.Execute(null);
        Assert.False(vm.IsPlaying);
    }

    [AvaloniaFact]
    public async Task Clear_resets_scene_and_animation()
    {
        var vm = new MainViewModel { Code = AnimateProgram };
        vm.ProcessCommand.Execute(null);
        Assert.True(await WaitUntilAsync(() => !vm.IsProcessing));
        Assert.True(vm.HasAnimation);

        vm.ClearCommand.Execute(null);

        Assert.False(vm.HasAnimation);
        Assert.Equal(0, vm.FrameCount);
        Assert.True(vm.Scene is not null);
        Assert.Empty(vm.Scene!.ToDraw);
        Assert.Equal("Canvas cleared", vm.StatusMessage);
        Assert.False(vm.PlayPauseCommand.CanExecute(null));
    }

    [AvaloniaFact]
    public async Task SetDefaultInk_is_applied_on_next_run()
    {
        var vm = new MainViewModel { Code = "draw circle(point(200, 200), 50);\n" };
        vm.SetDefaultInk("#ff0000");
        vm.ProcessCommand.Execute(null);
        Assert.True(await WaitUntilAsync(() => !vm.IsProcessing));

        Assert.True(vm.DisplayScene is not null);
        Assert.Contains("#ff0000", vm.DisplayScene!.ColorsTake(8));
    }

    [AvaloniaFact]
    public async Task DisplayScene_reflects_final_scene_after_run()
    {
        var vm = new MainViewModel { Code = TwoShapesProgram };
        Assert.True(vm.DisplayScene is null);

        vm.ProcessCommand.Execute(null);
        Assert.True(await WaitUntilAsync(() => !vm.IsProcessing));

        Assert.Equal(vm.Scene, vm.DisplayScene);
        Assert.Equal(2, vm.DisplayScene!.ToDraw.Count);
    }
}