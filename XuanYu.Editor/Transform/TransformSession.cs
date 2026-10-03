using XuanYu.Core.Gizmo;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Transform;
using XuanYu.World.Scene;
namespace XuanYu.Editor.Transform;
public sealed partial class TransformSession
{
    public bool IsActive { get; private set; }
    public long SessionId { get; private set; }
    public MoveGizmoAxis Axis { get; private set; }
    public TransformStartSnapshot StartSnapshot { get; private set; }
    public PreviewTransform? Preview { get; private set; }
    long EntityRevision { get; set; }
    public bool Begin(long sessionId, SceneEntitySnapshot entity, MoveGizmoAxis axis,
        long entityRevision = 0)
    {
        if (sessionId <= 0) throw new ArgumentOutOfRangeException(nameof(sessionId));
        if (!entity.IsValid) return false;
        if (IsActive) return false;
        IsActive = true;
        SessionId = sessionId;
        Axis = axis;
        EntityRevision = entityRevision;
        StartSnapshot = new TransformStartSnapshot(entity.EntityKey, entity.Transform);
        Preview = new PreviewTransform(entity.Transform);
        return true;
    }
    public bool TryPreview(long sessionId, Vector3d position)
    {
        if (!Owns(sessionId)) return false;
        Preview = new PreviewTransform(StartSnapshot.Transform.WithPosition(position));
        return true;
    }

    public bool TryPreviewTransform(long sessionId, CommittedTransform transform)
    {
        if (!Owns(sessionId)) return false;
        Preview = new PreviewTransform(transform);
        return true;
    }

    public bool TryPreviewRotation(long sessionId, Vector3d rotation) =>
        TryPreviewTransform(sessionId, StartSnapshot.Transform.WithRotation(rotation));

    public bool TryPreviewScale(long sessionId, Vector3d scale) =>
        TryPreviewTransform(sessionId, StartSnapshot.Transform.WithScale(scale));

    public bool TryCommit(long sessionId, IWorldTransformMutationContract world)
    {
        return TryCommit(sessionId, world, out _);
    }

    public bool TryCommit(
        long sessionId,
        IWorldTransformMutationContract world,
        out SceneTransformCommitResult commit)
    {
        commit = default;
        if (!TryCreateCommand(sessionId, out var command)) return false;
        return command.TryApply(world, out commit);
    }

    public bool TryCreateRequest(long sessionId, out TransformRequest request)
    {
        request = default;
        if (!Owns(sessionId)) return false;
        request = new(sessionId, StartSnapshot.EntityKey,
            Preview?.Transform ?? StartSnapshot.Transform, EntityRevision);
        End();
        return true;
    }

    public bool TryCreateCommand(long sessionId, out TransformCommand command)
    {
        command = default;
        if (!TryCreateRequest(sessionId, out var request)) return false;
        command = request.ToCommand();
        return true;
    }

    public bool TryCancel(long sessionId)
    {
        if (!Owns(sessionId)) return false;
        End();
        return true;
    }

    bool Owns(long sessionId) => IsActive && SessionId == sessionId;

    void End()
    {
        IsActive = false;
        SessionId = 0;
        EntityRevision = 0;
        Preview = null;
        RotateAxis = null;
        ScaleHandle = null;
    }
}
