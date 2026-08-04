namespace XuanYu.Render.Abstractions;

// MAP-A-R2-D3-F2：地图资源更新决策的显示文本（日志中文化）。
// 内部枚举保持英文，仅提供无 UI 依赖的显示映射；Vulkan 层可安全引用。
public static class MapSurfaceResourceUpdateText
{
    public static string Of(MapSurfaceResourceUpdateKind kind) => kind switch
    {
        MapSurfaceResourceUpdateKind.Recreate => "重新创建",
        MapSurfaceResourceUpdateKind.NoRebuild => "无需重建",
        MapSurfaceResourceUpdateKind.RejectStale => "拒绝旧快照",
        _ => kind.ToString()
    };
}
