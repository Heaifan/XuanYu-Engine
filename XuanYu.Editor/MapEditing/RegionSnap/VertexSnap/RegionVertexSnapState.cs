using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public sealed class RegionVertexSnapState
{
    public MapRegionId? TargetRegionId { get; private set; }
    public int TargetVertexIndex { get; private set; } = -1;
    public MapPoint TargetPoint { get; private set; }
    public bool IsSnapped => TargetRegionId.HasValue && TargetVertexIndex >= 0;

    public void Acquire(MapRegionId regionId, int vertexIndex, MapPoint point)
    {
        TargetRegionId = regionId;
        TargetVertexIndex = vertexIndex;
        TargetPoint = point;
    }

    public void Clear()
    {
        TargetRegionId = null;
        TargetVertexIndex = -1;
        TargetPoint = default;
    }
}
