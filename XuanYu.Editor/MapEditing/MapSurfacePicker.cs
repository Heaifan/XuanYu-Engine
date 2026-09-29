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
        => TryPick(map, projection, logicalX, logicalY, null, out point);

    public static bool TryPick(
        MapDefinition map,
        ViewProjectionState projection,
        double logicalX,
        double logicalY,
        IGroundSurface? terrain,
        out MapPoint point)
    {
        ArgumentNullException.ThrowIfNull(map);
        ArgumentNullException.ThrowIfNull(projection);
        var ray = WorldRayFactory.FromViewportPoint(projection, logicalX, logicalY);
        var result = GroundPickResolver.Resolve(ray, terrain, map.Surface.BaseHeightMeters);
        point = result.WorldXY;
        if (!result.IsValid) return false;
        return MapBounds.Contains(map.SizeMeters, point.X, point.Y);
    }
}
