using XuanYu.Core.Spatial;
namespace XuanYu.World.Spatial;

public sealed partial class DynamicAabbTree
{
    void InsertLeaf(int leaf)
    {
        if (_root < 0)
        {
            _root = leaf;
            _nodes[leaf].Parent = -1;
            return;
        }

        var sibling = FindBestSibling(_nodes[leaf].Bounds.WorldBounds);
        var oldParent = _nodes[sibling].Parent;
        var parent = AddNode(new SpatialBounds(_nodes[leaf].EntityKey, _nodes[leaf].Bounds.WorldBounds.Union(_nodes[sibling].Bounds.WorldBounds), SpatialQueryCategory.All));
        _nodes[parent].Parent = oldParent;
        _nodes[parent].Left = sibling;
        _nodes[parent].Right = leaf;
        _nodes[sibling].Parent = parent;
        _nodes[leaf].Parent = parent;
        if (oldParent < 0) _root = parent;
        else ReplaceChild(oldParent, sibling, parent);
        Refit(_nodes[parent].Parent);
    }

    int FindBestSibling(SpatialAabb bounds)
    {
        var index = _root;
        while (!_nodes[index].IsLeaf)
        {
            var left = _nodes[index].Left;
            var right = _nodes[index].Right;
            index = Cost(bounds, left) <= Cost(bounds, right) ? left : right;
        }

        return index;
    }

    double Cost(SpatialAabb bounds, int node)
    {
        return bounds.Union(_nodes[node].Bounds.WorldBounds).SurfaceArea - _nodes[node].Bounds.WorldBounds.SurfaceArea;
    }
}
