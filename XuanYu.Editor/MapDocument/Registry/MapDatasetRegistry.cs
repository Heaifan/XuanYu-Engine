namespace XuanYu.Editor.MapDocument;

public sealed record MapDatasetEntry(
    MapDatasetDescriptor Descriptor,
    MapDatasetStatus Status,
    string Message = "");

public sealed partial class MapDatasetRegistry
{
    readonly MapManifestStorageService _manifestStorage = new();
    readonly MapDatasetStorageService _datasetStorage = new();

    public MapDatasetRegistry(string mapPath, MapManifest manifest)
    {
        MapPath = Path.GetFullPath(mapPath);
        CurrentManifest = manifest;
    }

    public string MapPath { get; }
    public string MapRoot => Path.GetDirectoryName(MapPath) ?? Directory.GetCurrentDirectory();
    public MapManifest CurrentManifest { get; private set; }
}
