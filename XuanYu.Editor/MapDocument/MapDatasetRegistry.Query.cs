namespace XuanYu.Editor.MapDocument;

public sealed partial class MapDatasetRegistry
{
    public async Task<MapDatasetEntry> ResolveAsync(MapDatasetDescriptor descriptor)
    {
        if (!MapDatasetPathPolicy.TryResolve(MapRoot, descriptor.Source, out var path))
            return new(descriptor, MapDatasetStatus.Invalid, "Dataset source 不安全。");
        var loaded = await _datasetStorage.LoadAsync(path, descriptor);
        return new(descriptor, loaded.Status, loaded.Message);
    }

    public async Task<IReadOnlyList<MapDatasetEntry>> EnumerateAsync()
    {
        var entries = new List<MapDatasetEntry>();
        foreach (var descriptor in CurrentManifest.Datasets)
            entries.Add(await ResolveAsync(descriptor));
        return entries;
    }

    public async Task<MapDatasetEntry?> FindByIdAsync(string id)
    {
        var descriptor = CurrentManifest.Datasets.FirstOrDefault(
            item => string.Equals(item.Id, id, StringComparison.OrdinalIgnoreCase));
        return descriptor is null ? null : await ResolveAsync(descriptor);
    }
}
