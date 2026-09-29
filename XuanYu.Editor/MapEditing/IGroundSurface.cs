using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public interface IGroundSurface
{
    SurfaceBinding Binding { get; }
    int? Revision { get; }
    bool TryGetElevation(MapPoint worldXY, out double elevation);
}
