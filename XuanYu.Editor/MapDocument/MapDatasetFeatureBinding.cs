using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapDocument;

public static class MapDatasetFeatureBinding
{
    public static MapDocumentResult<MapDefinition> Build(MapDefinition current, MapManifest manifest, IEnumerable<MapDatasetDocument> documents)
    {
        var features = documents.Where(item => item.Type is MapDatasetTypes.Region or MapDatasetTypes.Road).ToArray();
        if (features.Length == 0) return MapDocumentResult<MapDefinition>.Ok(current);
        var ids = features.Select(item => MapDatasetLayerIdProjection.Project(item.Id)).ToHashSet();
        var legacy = current.Layers.Where(layer => layer.Kind == MapLayerKind.Region && !ids.Contains(layer.LayerId)).ToArray();
        if (legacy.Any(layer => current.Regions.Any(item => item.LayerId == layer.LayerId) || current.Roads.Any(item => item.LayerId == layer.LayerId))) return Fail("LegacyFeatureContentPresent", "旧用户图层含内容，拒绝静默替换。");
        var states = manifest.DatasetLayerStates.ToDictionary(item => item.DatasetId, StringComparer.OrdinalIgnoreCase);
        var ordered = features.OrderBy(item => states[item.Id].Order).ToArray();
        var layers = current.Layers.Where(layer => layer.Kind != MapLayerKind.Region).ToList();
        var regions = ImmutableArray.CreateBuilder<MapRegion>(); var roads = ImmutableArray.CreateBuilder<MapRoad>();
        for (var index = 0; index < ordered.Length; index++)
        {
            var document = ordered[index]; var state = states[document.Id]; var layerId = MapDatasetLayerIdProjection.Project(document.Id);
            layers.Add(new(layerId, manifest.Datasets.First(item => item.Id == document.Id).Name ?? document.Id, 2 + ordered.Length - 1 - index, MapLayerKind.Region, state.IsVisible, state.IsLocked));
            foreach (var raw in document.Features)
            {
                if (document.Type == MapDatasetTypes.Region)
                {
                    var read = MapRegionDatasetCodec.Read(raw); if (!read.Succeeded || read.Value is null) return Fail(read.ErrorCode, read.Message);
                    regions.Add(new(read.Value.RegionId, layerId, read.Value.Name, read.Value.Kind, read.Value.Points));
                }
                else
                {
                    var read = MapRoadDatasetCodec.Read(raw); if (!read.Succeeded || read.Value is null) return Fail(read.ErrorCode, read.Message);
                    roads.Add(new(read.Value.RoadId, layerId, read.Value.Name, read.Value.Kind, read.Value.Points));
                }
            }
        }
        var candidate = current with { Layers = layers.ToImmutableArray(), Regions = regions.ToImmutable(), Roads = roads.ToImmutable() };
        var valid = MapDefinitionValidator.Validate(candidate);
        return valid.Succeeded ? MapDocumentResult<MapDefinition>.Ok(candidate) : Fail(valid.ErrorCode, valid.Message);
    }
    static MapDocumentResult<MapDefinition> Fail(string code, string message) => MapDocumentResult<MapDefinition>.Fail(code, message, "Bind");
}
