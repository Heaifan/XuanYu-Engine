using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public sealed class RegionSnapState
{
    public RegionSnapKind Kind { get; private set; }
    public MapRegionId? TargetRegionId { get; private set; }
    public int TargetVertexIndex { get; private set; } = -1;
    public int TargetSegmentIndex { get; private set; } = -1;
    public MapPoint TargetPoint { get; private set; }
    public bool IsSnapped => Kind != RegionSnapKind.None;

    public void Acquire(RegionEdgeSnapResult result)
    {
        Kind = result.Kind;
        TargetRegionId = result.TargetRegionId;
        TargetVertexIndex = result.Kind == RegionSnapKind.Vertex ? result.TargetIndex : -1;
        TargetSegmentIndex = result.Kind == RegionSnapKind.Edge ? result.TargetIndex : -1;
        TargetPoint = result.ResolvedPoint;
    }

    public void Clear()
    {
        Kind = RegionSnapKind.None;
        TargetRegionId = null;
        TargetVertexIndex = -1;
        TargetSegmentIndex = -1;
        TargetPoint = default;
    }
}
