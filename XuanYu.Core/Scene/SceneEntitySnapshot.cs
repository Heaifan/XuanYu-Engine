using XuanYu.Core.Identity;

namespace XuanYu.Core.Scene;

public readonly record struct SceneEntitySnapshot(
    EntityId EntityKey,
    string Name,
    string Type,
    CommittedTransform Transform)
{
    public bool IsValid => EntityKey.IsValid;
}
