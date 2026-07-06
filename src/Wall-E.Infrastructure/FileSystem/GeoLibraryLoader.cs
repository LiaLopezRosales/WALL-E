using Wall_E.Application.Interfaces;

namespace Wall_E.Infrastructure.FileSystem;

public class GeoLibraryLoader
{
    public string[] ListGeoFiles(string basePath)
    {
        string geoPath = Path.Combine(basePath, "GeoLibrary");
        if (!Directory.Exists(geoPath))
            return Array.Empty<string>();
        return Directory.GetFiles(geoPath, "*.geo", SearchOption.AllDirectories);
    }

    public string ReadGeoFile(string path)
    {
        return File.ReadAllText(path);
    }
}
