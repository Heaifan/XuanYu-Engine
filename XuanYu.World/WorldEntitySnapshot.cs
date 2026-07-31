using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Spatial;

namespace XuanYu.World;

public readonly record struct WorldEntitySnapshot
{
    public WorldEntitySnapshot(
        EntityId entityKey,
        string name,
        string type,
        CommittedTransform transform,
        Vector3d globalPosition,
        RegionKey regionKey,
        WorldEntityActivity activity,
        SpatialAabb extent,
        EntityId parentId = default,
        int siblingOrder = 0)
    {
        if (!entityKey.IsValid) throw new ArgumentOutOfRangeException(nameof(entityKey));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("实体名称不能为空。", nameof(name));
        if (string.IsNullOrWhiteSpace(type)) throw new ArgumentException("实体类型不能为空。", nameof(type));
        EntityKey = entityKey;
        Name = name;
        Type = type;
        Transform = transform;
        GlobalPosition = globalPosition;
        RegionKey = regionKey;
        Activity = activity;
        Extent = extent;
        ParentId = parentId;
        SiblingOrder = siblingOrder;
    }

    public EntityId EntityKey { get; }
    public string Name { get; }
    public string Type { get; }
    public CommittedTransform Transform { get; }
    public Vector3d GlobalPosition { get; }
    public RegionKey RegionKey { get; }
    public WorldEntityActivity Activity { get; }

    // Spatial extent relative to the entity position (local box). The absolute world
    // bounds are derived by translating this extent to GlobalPosition. This is the
    // entity's OWN spatial description supplied at creation -- WorldQuery consumes it
    // and never invents a default size for entities (R2-R1).
    public SpatialAabb Extent { get; }
    public EntityId ParentId { get; }
    public int SiblingOrder { get; }

    public SpatialBounds Bounds =>
        new(EntityKey, Extent.Translate(GlobalPosition), SpatialQueryCategory.SceneEntity);
}
