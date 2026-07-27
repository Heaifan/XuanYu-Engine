using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;

namespace XuanYu.World.Scene;

public sealed partial class SceneStateOwner
{
    public bool CommitPosition(Vector3d position) =>
        CommitPositionWithResult(position).Changed;

    public SceneTransformCommitResult CommitPositionWithResult(Vector3d position) =>
        CommitPositionWithResult(_activeEntityKey, position);

    public SceneTransformCommitResult CommitTransformWithResult(CommittedTransform transform) =>
        CommitTransformWithResult(_activeEntityKey, transform);

    public SceneTransformCommitResult CommitPositionWithResult(
        EntityId entityKey,
        Vector3d position)
    {
        if (!_world.TryGet(entityKey, out var current))
            return UnchangedMissingTransform(entityKey);
        return CommitTransformWithResult(entityKey, current.Transform.WithPosition(position));
    }

    public SceneTransformCommitResult CommitTransformWithResult(
        EntityId entityKey,
        CommittedTransform transform)
    {
        if (!_world.TryGet(entityKey, out var current))
            return UnchangedMissingTransform(entityKey);
        if (EquivalentTransform(current.Transform, transform))
            return new SceneTransformCommitResult(
                entityKey,
                current.Transform,
                transform,
                false);
        return ApplyTransform(current, transform);
    }

    static bool EquivalentTransform(CommittedTransform a, CommittedTransform b)
    {
        const double eps = 1e-4;
        return ComponentNear(a.Position, b.Position, eps)
            && ComponentNear(a.Rotation, b.Rotation, eps)
            && ComponentNear(a.Scale, b.Scale, eps);
    }

    static bool ComponentNear(Vector3d a, Vector3d b, double e) =>
        System.Math.Abs(a.X - b.X) <= e
        && System.Math.Abs(a.Y - b.Y) <= e
        && System.Math.Abs(a.Z - b.Z) <= e;

    static SceneTransformCommitResult UnchangedMissingTransform(EntityId entityKey) =>
        new(
            entityKey,
            CommittedTransform.Identity,
            CommittedTransform.Identity,
            false);
}
