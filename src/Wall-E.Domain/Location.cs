namespace Wall_E.Domain;

/// <summary>Identifies a source position (file, line, column) for diagnostics.</summary>
public class Location
{
    /// <summary>The name of the source file.</summary>
    public string File { get; set; }
    /// <summary>The line number (as text).</summary>
    public string Line { get; set; }
    /// <summary>The column number (as text).</summary>
    public string Column { get; set; }

    /// <summary>Creates a source location from the given file, line, and column.</summary>
    public Location(string file, string line, string column)
    {
        File = file;
        Line = line;
        Column = column;
    }

    /// <summary>Formats the location as a human-readable string.</summary>
    public override string ToString() => string.Format("at {0}, {1}, {2}", File, Line, Column);
}