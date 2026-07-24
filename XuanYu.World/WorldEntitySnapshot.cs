using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;

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
        WorldEntityActivity activity)
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
    }

    public EntityId EntityKey { get; }
    public string Name { get; }
    public string Type { get; }
    public CommittedTransform Transform { get; }
    public Vector3d GlobalPosition { get; }
    public RegionKey RegionKey { get; }
    public WorldEntityActivity Activity { get; }
}
