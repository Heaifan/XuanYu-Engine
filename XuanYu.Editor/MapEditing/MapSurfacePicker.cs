using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public static class MapSurfacePicker
{
    public static bool TryPick(
        MapDefinition map,
        ViewProjectionState projection,
        double logicalX,
        double logicalY,
        out MapPoint point)
    {
        ArgumentNullException.ThrowIfNull(map);
        ArgumentNullException.ThrowIfNull(projection);
        var ray = WorldRayFactory.FromViewportPoint(projection, logicalX, logicalY);
        if (Math.Abs(ray.Direction.Z) < 1e-9)
        {
            point = default;
            return false;
        }

        var distance = (map.Surface.BaseHeightMeters - ray.Origin.Z) / ray.Direction.Z;
        if (!double.IsFinite(distance) || distance < 0.0)
        {
            point = default;
            return false;
        }

        var hit = ray.Origin + (ray.Direction * distance);
        point = MapCoordinateContract.WorldToMap(hit);
        return MapBounds.Contains(map.SizeMeters, point.X, point.Y);
    }
}
