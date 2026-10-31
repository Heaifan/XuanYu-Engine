using XuanYu.Core.Identity;
using XuanYu.Core.Scene;

namespace XuanYu.Editor.Transform;

public readonly record struct TransformRequest(
    long SessionId,
    EntityId EntityKey,
    CommittedTransform Transform,
    long EntityRevision)
{
    public TransformCommand ToCommand() =>
        new(SessionId, EntityKey, Transform, EntityRevision);
}
