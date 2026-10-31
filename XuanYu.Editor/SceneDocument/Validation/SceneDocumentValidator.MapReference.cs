using XuanYu.World.Map;

namespace XuanYu.Editor.SceneDocument;

// MAP-A-R1-D5-B：mapReference 校验（可空；空=旧场景无引用，正常打开）。
static partial class SceneDocumentValidator
{
    static bool ValidateMapReference(MapReferenceJson? mapRef)
    {
        if (mapRef is null) return true;
        if (!World.Map.MapId.TryParse(mapRef.MapId, out _)) return false;
        if (string.IsNullOrWhiteSpace(mapRef.AssetPath)) return false;
        return Assets.SceneAssetPathPolicy.IsSafeRelativePath(mapRef.AssetPath);
    }
}
