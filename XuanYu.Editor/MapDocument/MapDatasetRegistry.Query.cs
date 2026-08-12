using System.Collections.Immutable;

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

    public async Task<MapDocumentResult<IReadOnlyList<MapDatasetDocument>>> LoadRegionDocumentsAsync()
    {
        var loaded = await LoadFeatureDocumentsAsync();
        return loaded.Succeeded && loaded.Value is not null
            ? MapDocumentResult<IReadOnlyList<MapDatasetDocument>>.Ok(loaded.Value.Where(item => item.Type == MapDatasetTypes.Region).ToArray())
            : MapDocumentResult<IReadOnlyList<MapDatasetDocument>>.Fail(loaded.ErrorCode, loaded.Message, loaded.Stage);
    }

    public MapDocumentResult<IReadOnlyList<(string Path, MapDatasetDocument Document)>> BuildRegionSaveCandidates(
        XuanYu.World.Map.MapDefinition map)
    {
        var result = new List<(string Path, MapDatasetDocument Document)>();
        foreach (var descriptor in CurrentManifest.Datasets.Where(item => item.Type == MapDatasetTypes.Region))
        {
            if (!MapDatasetPathPolicy.TryResolve(MapRoot, descriptor.Source, out var path))
                return MapDocumentResult<IReadOnlyList<(string, MapDatasetDocument)>>.Fail("InvalidDatasetSource", "Dataset source 不安全。", "Save");
            var layerId = MapDatasetLayerIdProjection.Project(descriptor.Id);
            var features = map.Regions.Where(region => region.LayerId == layerId)
                .Select(MapRegionDatasetCodec.Write).ToImmutableArray();
            result.Add((path, new(MapDatasetDocument.CurrentFormat, MapDatasetDocument.CurrentVersion,
                descriptor.Id, descriptor.Type, features)));
        }
        return MapDocumentResult<IReadOnlyList<(string, MapDatasetDocument)>>.Ok(result);
    }

    public async Task<MapDatasetEntry?> FindByIdAsync(string id)
    {
        var descriptor = CurrentManifest.Datasets.FirstOrDefault(
            item => string.Equals(item.Id, id, StringComparison.OrdinalIgnoreCase));
        return descriptor is null ? null : await ResolveAsync(descriptor);
    }
}
