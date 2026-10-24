using XuanYu.Core.Identity;

namespace XuanYu.Core.Scene;

public readonly record struct SceneTransformCommitResult(
    EntityId EntityKey,
    CommittedTransform Before,
    CommittedTransform After,
    bool Changed);
