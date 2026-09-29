using XuanYu.Core.Space;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public static class GroundPickResolver
{
    public static GroundPickResult Resolve(WorldRay ray, IGroundSurface? terrain,
        double referencePlaneElevation)
    {
        if (terrain is null) return ResolveReferencePlane(ray, referencePlaneElevation);
        if (Math.Abs(ray.Direction.Z) < 1e-9) return GroundPickResult.Invalid;
        if (!TryResolveTerrain(ray, terrain, out var point, out var elevation))
            return GroundPickResult.Invalid;
        return GroundPickResult.Valid(point, terrain.Binding, elevation, terrain.Revision);
    }

    static GroundPickResult ResolveReferencePlane(WorldRay ray, double elevation)
    {
        if (Math.Abs(ray.Direction.Z) < 1e-9) return GroundPickResult.Invalid;
        var distance = (elevation - ray.Origin.Z) / ray.Direction.Z;
        if (!double.IsFinite(distance) || distance < 0) return GroundPickResult.Invalid;
        var hit = ray.Origin + ray.Direction * distance;
        return GroundPickResult.Valid(new(hit.X, hit.Y), SurfaceBinding.ReferencePlane, elevation);
    }

    static bool TryResolveTerrain(WorldRay ray, IGroundSurface terrain,
        out MapPoint point, out double elevation)
    {
        point = default; elevation = 0;
        var distance = (0 - ray.Origin.Z) / ray.Direction.Z;
        if (!double.IsFinite(distance) || distance < 0) return false;
        for (var attempt = 0; attempt < 4; attempt++)
        {
            var hit = ray.Origin + ray.Direction * distance;
            point = new(hit.X, hit.Y);
            if (!terrain.TryGetElevation(point, out elevation)) return false;
            var next = (elevation - ray.Origin.Z) / ray.Direction.Z;
            if (!double.IsFinite(next) || next < 0) return false;
            if (Math.Abs(next - distance) < 1e-7) return true;
            distance = next;
        }
        var finalHit = ray.Origin + ray.Direction * distance;
        point = new(finalHit.X, finalHit.Y);
        return terrain.TryGetElevation(point, out elevation);
    }
}
