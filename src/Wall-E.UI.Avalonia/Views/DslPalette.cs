using Avalonia.Media;

namespace Wall_E.UI.Avalonia.Views;

/// <summary>Maps DSL color names (and "#RGB"/"#RRGGBB" literals) to brushes.
/// Single source of truth for the canvas renderer and the ink strip.</summary>
public static class DslPalette
{
    public static IBrush ToBrush(string name)
    {
        var trimmed = name.Trim();
        if (trimmed.StartsWith('#'))
        {
            if (global::Avalonia.Media.Color.TryParse(trimmed, out var c))
                return new SolidColorBrush(c);
            return Brushes.Gray;
        }
        return trimmed.ToLowerInvariant() switch
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
}
