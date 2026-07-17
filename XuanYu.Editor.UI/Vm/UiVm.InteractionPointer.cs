using System.Collections.Generic;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public bool BeginViewportPointer(long pointerId, double x, double y, bool inViewport, bool hostValid)
    {
        if (!inViewport || !hostValid || !IsMoveTool) return false;
        var pointer = new EditorInteractionPointerSnapshot(pointerId, x, y, x, y, 0);
        var result = _editorState.Begin(new BeginInteractionCommand(ActiveTool, SelectionTitle, pointer));
        if (result is null) return false;
        FooterState = "状态：捕获中";
        FooterMessage = $"视口拖动开始：{pointer.Summary}";
        LogInteraction("开始捕获", $"Session={result.Snapshot.SessionId}；{pointer.Summary}");
        RaiseInteractionChanged();
        return true;
    }

    public bool PreviewViewportPointer(long pointerId, double x, double y)
    {
        var snap = _editorState.InteractionSnapshot;
        if (!snap.HasCapture || snap.Pointer.PointerId != pointerId) return false;
        var pointer = snap.Pointer.MoveTo(pointerId, x, y);
        var result = _editorState.Preview(new PreviewInteractionCommand(
            snap.SessionId, snap.OwnerTool, pointer.Summary, pointer));
        if (result is null) return false;
        FooterMessage = $"视口预览：{pointer.Summary}";
        RaiseInteractionChanged();
        return true;
    }

    public bool CommitViewportPointer(long pointerId, double x, double y)
    {
        if (!PreviewViewportPointer(pointerId, x, y)) return false;
        CommitInteraction();
        return true;
    }

    public bool HasInteractionCaptureForPointer(long pointerId)
    {
        var snap = _editorState.InteractionSnapshot;
        return snap.HasCapture && snap.Pointer.PointerId == pointerId;
    }

    IReadOnlyList<string> BuildDebugInputItems()
    {
        var snap = _editorState.InteractionSnapshot;
        return
        [
            $"PointerId：{(snap.Pointer.IsEmpty ? "无" : snap.Pointer.PointerId)}",
            $"起点：{snap.Pointer.StartX:F0}, {snap.Pointer.StartY:F0}",
            $"当前：{snap.Pointer.CurrentX:F0}, {snap.Pointer.CurrentY:F0}",
            $"位移：{snap.Pointer.DeltaX:F0}, {snap.Pointer.DeltaY:F0}",
            $"Preview次数：{snap.Pointer.PreviewCount}"
        ];
    }
}
