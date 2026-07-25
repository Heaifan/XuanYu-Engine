using XuanYu.Core.Gizmo;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Transform;
using XuanYu.World.Scene;

namespace XuanYu.Editor.Transform;

public sealed class TransformSession
{
    public bool IsActive { get; private set; }
    public long SessionId { get; private set; }
    public MoveGizmoAxis Axis { get; private set; }
    public TransformStartSnapshot StartSnapshot { get; private set; }
    public PreviewTransform? Preview { get; private set; }

    public bool Begin(long sessionId, SceneEntitySnapshot entity, MoveGizmoAxis axis)
    {
        if (sessionId <= 0) throw new ArgumentOutOfRangeException(nameof(sessionId));
        if (!entity.IsValid) return false;
        if (IsActive) return false;
        IsActive = true;
        SessionId = sessionId;
        Axis = axis;
        StartSnapshot = new TransformStartSnapshot(entity.EntityKey, entity.Transform);
        Preview = new PreviewTransform(entity.Transform.Position);
        return true;
    }

    public bool TryPreview(long sessionId, Vector3d position)
    {
        if (!Owns(sessionId)) return false;
        Preview = new PreviewTransform(position);
        return true;
    }

    public bool TryCommit(long sessionId, SceneStateOwner scene)
    {
        return TryCommit(sessionId, scene, out _);
    }

    public bool TryCommit(
        long sessionId,
        SceneStateOwner scene,
        out SceneTransformCommitResult commit)
    {
        commit = default;
        if (!Owns(sessionId) || !scene.RenderSnapshot.HasEntity) return false;
        if (scene.RenderSnapshot.Entity.EntityKey != StartSnapshot.EntityKey) return false;
        var position = Preview?.Position ?? StartSnapshot.Transform.Position;
        End();
        commit = scene.CommitPositionWithResult(position);
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
        Preview = null;
    }
}
