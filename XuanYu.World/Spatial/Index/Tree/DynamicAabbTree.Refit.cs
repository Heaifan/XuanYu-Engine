using XuanYu.Core.Spatial;
namespace XuanYu.World.Spatial;

public sealed partial class DynamicAabbTree
{
    void Refit(int index)
    {
        while (index >= 0)
        {
            var left = _nodes[index].Left;
            var right = _nodes[index].Right;
            var area = _nodes[left].Bounds.WorldBounds.Union(_nodes[right].Bounds.WorldBounds);
            _nodes[index].Bounds = new SpatialBounds(_nodes[index].EntityKey, area, SpatialQueryCategory.All);
            index = _nodes[index].Parent;
        }
    }
}
