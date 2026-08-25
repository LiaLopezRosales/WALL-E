using SkiaSharp;
using Wall_E.Domain;
using Wall_E.Application.Pipeline;
using Wall_E.Infrastructure.FileSystem;
using Wall_E.CLI;

string inputPath = args.Length > 0 ? args[0] : "";
string outputPath = args.Length > 1 ? args[1] : "";
int width = 1920, height = 1080;

if (string.IsNullOrEmpty(inputPath))
{
    Console.WriteLine("Usage: Wall-E.CLI <input.geo> [output.png|output.svg] [--width W] [--height H]");
    Console.WriteLine();
    Console.WriteLine("  input.geo       DSL source file to render");
    Console.WriteLine("  output.png      Export as PNG (default if no extension specified)");
    Console.WriteLine("  output.svg      Export as SVG");
    Console.WriteLine("  --width W       Image width (default: 1920)");
    Console.WriteLine("  --height H      Image height (default: 1080)");
    return;
}

for (int i = 2; i < args.Length; i++)
{
    if (args[i] == "--width" && i + 1 < args.Length) width = int.Parse(args[++i]);
    if (args[i] == "--height" && i + 1 < args.Length) height = int.Parse(args[++i]);
}

if (string.IsNullOrEmpty(outputPath))
{
    var ext = Path.GetExtension(inputPath);
    outputPath = ext == ".svg"
        ? Path.ChangeExtension(inputPath, ".svg")
        : Path.ChangeExtension(inputPath, ".png");
}

string source = File.ReadAllText(inputPath);
string basePath = Path.GetDirectoryName(Path.GetFullPath(inputPath)) ?? ".";

var pipeline = new PipelineOrchestrator(new GeoLibraryLoader(basePath));
pipeline.Execute(source, Path.GetFileName(inputPath));

if (pipeline.Errors.Count > 0)
{
    foreach (var e in pipeline.Errors)
        Console.Error.WriteLine($"  {e}");
    Console.Error.WriteLine($"{pipeline.Errors.Count} error(s) found.");
    Environment.Exit(1);
}

var objects = pipeline.Scene.Snapshot();
var labels = pipeline.Scene.Labels;
Console.WriteLine($"Rendered {objects.Count} objects, {labels.Count} labels.");

string ext2 = Path.GetExtension(outputPath).ToLowerInvariant();
if (ext2 == ".svg")
{
    ExportSvg(outputPath, objects, labels, width, height);
    Console.WriteLine($"SVG exported: {outputPath}");
}
else
{
    ExportPng(outputPath, objects, labels, width, height);
    Console.WriteLine($"PNG exported: {outputPath}");
}

void ExportPng(string path, List<Wall_E.Domain.DrawObject> objs, List<Wall_E.Domain.LabelObject> lbls, int w, int h)
{
    var info = new SKImageInfo(w, h);
    using var surface = SKSurface.Create(info);
    var canvas = surface.Canvas;
    HeadlessRenderer.Render(canvas, objs, lbls, w, h);
    using var image = surface.Snapshot();
    using var data = image.Encode(SKEncodedImageFormat.Png, 100);
    using var stream = File.OpenWrite(path);
    data.SaveTo(stream);
}

void ExportSvg(string path, List<Wall_E.Domain.DrawObject> objs, List<Wall_E.Domain.LabelObject> lbls, int w, int h)
{
    var sb = new System.Text.StringBuilder();
    sb.AppendLine($"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"{w}\" height=\"{h}\" viewBox=\"0 0 {w} {h}\">");
    sb.AppendLine($"  <rect width=\"{w}\" height=\"{h}\" fill=\"white\"/>");

    // Compute viewBox from bounds.
    var (minX, minY, maxX, maxY) = ComputeSvgBounds(objs, lbls);
    double svgMinX = minX - 40, svgMinY = minY - 40;
    double svgW = Math.Max(maxX - minX + 80, 1);
    double svgH = Math.Max(maxY - minY + 80, 1);
    sb.Insert(sb.Length - 2, $" viewBox=\"{svgMinX} {-maxY - 40} {svgW} {svgH}\"");

    foreach (var obj in objs.OrderBy(o => o.Layer))
    {
        string color = obj.UsedColor;
        string style = $"stroke:{color};fill:none;stroke-width:2";
        if (obj.LineStyle == LineStyle.Dashed) style += ";stroke-dasharray:12,6";
        else if (obj.LineStyle == LineStyle.Dotted) style += ";stroke-dasharray:2,6";
        else if (obj.LineStyle == LineStyle.DashDot) style += ";stroke-dasharray:12,4,2,4";

        if (obj.Figures is Point p)
            sb.AppendLine($"  <circle cx=\"{p.x}\" cy=\"{-p.y}\" r=\"4\" fill=\"{color}\"/>");
        else if (obj.Figures is Circle c)
        {
            string fill = obj.FillType == FillType.Solid ? $"fill:{color}" : "fill:none";
            sb.AppendLine($"  <circle cx=\"{c.center.x}\" cy=\"{-c.center.y}\" r=\"{c.radio}\" style=\"{style};{fill}\"/>");
        }
        else if (obj.Figures is Line l)
            sb.AppendLine($"  <line x1=\"{l.generalpoint1.x}\" y1=\"{-l.generalpoint1.y}\" x2=\"{l.generalpoint2.x}\" y2=\"{-l.generalpoint2.y}\" style=\"{style}\"/>");
        else if (obj.Figures is Segment s)
            sb.AppendLine($"  <line x1=\"{s.StartIn.x}\" y1=\"{-s.StartIn.y}\" x2=\"{s.EndsIn.x}\" y2=\"{-s.EndsIn.y}\" style=\"{style}\"/>");
        else if (obj.Figures is Ray r)
            sb.AppendLine($"  <line x1=\"{r.StartIn.x}\" y1=\"{-r.StartIn.y}\" x2=\"{r.PassFor.x}\" y2=\"{-r.PassFor.y}\" style=\"{style}\"/>");
        else if (obj.Figures is Polygon poly)
        {
            var verts = poly.Vertices();
            string pts = string.Join(" ", verts.Select(v => $"{v.x},{-v.y}"));
            string fill = obj.FillType == FillType.Solid ? $"fill:{color}" : "fill:none";
            sb.AppendLine($"  <polygon points=\"{pts}\" style=\"{style};{fill}\"/>");
        }
        else if (obj.Figures is Ellipse ell)
        {
            string fill = obj.FillType == FillType.Solid ? $"fill:{color}" : "fill:none";
            sb.AppendLine($"  <ellipse cx=\"{ell.Center.x}\" cy=\"{-ell.Center.y}\" rx=\"{ell.Rx}\" ry=\"{ell.Ry}\" style=\"{style};{fill}\"/>");
        }
    }

    foreach (var lbl in lbls)
        sb.AppendLine($"  <text x=\"{lbl.Position.x}\" y=\"{-lbl.Position.y}\" fill=\"{lbl.Color}\" font-size=\"14\" text-anchor=\"middle\">{lbl.Text}</text>");

    sb.AppendLine("</svg>");
    File.WriteAllText(path, sb.ToString());
}

(double minX, double minY, double maxX, double maxY) ComputeSvgBounds(
    List<Wall_E.Domain.DrawObject> objs, List<Wall_E.Domain.LabelObject> lbls)
{
    double minX = double.MaxValue, minY = double.MaxValue;
    double maxX = double.MinValue, maxY = double.MinValue;
    void Exp(double x, double y) { minX = Math.Min(minX, x); minY = Math.Min(minY, y); maxX = Math.Max(maxX, x); maxY = Math.Max(maxY, y); }
    foreach (var obj in objs)
    {
        switch (obj.Figures)
        {
            case Point p: Exp(p.x, p.y); break;
            case Circle c: Exp(c.center.x - c.radio, c.center.y - c.radio); Exp(c.center.x + c.radio, c.center.y + c.radio); break;
            case Line l: Exp(l.generalpoint1.x, l.generalpoint1.y); Exp(l.generalpoint2.x, l.generalpoint2.y); break;
            case Segment s: Exp(s.StartIn.x, s.StartIn.y); Exp(s.EndsIn.x, s.EndsIn.y); break;
            case Ray r: Exp(r.StartIn.x, r.StartIn.y); Exp(r.PassFor.x, r.PassFor.y); break;
            case Polygon poly: foreach (var v in poly.Vertices()) Exp(v.x, v.y); break;
            case Ellipse ell: Exp(ell.Center.x - ell.Rx, ell.Center.y - ell.Ry); Exp(ell.Center.x + ell.Rx, ell.Center.y + ell.Ry); break;
            case Arc arc: Exp(arc.center.x - arc.measure, arc.center.y - arc.measure); Exp(arc.center.x + arc.measure, arc.center.y + arc.measure); break;
        }
    }
    foreach (var lbl in lbls) Exp(lbl.Position.x, lbl.Position.y);
    if (minX == double.MaxValue) (minX, minY, maxX, maxY) = (0, 0, 100, 100);
    return (minX, minY, maxX, maxY);
}
