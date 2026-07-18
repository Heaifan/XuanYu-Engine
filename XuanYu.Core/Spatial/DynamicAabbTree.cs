using XuanYu.Core.Identity;

namespace XuanYu.Core.Spatial;

public sealed partial class DynamicAabbTree : ISpatialIndex
{
    readonly List<Node> _nodes = [];
    readonly Dictionary<EntityId, int> _leaves = [];
    int _root = -1;

    public int Count => _leaves.Count;

    public void Insert(SpatialBounds bounds)
    {
        if (_leaves.ContainsKey(bounds.EntityKey)) throw new InvalidOperationException("空间索引已存在该实体。");
        var leaf = AddNode(bounds);
        InsertLeaf(leaf);
        _leaves.Add(bounds.EntityKey, leaf);
    }

    public bool Remove(EntityId entityKey)
    {
        if (!_leaves.Remove(entityKey, out var leaf)) return false;
        RemoveLeaf(leaf);
        return true;
    }

    public bool Update(SpatialBounds bounds)
    {
        if (!_leaves.TryGetValue(bounds.EntityKey, out var leaf))
        {
            throw new KeyNotFoundException("空间索引不存在该实体。");
        }

        if (_nodes[leaf].Bounds == bounds) return false;
        RemoveLeaf(leaf);
        _nodes[leaf].Bounds = bounds;
        InsertLeaf(leaf);
        return true;
    }

    public SpatialQueryResult Query(SpatialAabb area, SpatialQueryCategory mask)
    {
        var candidates = new List<SpatialBounds>();
        var visited = QueryInto(area, mask, candidates);
        var stats = new SpatialQueryStats(0, Count, visited, candidates.Count);
        return new SpatialQueryResult(candidates, stats);
    }

    public SpatialQueryResult Query(SpatialRayQuery ray, SpatialQueryCategory mask)
    {
        var candidates = new List<SpatialBounds>();
        var visited = QueryInto(ray, mask, candidates);
        var stats = new SpatialQueryStats(0, Count, visited, candidates.Count);
        return new SpatialQueryResult(candidates, stats);
    }

    int AddNode(SpatialBounds bounds)
    {
        _nodes.Add(new Node { Bounds = bounds });
        return _nodes.Count - 1;
    }
}
