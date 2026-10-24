using XuanYu.Core.Identity;

namespace XuanYu.Core.Scene;

public readonly record struct EntitySnapshot(
    EntityId EntityKey,
    string Name,
    string Type,
    CommittedTransform Transform)
{
    public bool IsValid => EntityKey.IsValid;
}
