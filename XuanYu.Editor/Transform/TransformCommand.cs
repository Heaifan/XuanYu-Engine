using XuanYu.Core.Identity;
using XuanYu.Core.Scene;
using XuanYu.World.Scene;

namespace XuanYu.Editor.Transform;

public readonly record struct TransformCommand(
    long SessionId,
    EntityId EntityKey,
    CommittedTransform Transform,
    long EntityRevision = 0)
{
    public bool TryApply(
        IWorldTransformMutationContract world,
        out SceneTransformCommitResult result) =>
        world.TryCommitTransform(EntityKey, EntityRevision, Transform, out result);
}
