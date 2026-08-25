using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
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

    /// <summary>Frame budget: beyond this many shapes, dots are decimated
    /// (every stride-th one drawn) so streaming and zooming stay responsive.
    /// Lines, circles, arcs and polygons are never decimated.</summary>
    private const int MaxDrawnShapes = 100000;

    private RenderScene? _sourceScene;
    private int _builtCount;
    private int _labelBuiltCount;
    private List<Shape> _shapes = new();

    // Pens are immutable and reused across frames; widths are quantized so
    // the cache stays tiny even though sizes adapt to zoom.
    private readonly Dictionary<string, Pen> _penCache = new();

    // PaintPool: brushes cached by color name to avoid alloc/frame.
    private readonly Dictionary<string, IBrush> _brushCache = new();

    private static readonly Pen GridMinorPen = new(ColorFromHex("#E8ECF2"), 1);
    private static readonly Pen GridMajorPen = new(ColorFromHex("#CBD4DF"), 1);
    private static readonly Pen GridAxisPen = new(ColorFromHex("#8A93A6"), 1.5);
    private static readonly Pen OriginPen = new(ColorFromHex("#F5A623"), 2);

    private bool _hasBounds;
    private double _minX, _minY, _maxX, _maxY;

    private double _scale = DefaultScale;
    private double _centerX;
    private double _centerY;

    private bool _panning;
    private APoint _panLast;

    private IBrush _paper = Brushes.White;

    /// <summary>Canvas background ("paper"). White keeps DSL ink colors
    /// truthful; user-selectable from the canvas header.</summary>
    public IBrush Paper
    {
        get => _paper;
        set { _paper = value; InvalidateVisual(); }
    }

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
            Collect(_shapes, drawable.Figures, drawable.UsedColor, drawable.Tag, drawable.LineStyle, drawable.StrokeWidth, drawable.FillType, drawable.GradientColor1, drawable.GradientColor2, drawable.Layer);
            for (int i = shapeListStart; i < _shapes.Count; i++)
                GrowBounds(_shapes[i]);
        }
        _builtCount += fresh.Count;
        var freshLabels = _sourceScene.Labels;
        for (int li = _labelBuiltCount; li < freshLabels.Count; li++)
        {
            var lbl = freshLabels[li];
            _shapes.Add(new TagShape(lbl.Text, lbl.Position.x, lbl.Position.y, lbl.Color));
            GrowPoint(lbl.Position.x, lbl.Position.y);
        }
        _labelBuiltCount = freshLabels.Count;
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

    public async Task ExportPngAsync(string path, int width = 1920, int height = 1080)
    {
        var rtb = new RenderTargetBitmap(new PixelSize(width, height), new Vector(96, 96));
        using (var ctx = rtb.CreateDrawingContext())
        {
            ctx.FillRectangle(Paper, new global::Avalonia.Rect(0, 0, width, height));

            double origScale = _scale;
            double origCX = _centerX, origCY = _centerY;
            _scale = Math.Min(width / (Bounds.Width > 1 ? Bounds.Width : 1),
                              height / (Bounds.Height > 1 ? Bounds.Height : 1)) * 0.9;
            _centerX = 0; _centerY = 0;
            if (_hasBounds)
            {
                double w = _maxX - _minX, h = _maxY - _minY;
                if (w > 1e-9 || h > 1e-9)
                {
                    _scale = Math.Clamp(
                        Math.Min(width / Math.Max(w, 1e-9), height / Math.Max(h, 1e-9)),
                        MinScale, MaxScale) * 0.9;
                    _centerX = (_minX + _maxX) / 2;
                    _centerY = (_minY + _maxY) / 2;
                }
            }

            double dotR = Math.Clamp(_scale * 2.0, 0.75, 4.0);
            for (int si = 0; si < _shapes.Count; si++)
            {
                var shape = _shapes[si];
                bool isWhite = string.Equals(shape.Color.Trim(), "white", StringComparison.OrdinalIgnoreCase);
                double shapeW = Math.Clamp(shape.StrokeWidth * _scale, 0.6, 6.0);
                Pen halo = GetPen("#halo", shapeW + 1.5);
                Pen pen = GetPen(isWhite ? "white" : shape.Color, isWhite ? shapeW * 0.75 : shapeW, shape.LineStyle);

                switch (shape)
                {
                    case DotShape d:
                        var dc = Map(d.X, d.Y);
                        ctx.DrawEllipse(ParseColor(d.Color),
                            isWhite ? GetPen("#halo", Math.Max(shapeW * 0.5, 1)) : null,
                            new global::Avalonia.Rect(dc.X - dotR, dc.Y - dotR, 2 * dotR, 2 * dotR));
                        break;
                    case SegShape s:
                        if (isWhite) ctx.DrawLine(halo, Map(s.X1, s.Y1), Map(s.X2, s.Y2));
                        ctx.DrawLine(pen, Map(s.X1, s.Y1), Map(s.X2, s.Y2));
                        break;
                    case CircleShape c:
                        var topLeft = Map(c.X - c.R, c.Y + c.R);
                        var circleBounds = new global::Avalonia.Rect(topLeft.X, topLeft.Y, 2 * c.R * _scale, 2 * c.R * _scale);
                        var cFill = GetFillBrush(c, circleBounds);
                        if (cFill != null)
                        {
                            ctx.DrawEllipse(cFill, pen, circleBounds);
                        }
                        else
                        {
                            if (isWhite) ctx.DrawEllipse(null, halo, circleBounds);
                            ctx.DrawEllipse(null, pen, circleBounds);
                        }
                        break;
                    case PolyShape p:
                        if (p.Points.Count < 2) break;
                        var geo = new StreamGeometry();
                        using (var gctx = geo.Open())
                        {
                            gctx.BeginFigure(Map(p.Points[0].x, p.Points[0].y), false);
                            for (int i = 1; i < p.Points.Count; i++)
                                gctx.LineTo(Map(p.Points[i].x, p.Points[i].y));
                            gctx.EndFigure(false);
                        }
                        var pFill = GetFillBrush(p, geo.Bounds);
                        if (pFill != null)
                        {
                            ctx.DrawGeometry(pFill, pen, geo);
                        }
                        else
                        {
                            if (isWhite) ctx.DrawGeometry(null, halo, geo);
                            ctx.DrawGeometry(null, pen, geo);
                        }
                        break;
                }
            }

            _scale = origScale;
            _centerX = origCX;
            _centerY = origCY;
        }

        rtb.Save(path);
        await Task.CompletedTask;
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
        context.FillRectangle(Paper, new global::Avalonia.Rect(0, 0, Bounds.Width, Bounds.Height));
        if (!IsSizeValid()) return;

        if (_shapes.Count > 0 && !ContentFullyVisible())
            ComputeFit();

        DrawGrid(context);
        if (_shapes.Count == 0) return;

        var sorted = _shapes.OrderBy(s => s.Layer).ToList();

        int stride = sorted.Count > MaxDrawnShapes
            ? (int)Math.Ceiling((double)sorted.Count / MaxDrawnShapes)
            : 1;
        double dotR = Math.Clamp(_scale * 2.0, 0.75, 4.0);
        double strokeW = Math.Clamp(_scale, 0.6, 2.0);
        var hidden = _sourceScene?.HiddenLabels;

        // Submit batch GPU draw operation for dots/lines/circles/polygons.
        var op = new SkiaDrawOperation(
            new global::Avalonia.Rect(0, 0, Bounds.Width, Bounds.Height),
            sorted, _scale, _centerX, _centerY, dotR, strokeW, stride, Paper, hidden);
        context.Custom(op);

        // Tags still use Avalonia's FormattedText (needs font management not
        // easily available on raw SKCanvas).
        for (int si = 0; si < sorted.Count; si++)
        {
            var shape = sorted[si];
            if (shape is not TagShape t) continue;
            if (hidden != null && hidden.Contains(t.Tag)) continue;
            var pos = Map(t.X, t.Y);
            var ft = new FormattedText(t.Tag,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"), 12, ParseColor(t.Color));
            context.DrawText(ft, new global::Avalonia.Point(pos.X + 6, pos.Y - 14));
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

        var minor = GridMinorPen;
        var major = GridMajorPen;
        var axis = GridAxisPen;

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
        context.DrawEllipse(null, OriginPen,
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
        // 7.5% hysteresis on each side: while content grows during streaming
        // the camera holds still until it actually overflows the viewport,
        // instead of micro-refitting on every tick.
        double fx = (v.MaxX - v.MinX) * 0.075;
        double fy = (v.MaxY - v.MinY) * 0.075;
        return _minX >= v.MinX - fx && _maxX <= v.MaxX + fx &&
               _minY >= v.MinY - fy && _maxY <= v.MaxY + fy;
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

    private Pen GetPen(string colorName, double width, Wall_E.Domain.LineStyle ls = default)
    {
        double w = Math.Round(width * 2, MidpointRounding.AwayFromZero) / 2;
        string key = colorName + ":" + w.ToString(CultureInfo.InvariantCulture) + ":" + ((int)ls).ToString();
        if (!_penCache.TryGetValue(key, out var pen))
        {
            global::Avalonia.Media.IDashStyle? dash = ls switch
            {
                Wall_E.Domain.LineStyle.Dashed => global::Avalonia.Media.DashStyle.Dash,
                Wall_E.Domain.LineStyle.Dotted => global::Avalonia.Media.DashStyle.Dot,
                Wall_E.Domain.LineStyle.DashDot => global::Avalonia.Media.DashStyle.DashDot,
                _ => null,
            };
            pen = colorName == "#halo"
                ? new Pen(Brushes.Gray, w, dashStyle: dash)
                : new Pen(ParseColor(colorName), w, dashStyle: dash);
            _penCache[key] = pen;
        }
        return pen;
    }

    internal abstract class Shape
    {
        protected Shape(string color, Wall_E.Domain.LineStyle lineStyle = default, double strokeWidth = 1.0,
            Wall_E.Domain.FillType fillType = default, string grad1 = "", string grad2 = "", int layer = 0)
        { Color = color; LineStyle = lineStyle; StrokeWidth = strokeWidth; FillType = fillType; GradientColor1 = grad1; GradientColor2 = grad2; Layer = layer; }
        public string Color { get; }
        public Wall_E.Domain.LineStyle LineStyle { get; }
        public double StrokeWidth { get; }
        public Wall_E.Domain.FillType FillType { get; }
        public bool IsFilled => FillType == Wall_E.Domain.FillType.Solid;
        public string GradientColor1 { get; }
        public string GradientColor2 { get; }
        public int Layer { get; }
    }

    internal sealed class DotShape : Shape
    {
        public DotShape(double x, double y, string color, int layer = 0) : base(color, layer: layer) { X = x; Y = y; }
        public double X { get; } public double Y { get; }
    }

    internal sealed class SegShape : Shape
    {
        public SegShape(double x1, double y1, double x2, double y2, string color,
            Wall_E.Domain.LineStyle ls = default, double sw = 1.0, int layer = 0)
            : base(color, ls, sw, layer: layer)
        { X1 = x1; Y1 = y1; X2 = x2; Y2 = y2; }
        public double X1 { get; } public double Y1 { get; } public double X2 { get; } public double Y2 { get; }
    }

    internal sealed class CircleShape : Shape
    {
        public CircleShape(double x, double y, double r, string color,
            Wall_E.Domain.LineStyle ls = default, double sw = 1.0,
            Wall_E.Domain.FillType ft = default, string g1 = "", string g2 = "", int layer = 0)
            : base(color, ls, sw, ft, g1, g2, layer)
        { X = x; Y = y; R = r; }
        public double X { get; } public double Y { get; } public double R { get; }
    }

    internal sealed class PolyShape : Shape
    {
        public PolyShape(List<DPoint> points, string color,
            Wall_E.Domain.LineStyle ls = default, double sw = 1.0,
            Wall_E.Domain.FillType ft = default, string g1 = "", string g2 = "", int layer = 0)
            : base(color, ls, sw, ft, g1, g2, layer)
            => Points = points;
        public List<DPoint> Points { get; }
    }

    internal sealed class TagShape : Shape
    {
        public TagShape(string tag, double x, double y, string color, int layer = 0) : base(color, layer: layer)
        { Tag = tag; X = x; Y = y; }
        public string Tag { get; }
        public double X { get; } public double Y { get; }
    }

    private static void Collect(List<Shape> shapes, object? value, string color, string tag = "",
        Wall_E.Domain.LineStyle lineStyle = default, double strokeWidth = 1.0,
        Wall_E.Domain.FillType fillType = default, string grad1 = "", string grad2 = "", int layer = 0)
    {
        switch (value)
        {
            case DPoint p:
                if (!string.IsNullOrEmpty(tag))
                    shapes.Add(new TagShape(tag, p.x, p.y, color, layer));
                shapes.Add(new DotShape(p.x, p.y, color, layer));
                break;
            case Circle c:
                if (!string.IsNullOrEmpty(tag))
                    shapes.Add(new TagShape(tag, c.center.x, c.center.y + c.radio + 1, color, layer));
                shapes.Add(new CircleShape(c.center.x, c.center.y, c.radio, color, lineStyle, strokeWidth, fillType, grad1, grad2, layer));
                break;
            case Segment s:
                if (!string.IsNullOrEmpty(tag))
                {
                    double mx = (s.StartIn.x + s.EndsIn.x) / 2;
                    double my = (s.StartIn.y + s.EndsIn.y) / 2;
                    shapes.Add(new TagShape(tag, mx, my, color, layer));
                }
                shapes.Add(new SegShape(s.StartIn.x, s.StartIn.y, s.EndsIn.x, s.EndsIn.y, color, lineStyle, strokeWidth, layer));
                break;
            case Line l:
                if (!string.IsNullOrEmpty(tag))
                {
                    double mx = (l.generalpoint1.x + l.generalpoint2.x) / 2;
                    double my = (l.generalpoint1.y + l.generalpoint2.y) / 2;
                    shapes.Add(new TagShape(tag, mx, my, color, layer));
                }
                shapes.Add(new SegShape(l.generalpoint1.x, l.generalpoint1.y,
                    l.generalpoint2.x, l.generalpoint2.y, color, lineStyle, strokeWidth, layer));
                break;
            case Ray r:
            {
                double dx = r.PassFor.x - r.StartIn.x, dy = r.PassFor.y - r.StartIn.y;
                double len = System.Math.Sqrt(dx * dx + dy * dy);
                if (len < 1e-9) len = 1;
                if (!string.IsNullOrEmpty(tag))
                    shapes.Add(new TagShape(tag, r.StartIn.x, r.StartIn.y, color, layer));
                shapes.Add(new SegShape(r.StartIn.x, r.StartIn.y,
                    r.StartIn.x + dx / len * RayDrawLength,
                    r.StartIn.y + dy / len * RayDrawLength, color, lineStyle, strokeWidth, layer));
                break;
            }
            case Arc a:
                if (!string.IsNullOrEmpty(tag))
                    shapes.Add(new TagShape(tag, a.center.x, a.center.y + a.measure + 1, color, layer));
                shapes.Add(SampleArc(a, color, lineStyle, strokeWidth, layer));
                break;
            case Polygon poly:
            {
                var verts = poly.Vertices();
                var pts = new List<DPoint>(verts.Count + 1);
                pts.AddRange(verts);
                if (pts.Count > 0) pts.Add(pts[0]);
                if (!string.IsNullOrEmpty(tag))
                    shapes.Add(new TagShape(tag, poly.Center.x, poly.Center.y + poly.Radius + 1, color, layer));
                if (pts.Count >= 2)
                    shapes.Add(new PolyShape(pts, color, lineStyle, strokeWidth, fillType, grad1, grad2, layer));
                break;
            }
            case Ellipse ell:
                if (!string.IsNullOrEmpty(tag))
                    shapes.Add(new TagShape(tag, ell.Center.x, ell.Center.y + Math.Max(ell.Rx, ell.Ry) + 1, color, layer));
                shapes.Add(SampleEllipse(ell, color, lineStyle, strokeWidth, fillType, grad1, grad2, layer));
                break;
            case Finite_Sequence<object> fso:
                int takenFso = 0;
                foreach (var item in fso.Sequence!)
                {
                    if (takenFso++ >= MaxSequenceDots) break;
                    Collect(shapes, item, color, takenFso == 1 ? tag : "", lineStyle, strokeWidth, fillType, grad1, grad2, layer);
                }
                break;
            case InfinitePointSequence ips:
            {
                int takenIps = 0;
                foreach (var pt in ips.Sequence!)
                {
                    if (takenIps++ >= MaxSequenceDots) break;
                    if (takenIps == 1 && !string.IsNullOrEmpty(tag))
                        shapes.Add(new TagShape(tag, pt.x, pt.y, color, layer));
                    shapes.Add(new DotShape(pt.x, pt.y, color, layer));
                }
                break;
            }
            case GenericSequence<DPoint> gsp:
            {
                int takenGsp = 0;
                foreach (var pt in gsp.Sequence!)
                {
                    if (takenGsp++ >= MaxSequenceDots) break;
                    if (takenGsp == 1 && !string.IsNullOrEmpty(tag))
                        shapes.Add(new TagShape(tag, pt.x, pt.y, color, layer));
                    shapes.Add(new DotShape(pt.x, pt.y, color, layer));
                }
                break;
            }
        }
    }

    private static PolyShape SampleArc(Arc a, string color,
        Wall_E.Domain.LineStyle ls = default, double sw = 1.0, int layer = 0)
    {
        const int Steps = 64;
        var points = new List<DPoint>(Steps + 1);
        for (int i = 0; i <= Steps; i++)
        {
            double t = a.MainAngle + a.SweepAngle * i / Steps;
            points.Add(new DPoint(a.center.x + System.Math.Cos(t) * a.measure,
                                  a.center.y + System.Math.Sin(t) * a.measure));
        }
        return new PolyShape(points, color, ls, sw, layer: layer);
    }

    private static PolyShape SampleEllipse(Ellipse e, string color,
        Wall_E.Domain.LineStyle ls = default, double sw = 1.0,
        Wall_E.Domain.FillType ft = default, string g1 = "", string g2 = "", int layer = 0)
    {
        const int Steps = 64;
        var points = new List<DPoint>(Steps + 1);
        for (int i = 0; i <= Steps; i++)
        {
            double t = 2 * System.Math.PI * i / Steps;
            points.Add(new DPoint(e.Center.x + e.Rx * System.Math.Cos(t),
                                  e.Center.y + e.Ry * System.Math.Sin(t)));
        }
        return new PolyShape(points, color, ls, sw, ft, g1, g2, layer);
    }

    private IBrush ParseColor(string name)
    {
        if (!_brushCache.TryGetValue(name, out var brush))
        {
            brush = DslPalette.ToBrush(name);
            _brushCache[name] = brush;
        }
        return brush;
    }

    private IBrush? GetFillBrush(Shape shape, global::Avalonia.Rect bounds)
    {
        switch (shape.FillType)
        {
            case Wall_E.Domain.FillType.Solid:
                return ParseColor(shape.Color);
            case Wall_E.Domain.FillType.LinearGradient:
            {
                var c1 = global::Avalonia.Media.Color.Parse(shape.GradientColor1);
                var c2 = global::Avalonia.Media.Color.Parse(shape.GradientColor2);
                return new LinearGradientBrush
                {
                    StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                    EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                    GradientStops = new GradientStops
                    {
                        new GradientStop(c1, 0),
                        new GradientStop(c2, 1)
                    }
                };
            }
            case Wall_E.Domain.FillType.RadialGradient:
            {
                var c1 = global::Avalonia.Media.Color.Parse(shape.GradientColor1);
                var c2 = global::Avalonia.Media.Color.Parse(shape.GradientColor2);
                return new RadialGradientBrush
                {
                    GradientOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative),
                    GradientStops = new GradientStops
                    {
                        new GradientStop(c1, 0),
                        new GradientStop(c2, 1)
                    }
                };
            }
            default:
                return null;
        }
    }
}
