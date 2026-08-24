using Avalonia.Media;

namespace Wall_E.UI.Avalonia.Views;

/// <summary>Maps the 9 DSL color names to brushes. Single source of truth
/// for the canvas renderer and the status-bar ink strip.</summary>
public static class DslPalette
{
    public static IBrush ToBrush(string name) => name.ToLowerInvariant() switch
    {
        "black" => Brushes.Black,
        "white" => Brushes.White,
        "blue" => Brushes.DodgerBlue,
        "red" => Brushes.Red,
        "yellow" => Brushes.Yellow,
        "green" => Brushes.LimeGreen,
        "cyan" => Brushes.Cyan,
        "magenta" => Brushes.Magenta,
        "grey" => Brushes.Gray,
        _ => Brushes.Gray,
    };
}
