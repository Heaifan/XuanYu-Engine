namespace XuanYu.World.Map;

// MAP-A-R2-D1：有限地图边界（米）。地图中心为世界原点，范围 X/Y ∈ [-W/2, W/2]。
// 与 WorldMapState.Contains 闭区间语义一致（边界属于地图）。
public static class MapBounds
{
    public static double MinX(MapSize size) => -size.Width / 2.0;

    public static double MaxX(MapSize size) => size.Width / 2.0;

    public static double MinY(MapSize size) => -size.Depth / 2.0;

    public static double MaxY(MapSize size) => size.Depth / 2.0;

    public static bool Contains(MapSize size, double x, double y) =>
        x >= MinX(size) && x <= MaxX(size) &&
        y >= MinY(size) && y <= MaxY(size);
}
