using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public readonly record struct GroundPickResult(
    bool IsValid,
    MapPoint WorldXY,
    SurfaceBinding SurfaceBinding,
    double ResolvedElevation,
    int? TerrainRevision)
{
    public static GroundPickResult Invalid { get; } = new(
        false, default, SurfaceBinding.ReferencePlane, 0, null);

    public static GroundPickResult Valid(MapPoint xy, SurfaceBinding binding,
        double elevation, int? revision = null) =>
        new(true, xy, binding, elevation, revision);
}
