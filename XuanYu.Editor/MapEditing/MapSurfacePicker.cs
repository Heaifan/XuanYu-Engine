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
        if (!TryPickGround(map, projection, logicalX, logicalY, terrain, out var result))
        {
            point = default;
            return false;
        }
        point = result.WorldXY;
        return true;
    }

    public static bool TryPickGround(
        MapDefinition map,
        ViewProjectionState projection,
        double logicalX,
        double logicalY,
        IGroundSurface? terrain,
        out GroundPickResult result)
    {
        ArgumentNullException.ThrowIfNull(map);
        ArgumentNullException.ThrowIfNull(projection);
        var ray = WorldRayFactory.FromViewportPoint(projection, logicalX, logicalY);
        result = GroundPickResolver.Resolve(ray, terrain, map.Surface.BaseHeightMeters);
        if (!result.IsValid) return false;
        if (MapBounds.Contains(map.SizeMeters, result.WorldXY.X, result.WorldXY.Y)) return true;
        result = GroundPickResult.Invalid;
        return false;
    }
}
