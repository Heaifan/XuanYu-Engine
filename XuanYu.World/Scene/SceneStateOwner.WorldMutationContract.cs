using XuanYu.Core.Identity;
using XuanYu.Core.Scene;

namespace XuanYu.World.Scene;

public sealed partial class SceneStateOwner : IWorldTransformMutationContract
{
    long IWorldTransformMutationContract.WorldRevision =>
        GetEntityMutationRevision(_activeEntityKey);

    public long GetEntityMutationRevision(EntityId entityKey) =>
        _entityMutationRevisions.TryGetValue(entityKey, out var revision)
            ? revision
            : 0;

    void TouchEntityMutation(EntityId entityKey)
    {
        if (entityKey == EntityId.None) return;
        _entityMutationRevisions[entityKey] =
            GetEntityMutationRevision(entityKey) + 1;
    }

    bool IWorldTransformMutationContract.TryCommitTransform(
        EntityId entityKey,
        long expectedEntityRevision,
        CommittedTransform transform,
        out SceneTransformCommitResult result)
    {
        if (expectedEntityRevision > 0
            && expectedEntityRevision != GetEntityMutationRevision(entityKey))
        {
            result = default;
            return false;
        }
        if (!TryGetEntity(entityKey, out _))
        {
            result = default;
            return false;
        }
        result = CommitTransformWithResult(entityKey, transform);
        return true;
    }
}
