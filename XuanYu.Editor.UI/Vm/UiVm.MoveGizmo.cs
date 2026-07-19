using XuanYu.Core.Gizmo;
using XuanYu.Core.Space;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public bool TryBeginMoveGizmoCapture(long pointerId, double x, double y, ViewportState viewport, bool hostValid)
    {
        var entity = _sceneState.RenderSnapshot.Entity;
        if (!hostValid || !HasSelection || SelectionKey != entity.EntityKey.ToString()) return false;
        var state = ViewProjectionState.Create(DefaultEditorCamera.Create(viewport.Revision), viewport);
        var axis = MoveGizmoLayout.Project(state, entity.Transform.Position).HitTest(x, y);
        if (axis is null) return false;

        var pointer = new EditorInteractionPointerSnapshot(pointerId, x, y, x, y, 0);
        var start = $"Entity={entity.EntityKey}; Axis={axis}";
        var result = _editorState.Begin(new BeginInteractionCommand("移动", start, pointer));
        if (result is null) return false;
        FooterState = "状态：捕获中";
        FooterMessage = $"移动轴 {axis} 已捕获";
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture,
            "【ARCH-C-R4】移动工具捕获开始",
            $"实体={entity.EntityKey}; 轴={axis}; Session={result.Snapshot.SessionId}");
        RefreshLogBindings();
        RaiseInteractionChanged();
        return true;
    }
}
