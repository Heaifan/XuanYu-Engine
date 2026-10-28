using XuanYu.Core.Identity;

using XuanYu.Core.Spatial;
namespace XuanYu.World.Spatial;

public sealed partial class DynamicAabbTree
{
    sealed class Node
    {
        public int Parent = -1;
        public int Left = -1;
        public int Right = -1;
        public SpatialBounds Bounds;

        public bool IsLeaf => Left < 0;

        public EntityId EntityKey => Bounds.EntityKey;
    }
}
