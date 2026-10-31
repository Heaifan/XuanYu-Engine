using XuanYu.World.Map;
using XuanYu.World;

namespace XuanYu.Editor.MapEditing;

public interface IGroundSurface
{
    SurfaceBinding Binding { get; }
    int? Revision { get; }
    SurfaceQueryResult QuerySurface(MapPoint worldXY) =>
        TryGetElevation(worldXY, out var elevation)
            ? SurfaceQueryResult.Valid(elevation)
            : SurfaceQueryResult.NoData;
    bool TryGetElevation(MapPoint worldXY, out double elevation);
}
