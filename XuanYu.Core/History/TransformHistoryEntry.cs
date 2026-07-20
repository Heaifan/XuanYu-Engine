using XuanYu.Core.Identity;
using XuanYu.Core.Scene;

namespace XuanYu.Core.History;

public readonly record struct TransformHistoryEntry(
    EntityId EntityKey,
    CommittedTransform Before,
    CommittedTransform After);
