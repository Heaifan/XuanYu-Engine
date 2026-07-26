using XuanYu.Core.Gizmo;
using XuanYu.Core.Space;
using XuanYu.Core.Transform;
using XuanYu.Editor.Transform;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly TransformSession _transformSession = new();
    MoveGizmoDragConstraint? _moveDragConstraint;

    public bool TryBeginMoveGizmoCapture(
        long pointerId,
        double x,
        double y,
        ViewportState viewport,
        bool hostValid)
    {
        if (_cameraSession is not null) return false;
        var entity = _sceneState.RenderSnapshot.Entity;
        var sessionTool = ActiveTool;
        if (!entity.IsValid) return false;
        if (!hostValid || !HasSelection ||
            SelectionKey != entity.EntityKey.ToString()) return false;
        if (!EditorTransformCapturePolicy.CanBeginMoveGizmo(_editorState.ToolSnapshot))
        {
            LogTransformCaptureRejected(sessionTool, EditorDisplayText.Entity(entity.EntityKey));
            return false;
        }

        var state = ViewProjectionState.Create(CurrentCamera(viewport.Revision), viewport);
        var layout = MoveGizmoLayout.Project(state, entity.Transform.Position);
        var axis = layout.HitTest(x, y);
        if (axis is null) return false;

        var pointer = new EditorInteractionPointerSnapshot(pointerId, x, y, x, y, 0);
        var start = $"实体={EditorDisplayText.Entity(entity.EntityKey)}；轴={axis}";
        var result = _editorState.Begin(new BeginInteractionCommand(sessionTool, start, pointer));
        if (result is null) return false;
        if (!_transformSession.Begin(result.Snapshot.SessionId, entity, axis.Value))
        {
            _editorState.Cancel(new CancelInteractionCommand(
                result.Snapshot.SessionId, sessionTool, "Transform Session 启动失败"));
            return false;
        }
        _moveDragConstraint = CreateMoveDragConstraint(layout, axis.Value, x, y);
        FooterState = "状态：捕获中";
        FooterMessage = $"移动轴 {axis} 已捕获";
        var detail = $"实体={EditorDisplayText.Entity(entity.EntityKey)}；轴={axis}";
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture,
            "移动工具会话开始",
            $"{detail}；会话={result.Snapshot.SessionId}");
        LogTransformCaptureBegin(
            sessionTool,
            result.Snapshot.OwnerTool,
            EditorDisplayText.Entity(entity.EntityKey),
            axis.Value);
        RefreshLogBindings();
        RaiseInteractionChanged();
        return true;
    }

    bool PreviewMoveGizmo(long sessionId, double x, double y)
    {
        if (!_transformSession.IsActive || _moveDragConstraint is not { } constraint) return false;
        var position = constraint.Solve(_transformSession.StartSnapshot.Transform.Position, x, y);
        if (!_transformSession.TryPreview(sessionId, position)) return false;
        PublishSceneRenderSnapshot();
        return true;
    }

    static MoveGizmoDragConstraint CreateMoveDragConstraint(
        MoveGizmoLayout layout,
        MoveGizmoAxis axis,
        double x,
        double y)
    {
        if (axis is MoveGizmoAxis.X or MoveGizmoAxis.Y or MoveGizmoAxis.Z)
            return new MoveGizmoDragConstraint(layout.Segments.Single(s => s.Axis == axis), x, y);
        var a = axis is MoveGizmoAxis.YZ ? MoveGizmoAxis.Y : MoveGizmoAxis.X;
        var b = axis is MoveGizmoAxis.XY ? MoveGizmoAxis.Y : MoveGizmoAxis.Z;
        return MoveGizmoDragConstraint.Plane(
            axis,
            layout.Segments.Single(s => s.Axis == a),
            layout.Segments.Single(s => s.Axis == b),
            x,
            y);
    }
}
