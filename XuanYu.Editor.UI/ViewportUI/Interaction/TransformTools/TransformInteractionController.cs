using XuanYu.Core.Gizmo;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Transform;
using XuanYu.Editor.Transform;

namespace XuanYu.Editor.UI;

internal sealed class TransformInteractionController
{
    readonly TransformSession _session = new();
    MoveDragConstraint? _move;
    RotateGizmoDrag? _rotate;
    ScaleGizmoDrag? _scale;

    public bool IsActive => _session.IsActive;
    public TransformStartSnapshot StartSnapshot => _session.StartSnapshot;
    public PreviewTransform? Preview => _session.Preview;

    public bool BeginMove(long id, EntitySnapshot entity, MoveGizmoAxis axis,
        long revision, MoveDragConstraint constraint)
    {
        if (!_session.Begin(id, entity, axis, revision)) return false;
        _move = constraint;
        return true;
    }

    public RotateBeginResult BeginRotate(long id, EntitySnapshot entity, RotateGizmoAxis axis,
        long revision, RotateGizmoDrag drag, double x, double y)
    {
        if (!_session.BeginRotate(id, entity, axis, revision)) return RotateBeginResult.SessionRejected;
        if (!drag.TryInitialize(x, y)) return RotateBeginResult.InvalidRay;
        _rotate = drag;
        return RotateBeginResult.Started;
    }

    public bool BeginScale(long id, EntitySnapshot entity, ScaleGizmoHandle handle,
        long revision, ScaleGizmoDrag drag)
    {
        if (!_session.BeginScale(id, entity, handle, revision)) return false;
        _scale = drag;
        return true;
    }

    public bool PreviewMove(long id, double x, double y)
    {
        if (!_session.IsActive || _move is not { } constraint) return false;
        var position = constraint.Solve(_session.StartSnapshot.Transform.Position, x, y);
        return _session.TryPreview(id, position);
    }

    public bool PreviewRotate(long id, double x, double y)
    {
        if (!_session.IsActive || _rotate is not { } drag) return false;
        var rotation = drag.Solve(x, y);
        if (rotation is null || !_session.TryPreviewRotation(id, rotation.Value)) return false;
        _rotate = drag;
        return true;
    }

    public bool PreviewScale(long id, double x, double y)
    {
        if (!_session.IsActive || _scale is not { } drag) return false;
        var scale = drag.Solve(x, y);
        if (!_session.TryPreviewScale(id, scale)) return false;
        _scale = drag;
        return true;
    }

    public bool TryCreateCommand(long id, out TransformCommand command) =>
        _session.TryCreateCommand(id, out command);

    public bool TryCancel(long id) => _session.TryCancel(id);

    public void ClearPointerState()
    {
        _move = null;
        _rotate = null;
        _scale = null;
    }
}

internal enum RotateBeginResult { Started, SessionRejected, InvalidRay }
