namespace XuanYu.Render.Abstractions;

// MAP-A-R2-D3-A1 收口：地图 GPU 资源更新决策（纯策略，不依赖 Vulkan，可独立测试）。
// 职责分离：SourceChangeSequence 只判断快照新旧（防旧覆盖新），
// MapSurfaceResourceKey 决定几何资源是否重建。
public enum MapSurfaceResourceUpdateKind
{
    RejectStale, // 旧序号快照：拒绝覆盖，不消费、不重建
    NoRebuild,   // 资源键未变（如 Rename）：只推进已消费序号
    Recreate     // 资源键变化（Resize/BaseHeight/换地图）：清空并重建
}

public readonly record struct MapSurfaceResourceUpdate(
    MapSurfaceResourceUpdateKind Kind,
    MapSurfaceResourceKey Key);

public static class MapSurfaceResourceUpdatePolicy
{
    public static MapSurfaceResourceUpdate Decide(
        MapRenderSnapshot snapshot,
        long lastConsumedSequence,
        MapSurfaceResourceKey? currentKey)
    {
        if (snapshot.SourceChangeSequence < lastConsumedSequence)
            return new MapSurfaceResourceUpdate(MapSurfaceResourceUpdateKind.RejectStale, default);
        var key = MapSurfaceResourceKey.From(snapshot);
        var kind = currentKey == key
            ? MapSurfaceResourceUpdateKind.NoRebuild
            : MapSurfaceResourceUpdateKind.Recreate;
        return new MapSurfaceResourceUpdate(kind, key);
    }
}
