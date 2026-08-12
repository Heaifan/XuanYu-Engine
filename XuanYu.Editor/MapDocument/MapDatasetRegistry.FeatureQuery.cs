using System.Collections.Immutable;
using System.Text.Json;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapDocument;

public sealed partial class MapDatasetRegistry
{
    public async Task<MapDocumentResult<IReadOnlyList<MapDatasetDocument>>> LoadFeatureDocumentsAsync()
    {
        var result = new List<MapDatasetDocument>();
        foreach (var descriptor in CurrentManifest.Datasets.Where(item => item.Type is MapDatasetTypes.Region or MapDatasetTypes.Road))
        {
            if (!MapDatasetPathPolicy.TryResolve(MapRoot, descriptor.Source, out var path)) return Fail("InvalidDatasetSource", "Dataset source 不安全。", "Load");
            var loaded = await _datasetStorage.LoadAsync(path, descriptor);
            if (loaded.Status == MapDatasetStatus.Normal && loaded.Document is not null) result.Add(loaded.Document);
        }
        return MapDocumentResult<IReadOnlyList<MapDatasetDocument>>.Ok(result);
    }

    public MapDocumentResult<IReadOnlyList<(string Path, MapDatasetDocument Document)>> BuildFeatureSaveCandidates(MapDefinition map)
    {
        var result = new List<(string, MapDatasetDocument)>();
        foreach (var descriptor in CurrentManifest.Datasets.Where(item => item.Type is MapDatasetTypes.Region or MapDatasetTypes.Road))
        {
            if (!MapDatasetPathPolicy.TryResolve(MapRoot, descriptor.Source, out var path)) return FailCandidates("InvalidDatasetSource", "Dataset source 不安全。");
            var layerId = MapDatasetLayerIdProjection.Project(descriptor.Id);
            var features = descriptor.Type == MapDatasetTypes.Region
                ? map.Regions.Where(item => item.LayerId == layerId).Select(MapRegionDatasetCodec.Write).ToImmutableArray()
                : map.Roads.Where(item => item.LayerId == layerId).Select(MapRoadDatasetCodec.Write).ToImmutableArray();
            var version = descriptor.Type == MapDatasetTypes.Region ? ExistingRegionVersion(path) : MapDatasetDocument.CurrentVersion;
            result.Add((path, new(MapDatasetDocument.CurrentFormat, version, descriptor.Id, descriptor.Type, features)));
        }
        return MapDocumentResult<IReadOnlyList<(string, MapDatasetDocument)>>.Ok(result);
    }

    static MapDocumentResult<IReadOnlyList<MapDatasetDocument>> Fail(string code, string message, string stage) => MapDocumentResult<IReadOnlyList<MapDatasetDocument>>.Fail(code, message, stage);
    static MapDocumentResult<IReadOnlyList<(string Path, MapDatasetDocument Document)>> FailCandidates(string code, string message) => MapDocumentResult<IReadOnlyList<(string, MapDatasetDocument)>>.Fail(code, message, "Save");
    static string ExistingRegionVersion(string path)
    {
        try
        {
            if (!File.Exists(path)) return MapDatasetDocument.CurrentVersion;
            using var json = JsonDocument.Parse(File.ReadAllText(path));
            return json.RootElement.TryGetProperty("version", out var version) && version.GetString() == "0.2.0" ? "0.2.0" : MapDatasetDocument.CurrentVersion;
        }
        catch (JsonException) { return MapDatasetDocument.CurrentVersion; }
    }
}
