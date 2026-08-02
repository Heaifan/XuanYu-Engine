using XuanYu.Core.Map;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapDocument;

// MAP-A-R1-D3/D4：MapDocument（Editor 文档）→ WorldMapState（World 状态）桥接。
// 对齐 SceneDocumentWorldBridge 模式；D4 前不做反向桥接。
// 环境语义：sunDirection = 指向光源方向（与 .xymap 合同一致，Z 分量 > 0 朝上）。
public static class MapDocumentWorldBridge
{
    public static WorldMapState ToWorldState(MapDocument doc)
    {
        var kind = ParseKind(doc.Surface.Kind);
        return new WorldMapState(
            doc.MapId.Value,
            doc.Name,
            doc.SizeMeters.Width,
            doc.SizeMeters.Depth,
            kind,
            doc.Surface.BaseHeightMeters,
            doc.Surface.AmplitudeMeters,
            doc.Surface.WavelengthMeters,
            doc.Surface.Seed,
            doc.Environment.SunDirection.X,
            doc.Environment.SunDirection.Y,
            doc.Environment.SunDirection.Z,
            doc.Environment.SunIntensity,
            doc.Environment.AmbientIntensity);
    }

    static MapSurfaceKind ParseKind(string kind) =>
        kind == MapSurfaceKinds.Flat ? MapSurfaceKind.Flat
        : kind == MapSurfaceKinds.GentleHillsV1 ? MapSurfaceKind.GentleHillsV1
        : throw new ArgumentException($"不支持的地表类型：{kind}", nameof(kind));
}
