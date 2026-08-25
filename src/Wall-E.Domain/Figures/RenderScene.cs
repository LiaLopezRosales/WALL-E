namespace Wall_E.Domain;

/// <summary>A text label placed at a position on the scene.</summary>
public record LabelObject(Point Position, string Text, double FontSize, string Color);

/// <summary>
/// Mutable scene accumulated while statements execute. Draw mutations go
/// through a lock so the UI can poll a consistent Snapshot() while the
/// pipeline runs on a background thread (M3 progressive streaming).
/// </summary>
public class RenderScene
{
    private readonly object _sync = new();

    /// <summary>All drawable objects added during evaluation.</summary>
    public List<DrawObject> ToDraw { get; set; } = new();
    public List<LabelObject> Labels { get; set; } = new();
    /// <summary>Stack of colors used by color/restore statements.</summary>
    public Stack<string> UtilizedColors { get; set; } = new();
    /// <summary>Current stroke style applied to newly drawn figures.</summary>
    public LineStyle CurrentLineStyle { get; set; } = LineStyle.Solid;
    /// <summary>Current stroke width applied to newly drawn figures.</summary>
    public double CurrentStrokeWidth { get; set; } = 1.0;
    /// <summary>Current fill type applied to newly drawn figures.</summary>
    public FillType CurrentFillType { get; set; } = FillType.None;
    public string CurrentGradientColor1 { get; set; } = "";
    public string CurrentGradientColor2 { get; set; } = "";
    public bool CurrentFillEnabled => CurrentFillType != FillType.None;
    /// <summary>Current layer index for newly drawn figures.</summary>
    public int CurrentLayer { get; set; } = 0;
    public double SnapValue { get; set; } = 0;
    public HashSet<string> HiddenLabels { get; set; } = new();

    /// <summary>Synchronized element count - safe to read mid-execution.</summary>
    public int DrawCount
    {
        get { lock (_sync) return ToDraw.Count; }
    }

    public RenderScene()
    {
        UtilizedColors.Push("black");
    }

    /// <summary>Single mutation entry point for the evaluator.</summary>
    public void Add(DrawObject drawable)
    {
        lock (_sync) ToDraw.Add(drawable);
    }

    public void AddLabel(LabelObject label)
    {
        lock (_sync) Labels.Add(label);
    }

    /// <summary>Point-in-time copy of the drawn objects, safe to enumerate
    /// from another thread while evaluation keeps appending.</summary>
    public List<DrawObject> Snapshot()
    {
        lock (_sync) return new List<DrawObject>(ToDraw);
    }

    /// <summary>Copy of everything drawn from <paramref name="start"/> on,
    /// safe to take from another thread - lets a renderer consume draws
    /// incrementally without re-walking the whole list.</summary>
    public List<DrawObject> SnapshotRange(int start)
    {
        lock (_sync)
        {
            if (start >= ToDraw.Count) return new List<DrawObject>();
            return ToDraw.GetRange(start, ToDraw.Count - start);
        }
    }

    /// <summary>Single mutation entry point for 'color' statements.</summary>
    public void PushColor(string color)
    {
        lock (_sync)
        {
            if (UtilizedColors.Peek() != color)
                UtilizedColors.Push(color);
        }
    }

    /// <summary>Single mutation entry point for 'restore' statements.</summary>
    public void RestoreColor()
    {
        lock (_sync)
        {
            if (UtilizedColors.Count > 1)
                UtilizedColors.Pop();
        }
    }

    /// <summary>The active drawing color from the top of the color stack.</summary>
    public string CurrentColor
    {
        get { lock (_sync) return UtilizedColors.Peek(); }
    }

    /// <summary>Top-first copy of the used-color stack, safe mid-execution.</summary>
    public List<string> ColorsSnapshot()
    {
        lock (_sync) return UtilizedColors.ToList();
    }

    /// <summary>Resets the scene to its initial empty state.</summary>
    public void Clear()
    {
        lock (_sync)
        {
            ToDraw.Clear();
            Labels.Clear();
            UtilizedColors.Clear();
            UtilizedColors.Push("black");
        }
    }
}
