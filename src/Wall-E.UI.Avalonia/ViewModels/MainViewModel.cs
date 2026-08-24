using System.Collections.ObjectModel;
using Wall_E.Application.Pipeline;
using Wall_E.Domain;

namespace Wall_E.UI.Avalonia.ViewModels;

public class MainViewModel : ViewModelBase
{
    private string _code = "p1 = point(100, 100);\ndraw p1;\n\nc = circle(point(200, 200), 50);\ndraw c;\n\ncolor blue;\ns = segment(point(80, 80), point(150, 260));\ndraw s;";
    private bool _isProcessing;
    private string _statusMessage = "Ready";
    private RenderScene? _scene;

    private readonly PipelineOrchestrator _pipeline = new();

    public string Code
    {
        get => _code;
        set => SetField(ref _code, value);
    }

    public bool IsProcessing
    {
        get => _isProcessing;
        private set => SetField(ref _isProcessing, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetField(ref _statusMessage, value);
    }

    /// <summary>Latest rendered scene; the canvas listens for SceneChanged.</summary>
    public RenderScene? Scene => _scene;

    public ObservableCollection<string> Errors { get; } = new();

    public bool HasErrors => Errors.Count > 0;
    public int ErrorCount => Errors.Count;

    private bool _statusIsError;
    public bool StatusIsError
    {
        get => _statusIsError;
        private set => SetField(ref _statusIsError, value);
    }

    public event EventHandler? SceneChanged;

    public RelayCommand ProcessCommand { get; }
    public RelayCommand ClearCommand { get; }

    public MainViewModel()
    {
        ProcessCommand = new RelayCommand(_ => Execute(), _ => !IsProcessing);
        ClearCommand = new RelayCommand(_ => Clear());
        Errors.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(HasErrors));
            OnPropertyChanged(nameof(ErrorCount));
        };
    }

    private void Execute()
    {
        Errors.Clear();
        IsProcessing = true;
        StatusMessage = "Processing...";
        StatusIsError = false;
        try
        {
            _pipeline.Execute(Code, "main.geo");

            foreach (var error in _pipeline.Errors)
                Errors.Add(error.ToString());

            _scene = _pipeline.Scene;
            var figureCount = _scene.ToDraw.Count;
            StatusMessage = _pipeline.Errors.Count > 0
                ? $"Finished with {_pipeline.Errors.Count} error(s)"
                : $"OK - {figureCount} object(s) to draw";
            StatusIsError = _pipeline.Errors.Count > 0;
        }
        catch (System.OperationCanceledException)
        {
            StatusMessage = "Cancelled";
            StatusIsError = false;
        }
        catch (System.Exception ex)
        {
            Errors.Add(ex.Message);
            StatusMessage = "Unexpected error";
            StatusIsError = true;
        }
        finally
        {
            IsProcessing = false;
            OnPropertyChanged(nameof(Scene));
            SceneChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Clear()
    {
        _scene = new RenderScene();
        Errors.Clear();
        StatusMessage = "Canvas cleared";
        StatusIsError = false;
        OnPropertyChanged(nameof(Scene));
        SceneChanged?.Invoke(this, EventArgs.Empty);
    }
}
