using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

internal sealed class RegionSpatialNode
{
    public RegionSpatialNode(RegionSpatialBounds bounds, MapRegionId? regionId = null)
    {
        Bounds = bounds;
        RegionId = regionId;
    }

    public RegionSpatialNode? Parent { get; set; }
    public RegionSpatialNode? Left { get; set; }
    public RegionSpatialNode? Right { get; set; }
    public RegionSpatialBounds Bounds { get; set; }
    public MapRegionId? RegionId { get; }
    public int Height { get; private set; }
    public bool IsLeaf => RegionId.HasValue;

    public void Refit()
    {
        if (IsLeaf) return;
        var left = Left ?? throw new InvalidOperationException("空间索引内部节点缺少左子树。");
        var right = Right ?? throw new InvalidOperationException("空间索引内部节点缺少右子树。");
        Bounds = left.Bounds.Union(right.Bounds);
        Height = 1 + Math.Max(left.Height, right.Height);
    }
}
