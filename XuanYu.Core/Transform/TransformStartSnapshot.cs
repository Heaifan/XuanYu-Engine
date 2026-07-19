using XuanYu.Core.Identity;
using XuanYu.Core.Scene;

namespace XuanYu.Core.Transform;

public readonly record struct TransformStartSnapshot(
    EntityId EntityKey,
    CommittedTransform Transform);
