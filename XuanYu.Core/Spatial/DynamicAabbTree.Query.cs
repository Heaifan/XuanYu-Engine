namespace XuanYu.Core.Spatial;

public sealed partial class DynamicAabbTree
{
    int QueryInto(SpatialAabb area, SpatialQueryCategory mask, List<SpatialBounds> candidates)
    {
        if (_root < 0) return 0;
        var visited = 0;
        Span<int> small = stackalloc int[128];
        var overflow = new Stack<int>();
        var count = 1;
        small[0] = _root;
        while (count > 0 || overflow.Count > 0)
        {
            var index = count > 0 ? small[--count] : overflow.Pop();
            visited++;
            var node = _nodes[index];
            if (!node.Bounds.WorldBounds.Intersects(area)) continue;
            if (node.IsLeaf)
            {
                if ((node.Bounds.Category & mask) != 0) candidates.Add(node.Bounds);
            }
            else
            {
                Push(node.Left, ref count, small, overflow);
                Push(node.Right, ref count, small, overflow);
            }
        }

        return visited;
    }

    int QueryInto(SpatialRayQuery ray, SpatialQueryCategory mask, List<SpatialBounds> candidates)
    {
        return QueryTree(index =>
        {
            var node = _nodes[index];
            if (!SpatialRayAabb.Intersects(ray, node.Bounds.WorldBounds)) return false;
            if (node.IsLeaf && (node.Bounds.Category & mask) != 0) candidates.Add(node.Bounds);
            return true;
        });
    }

    int QueryTree(Func<int, bool> visit)
    {
        if (_root < 0) return 0;
        var visited = 0;
        Span<int> small = stackalloc int[128];
        var overflow = new Stack<int>();
        var count = 1;
        small[0] = _root;
        while (count > 0 || overflow.Count > 0)
        {
            var index = count > 0 ? small[--count] : overflow.Pop();
            visited++;
            var node = _nodes[index];
            if (!visit(index) || node.IsLeaf) continue;
            Push(node.Left, ref count, small, overflow);
            Push(node.Right, ref count, small, overflow);
        }

        return visited;
    }

    static void Push(int value, ref int count, Span<int> small, Stack<int> overflow)
    {
        if (count < small.Length) small[count++] = value;
        else overflow.Push(value);
    }
}
