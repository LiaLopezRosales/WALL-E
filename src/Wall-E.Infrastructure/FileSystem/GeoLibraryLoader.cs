using Wall_E.Application.Interfaces;

namespace Wall_E.Infrastructure.FileSystem;

public class GeoLibraryLoader : IGeoLibrarySource
{
    private readonly string _basePath;

    public GeoLibraryLoader(string basePath)
    {
        _basePath = basePath;
    }

    public string[] ListGeoFiles()
    {
        string geoPath = Path.Combine(_basePath, "GeoLibrary");
        if (!Directory.Exists(geoPath))
            return Array.Empty<string>();
        return Directory.GetFiles(geoPath, "*.geo", SearchOption.AllDirectories);
    }

    public string ReadGeoFile(string path)
    {
        return File.ReadAllText(path);
    }

    // Legacy rule: .geo files live under <basePath>/GeoLibrary/ and the file name
    // must be unique across subdirectories; the import statement references it
    // without path or extension.
    public string? Resolve(string libraryName)
    {
        foreach (string path in ListGeoFiles())
        {
            if (Path.GetFileNameWithoutExtension(path) == libraryName)
                return ReadGeoFile(path);
        }
        return null;
    }
}
