using System.IO;

namespace Wall_E.UI.Avalonia.Views;

/// <summary>Enumerates and reads the bundled example programs shipped in the
/// build output under <c>programs/</c>. Each file is a ready-to-run demo of one
/// DSL feature area, surfaced in the UI via the Ejemplos picker.</summary>
public static class ProgramsCatalog
{
    /// <summary>Display names (file base names) of the bundled examples, sorted.</summary>
    public static IReadOnlyList<string> List()
    {
        var dir = DirectoryPath;
        if (dir is null)
            return Array.Empty<string>();

        string[] files = System.IO.Directory.GetFiles(dir, "*.geo", SearchOption.TopDirectoryOnly);
        Array.Sort(files, StringComparer.OrdinalIgnoreCase);
        return files
            .Select(Path.GetFileNameWithoutExtension)
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Cast<string>()
            .ToList();
    }

    /// <summary>Reads the content of a bundled example by its display name, or
    /// <see langword="null"/> if the file is missing.</summary>
    public static string? Read(string name)
    {
        var dir = DirectoryPath;
        if (dir is null || string.IsNullOrWhiteSpace(name)) return null;

        string path = Path.Combine(dir, name + ".geo");
        return File.Exists(path) ? File.ReadAllText(path) : null;
    }

    private static string? DirectoryPath
        => System.IO.Directory.Exists(Path.Combine(AppContext.BaseDirectory, "programs"))
            ? Path.Combine(AppContext.BaseDirectory, "programs")
            : null;
}
