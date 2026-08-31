using System;
using System.Collections.Generic;
using AvaloniaEdit.Document;
using AvaloniaEdit.Highlighting;
using AMedia = global::Avalonia.Media;

namespace Wall_E.UI.Avalonia.Views;

/// <summary>
/// Programmatic AvaloniaEdit highlighting definition for the GeoWall-E DSL.
/// Highlights keywords, figure/function calls, math constants, numbers, hex
/// colors, strings and comments. Theme-aware: two palettes (light/dark) so the
/// colors stay legible in both themes.
/// </summary>
public sealed class GeoWallEDslHighlighting : IHighlightingDefinition
{
    private static readonly IReadOnlyDictionary<bool, IHighlightingDefinition> Cache =
        new Dictionary<bool, IHighlightingDefinition>
        {
            [false] = Build(new Palette("#1A73E8", "#8E24AA", "#E67E22", "#4CAF50", "#9099A3", "#009688")),
            [true] = Build(new Palette("#7CB6FF", "#C792EA", "#F78C6C", "#C3E88D", "#7F8693", "#7DF3D1")),
        };

    private sealed record Palette(
        string Keyword, string Figure, string Number, string String, string Comment, string Hex);

    public string Name { get; } = "GeoWall-E";
    public HighlightingRuleSet MainRuleSet { get; } = new();

    public IEnumerable<HighlightingColor> NamedHighlightingColors { get; } = new List<HighlightingColor>();
    public IDictionary<string, string> Properties { get; } = new Dictionary<string, string>();

    public HighlightingRuleSet? GetNamedRuleSet(string name) => null;
    public HighlightingColor? GetNamedColor(string name) => null;

    /// <summary>Returns a cached definition for the requested theme.</summary>
    public static IHighlightingDefinition ForTheme(bool dark) => Cache[dark];

    private static IHighlightingDefinition Build(Palette p)
    {
        var def = new GeoWallEDslHighlighting();

        var keywordColor = new HighlightingColor { Foreground = Brush(p.Keyword), FontWeight = AMedia.FontWeight.SemiBold };
        var figureColor = new HighlightingColor { Foreground = Brush(p.Figure) };
        var numberColor = new HighlightingColor { Foreground = Brush(p.Number) };
        var stringColor = new HighlightingColor { Foreground = Brush(p.String) };
        var commentColor = new HighlightingColor { Foreground = Brush(p.Comment), FontStyle = AMedia.FontStyle.Italic };
        var hexColor = new HighlightingColor { Foreground = Brush(p.Hex), FontWeight = AMedia.FontWeight.SemiBold };

        // Block spans: line comments and quoted strings.
        def.MainRuleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new System.Text.RegularExpressions.Regex(@"//"),
            SpanColor = commentColor,
        });
        def.MainRuleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new System.Text.RegularExpressions.Regex(@""""),
            SpanColor = stringColor,
        });

        // Word keyword rules (word-boundary anchored).
        def.MainRuleSet.Rules.Add(Rule(@"\b(?:repeat|for|animate|from|to|in|let|if|else|then|import|draw|color|restore|label|fill|unfill|linear|radial|layer|hide|show|snap|seed|print|grosor|solid|dashed|dotted|dashdot|intersect|count)\b", keywordColor));
        def.MainRuleSet.Rules.Add(Rule(@"\b(?:point|line|segment|ray|circle|polygon|ellipse|arc|sequence|samples|randoms|points|measure)\b", figureColor));
        def.MainRuleSet.Rules.Add(Rule(@"\b(?:sin|cos|tan|atan|abs|floor|ceil|sqrt|exp|log|mix|lighten|darken|complement|rgb|rgba|hsl|phi|sqrt2|PI|E)\b", figureColor));

        // Numbers and hex colors.
        def.MainRuleSet.Rules.Add(Rule(@"\b(?:#(?:[0-9a-fA-F]{8}|[0-9a-fA-F]{6}|[0-9a-fA-F]{4}|[0-9a-fA-F]{3}))\b", hexColor));
        def.MainRuleSet.Rules.Add(Rule(@"\b\d+(?:\.\d+)?\b", numberColor));

        return def;
    }

    private static HighlightingRule Rule(string pattern, HighlightingColor color) =>
        new() { Regex = new System.Text.RegularExpressions.Regex(pattern), Color = color };

    private static HighlightingBrush Brush(string hex) =>
        new SimpleHighlightingBrush(AMedia.Color.Parse(hex));
}
