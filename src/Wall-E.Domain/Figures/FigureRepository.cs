namespace Wall_E.Domain;

public class FigureRepository
{
    public const int MaxExistingPoints = 100000;
    public const int MaxExistingCircles = 50000;
    public const int MaxExistingLines = 50000;
    public const int MaxExistingSegments = 50000;
    public const int MaxExistingRays = 50000;

    public List<Circle> ExistingCircles { get; set; } = new();
    public List<Point> ExistingPoints { get; set; } = new();
    public List<Segment> ExistingSegments { get; set; } = new();
    public List<Line> ExistingLines { get; set; } = new();
    public List<Ray> ExistingRays { get; set; } = new();

    public void Clear()
    {
        ExistingCircles.Clear();
        ExistingLines.Clear();
        ExistingPoints.Clear();
        ExistingRays.Clear();
        ExistingSegments.Clear();
    }

    public bool TryAddExistingPoint(Point p)
    {
        if (ExistingPoints.Count >= MaxExistingPoints) return false;
        ExistingPoints.Add(p);
        return true;
    }
    public bool TryAddExistingCircle(Circle c)
    {
        if (ExistingCircles.Count >= MaxExistingCircles) return false;
        ExistingCircles.Add(c);
        return true;
    }
    public bool TryAddExistingLine(Line l)
    {
        if (ExistingLines.Count >= MaxExistingLines) return false;
        ExistingLines.Add(l);
        return true;
    }
    public bool TryAddExistingSegment(Segment s)
    {
        if (ExistingSegments.Count >= MaxExistingSegments) return false;
        ExistingSegments.Add(s);
        return true;
    }
    public bool TryAddExistingRay(Ray r)
    {
        if (ExistingRays.Count >= MaxExistingRays) return false;
        ExistingRays.Add(r);
        return true;
    }
}
