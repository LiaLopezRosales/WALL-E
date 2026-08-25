using SkiaSharp;
using Wall_E.Domain;

namespace Wall_E.CLI;

/// <summary>
/// Headless SkiaSharp renderer — renders DrawObjects to SKCanvas without any UI.
/// Used by CLI to produce PNG/SVG output from .geo files.
/// </summary>
internal static class HeadlessRenderer
{
    public static void Render(SKCanvas canvas, List<DrawObject> objects, List<LabelObject> labels,
        int width, int height, string paperColor = "#FFFFFF")
    {
        var bgColor = ParseColor(paperColor);
        canvas.Clear(bgColor);

        // Auto-fit: find bounding box of all figures.
        var (minX, minY, maxX, maxY) = ComputeBounds(objects, labels);
        double worldW = Math.Max(maxX - minX, 1);
        double worldH = Math.Max(maxY - minY, 1);
        double margin = 40;
        double scaleX = (width - margin * 2) / worldW;
        double scaleY = (height - margin * 2) / worldH;
        double scale = Math.Min(scaleX, scaleY);
        double cx = (minX + maxX) / 2;
        double cy = (minY + maxY) / 2;

        canvas.Save();
        canvas.Translate((float)(width / 2 - cx * scale), (float)(height / 2 + cy * scale));
        canvas.Scale((float)scale, (float)-scale);

        float lineW = 2f / (float)scale;
        float dotR = 4f / (float)scale;

        // Sort by layer.
        var sorted = objects.OrderBy(o => o.Layer).ToList();

        // Batch dots per color.
        var dotsByColor = new Dictionary<string, List<SKPoint>>();
        var others = new List<DrawObject>();

        foreach (var obj in sorted)
        {
            if (obj.Figures is Point p)
            {
                if (!dotsByColor.TryGetValue(obj.UsedColor, out var list))
                {
                    list = new List<SKPoint>();
                    dotsByColor[obj.UsedColor] = list;
                }
                list.Add(new SKPoint((float)p.x, (float)p.y));
            }
            else if (obj.Figures is GenericSequence<Point> seq)
            {
                foreach (var pt in seq.Sequence!)
                {
                    if (!dotsByColor.TryGetValue(obj.UsedColor, out var list))
                    {
                        list = new List<SKPoint>();
                        dotsByColor[obj.UsedColor] = list;
                    }
                    list.Add(new SKPoint((float)pt.x, (float)pt.y));
                }
            }
            else
            {
                others.Add(obj);
            }
        }

        // Draw batched dots.
        using var dotPaint = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill };
        foreach (var kvp in dotsByColor)
        {
            var pts = kvp.Value;
            if (pts.Count == 0) continue;
            dotPaint.Color = ParseColor(kvp.Key);
            dotPaint.StrokeWidth = dotR * 2;
            dotPaint.StrokeCap = SKStrokeCap.Round;
            canvas.DrawPoints(SKPointMode.Points, pts.ToArray(), dotPaint);
        }

        // Draw other shapes.
        using var linePaint = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke };
        using var fillPaint = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill };
        using var shadowPaint = new SKPaint
        {
            IsAntialias = true, Style = SKPaintStyle.Fill,
            Color = new SKColor(0, 0, 0, 25),
            MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 3f)
        };

        foreach (var obj in others)
        {
            DrawObject(canvas, obj, linePaint, fillPaint, shadowPaint, lineW);
        }

        // Draw labels.
        using var textPaint = new SKPaint
        {
            IsAntialias = true,
            Color = SKColors.Black,
            TextSize = Math.Max(12f, 14f / (float)scale),
            Typeface = SKTypeface.Default
        };
        foreach (var lbl in labels)
        {
            textPaint.Color = ParseColor(lbl.Color);
            canvas.DrawText(lbl.Text, (float)lbl.Position.x, (float)lbl.Position.y, textPaint);
        }

        canvas.Restore();
    }

    private static void DrawObject(SKCanvas canvas, DrawObject obj,
        SKPaint linePaint, SKPaint fillPaint, SKPaint shadowPaint, float lineW)
    {
        var skColor = ParseColor(obj.UsedColor);

        if (obj.Figures is Circle c)
        {
            using var path = new SKPath();
            path.AddCircle((float)c.center.x, (float)c.center.y, (float)c.radio);

            if (obj.FillType != FillType.None)
            {
                canvas.Save();
                canvas.Translate(2f, 2f);
                canvas.DrawPath(path, shadowPaint);
                canvas.Restore();
            }
            if (obj.FillType == FillType.Solid)
            {
                fillPaint.Color = skColor;
                fillPaint.Style = SKPaintStyle.Fill;
                canvas.DrawPath(path, fillPaint);
            }
            else if (obj.FillType == FillType.LinearGradient || obj.FillType == FillType.RadialGradient)
            {
                DrawGradient(canvas, fillPaint, path, obj,
                    new SKRect((float)(c.center.x - c.radio), (float)(c.center.y - c.radio),
                               (float)(c.center.x + c.radio), (float)(c.center.y + c.radio)));
            }
            linePaint.Color = skColor;
            linePaint.StrokeWidth = lineW;
            linePaint.Style = SKPaintStyle.Stroke;
            ApplyDash(linePaint, obj.LineStyle);
            canvas.DrawPath(path, linePaint);
        }
        else if (obj.Figures is Line l)
        {
            linePaint.Color = skColor;
            linePaint.StrokeWidth = lineW;
            linePaint.Style = SKPaintStyle.Stroke;
            ApplyDash(linePaint, obj.LineStyle);
            canvas.DrawLine((float)l.generalpoint1.x, (float)l.generalpoint1.y,
                            (float)l.generalpoint2.x, (float)l.generalpoint2.y, linePaint);
        }
        else if (obj.Figures is Segment s)
        {
            linePaint.Color = skColor;
            linePaint.StrokeWidth = lineW;
            linePaint.Style = SKPaintStyle.Stroke;
            ApplyDash(linePaint, obj.LineStyle);
            canvas.DrawLine((float)s.StartIn.x, (float)s.StartIn.y,
                            (float)s.EndsIn.x, (float)s.EndsIn.y, linePaint);
        }
        else if (obj.Figures is Ray r)
        {
            linePaint.Color = skColor;
            linePaint.StrokeWidth = lineW;
            linePaint.Style = SKPaintStyle.Stroke;
            ApplyDash(linePaint, obj.LineStyle);
            canvas.DrawLine((float)r.StartIn.x, (float)r.StartIn.y,
                            (float)r.PassFor.x, (float)r.PassFor.y, linePaint);
        }
        else if (obj.Figures is Polygon poly)
        {
            var pts = poly.Vertices();
            if (pts.Count < 2) return;
            using var path = new SKPath();
            path.MoveTo((float)pts[0].x, (float)pts[0].y);
            for (int i = 1; i < pts.Count; i++)
                path.LineTo((float)pts[i].x, (float)pts[i].y);
            path.Close();

            if (obj.FillType != FillType.None)
            {
                canvas.Save();
                canvas.Translate(2f, 2f);
                canvas.DrawPath(path, shadowPaint);
                canvas.Restore();
            }
            if (obj.FillType == FillType.Solid)
            {
                fillPaint.Color = skColor;
                fillPaint.Style = SKPaintStyle.Fill;
                canvas.DrawPath(path, fillPaint);
            }
            else if (obj.FillType == FillType.LinearGradient || obj.FillType == FillType.RadialGradient)
            {
                path.GetBounds(out var b);
                DrawGradient(canvas, fillPaint, path, obj, b);
            }
            linePaint.Color = skColor;
            linePaint.StrokeWidth = lineW;
            linePaint.Style = SKPaintStyle.Stroke;
            ApplyDash(linePaint, obj.LineStyle);
            canvas.DrawPath(path, linePaint);
        }
        else if (obj.Figures is Ellipse ell)
        {
            using var path = new SKPath();
            path.AddOval(new SKRect(
                (float)(ell.Center.x - ell.Rx), (float)(ell.Center.y - ell.Ry),
                (float)(ell.Center.x + ell.Rx), (float)(ell.Center.y + ell.Ry)));

            if (obj.FillType != FillType.None)
            {
                canvas.Save();
                canvas.Translate(2f, 2f);
                canvas.DrawPath(path, shadowPaint);
                canvas.Restore();
            }
            if (obj.FillType == FillType.Solid)
            {
                fillPaint.Color = skColor;
                fillPaint.Style = SKPaintStyle.Fill;
                canvas.DrawPath(path, fillPaint);
            }
            linePaint.Color = skColor;
            linePaint.StrokeWidth = lineW;
            linePaint.Style = SKPaintStyle.Stroke;
            ApplyDash(linePaint, obj.LineStyle);
            canvas.DrawPath(path, linePaint);
        }
        else if (obj.Figures is Arc arc)
        {
            var pts = SampleArc(arc, 60);
            if (pts.Count < 2) return;
            using var path = new SKPath();
            path.MoveTo((float)pts[0].x, (float)pts[0].y);
            for (int i = 1; i < pts.Count; i++)
                path.LineTo((float)pts[i].x, (float)pts[i].y);

            linePaint.Color = skColor;
            linePaint.StrokeWidth = lineW;
            linePaint.Style = SKPaintStyle.Stroke;
            ApplyDash(linePaint, obj.LineStyle);
            canvas.DrawPath(path, linePaint);
        }
    }

    private static SKColor ParseColor(string color) => SKColor.Parse(ColorTable.Resolve(color));

    private static void DrawGradient(SKCanvas canvas, SKPaint paint, SKPath path, DrawObject obj, SKRect bounds)
    {
        using var shader = obj.FillType == FillType.LinearGradient
            ? SKShader.CreateLinearGradient(
                new SKPoint(bounds.Left, bounds.Top),
                new SKPoint(bounds.Right, bounds.Bottom),
                new[] { ParseColor(obj.GradientColor1), ParseColor(obj.GradientColor2) },
                new float[] { 0, 1 }, SKShaderTileMode.Clamp)
            : SKShader.CreateRadialGradient(
                new SKPoint(bounds.MidX, bounds.MidY),
                Math.Max(bounds.Width, bounds.Height) / 2f,
                new[] { ParseColor(obj.GradientColor1), ParseColor(obj.GradientColor2) },
                new float[] { 0, 1 }, SKShaderTileMode.Clamp);
        paint.Shader = shader;
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawPath(path, paint);
        paint.Shader = null;
    }

    private static void ApplyDash(SKPaint paint, LineStyle ls)
    {
        if (ls == LineStyle.Solid || ls == default) { paint.PathEffect = null; return; }
        float[] intervals = ls switch
        {
            LineStyle.Dashed => new[] { 12f, 6f },
            LineStyle.Dotted => new[] { 2f, 6f },
            LineStyle.DashDot => new[] { 12f, 4f, 2f, 4f },
            _ => Array.Empty<float>()
        };
        paint.PathEffect = SKPathEffect.CreateDash(intervals, 0);
    }

    private static List<Point> SampleArc(Arc arc, int samples)
    {
        var pts = new List<Point>(samples + 1);
        double startAngle = arc.MainAngle * Math.PI / 180;
        double sweepAngle = arc.SweepAngle * Math.PI / 180;
        for (int i = 0; i <= samples; i++)
        {
            double t = startAngle + sweepAngle * i / samples;
            pts.Add(new Point(
                arc.center.x + arc.measure * Math.Cos(t),
                arc.center.y + arc.measure * Math.Sin(t)));
        }
        return pts;
    }

    private static (double minX, double minY, double maxX, double maxY) ComputeBounds(
        List<DrawObject> objects, List<LabelObject> labels)
    {
        double minX = double.MaxValue, minY = double.MaxValue;
        double maxX = double.MinValue, maxY = double.MinValue;

        void Expand(double x, double y)
        {
            minX = Math.Min(minX, x); minY = Math.Min(minY, y);
            maxX = Math.Max(maxX, x); maxY = Math.Max(maxY, y);
        }

        foreach (var obj in objects)
        {
            switch (obj.Figures)
            {
                case Point p: Expand(p.x, p.y); break;
                case Circle c: Expand(c.center.x - c.radio, c.center.y - c.radio); Expand(c.center.x + c.radio, c.center.y + c.radio); break;
                case Line l: Expand(l.generalpoint1.x, l.generalpoint1.y); Expand(l.generalpoint2.x, l.generalpoint2.y); break;
                case Segment s: Expand(s.StartIn.x, s.StartIn.y); Expand(s.EndsIn.x, s.EndsIn.y); break;
                case Ray r: Expand(r.StartIn.x, r.StartIn.y); Expand(r.PassFor.x, r.PassFor.y); break;
                case Polygon poly:
                    foreach (var v in poly.Vertices()) Expand(v.x, v.y);
                    break;
                case Ellipse ell:
                    Expand(ell.Center.x - ell.Rx, ell.Center.y - ell.Ry);
                    Expand(ell.Center.x + ell.Rx, ell.Center.y + ell.Ry);
                    break;
                case Arc arc:
                    Expand(arc.center.x - arc.measure, arc.center.y - arc.measure);
                    Expand(arc.center.x + arc.measure, arc.center.y + arc.measure);
                    break;
            }
        }
        foreach (var lbl in labels) Expand(lbl.Position.x, lbl.Position.y);

        if (minX == double.MaxValue) { (minX, minY, maxX, maxY) = (0, 0, 100, 100); }
        return (minX, minY, maxX, maxY);
    }
}
