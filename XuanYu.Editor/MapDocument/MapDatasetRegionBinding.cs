using XuanYu.World.Map;

namespace XuanYu.Editor.MapDocument;

public static class MapDatasetRegionBinding
{
    public static MapDocumentResult<MapDefinition> Build(
        MapDefinition current, MapManifest manifest, IEnumerable<MapDatasetDocument> documents) =>
        MapDatasetFeatureBinding.Build(current, manifest, documents.Where(item => item.Type == MapDatasetTypes.Region));
}
