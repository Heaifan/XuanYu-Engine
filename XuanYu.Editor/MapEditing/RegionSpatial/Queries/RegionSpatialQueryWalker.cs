using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

internal static class RegionSpatialQueryWalker
{
    public static RegionSpatialQueryResult Query(
        RegionSpatialNode? root,
        RegionSpatialBounds query)
    {
        if (root is null) return new([], new(0, 0, 0, 0));
        var matches = new List<MapRegionId>();
        var stack = new Stack<RegionSpatialNode>();
        var visited = 0;
        var tested = 0;
        stack.Push(root);
        while (stack.TryPop(out var node))
        {
            visited++;
            if (!node.Bounds.Intersects(query)) continue;
            if (node.RegionId is { } regionId)
            {
                tested++;
                matches.Add(regionId);
                continue;
            }
            stack.Push(node.Left!);
            stack.Push(node.Right!);
        }
        matches.Sort(static (left, right) => string.CompareOrdinal(left.Value, right.Value));
        var ids = matches.ToImmutableArray();
        return new(ids, new(visited, tested, ids.Length, root.Height));
    }
}
