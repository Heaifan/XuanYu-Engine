using XuanYu.Core.Identity;
using XuanYu.Core.Scene;

namespace XuanYu.World.Scene;

public interface IWorldTransformMutationContract
{
    // Compatibility capture property: scoped to the currently active entity,
    // never a coarse global world revision.
    long WorldRevision { get; }

    long GetEntityMutationRevision(EntityId entityKey);

    bool TryCommitTransform(
        EntityId entityKey,
        long expectedEntityRevision,
        CommittedTransform transform,
        out SceneTransformCommitResult result);
}
