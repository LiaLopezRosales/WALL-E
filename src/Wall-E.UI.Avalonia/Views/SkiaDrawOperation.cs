using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;
using Wall_E.Domain;

namespace Wall_E.UI.Avalonia.Views;

/// <summary>
/// Custom draw operation that renders pre-computed dot arrays and shape
/// lists via raw SKCanvas. Dot batching and shape classification happen
/// once in DrawingCanvas.PrecomputeDrawData(); this operation just draws
/// the pre-computed data with the current viewport transform + culling.
/// </summary>
internal sealed class SkiaDrawOperation : ICustomDrawOperation
{
    private readonly Dictionary<string, SKPoint[]>? _dotArrays;
    private readonly IReadOnlyList<DrawingCanvas.Shape>? _others;
    private readonly double _scale, _centerX, _centerY;
    private readonly double _dotRadius, _strokeWidth;
    private readonly IBrush _paper;

    private readonly SKPaint _dotPaint = new() { IsAntialias = true, Style = SKPaintStyle.Fill };
    private readonly SKPaint _linePaint = new() { IsAntialias = true, Style = SKPaintStyle.Stroke };
    private readonly SKPaint _fillStrokePaint = new() { IsAntialias = true, Style = SKPaintStyle.Stroke };
    private readonly SKPaint _whiteHaloPaint = new()
    {
        IsAntialias = true, Style = SKPaintStyle.Stroke,
        Color = new SKColor(200, 200, 200), StrokeWidth = 1
    };
    private readonly SKPaint _shadowPaint = new()
    {
        IsAntialias = true, Style = SKPaintStyle.Fill,
        Color = new SKColor(0, 0, 0, 25), MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 3f)
    };

    private readonly Dictionary<string, SKColor> _colorCache = new();

    public SkiaDrawOperation(
        Rect bounds,
        Dictionary<string, SKPoint[]>? dotArrays,
        IReadOnlyList<DrawingCanvas.Shape>? others,
        double scale, double centerX, double centerY,
        double dotRadius, double strokeWidth,
        IBrush paper,
        HashSet<string>? hiddenLabels)
    {
        Bounds = bounds;
        _dotArrays = dotArrays;
        _others = others;
        _scale = scale;
        _centerX = centerX;
        _centerY = centerY;
        _dotRadius = dotRadius;
        _strokeWidth = strokeWidth;
        _paper = paper;
    }

    public Rect Bounds { get; }

    private SKColor ResolveColor(string name)
    {
        if (!_colorCache.TryGetValue(name, out var c))
        {
            c = SKColor.Parse(ColorTable.Resolve(name));
            _colorCache[name] = c;
        }
        return c;
    }

    public void Render(ImmediateDrawingContext context)
    {
        var feature = context.TryGetFeature(typeof(ISkiaSharpApiLeaseFeature)) as ISkiaSharpApiLeaseFeature;
        if (feature is null) return;

        using var lease = feature.Lease();
        var canvas = lease.SkCanvas;

        canvas.Save();
        canvas.Translate((float)(Bounds.Width / 2 - _centerX * _scale),
                         (float)(Bounds.Height / 2 + _centerY * _scale));
        canvas.Scale((float)_scale, (float)-_scale);

        float dotR = (float)(_dotRadius / _scale);
        float lineW = (float)(_strokeWidth / _scale);

        // Draw pre-computed dot arrays — zero iteration, just DrawPoints calls.
        if (_dotArrays is not null)
        {
            foreach (var kvp in _dotArrays)
            {
                if (kvp.Value.Length == 0) continue;
                _dotPaint.Color = ResolveColor(kvp.Key);
                _dotPaint.StrokeWidth = dotR * 2;
                _dotPaint.StrokeCap = SKStrokeCap.Round;
                canvas.DrawPoints(SKPointMode.Points, kvp.Value, _dotPaint);
            }
        }

        // Draw others with viewport culling.
        if (_others is not null && _others.Count > 0)
        {
            double hw = Bounds.Width / (2 * _scale);
            double hh = Bounds.Height / (2 * _scale);
            float margin = (float)(10 / _scale);
            float visL = (float)(_centerX - hw) - margin;
            float visR = (float)(_centerX + hw) + margin;
            float visB = (float)(_centerY - hh) - margin;
            float visT = (float)(_centerY + hh) + margin;

            for (int i = 0; i < _others.Count; i++)
            {
                var shape = _others[i];
                if (!IsVisible(shape, visL, visR, visB, visT)) continue;
                DrawShape(canvas, shape, lineW);
            }
        }

        canvas.Restore();
    }

    private static bool IsVisible(DrawingCanvas.Shape shape, float visL, float visR, float visB, float visT)
    {
        switch (shape)
        {
            case DrawingCanvas.SegShape s:
                return !(s.X1 < visL && s.X2 < visL) && !(s.X1 > visR && s.X2 > visR) &&
                       !(s.Y1 < visB && s.Y2 < visB) && !(s.Y1 > visT && s.Y2 > visT);
            case DrawingCanvas.CircleShape c:
                return !(c.X + c.R < visL) && !(c.X - c.R > visR) &&
                       !(c.Y + c.R < visB) && !(c.Y - c.R > visT);
            default:
                return true;
        }
    }

    private void DrawShape(SKCanvas canvas, DrawingCanvas.Shape shape, float lineW)
    {
        var skColor = ResolveColor(shape.Color);
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
                if (c.FillType != FillType.None)
                {
                    canvas.Save();
                    canvas.Translate(2f, 2f);
                    canvas.DrawPath(path, _shadowPaint);
                    canvas.Restore();
                }
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
                if (p.FillType != FillType.None)
                {
                    canvas.Save();
                    canvas.Translate(2f, 2f);
                    canvas.DrawPath(path, _shadowPaint);
                    canvas.Restore();
                }
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
                _linePaint.Color = isWhite ? SKColors.White : skColor;
                _linePaint.StrokeWidth = lineW;
                _linePaint.Style = SKPaintStyle.Stroke;
                ApplyDashStyle(_linePaint, shape.LineStyle);
                canvas.DrawPath(path, _linePaint);
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
                new[] { ResolveColor(shape.GradientColor1), ResolveColor(shape.GradientColor2) },
                new float[] { 0, 1 },
                SKShaderTileMode.Clamp)
            : SKShader.CreateRadialGradient(
                new SKPoint(bounds.MidX, bounds.MidY),
                Math.Max(bounds.Width, bounds.Height) / 2f,
                new[] { ResolveColor(shape.GradientColor1), ResolveColor(shape.GradientColor2) },
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
        float[] intervals = ls switch
        {
            LineStyle.Dashed => new[] { 12f, 6f },
            LineStyle.Dotted => new[] { 2f, 6f },
            LineStyle.DashDot => new[] { 12f, 4f, 2f, 4f },
            _ => Array.Empty<float>()
        };
        paint.PathEffect = SKPathEffect.CreateDash(intervals, 0);
    }

    public bool HitTest(global::Avalonia.Point p) => Bounds.Contains(p);
    public bool Equals(ICustomDrawOperation? other) => ReferenceEquals(this, other);
    public void Dispose()
    {
        _dotPaint.Dispose();
        _linePaint.Dispose();
        _fillStrokePaint.Dispose();
        _whiteHaloPaint.Dispose();
        _shadowPaint.Dispose();
    }
}
