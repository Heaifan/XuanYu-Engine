using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.Editor.Input;

public sealed partial class ViewportInputRouter
{
    static bool IsGlobalCancel(EditorPointerEventKind kind) => kind is
        EditorPointerEventKind.Escape or EditorPointerEventKind.Cancel or EditorPointerEventKind.CaptureLost
        or EditorPointerEventKind.FocusLost or EditorPointerEventKind.WindowDeactivated
        or EditorPointerEventKind.ToolChanged or EditorPointerEventKind.ModeChanged
        or EditorPointerEventKind.ViewportDisposed;

    ViewportInputDispatchResult Cancel(EditorPointerEventKind kind)
    {
        _lifecycle.Cancel(kind switch
        {
            EditorPointerEventKind.Escape => ViewportCancellationReason.Escape,
            EditorPointerEventKind.CaptureLost => ViewportCancellationReason.CaptureLost,
            EditorPointerEventKind.FocusLost => ViewportCancellationReason.FocusLost,
            EditorPointerEventKind.WindowDeactivated => ViewportCancellationReason.WindowDeactivated,
            EditorPointerEventKind.ToolChanged => ViewportCancellationReason.ToolChanged,
            EditorPointerEventKind.ModeChanged => ViewportCancellationReason.ModeChanged,
            EditorPointerEventKind.ViewportDisposed => ViewportCancellationReason.ViewportDisposed,
            _ => ViewportCancellationReason.ExplicitCancel,
        });
        _activeConsumer.Clear();
        return ViewportInputDispatchResult.Cancelled;
    }
}
