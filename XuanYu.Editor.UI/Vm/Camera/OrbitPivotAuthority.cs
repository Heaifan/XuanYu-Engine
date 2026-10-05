using XuanYu.Core.Math;
using XuanYu.Editor.MapEditing;

namespace XuanYu.Editor.UI;

public enum OrbitPivotSource { Selection, Terrain, ReferencePlane, PreviousValidPivot, ObservationCenter }

public sealed record OrbitPivotResolution(
    Vector3d Pivot,
    OrbitPivotSource Source,
    GroundPickResult TerrainHit,
    GroundPickResult ReferencePlaneHit)
{
    public string SurfaceSource => Source.ToString();
}

public static class OrbitPivotAuthority
{
    public static OrbitPivotResolution Resolve(Vector3d? selectionPivot,
        GroundPickResult terrainHit, GroundPickResult referencePlaneHit,
        Vector3d previousValidPivot)
    {
        if (selectionPivot is { } selection && IsFinite(selection))
            return new(selection, OrbitPivotSource.Selection, terrainHit, referencePlaneHit);
        if (TryPoint(terrainHit, out var terrain))
            return new(terrain, OrbitPivotSource.Terrain, terrainHit, referencePlaneHit);
        if (TryPoint(referencePlaneHit, out var plane))
            return new(plane, OrbitPivotSource.ReferencePlane, terrainHit, referencePlaneHit);
        var previous = IsFinite(previousValidPivot) ? previousValidPivot : Vector3d.Zero;
        return new(previous, OrbitPivotSource.PreviousValidPivot, terrainHit, referencePlaneHit);
    }

    static bool TryPoint(GroundPickResult hit, out Vector3d point)
    {
        point = new(hit.WorldXY.X, hit.WorldXY.Y, hit.ResolvedElevation);
        return hit.IsValid && IsFinite(point);
    }

    static bool IsFinite(Vector3d point) => double.IsFinite(point.X) &&
        double.IsFinite(point.Y) && double.IsFinite(point.Z);
}
