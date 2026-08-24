using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Wall_E.Domain;
using APoint = global::Avalonia.Point;
using DPoint = Wall_E.Domain.Point;

namespace Wall_E.UI.Avalonia.Views;

/// <summary>
/// M2/M3 renderer: fixed cartesian viewport (origin-centered world window),
/// adaptive grid with highlighted axes, wheel zoom around the pointer,
/// drag-to-pan, double-tap reset and cursor world-coordinate events.
/// Scenes are consumed INCREMENTALLY: only draw objects not yet seen are
/// flattened into internal shapes, so streaming hundreds of thousands of
/// draws stays cheap between repaints. Renders through Avalonia's Skia
/// pipeline (Control.Render).
/// </summary>
public class DrawingCanvas : Control
{
    private const double ViewMargin = 30;
    private const double RayDrawLength = 300; // rays are infinite; draw a bounded stub
    private const int MaxSequenceDots = 2000; // UI responsiveness cap (< MaxElements invariant)

    private const double MinScale = 0.05;
    private const double MaxScale = 80;
    private const double DefaultScale = 2.0;

    private RenderScene? _sourceScene;
    private int _builtCount;
    private List<Shape> _shapes = new();

    private bool _hasBounds;
    private double _minX, _minY, _maxX, _maxY;

    private double _scale = DefaultScale;
    private double _centerX;
    private double _centerY;

    private bool _panning;
    private APoint _panLast;

    public DrawingCanvas()
    {
        DoubleTapped += (_, _) => ResetView();
    }

    /// <summary>World coordinates under the pointer, raised on move.</summary>
    public event Action<double, double>? CursorWorldPositionChanged;

    /// <summary>Raised when the pointer leaves the drawable area.</summary>
    public event Action? CursorLeftCanvas;

    public void SetScene(RenderScene? scene)
    {
        if (scene is null || !ReferenceEquals(_sourceScene, scene))
        {
            _sourceScene = scene;
            _builtCount = 0;
            _shapes = new List<Shape>();
            _hasBounds = false;
        }
        AppendNewDraws();
        InvalidateVisual();
    }

    /// <summary>Flattens only the draw objects not yet consumed.</summary>
    private void AppendNewDraws()
    {
        if (_sourceScene is null) return;
        var fresh = _sourceScene.SnapshotRange(_builtCount);
        if (fresh.Count == 0) return;
        foreach (var drawable in fresh)
        {
            var shapeListStart = _shapes.Count;
            Collect(_shapes, drawable.Figures, drawable.UsedColor);
            for (int i = shapeListStart; i < _shapes.Count; i++)
                GrowBounds(_shapes[i]);
        }
        _builtCount += fresh.Count;
    }

    public void ResetView()
    {
        _scale = DefaultScale;
        _centerX = 0;
        _centerY = 0;
        InvalidateVisual();
    }

    public void FitToContent()
    {
        ComputeFit();
        InvalidateVisual();
    }

    /// <summary>Computes the fit transform without invalidating - safe to
    /// call from inside Render, where InvalidateVisual is forbidden.</summary>
    private void ComputeFit()
    {
        if (!_hasBounds || !IsSizeValid()) return;

        double w = _maxX - _minX, h = _maxY - _minY;
        if (w <= 1e-9 && h <= 1e-9)
        {
            // single point: center on it instead of jumping to the origin
            _scale = DefaultScale;
            _centerX = _minX;
            _centerY = _minY;
            return;
        }

        double availW = Bounds.Width - 2 * ViewMargin;
        double availH = Bounds.Height - 2 * ViewMargin;
        _scale = Math.Clamp(
            Math.Min(availW / Math.Max(w, 1e-9), availH / Math.Max(h, 1e-9)),
            MinScale, MaxScale);
        _centerX = (_minX + _maxX) / 2;
        _centerY = (_minY + _maxY) / 2;
    }

    public override void Render(DrawingContext context)
    {
        context.FillRectangle(Brushes.White, new global::Avalonia.Rect(0, 0, Bounds.Width, Bounds.Height));
        if (!IsSizeValid()) return;

        // Smart camera: move the viewport only when content falls outside
        // it, so manual zoom survives iterative editing.
        if (_shapes.Count > 0 && !ContentFullyVisible())
            ComputeFit();

        DrawGrid(context);
        if (_shapes.Count == 0) return;

        // 'white' would vanish on the paper background: draw it over a
        // gray halo so every palette color keeps minimum contrast.
        foreach (var shape in _shapes)
        {
            bool isWhite = string.Equals(shape.Color.Trim(), "white", StringComparison.OrdinalIgnoreCase);
            var halo = new Pen(Brushes.Gray, 3);
            var pen = new Pen(isWhite ? Brushes.White : ParseColor(shape.Color), isWhite ? 1.5 : 2);
            void Stroke(Action<Pen> draw)
            {
                if (isWhite) draw(halo);
                draw(pen);
            }

            switch (shape)
            {
                case DotShape d:
                {
                    var c = Map(d.X, d.Y);
                    context.DrawEllipse(ParseColor(d.Color),
                        isWhite ? new Pen(Brushes.Gray, 1) : null,
                        new global::Avalonia.Rect(c.X - 4, c.Y - 4, 8, 8));
                    break;
                }
                case SegShape s:
                    Stroke(p => context.DrawLine(p, Map(s.X1, s.Y1), Map(s.X2, s.Y2)));
                    break;
                case CircleShape c:
                {
                    var topLeft = Map(c.X - c.R, c.Y + c.R);
                    Stroke(p => context.DrawEllipse(null, p,
                        new global::Avalonia.Rect(topLeft.X, topLeft.Y,
                            2 * c.R * _scale, 2 * c.R * _scale)));
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
                    Stroke(pp => context.DrawGeometry(null, pp, geo));
                    break;
                }
            }
        }
    }

    // ---- cartesian grid -------------------------------------------------

    private void DrawGrid(DrawingContext context)
    {
        var visible = VisibleWorldRect();
        double step = NiceStep(36 / _scale);
        double majorEvery = 5;

        double startX = Math.Floor(visible.MinX / step) * step;
        double endX = visible.MaxX;
        double startY = Math.Floor(visible.MinY / step) * step;
        double endY = visible.MaxY;

        var minor = new Pen(ColorFromHex("#E8ECF2"), 1);
        var major = new Pen(ColorFromHex("#CBD4DF"), 1);
        var axis = new Pen(ColorFromHex("#8A93A6"), 1.5);

        for (double x = startX; x <= endX; x += step)
        {
            bool isMajor = IsNearMultipleOf(x, step * majorEvery);
            if (!isMajor)
                context.DrawLine(minor, Map(x, visible.MinY), Map(x, visible.MaxY));
        }
        for (double y = startY; y <= endY; y += step)
        {
            bool isMajor = IsNearMultipleOf(y, step * majorEvery);
            if (!isMajor)
                context.DrawLine(minor, Map(visible.MinX, y), Map(visible.MaxX, y));
        }
        for (double x = startX; x <= endX; x += step)
        {
            if (IsNearMultipleOf(x, step * majorEvery))
                context.DrawLine(major, Map(x, visible.MinY), Map(x, visible.MaxY));
        }
        for (double y = startY; y <= endY; y += step)
        {
            if (IsNearMultipleOf(y, step * majorEvery))
                context.DrawLine(major, Map(visible.MinX, y), Map(visible.MaxX, y));
        }

        // axes on top
        if (visible.MinY <= 0 && visible.MaxY >= 0)
            context.DrawLine(axis, Map(visible.MinX, 0), Map(visible.MaxX, 0));
        if (visible.MinX <= 0 && visible.MaxX >= 0)
            context.DrawLine(axis, Map(0, visible.MinY), Map(0, visible.MaxY));

        // origin marker in accent amber
        var o = Map(0, 0);
        context.DrawEllipse(null, new Pen(ColorFromHex("#F5A623"), 2),
            new global::Avalonia.Rect(o.X - 5, o.Y - 5, 10, 10));
    }

    private static double NiceStep(double minStep)
    {
        double pow = Math.Pow(10, Math.Floor(Math.Log10(Math.Max(minStep, 1e-9))));
        foreach (var m in new[] { 1d, 2d, 5d, 10d })
            if (m * pow >= minStep)
                return m * pow;
        return 10 * pow;
    }

    private static bool IsNearMultipleOf(double v, double m)
    {
        double r = v / m;
        return Math.Abs(r - Math.Round(r)) < 1e-6;
    }

    private static IBrush ColorFromHex(string hex) =>
        new SolidColorBrush(Color.Parse(hex));

    // ---- transforms -----------------------------------------------------

    private (double MinX, double MinY, double MaxX, double MaxY) VisibleWorldRect()
    {
        double hw = Bounds.Width / (2 * _scale);
        double hh = Bounds.Height / (2 * _scale);
        return (_centerX - hw, _centerY - hh, _centerX + hw, _centerY + hh);
    }

    private bool ContentFullyVisible()
    {
        if (!_hasBounds) return true;
        var v = VisibleWorldRect();
        return _minX >= v.MinX && _maxX <= v.MaxX &&
               _minY >= v.MinY && _maxY <= v.MaxY;
    }

    private APoint Map(double x, double y) => new(
        Bounds.Width / 2 + (x - _centerX) * _scale,
        Bounds.Height / 2 - (y - _centerY) * _scale); // Y inverted (cartesian look)

    private (double X, double Y) ScreenToWorld(APoint p) => (
        _centerX + (p.X - Bounds.Width / 2) / _scale,
        _centerY - (p.Y - Bounds.Height / 2) / _scale);

    private void ZoomAt(APoint p, double factor)
    {
        if (!IsSizeValid()) return;
        var (wx, wy) = ScreenToWorld(p);
        _scale = Math.Clamp(_scale * factor, MinScale, MaxScale);
        _centerX = wx - (p.X - Bounds.Width / 2) / _scale;
        _centerY = wy + (p.Y - Bounds.Height / 2) / _scale;
        InvalidateVisual();
    }

    private bool IsSizeValid() =>
        Bounds.Width > 1 && Bounds.Height > 1 &&
        !double.IsNaN(Bounds.Width) && !double.IsNaN(Bounds.Height);

    // ---- input: zoom / pan / cursor readout ------------------------------

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        double factor = Math.Pow(1.15, e.Delta.Y);
        ZoomAt(e.GetPosition(this), factor);
        e.Handled = true;
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        var props = e.GetCurrentPoint(this).Properties;
        if (props.IsLeftButtonPressed)
        {
            _panning = true;
            _panLast = e.GetPosition(this);
            e.Pointer.Capture(this);
            e.Handled = true;
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        var p = e.GetPosition(this);
        if (_panning)
        {
            _centerX -= (p.X - _panLast.X) / _scale;
            _centerY += (p.Y - _panLast.Y) / _scale;
            _panLast = p;
            InvalidateVisual();
        }
        var (wx, wy) = ScreenToWorld(p);
        CursorWorldPositionChanged?.Invoke(wx, wy);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_panning)
        {
            _panning = false;
            e.Pointer.Capture(null);
        }
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        CursorLeftCanvas?.Invoke();
    }

    // ---- shape model ------------------------------------------------------

    private void GrowBounds(Shape s)
    {
        switch (s)
        {
            case DotShape d:
                GrowPoint(d.X, d.Y);
                break;
            case SegShape g:
                GrowPoint(g.X1, g.Y1);
                GrowPoint(g.X2, g.Y2);
                break;
            case CircleShape c:
                GrowPoint(c.X - c.R, c.Y - c.R);
                GrowPoint(c.X + c.R, c.Y + c.R);
                break;
            case PolyShape p:
                foreach (var pt in p.Points)
                    GrowPoint(pt.x, pt.y);
                break;
        }
    }

    private void GrowPoint(double x, double y)
    {
        if (!_hasBounds)
        {
            _hasBounds = true;
            _minX = _maxX = x;
            _minY = _maxY = y;
            return;
        }
        if (x < _minX) _minX = x;
        if (y < _minY) _minY = y;
        if (x > _maxX) _maxX = x;
        if (y > _maxY) _maxY = y;
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
            points.Add(new DPoint(a.center.x + System.Math.Cos(t) * a.measure,
                                  a.center.y + System.Math.Sin(t) * a.measure));
        }
        return new PolyShape(points, color);
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
        _ => Brushes.Gray,
    };
}
