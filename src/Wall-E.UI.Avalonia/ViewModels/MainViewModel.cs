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

    private readonly PipelineOrchestrator _pipeline = new();
    private readonly DispatcherTimer _streamTimer;

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

    /// <summary>Latest finished scene snapshot.</summary>
    public RenderScene? Scene => _scene;

    /// <summary>What the canvas should render right now: the live pipeline
    /// scene during streaming, the finished scene afterwards.</summary>
    public RenderScene? DisplayScene => IsProcessing ? _pipeline.Scene : _scene;

    public ObservableCollection<string> Errors { get; } = new();

    public bool HasErrors => Errors.Count > 0;
    public int ErrorCount => Errors.Count;

    public event EventHandler? SceneChanged;

    public RelayCommand ProcessCommand { get; }
    public RelayCommand ClearCommand { get; }
    public RelayCommand StopCommand { get; }

    public MainViewModel()
    {
        ProcessCommand = new RelayCommand(_ => _ = RunAsync(), _ => !IsProcessing);
        ClearCommand = new RelayCommand(_ => Clear(), _ => !IsProcessing);
        StopCommand = new RelayCommand(_ => _pipeline.Cancel(), _ => IsProcessing);

        // Progressive streaming (M3): poll the synchronized scene while the
        // pipeline runs on a background thread; each tick with new content
        // raises SceneChanged so the canvas repaints incrementally.
        _streamTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(90) };
        _streamTimer.Tick += (_, _) => PollSceneProgress();

        Errors.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(HasErrors));
            OnPropertyChanged(nameof(ErrorCount));
        };
    }

    private async Task RunAsync()
    {
        Errors.Clear();
        IsProcessing = true;
        StatusIsError = false;
        StatusMessage = "Processing...";
        _lastDrawCount = 0;

        _streamTimer.Start();
        try
        {
            await Task.Run(() => _pipeline.Execute(Code, "main.geo"));

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
            OnPropertyChanged(nameof(Scene));
            OnPropertyChanged(nameof(DisplayScene));
            SceneChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void PollSceneProgress()
    {
        int count = _pipeline.Scene.DrawCount;
        if (count == _lastDrawCount) return;
        _lastDrawCount = count;
        StatusMessage = $"Drawing... {count} object(s)";
        SceneChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Clear()
    {
        if (IsProcessing) return;
        _scene = new RenderScene();
        Errors.Clear();
        StatusMessage = "Canvas cleared";
        StatusIsError = false;
        OnPropertyChanged(nameof(Scene));
        SceneChanged?.Invoke(this, EventArgs.Empty);
    }
}
