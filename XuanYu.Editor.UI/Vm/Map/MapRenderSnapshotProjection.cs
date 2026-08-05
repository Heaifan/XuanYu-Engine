using XuanYu.Render.Abstractions;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

// MAP-A-R2-D3/D4：MapDefinition → MapRenderSnapshot 纯投影（渲染唯一输入）。
// 只取渲染所需字段；名称/区域不进入快照（Rename 等几何无关变化不引发 GPU 资源重建）。
// D4：地面/边界图层可见性进入快照（渲染过滤）；ShowGround/ShowBoundary 不进资源判等键。
// 快照不含会话/历史/Dirty/路径；SourceChangeSequence 来自会话单调递增序号。
public static class MapRenderSnapshotProjection
{
    public static MapRenderSnapshot Project(MapDefinition map, long changeSequence)
    {
        var ground = map.Layers.FirstOrDefault(l => l.Kind == MapLayerKind.Ground);
        var boundary = map.Layers.FirstOrDefault(l => l.Kind == MapLayerKind.Boundary);
        return new MapRenderSnapshot(
            map.MapId.Value,
            map.SizeMeters.Width,
            map.SizeMeters.Depth,
            MapSurfaceKinds.ToKind(map.Surface.Kind),
            map.Surface.BaseHeightMeters,
            map.Surface.AmplitudeMeters,
            map.Surface.WavelengthMeters,
            map.Surface.Seed,
            changeSequence,
            IsVisible: true,
            ShowGround: ground?.IsVisible ?? true,
            ShowBoundary: boundary?.IsVisible ?? true);
    }
}
