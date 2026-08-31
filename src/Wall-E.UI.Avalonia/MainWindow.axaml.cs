using System.IO;
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
        OpenButton.Click += async (_, _) =>
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel is null || _vm is null) return;
            var file = await topLevel.StorageProvider.OpenFilePickerAsync(new global::Avalonia.Platform.Storage.FilePickerOpenOptions
            {
                Title = "Cargar programa (.geo)",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new global::Avalonia.Platform.Storage.FilePickerFileType("GeoWall-E Program")
                    {
                        Patterns = new[] { "*.geo" }
                    },
                    global::Avalonia.Platform.Storage.FilePickerFileTypes.All
                }
            });
            if (file is null || file.Count == 0) return;
            try
            {
                var path = file[0].Path.LocalPath;
                _vm.Code = await File.ReadAllTextAsync(path);
            }
            catch (Exception ex)
            {
                _vm.ReportStatus($"Error al abrir: {ex.Message}", isError: true);
            }
        };
        SaveButton.Click += async (_, _) =>
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel is null || _vm is null) return;
            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new global::Avalonia.Platform.Storage.FilePickerSaveOptions
            {
                Title = "Guardar programa como .geo",
                SuggestedFileName = "program.geo",
                FileTypeChoices = new[]
                {
                    new global::Avalonia.Platform.Storage.FilePickerFileType("GeoWall-E Program")
                    {
                        Patterns = new[] { "*.geo" }
                    }
                }
            });
            if (file is not null)
            {
                try
                {
                    await File.WriteAllTextAsync(file.Path.LocalPath, _vm.Code);
                }
                catch (Exception ex)
                {
                    _vm.ReportStatus($"Error al guardar: {ex.Message}", isError: true);
                }
            }
        };
        ExportPngButton.Click += async (_, _) =>
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel is null) return;
            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new global::Avalonia.Platform.Storage.FilePickerSaveOptions
            {
                Title = "Exportar como PNG",
                SuggestedFileName = "scene.png",
                FileTypeChoices = new[]
                {
                    new global::Avalonia.Platform.Storage.FilePickerFileType("PNG Image")
                    {
                        Patterns = new[] { "*.png" }
                    }
                }
            });
            if (file is not null)
            {
                var path = file.Path.LocalPath;
                await Canvas.ExportPngAsync(path);
            }
        };

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

        // Same flyout resolution for the initial-ink picker: it feeds the
        // orchestrator's InitialInk (applied on the next run) and repaints
        // its swatch face.
        if (InkPickerButton.Flyout is global::Avalonia.Controls.Flyout inkFlyout &&
            inkFlyout.Content is global::Avalonia.Controls.ColorPicker inkPicker)
        {
            inkPicker.Color = global::Avalonia.Media.Colors.Black;
            inkPicker.ColorChanged += (_, e) =>
            {
                var c = e.NewColor;
                var hex = $"#{c.R:X2}{c.G:X2}{c.B:X2}";
                _vm?.SetDefaultInk(hex);
                InkSwatch.Fill = new global::Avalonia.Media.SolidColorBrush(
                    global::Avalonia.Media.Color.FromArgb(255, c.R, c.G, c.B));
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
