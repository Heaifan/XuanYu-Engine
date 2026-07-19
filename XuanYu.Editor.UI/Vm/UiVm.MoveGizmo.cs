using XuanYu.Core.Gizmo;
using XuanYu.Core.Space;
using XuanYu.Core.Transform;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly TransformSession _transformSession = new();
    MoveGizmoDragConstraint? _moveDragConstraint;

    public bool TryBeginMoveGizmoCapture(long pointerId, double x, double y, ViewportState viewport, bool hostValid)
    {
        var entity = _sceneState.RenderSnapshot.Entity;
        if (!hostValid || !HasSelection || SelectionKey != entity.EntityKey.ToString()) return false;
        var state = ViewProjectionState.Create(DefaultEditorCamera.Create(viewport.Revision), viewport);
        var layout = MoveGizmoLayout.Project(state, entity.Transform.Position);
        var axis = layout.HitTest(x, y) ?? layout.GuardHitTest(x, y);
        if (axis is null) return false;

        var pointer = new EditorInteractionPointerSnapshot(pointerId, x, y, x, y, 0);
        var start = $"Entity={entity.EntityKey}; Axis={axis}";
        var result = _editorState.Begin(new BeginInteractionCommand("移动", start, pointer));
        if (result is null) return false;
        if (!_transformSession.Begin(result.Snapshot.SessionId, entity, axis.Value))
        {
            _editorState.Cancel(new CancelInteractionCommand(result.Snapshot.SessionId, "移动", "Transform Session 启动失败"));
            return false;
        }
        var segment = layout.Segments.Single(item => item.Axis == axis.Value);
        _moveDragConstraint = new MoveGizmoDragConstraint(segment, x, y);
        FooterState = "状态：捕获中";
        FooterMessage = $"移动轴 {axis} 已捕获";
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture,
            "【ARCH-C-R5】移动工具会话开始",
            $"实体={entity.EntityKey}; 轴={axis}; Session={result.Snapshot.SessionId}");
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
}
