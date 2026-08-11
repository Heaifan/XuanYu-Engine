using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapDocument;

public static class MapDatasetRegionBinding
{
    public static MapDocumentResult<MapDefinition> Build(
        MapDefinition current, MapManifest manifest, IEnumerable<MapDatasetDocument> documents)
    {
        var regionDocuments = documents.Where(item => item.Type == MapDatasetTypes.Region).ToArray();
        if (regionDocuments.Length == 0) return MapDocumentResult<MapDefinition>.Ok(current);
        var legacy = current.Layers.Where(layer => layer.Kind == MapLayerKind.Region &&
            !regionDocuments.Any(document => MapDatasetLayerIdProjection.Project(document.Id) == layer.LayerId)).ToArray();
        if (legacy.Any(layer => current.Regions.Any(region => region.LayerId == layer.LayerId)))
            return Fail("LegacyRegionContentPresent", "旧 Region Layer 含内容，拒绝静默替换。");
        var states = manifest.DatasetLayerStates.ToDictionary(item => item.DatasetId, StringComparer.OrdinalIgnoreCase);
        var ordered = regionDocuments.OrderBy(document => states[document.Id].Order).ToArray();
        var layers = current.Layers.Where(layer => layer.Kind != MapLayerKind.Region).ToList();
        var regions = ImmutableArray.CreateBuilder<MapRegion>();
        for (var index = 0; index < ordered.Length; index++)
        {
            var document = ordered[index]; var state = states[document.Id]; var layerId = MapDatasetLayerIdProjection.Project(document.Id);
            layers.Add(new(layerId, manifest.Datasets.First(item => item.Id == document.Id).Name ?? document.Id,
                2 + ordered.Length - 1 - index, MapLayerKind.Region, state.IsVisible, state.IsLocked));
            foreach (var raw in document.Features)
            {
                var feature = MapRegionDatasetCodec.Read(raw);
                if (!feature.Succeeded || feature.Value is null) return Fail(feature.ErrorCode, feature.Message);
                regions.Add(new(feature.Value.RegionId, layerId, feature.Value.Name, feature.Value.Kind, feature.Value.Points));
            }
        }
        var candidate = current with { Layers = layers.ToImmutableArray(), Regions = regions.ToImmutable() };
        var valid = MapDefinitionValidator.Validate(candidate);
        return valid.Succeeded ? MapDocumentResult<MapDefinition>.Ok(candidate) : Fail(valid.ErrorCode, valid.Message);
    }

    static MapDocumentResult<MapDefinition> Fail(string code, string message) =>
        MapDocumentResult<MapDefinition>.Fail(code, message, "Bind");
}
