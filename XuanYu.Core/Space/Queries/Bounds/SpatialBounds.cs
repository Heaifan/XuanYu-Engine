using XuanYu.Core.Identity;

namespace XuanYu.Core.Spatial;

public readonly record struct SpatialBounds
{
    public SpatialBounds(EntityId entityKey, SpatialAabb worldBounds, SpatialQueryCategory category)
    {
        if (!entityKey.IsValid) throw new ArgumentOutOfRangeException(nameof(entityKey));
        if (category == SpatialQueryCategory.None) throw new ArgumentOutOfRangeException(nameof(category));

        EntityKey = entityKey;
        WorldBounds = worldBounds;
        Category = category;
    }

    public EntityId EntityKey { get; }

    public SpatialAabb WorldBounds { get; }

    public SpatialQueryCategory Category { get; }
}
