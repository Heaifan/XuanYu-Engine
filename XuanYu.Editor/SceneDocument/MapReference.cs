using XuanYu.Editor.Assets;

namespace XuanYu.Editor.SceneDocument;

// MAP-A-R1-D5-B：场景对地图的可选引用（D1 合同冻结）。
// 只保存 mapId + 项目相对 assetPath，不复制地图尺寸/地表/环境参数。
public sealed record MapReference(
    string MapId,
    string AssetPath)
{
    public bool IsValid =>
        XuanYu.Editor.MapDocument.MapId.TryParse(MapId, out _) &&
        !string.IsNullOrWhiteSpace(AssetPath) &&
        SceneAssetPathPolicy.IsSafeRelativePath(AssetPath);
}
