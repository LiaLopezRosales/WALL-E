using System.Globalization;
using Avalonia;
using Avalonia.Media;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;
using Wall_E.Domain;

namespace Wall_E.UI.Avalonia.Views;

/// <summary>
/// Custom draw operation that renders the scene graph via raw SKCanvas.
/// Enables batch GPU rendering: dots are batched per color into single
/// DrawPoints calls, and polygons/lines use native SKPath for minimal
/// draw-call overhead.
/// </summary>
internal sealed class SkiaDrawOperation : ICustomDrawOperation
{
    private readonly IReadOnlyList<DrawingCanvas.Shape> _shapes;
    private readonly double _scale, _centerX, _centerY;
    private readonly double _dotRadius, _strokeWidth;
    private readonly int _stride;
    private readonly IBrush _paper;
    private readonly HashSet<string>? _hiddenLabels;

    // Reusable SKPaint objects — allocated once, reused per frame.
    private readonly SKPaint _dotPaint = new() { IsAntialias = true, Style = SKPaintStyle.Fill };
    private readonly SKPaint _linePaint = new() { IsAntialias = true, Style = SKPaintStyle.Stroke };
    private readonly SKPaint _fillStrokePaint = new() { IsAntialias = true, Style = SKPaintStyle.Stroke };
    private readonly SKPaint _whiteHaloPaint = new()
    {
        IsAntialias = true, Style = SKPaintStyle.Stroke,
        Color = new SKColor(200, 200, 200), StrokeWidth = 1
    };

    public SkiaDrawOperation(
        Rect bounds,
        IReadOnlyList<DrawingCanvas.Shape> shapes,
        double scale, double centerX, double centerY,
        double dotRadius, double strokeWidth,
        int stride,
        IBrush paper,
        HashSet<string>? hiddenLabels)
    {
        Bounds = bounds;
        _shapes = shapes;
        _scale = scale;
        _centerX = centerX;
        _centerY = centerY;
        _dotRadius = dotRadius;
        _strokeWidth = strokeWidth;
        _stride = stride;
        _paper = paper;
        _hiddenLabels = hiddenLabels;
    }

    public Rect Bounds { get; }

    public void Render(ImmediateDrawingContext context)
    {
        var feature = context.TryGetFeature(typeof(ISkiaSharpApiLeaseFeature)) as ISkiaSharpApiLeaseFeature;
        if (feature is null) return;

        using var lease = feature.Lease();
        var canvas = lease.SkCanvas;

        // Draw paper background.
        var paperColor = (_paper as ISolidColorBrush)?.Color ?? Colors.White;
        canvas.Clear(new SKColor(paperColor.R, paperColor.G, paperColor.B, paperColor.A));

        canvas.Save();
        // Map world→screen: translate center, scale.
        canvas.Translate((float)(Bounds.Width / 2 - _centerX * _scale),
                         (float)(Bounds.Height / 2 + _centerY * _scale));
        canvas.Scale((float)_scale, (float)-_scale);

        float dotR = (float)(_dotRadius / _scale);
        float lineW = (float)(_strokeWidth / _scale);

        // --- Batch dots per color ---
        var dotsByColor = new Dictionary<string, List<SKPoint>>();
        // Collect everything else for sequential draw.
        var others = new List<DrawingCanvas.Shape>();

        for (int i = 0; i < _shapes.Count; i++)
        {
            var shape = _shapes[i];
            if (_stride > 1 && shape is DrawingCanvas.DotShape && i % _stride != 0) continue;
            if (shape is DrawingCanvas.TagShape ts && _hiddenLabels != null && _hiddenLabels.Contains(ts.Tag)) continue;

            switch (shape)
            {
                case DrawingCanvas.DotShape d:
                    if (!dotsByColor.TryGetValue(d.Color, out var list))
                    {
                        list = new List<SKPoint>();
                        dotsByColor[d.Color] = list;
                    }
                    list.Add(new SKPoint((float)d.X, (float)d.Y));
                    break;
                default:
                    others.Add(shape);
                    break;
            }
        }

        // Draw batched dots.
        foreach (var kvp in dotsByColor)
        {
            var pts = kvp.Value;
            if (pts.Count == 0) continue;
            var skColor = SKColor.Parse(kvp.Key);
            _dotPaint.Color = skColor;
            _dotPaint.StrokeWidth = dotR * 2;
            _dotPaint.StrokeCap = SKStrokeCap.Round;
            canvas.DrawPoints(SKPointMode.Points, pts.ToArray(), _dotPaint);
        }

        // Draw remaining shapes (lines, circles, polygons, tags).
        foreach (var shape in others)
        {
            DrawShape(canvas, shape, lineW);
        }

        canvas.Restore();
    }

    private void DrawShape(SKCanvas canvas, DrawingCanvas.Shape shape, float lineW)
    {
        var skColor = SKColor.Parse(shape.Color);
        bool isWhite = string.Equals(shape.Color.Trim(), "white", StringComparison.OrdinalIgnoreCase);

        switch (shape)
        {
            case DrawingCanvas.SegShape s:
            {
                if (isWhite)
                {
                    _whiteHaloPaint.StrokeWidth = lineW * 1.5f;
                    canvas.DrawLine((float)s.X1, (float)s.Y1, (float)s.X2, (float)s.Y2, _whiteHaloPaint);
                }
                _linePaint.Color = isWhite ? SKColors.White : skColor;
                _linePaint.StrokeWidth = lineW;
                _linePaint.StrokeCap = SKStrokeCap.Round;
                ApplyDashStyle(_linePaint, shape.LineStyle);
                canvas.DrawLine((float)s.X1, (float)s.Y1, (float)s.X2, (float)s.Y2, _linePaint);
                break;
            }
            case DrawingCanvas.CircleShape c:
            {
                using var path = new SKPath();
                path.AddCircle((float)c.X, (float)c.Y, (float)c.R);
                // Fill
                if (c.FillType == FillType.Solid)
                {
                    _fillStrokePaint.Color = skColor;
                    _fillStrokePaint.Style = SKPaintStyle.Fill;
                    canvas.DrawPath(path, _fillStrokePaint);
                }
                else if (c.FillType == FillType.LinearGradient || c.FillType == FillType.RadialGradient)
                {
                    DrawGradientFill(canvas, path, c, new SKRect(
                        (float)(c.X - c.R), (float)(c.Y - c.R),
                        (float)(c.X + c.R), (float)(c.Y + c.R)));
                }
                // Stroke
                _linePaint.Color = isWhite ? SKColors.White : skColor;
                _linePaint.StrokeWidth = lineW;
                _linePaint.Style = SKPaintStyle.Stroke;
                ApplyDashStyle(_linePaint, shape.LineStyle);
                canvas.DrawPath(path, _linePaint);
                break;
            }
            case DrawingCanvas.PolyShape p:
            {
                if (p.Points.Count < 2) break;
                using var path = new SKPath();
                path.MoveTo((float)p.Points[0].x, (float)p.Points[0].y);
                for (int i = 1; i < p.Points.Count; i++)
                    path.LineTo((float)p.Points[i].x, (float)p.Points[i].y);
                path.Close();
                // Fill
                if (p.FillType == FillType.Solid)
                {
                    _fillStrokePaint.Color = skColor;
                    _fillStrokePaint.Style = SKPaintStyle.Fill;
                    canvas.DrawPath(path, _fillStrokePaint);
                }
                else if (p.FillType == FillType.LinearGradient || p.FillType == FillType.RadialGradient)
                {
                    path.GetBounds(out var bounds);
                    DrawGradientFill(canvas, path, p, bounds);
                }
                // Stroke
                _linePaint.Color = isWhite ? SKColors.White : skColor;
                _linePaint.StrokeWidth = lineW;
                _linePaint.Style = SKPaintStyle.Stroke;
                ApplyDashStyle(_linePaint, shape.LineStyle);
                canvas.DrawPath(path, _linePaint);
                break;
            }
            case DrawingCanvas.TagShape t:
            {
                // Tags use the Avalonia FormattedText path — too complex for
                // raw SKCanvas without font management. Skip in GPU mode;
                // the fallback Avalonia Render handles tags.
                break;
            }
        }
    }

    private void DrawGradientFill(SKCanvas canvas, SKPath path, DrawingCanvas.Shape shape, SKRect bounds)
    {
        using var shader = shape.FillType == FillType.LinearGradient
            ? SKShader.CreateLinearGradient(
                new SKPoint(bounds.Left, bounds.Top),
                new SKPoint(bounds.Right, bounds.Bottom),
                new[] { SKColor.Parse(shape.GradientColor1), SKColor.Parse(shape.GradientColor2) },
                new float[] { 0, 1 },
                SKShaderTileMode.Clamp)
            : SKShader.CreateRadialGradient(
                new SKPoint(bounds.MidX, bounds.MidY),
                Math.Max(bounds.Width, bounds.Height) / 2f,
                new[] { SKColor.Parse(shape.GradientColor1), SKColor.Parse(shape.GradientColor2) },
                new float[] { 0, 1 },
                SKShaderTileMode.Clamp);
        _fillStrokePaint.Shader = shader;
        _fillStrokePaint.Style = SKPaintStyle.Fill;
        canvas.DrawPath(path, _fillStrokePaint);
        _fillStrokePaint.Shader = null;
    }

    private static void ApplyDashStyle(SKPaint paint, LineStyle ls)
    {
        if (ls == LineStyle.Solid || ls == default)
        {
            paint.PathEffect = null;
            return;
        }
        float phase = 0;
        float[] intervals = ls switch
        {
            LineStyle.Dashed => new[] { 12f, 6f },
            LineStyle.Dotted => new[] { 2f, 6f },
            LineStyle.DashDot => new[] { 12f, 4f, 2f, 4f },
            _ => Array.Empty<float>()
        };
        paint.PathEffect = SKPathEffect.CreateDash(intervals, phase);
    }

    public bool HitTest(global::Avalonia.Point p) => Bounds.Contains(p);
    public bool Equals(ICustomDrawOperation? other) => false;
    public void Dispose() { }
}
