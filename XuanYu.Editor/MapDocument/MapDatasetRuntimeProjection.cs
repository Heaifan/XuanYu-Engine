using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapDocument;

public static class MapDatasetRuntimeProjection
{
    public static MapDocumentResult<MapDefinition> Apply(MapDefinition current, MapManifest manifest)
    {
        var ids = manifest.Datasets.Where(item => item.Type == MapDatasetTypes.Region)
            .Select(item => MapDatasetLayerIdProjection.Project(item.Id)).ToHashSet();
        if (ids.Count == 0) return MapDocumentResult<MapDefinition>.Ok(current);
        var legacy = current.Layers.Where(item => item.Kind == MapLayerKind.Region && !ids.Contains(item.LayerId)).ToArray();
        if (legacy.Any(item => current.Regions.Any(region => region.LayerId == item.LayerId)))
            return MapDocumentResult<MapDefinition>.Fail("LegacyRegionContentPresent", "旧 Region Layer 含内容，拒绝静默替换。", "Project");
        var retained = current.Layers.Where(item => item.Kind != MapLayerKind.Region);
        var states = manifest.DatasetLayerStates.ToDictionary(item => item.DatasetId, StringComparer.OrdinalIgnoreCase);
        var datasets = manifest.Datasets.Where(item => item.Type == MapDatasetTypes.Region)
            .OrderBy(item => states[item.Id].Order).ToArray();
        var layers = retained.Concat(datasets.Select((item, index) => new MapLayer(
            MapDatasetLayerIdProjection.Project(item.Id), item.Name ?? item.Id, 2 + datasets.Length - 1 - index,
            MapLayerKind.Region, states[item.Id].IsVisible, states[item.Id].IsLocked))).ToImmutableArray();
        var candidate = current with { Layers = layers };
        var valid = MapDefinitionValidator.Validate(candidate);
        return valid.Succeeded ? MapDocumentResult<MapDefinition>.Ok(candidate) :
            MapDocumentResult<MapDefinition>.Fail(valid.ErrorCode, valid.Message, "Project", valid.Detail);
    }

    public static MapDefinition Remove(MapDefinition current, string datasetId)
    {
        var layerId = MapDatasetLayerIdProjection.Project(datasetId);
        var candidate = current with
        {
            Layers = current.Layers.Where(item => item.LayerId != layerId).ToImmutableArray(),
            Regions = current.Regions.Where(item => item.LayerId != layerId).ToImmutableArray()
        };
        return candidate.Layers.Any(item => item.Kind == MapLayerKind.Region)
            ? candidate : candidate with { Layers = candidate.Layers.Add(
                MapLayerStack.CreateRegionLayer(candidate.Layers, "区域 1")) };
    }
}
