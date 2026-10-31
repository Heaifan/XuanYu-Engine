namespace XuanYu.Editor.MapDocument;

public sealed record MapDatasetEntry(
    MapDatasetDescriptor Descriptor,
    MapDatasetStatus Status,
    string Message = "");

public sealed partial class MapDatasetRegistry
{
    readonly MapManifestStorageService _manifestStorage = new();
    readonly MapDatasetStorageService _datasetStorage = new();
    readonly MapManifestOwner? _manifestOwner;
    MapManifest? _localManifest;

    public MapDatasetRegistry(string mapPath, MapManifest manifest)
    {
        MapPath = Path.GetFullPath(mapPath);
        _localManifest = manifest;
    }

    public MapDatasetRegistry(string mapPath, MapManifestOwner manifestOwner)
    {
        MapPath = Path.GetFullPath(mapPath);
        _manifestOwner = manifestOwner;
        if (manifestOwner.CurrentManifest is null)
            throw new ArgumentException("Manifest Owner 尚未初始化。", nameof(manifestOwner));
    }

    public string MapPath { get; }
    public string MapRoot => Path.GetDirectoryName(MapPath) ?? Directory.GetCurrentDirectory();
    public MapManifest CurrentManifest => _manifestOwner?.CurrentManifest ?? _localManifest!;

    void PublishManifest(MapManifest manifest)
    {
        if (_manifestOwner is null) _localManifest = manifest;
        else if (!_manifestOwner.Modify(manifest))
            throw new InvalidOperationException("Manifest Owner 已失效，拒绝创建第二状态源。");
    }
}
