using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

// MapRegion 是唯一数据真源；本类型只保存可重建的 RegionId + AABB 平衡查询树。
internal sealed class RegionSpatialIndex
{
    static readonly RegionSpatialBounds MapBounds = CreateMapBounds();
    RegionSpatialNode? _root;
    Dictionary<MapRegionId, RegionSpatialNode> _leaves = [];

    public int Count => _leaves.Count;
    internal int NodeCount => Count == 0 ? 0 : (2 * Count) - 1;
    internal int Height => _root?.Height ?? 0;

    public void Rebuild(IEnumerable<MapRegion> regions)
    {
        var replacement = new RegionSpatialIndex();
        foreach (var region in regions) replacement.Upsert(region);
        _root = replacement._root;
        _leaves = replacement._leaves;
    }

    public void Upsert(MapRegion region)
    {
        var bounds = RegionSpatialBounds.From(region);
        if (!MapBounds.Contains(bounds))
            throw new ArgumentOutOfRangeException(nameof(region), "区域边界超出地图空间索引合同。");
        Remove(region.RegionId);
        var leaf = new RegionSpatialNode(bounds, region.RegionId);
        RegionSpatialTreeEditor.Insert(ref _root, leaf);
        _leaves.Add(region.RegionId, leaf);
    }

    public bool Remove(MapRegionId regionId)
    {
        if (!_leaves.Remove(regionId, out var leaf)) return false;
        RegionSpatialTreeEditor.Remove(ref _root, leaf);
        return true;
    }

    public ImmutableArray<MapRegionId> Query(RegionSpatialBounds bounds) =>
        QueryWithStats(bounds).RegionIds;

    internal RegionSpatialQueryResult QueryWithStats(RegionSpatialBounds bounds) =>
        RegionSpatialQueryWalker.Query(_root, bounds);

    static RegionSpatialBounds CreateMapBounds()
    {
        var extent = MapDefinitionValidator.MaxSizeMeters / 2.0;
        return new(-extent, -extent, extent, extent);
    }
}
