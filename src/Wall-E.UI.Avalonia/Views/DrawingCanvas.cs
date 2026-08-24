using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;
using Wall_E.Domain;
using APoint = global::Avalonia.Point;
using DPoint = Wall_E.Domain.Point;

namespace Wall_E.UI.Avalonia.Views;

/// <summary>
/// M1 renderer: maps RenderScene drawables to Avalonia vector primitives.
/// Uses Avalonia's Skia-backed rendering pipeline (no external SKCanvasView).
/// Fit-to-view transform with cartesian Y inversion; recomputed every frame.
/// </summary>
public class DrawingCanvas : Control
{
    private const double Margin = 30;
    private const double RayDrawLength = 300; // rays are infinite; draw a bounded stub
    private const int MaxSequenceDots = 2000; // UI responsiveness cap (< MaxElements invariant)

    private RenderScene? _scene;

    public void SetScene(RenderScene? scene)
    {
        _scene = scene;
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        var bg = new SolidColorBrush(Color.FromRgb(0x1B, 0x1E, 0x23));
        context.FillRectangle(bg, new global::Avalonia.Rect(0, 0, Bounds.Width, Bounds.Height));

        if (_scene is null || _scene.ToDraw.Count == 0) return;

        var shapes = new List<Shape>();
        foreach (var drawable in _scene.ToDraw)
            Collect(shapes, drawable.Figures, drawable.UsedColor);

        if (shapes.Count == 0) return;

        var (minX, minY, maxX, maxY) = ComputeBounds(shapes);
        double worldW = maxX - minX, worldH = maxY - minY;
        double availW = Bounds.Width - 2 * Margin, availH = Bounds.Height - 2 * Margin;
        double scale = worldW > 0 && worldH > 0
            ? System.Math.Min(availW / worldW, availH / worldH)
            : 1.0;

        APoint Map(double x, double y) => new(
            Margin + (x - minX) * scale,
            Margin + (maxY - y) * scale); // Y inverted (cartesian look)

        foreach (var shape in shapes)
        {
            var pen = new Pen(ParseColor(shape.Color), 2);
            switch (shape)
            {
                case DotShape d:
                {
                    var c = Map(d.X, d.Y);
                    var brush = ParseColor(d.Color);
                    context.DrawEllipse(brush, null, new global::Avalonia.Rect(c.X - 4, c.Y - 4, 8, 8));
                    break;
                }
                case SegShape s:
                    context.DrawLine(pen, Map(s.X1, s.Y1), Map(s.X2, s.Y2));
                    break;
                case CircleShape c:
                {
                    // circles stay circular: uniform scale was chosen above
                    var topLeft = Map(c.X - c.R, c.Y + c.R);
                    context.DrawEllipse(null, pen,
                        new global::Avalonia.Rect(topLeft.X, topLeft.Y, 2 * c.R * scale, 2 * c.R * scale));
                    break;
                }
                case PolyShape p:
                {
                    if (p.Points.Count < 2) break;
                    var geo = new StreamGeometry();
                    using (var gctx = geo.Open())
                    {
                        gctx.BeginFigure(Map(p.Points[0].x, p.Points[0].y), false);
                        for (int i = 1; i < p.Points.Count; i++)
                            gctx.LineTo(Map(p.Points[i].x, p.Points[i].y));
                        gctx.EndFigure(false);
                    }
                    context.DrawGeometry(null, pen, geo);
                    break;
                }
            }
        }
    }

    private abstract class Shape
    {
        protected Shape(string color) => Color = color;
        public string Color { get; }
    }

    private sealed class DotShape : Shape
    {
        public DotShape(double x, double y, string color) : base(color) { X = x; Y = y; }
        public double X { get; } public double Y { get; }
    }

    private sealed class SegShape : Shape
    {
        public SegShape(double x1, double y1, double x2, double y2, string color) : base(color)
        { X1 = x1; Y1 = y1; X2 = x2; Y2 = y2; }
        public double X1 { get; } public double Y1 { get; } public double X2 { get; } public double Y2 { get; }
    }

    private sealed class CircleShape : Shape
    {
        public CircleShape(double x, double y, double r, string color) : base(color)
        { X = x; Y = y; R = r; }
        public double X { get; } public double Y { get; } public double R { get; }
    }

    private sealed class PolyShape : Shape
    {
        public PolyShape(List<DPoint> points, string color) : base(color) => Points = points;
        public List<DPoint> Points { get; }
    }

    private static void Collect(List<Shape> shapes, object? value, string color)
    {
        switch (value)
        {
            case DPoint p:
                shapes.Add(new DotShape(p.x, p.y, color));
                break;
            case Circle c:
                shapes.Add(new CircleShape(c.center.x, c.center.y, c.radio, color));
                break;
            case Segment s:
                shapes.Add(new SegShape(s.StartIn.x, s.StartIn.y, s.EndsIn.x, s.EndsIn.y, color));
                break;
            case Line l:
                shapes.Add(new SegShape(l.generalpoint1.x, l.generalpoint1.y,
                    l.generalpoint2.x, l.generalpoint2.y, color));
                break;
            case Ray r:
            {
                double dx = r.PassFor.x - r.StartIn.x, dy = r.PassFor.y - r.StartIn.y;
                double len = System.Math.Sqrt(dx * dx + dy * dy);
                if (len < 1e-9) len = 1;
                shapes.Add(new SegShape(r.StartIn.x, r.StartIn.y,
                    r.StartIn.x + dx / len * RayDrawLength,
                    r.StartIn.y + dy / len * RayDrawLength, color));
                break;
            }
            case Arc a:
                shapes.Add(SampleArc(a, color));
                break;
            case Finite_Sequence<object> fso:
                int takenFso = 0;
                foreach (var item in fso.Sequence!)
                {
                    if (takenFso++ >= MaxSequenceDots) break;
                    Collect(shapes, item, color);
                }
                break;
            case InfinitePointSequence ips:
            {
                int takenIps = 0;
                foreach (var pt in ips.Sequence!)
                {
                    if (takenIps++ >= MaxSequenceDots) break;
                    shapes.Add(new DotShape(pt.x, pt.y, color));
                }
                break;
            }
            case GenericSequence<Point> gsp:
            {
                int takenGsp = 0;
                foreach (var pt in gsp.Sequence!)
                {
                    if (takenGsp++ >= MaxSequenceDots) break;
                    shapes.Add(new DotShape(pt.x, pt.y, color));
                }
                break;
            }
        }
    }

    private static PolyShape SampleArc(Arc a, string color)
    {
        const int Steps = 64;
        var points = new List<DPoint>(Steps + 1);
        for (int i = 0; i <= Steps; i++)
        {
            double t = a.MainAngle + a.SweepAngle * i / Steps;
            points.Add(new DPoint(a.center.x + a.measure * System.Math.Cos(t),
                                  a.center.y + a.measure * System.Math.Sin(t)));
        }
        return new PolyShape(points, color);
    }

    private static (double MinX, double MinY, double MaxX, double MaxY) ComputeBounds(List<Shape> shapes)
    {
        double minX = double.MaxValue, minY = double.MaxValue, maxX = double.MinValue, maxY = double.MinValue;
        void Grow(double x, double y)
        {
            if (x < minX) minX = x;
            if (y < minY) minY = y;
            if (x > maxX) maxX = x;
            if (y > maxY) maxY = y;
        }
        foreach (var s in shapes)
        {
            switch (s)
            {
                case DotShape d: Grow(d.X, d.Y); break;
                case SegShape g: Grow(g.X1, g.Y1); Grow(g.X2, g.Y2); break;
                case CircleShape c: Grow(c.X - c.R, c.Y - c.R); Grow(c.X + c.R, c.Y + c.R); break;
                case PolyShape p:
                    foreach (var pt in p.Points) Grow(pt.x, pt.y);
                    break;
            }
        }
        return (minX, minY, maxX, maxY);
    }

    private static IBrush ParseColor(string name) => name.ToLowerInvariant() switch
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
        _ => Brushes.White,
    };
}
