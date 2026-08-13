using XuanYu.Core.Gizmo;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public readonly record struct RegionEdgeSnapVertex(
    MapRegionId RegionId,
    int VertexIndex,
    MapPoint WorldPoint,
    ScreenPoint ScreenPoint);
