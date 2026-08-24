using Avalonia.Controls;
using Avalonia.Input;
using Wall_E.UI.Avalonia.ViewModels;

namespace Wall_E.UI.Avalonia;

public partial class MainWindow : Window
{
    private MainViewModel? _vm;

    public MainWindow()
    {
        InitializeComponent();
        DataContextChanged += (_, _) => WireViewModel();
        WireViewModel();

        Canvas.CursorWorldPositionChanged += (x, y) =>
            CursorPos.Text = $"x: {x:F1}   y: {y:F1}";
        Canvas.CursorLeftCanvas += () => CursorPos.Text = "x: —   y: —";
        FitButton.Click += (_, _) => Canvas.FitToContent();

        // Named fields are not generated for controls inside a Flyout -
        // resolve the paper picker through the flyout's content.
        if (PaperButton.Flyout is global::Avalonia.Controls.Flyout paperFlyout &&
            paperFlyout.Content is global::Avalonia.Controls.ColorPicker paperPicker)
        {
            paperPicker.Color = global::Avalonia.Media.Colors.White;
            paperPicker.ColorChanged += (_, e) =>
            {
                var c = e.NewColor;
                Canvas.Paper = new global::Avalonia.Media.SolidColorBrush(
                    global::Avalonia.Media.Color.FromArgb(255, c.R, c.G, c.B));
                PaperSwatch.Fill = Canvas.Paper;
            };
        }
    }

    private void WireViewModel()
    {
        if (_vm is not null) _vm.SceneChanged -= VmOnSceneChanged;
        _vm = DataContext as MainViewModel;
        if (_vm is not null) _vm.SceneChanged += VmOnSceneChanged;
    }

    private void VmOnSceneChanged(object? sender, EventArgs e)
    {
        Canvas.SetScene(_vm!.DisplayScene);
        ProcessButton.Focus();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.F5 && _vm is { IsProcessing: false })
            _vm.ProcessCommand.Execute(null);
    }
}
