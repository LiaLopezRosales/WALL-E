using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Wall_E.UI.Avalonia.Views;

/// <summary>DSL color name -> brush, for XAML bindings (ink strip swatches).</summary>
public class DslPaletteToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is string name ? DslPalette.ToBrush(name) : Brushes.Gray;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
