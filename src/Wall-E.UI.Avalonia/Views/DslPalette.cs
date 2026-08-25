using Avalonia.Media;
using Wall_E.Domain;

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
        if (ColorTable.TryGetHex(trimmed, out var hex) &&
            global::Avalonia.Media.Color.TryParse(hex, out var css))
            return new SolidColorBrush(css);
        return Brushes.Gray;
    }
}
