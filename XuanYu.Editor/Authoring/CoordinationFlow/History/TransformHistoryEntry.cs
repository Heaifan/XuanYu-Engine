using XuanYu.Core.Identity;
using XuanYu.Core.Scene;

namespace XuanYu.Editor.History;

public readonly record struct TransformHistoryEntry(
    EntityId EntityKey,
    CommittedTransform Before,
    CommittedTransform After);
