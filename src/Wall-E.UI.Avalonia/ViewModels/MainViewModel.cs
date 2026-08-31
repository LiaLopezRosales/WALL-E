using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using Wall_E.Application.Pipeline;
using Wall_E.Domain;

namespace Wall_E.UI.Avalonia.ViewModels;

public class MainViewModel : ViewModelBase
{
    private string _code = "p1 = point(100, 100);\ndraw p1;\n\nc = circle(point(200, 200), 50);\ndraw c;\n\ncolor blue;\ns = segment(point(80, 80), point(150, 260));\ndraw s;";
    private bool _isProcessing;
    private string _statusMessage = "Ready";
    private bool _statusIsError;
    private RenderScene? _scene;
    private int _lastDrawCount;
    private RenderScene? _lastScene;
    private int _lastColorCount;

    private readonly PipelineOrchestrator _pipeline = new();
    private readonly DispatcherTimer _streamTimer;
    private readonly DispatcherTimer _playTimer;

    private System.Collections.Generic.List<RenderScene> _animationFrames = new();
    private int _frameIndex;
    private bool _isPlaying;

    public string Code
    {
        get => _code;
        set => SetField(ref _code, value);
    }

    public bool IsProcessing
    {
        get => _isProcessing;
        private set
        {
            if (SetField(ref _isProcessing, value))
            {
                ProcessCommand?.RaiseCanExecuteChanged();
                StopCommand?.RaiseCanExecuteChanged();
                StressRunCommand?.RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(DisplayScene));
            }
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetField(ref _statusMessage, value);
    }

    public bool StatusIsError
    {
        get => _statusIsError;
        private set => SetField(ref _statusIsError, value);
    }

    /// <summary>Shows a status-bar message from UI-side operations (e.g. file
    /// open/save) that don't go through the pipeline.</summary>
    public void ReportStatus(string message, bool isError = false)
    {
        StatusMessage = message;
        StatusIsError = isError;
        OnPropertyChanged(nameof(StatusMessage));
        OnPropertyChanged(nameof(StatusIsError));
    }

    /// <summary>Latest finished scene snapshot.</summary>
    public RenderScene? Scene => _scene;

    /// <summary>What the canvas should render right now: the live pipeline
    /// scene during streaming, the current animation frame while playing,
    /// the finished scene otherwise.</summary>
    public RenderScene? DisplayScene => IsProcessing ? _pipeline.Scene : (IsPlaying ? CurrentFrame : _scene);

    public bool HasAnimation => _animationFrames.Count > 0;
    public int FrameCount => _animationFrames.Count;

    public bool IsPlaying
    {
        get => _isPlaying;
        private set
        {
            if (SetField(ref _isPlaying, value))
            {
                PlayPauseCommand?.RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(DisplayScene));
            }
        }
    }

    /// <summary>Current playback frame, clamped to the frame range.
    /// Exposed so the status bar can show "frame k / n".</summary>
    public int FrameIndex
    {
        get => _frameIndex;
        private set
        {
            if (SetField(ref _frameIndex, value))
                OnPropertyChanged(nameof(FrameLabel));
        }
    }

    /// <summary>Short "k/n" frame readout shown next to the Play control.</summary>
    public string FrameLabel => HasAnimation ? $"{FrameIndex + 1}/{FrameCount}" : "";

    private RenderScene? CurrentFrame =>
        _animationFrames.Count > 0 ? _animationFrames[FrameIndex % _animationFrames.Count] : null;

    public ObservableCollection<string> Errors { get; } = new();

    public bool HasErrors => Errors.Count > 0;
    public int ErrorCount => Errors.Count;

    /// <summary>Active DSL ink stack (top-first), shown as named swatches
    /// in the canvas header.</summary>
    public System.Collections.ObjectModel.ObservableCollection<string> InkColors { get; } = new();

    private bool _inkEmpty = true;
    public bool InkEmpty
    {
        get => _inkEmpty;
        private set => SetField(ref _inkEmpty, value);
    }

    public bool IsDarkTheme =>
        global::Avalonia.Application.Current?.RequestedThemeVariant != global::Avalonia.Styling.ThemeVariant.Light;

    public event EventHandler? SceneChanged;

    public RelayCommand ProcessCommand { get; }
    public RelayCommand ClearCommand { get; }
    public RelayCommand StopCommand { get; }
    public RelayCommand StressRunCommand { get; }
    public RelayCommand ToggleThemeCommand { get; }
    public RelayCommand PlayPauseCommand { get; }

    public MainViewModel()
    {
        ProcessCommand = new RelayCommand(_ => _ = RunAsync(), _ => !IsProcessing);
        ClearCommand = new RelayCommand(_ => Clear(), _ => !IsProcessing);
        StopCommand = new RelayCommand(_ => _pipeline.Cancel(), _ => IsProcessing);
        StressRunCommand = new RelayCommand(_ => _ = RunAsync(BuildStressProgram()), _ => !IsProcessing);
        ToggleThemeCommand = new RelayCommand(_ => ToggleTheme());
        PlayPauseCommand = new RelayCommand(_ => TogglePlayback(), _ => HasAnimation && !IsProcessing);

        // Progressive streaming (M3): poll the synchronized scene while the
        // pipeline runs on a background thread; each tick with new content
        // raises SceneChanged so the canvas repaints incrementally.
        _streamTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
        _streamTimer.Tick += (_, _) => PollSceneProgress();

        // Animation playback (M12a): advance frame index and repaint.
        _playTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(25) };
        _playTimer.Tick += (_, _) => PlaybackTick();

        Errors.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(HasErrors));
            OnPropertyChanged(nameof(ErrorCount));
        };
    }

    private async Task RunAsync(string? sourceOverride = null)
    {
        if (IsProcessing) return;
        StopPlayback();
        Errors.Clear();
        IsProcessing = true;
        StatusIsError = false;
        StatusMessage = "Processing...";
        _lastDrawCount = 0;
        _lastScene = null;

        string source = sourceOverride ?? Code;
        _streamTimer.Start();
        try
        {
            await Task.Run(() => _pipeline.Execute(source, "main.geo"));

            foreach (var error in _pipeline.Errors)
                Errors.Add(error.ToString());

            _scene = _pipeline.Scene;
            StatusMessage = _pipeline.Errors.Count > 0
                ? $"Finished with {_pipeline.Errors.Count} error(s)"
                : $"OK - {_scene.ToDraw.Count} object(s) to draw";
            StatusIsError = _pipeline.Errors.Count > 0;
        }
        catch (OperationCanceledException)
        {
            foreach (var error in _pipeline.Errors)
                Errors.Add(error.ToString());
            StatusMessage = $"Cancelled - {_lastDrawCount} object(s) drawn";
            _scene = _pipeline.Scene;
        }
        catch (Exception ex)
        {
            Errors.Add(ex.Message);
            StatusMessage = "Unexpected error";
            StatusIsError = true;
        }
        finally
        {
            _streamTimer.Stop();
            IsProcessing = false;

            // Surface animation frames (if any) for playback.
            var frames = _pipeline.Frames;
            _animationFrames = frames.Count > 0 ? new System.Collections.Generic.List<RenderScene>(frames) : new();
            FrameIndex = 0;
            OnPropertyChanged(nameof(HasAnimation));
            OnPropertyChanged(nameof(FrameCount));
            OnPropertyChanged(nameof(FrameLabel));
            OnPropertyChanged(nameof(Scene));
            OnPropertyChanged(nameof(DisplayScene));
            UpdateInkStrip();
            SceneChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void TogglePlayback()
    {
        if (_animationFrames.Count == 0 || IsProcessing) return;
        IsPlaying = !IsPlaying;
        StatusMessage = IsPlaying ? $"Playing frame {FrameIndex + 1}/{_animationFrames.Count}" : $"Paused at frame {FrameIndex + 1}/{_animationFrames.Count}";
        StatusIsError = false;
        if (IsPlaying) _playTimer.Start(); else _playTimer.Stop();
    }

    private void StopPlayback()
    {
        _playTimer.Stop();
        IsPlaying = false;
    }

    private void PlaybackTick()
    {
        FrameIndex = (FrameIndex + 1) % _animationFrames.Count;
        StatusMessage = $"Playing frame {FrameIndex + 1}/{_animationFrames.Count}";
        SceneChanged?.Invoke(this, EventArgs.Empty);
    }

    private void PollSceneProgress()
    {
        var scene = _pipeline.Scene;
        int count = scene.DrawCount;
        bool sceneSwapped = !ReferenceEquals(_lastScene, scene);
        _lastScene = scene;
        int colorCount = scene.ColorCount;
        bool colorsChanged = colorCount != _lastColorCount;
        if (count == _lastDrawCount && !sceneSwapped && !colorsChanged) return;
        _lastDrawCount = count;
        _lastColorCount = colorCount;
        StatusMessage = $"Drawing... {count} object(s)";
        if (colorsChanged || sceneSwapped)
            UpdateInkStrip();
        SceneChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ToggleTheme()
    {
        var app = global::Avalonia.Application.Current;
        if (app is null) return;
        app.RequestedThemeVariant = IsDarkTheme
            ? global::Avalonia.Styling.ThemeVariant.Light
            : global::Avalonia.Styling.ThemeVariant.Dark;
        OnPropertyChanged(nameof(IsDarkTheme));
    }

    /// <summary>Sets the initial ink used by programs without an explicit
    /// color statement. Stored as hex on the orchestrator and applied when
    /// the next run creates its scene (safe to change mid-run).</summary>
    public void SetDefaultInk(string hex)
    {
        _pipeline.InitialInk = hex;
        StatusMessage = $"Tinta inicial: {hex} (se aplica en la próxima ejecución)";
        StatusIsError = false;
    }

    /// <summary>Refreshes the ink swatch strip from the scene's color stack
    /// (top-first, capped at 8 for space).</summary>
    private void UpdateInkStrip()
    {
        var colors = _pipeline.Scene.ColorsTake(8);
        InkColors.Clear();
        foreach (var c in colors)
            InkColors.Add(c);
        InkEmpty = InkColors.Count == 0;
    }

    private void Clear()
    {
        if (IsProcessing) return;
        StopPlayback();
        _animationFrames = new();
        FrameIndex = 0;
        _scene = new RenderScene();
        Errors.Clear();
        StatusMessage = "Canvas cleared";
        StatusIsError = false;
        InkColors.Clear();
        InkEmpty = true;
        OnPropertyChanged(nameof(HasAnimation));
        OnPropertyChanged(nameof(FrameCount));
        OnPropertyChanged(nameof(FrameLabel));
        OnPropertyChanged(nameof(Scene));
        SceneChanged?.Invoke(this, EventArgs.Empty);
    }

    private const int StressPoints = 200000; // ~2.5s at ~1.27ms/100 statements
    private static readonly string[] DemoColors = { "cyan", "green", "yellow", "red", "magenta", "blue" };

    /// <summary>Generates a long valid DSL program (a rainbow spiral of
    /// points around two anchor circles). Public static: the stress probe
    /// runs it through the pipeline without duplicating the generator.</summary>
    public static string BuildStressProgram(int points = StressPoints)
    {
        var sb = new System.Text.StringBuilder(points * 26);
        sb.AppendLine("c0 = circle(point(0,0), 30);");
        sb.AppendLine("draw c0;");
        sb.AppendLine("color blue;");
        sb.AppendLine("draw circle(point(0,0), 12);");
        int colorStride = System.Math.Max(points / DemoColors.Length, 1);
        for (int i = 0; i < points; i++)
        {
            if (i % colorStride == 0)
            {
                int colorIdx = System.Math.Min(i / colorStride, DemoColors.Length - 1);
                sb.Append("color ").Append(DemoColors[colorIdx]).Append(';').AppendLine();
            }
            double angle = i * System.Math.PI / 180 * 2.2;
            double radius = 3 + i * 0.30;
            double x = System.Math.Round(radius * System.Math.Cos(angle), 2);
            double y = System.Math.Round(radius * System.Math.Sin(angle), 2);
            sb.Append("draw point(")
              .Append(x.ToString(System.Globalization.CultureInfo.InvariantCulture))
              .Append(',')
              .Append(y.ToString(System.Globalization.CultureInfo.InvariantCulture))
              .Append(");").Append('\n');
        }
        return sb.ToString();
    }
}
