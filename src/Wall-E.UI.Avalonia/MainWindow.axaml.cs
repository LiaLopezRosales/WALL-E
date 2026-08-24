using Avalonia.Controls;
using Avalonia.Input;
using Wall_E.UI.Avalonia.ViewModels;

namespace Wall_E.UI.Avalonia;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContextChanged += (_, _) => WireViewModel();
        WireViewModel();
    }

    private void WireViewModel()
    {
        if (DataContext is not MainViewModel vm) return;
        vm.SceneChanged += (_, _) =>
        {
            Canvas.SetScene(vm.Scene);
            ProcessButton.Focus();
        };
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.F5 && DataContext is MainViewModel vm && !vm.IsProcessing)
            vm.ProcessCommand.Execute(null);
    }
}
