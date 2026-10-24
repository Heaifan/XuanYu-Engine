using XuanYu.Core.Identity;

namespace XuanYu.Core.Scene;

public readonly record struct TransformCommit(
    EntityId EntityKey,
    CommittedTransform Before,
    CommittedTransform After,
    bool Changed);
