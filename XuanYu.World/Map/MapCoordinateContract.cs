using XuanYu.Core.Math;

namespace XuanYu.World.Map;

// MAP-A-R3-D2-F1：MapPoint 直写世界 XY，地图中心为世界原点。
public static class MapCoordinateContract
{
    public static Vector3d MapToWorld(MapPoint point, double z) =>
        new(point.X, point.Y, z);

    public static MapPoint WorldToMap(Vector3d point) =>
        new(point.X, point.Y);
}
