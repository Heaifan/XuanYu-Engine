using XuanYu.Core.Map;

namespace XuanYu.Render.Abstractions;

// MAP-A-R2-D3/D4：地图渲染快照（唯一渲染输入；渲染层/Vulkan 只读，禁止反向访问编辑会话）。
// 由 Editor 适配器从 MapEditSession.CurrentMap 投影（订阅 ContentChanged 低频事件）。
// 不包含：会话/Undo 历史/Dirty/文件路径/UI 控件/可变集合/Vulkan 句柄。
// SourceChangeSequence：D2 会话单调递增序号，用于判断快照是否已消费最新地图内容；
// 禁止在 Render 中自行递增，禁止用于 Dirty 判断。
// ShowGround/ShowBoundary（D4）：地面/边界图层显隐，渲染过滤（不删除领域数据）；
// 不进 GPU 资源判等键——显隐切换不得触发资源重建。
public readonly record struct MapRenderSnapshot(
    string MapId,
    double WidthMeters,
    double DepthMeters,
    MapSurfaceKind SurfaceKind,
    double BaseHeightMeters,
    double AmplitudeMeters,
    double WavelengthMeters,
    int Seed,
    long SourceChangeSequence,
    bool IsVisible = true,
    bool ShowGround = true,
    bool ShowBoundary = true)
{
    public static MapRenderSnapshot Empty { get; } =
        new("", 0, 0, MapSurfaceKind.Flat, 0, 0, 1, 0, 0, false, false, false);

    public bool HasMap => !string.IsNullOrEmpty(MapId) && IsVisible;

    public double MinX => -WidthMeters / 2.0;

    public double MaxX => WidthMeters / 2.0;

    public double MinY => -DepthMeters / 2.0;

    public double MaxY => DepthMeters / 2.0;
}
