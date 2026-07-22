using XuanYu.Core.Identity;
using XuanYu.Core.Scene;

namespace XuanYu.Core.World;

public sealed class GlobalWorld
{
    readonly EntityRegistry _registry = new();

    public int EntityCount => _registry.Count;

    public IReadOnlyList<WorldEntitySnapshot> Entities => _registry.Snapshot;

    public WorldEntitySnapshot Create(
        string name,
        string type = "WorldEntity",
        CommittedTransform? transform = null) =>
        _registry.Create(name, type, transform);

    public bool Destroy(EntityId entityKey) => _registry.Destroy(entityKey);

    public bool UpdateTransform(EntityId entityKey, CommittedTransform transform) =>
        _registry.UpdateTransform(entityKey, transform);

    public WorldEntitySnapshot Get(EntityId entityKey) => _registry.Get(entityKey);

    public bool TryGet(EntityId entityKey, out WorldEntitySnapshot entity) =>
        _registry.TryGet(entityKey, out entity);

    public bool Exists(EntityId entityKey) => _registry.Exists(entityKey);
}
