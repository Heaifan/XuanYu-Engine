using XuanYu.Editor.Input;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public void CancelInteractionFromEscape() =>
        ViewportInput.Terminate(EditorPointerEventKind.Escape);

    public void CancelInteractionFromWindowDeactivated() =>
        ViewportInput.Terminate(EditorPointerEventKind.WindowDeactivated);

    public void CancelInteractionFromWindowClosing() =>
        ViewportInput.Terminate(EditorPointerEventKind.ViewportDisposed);

    public void CancelInteractionFromHostDetach() =>
        ViewportInput.Terminate(EditorPointerEventKind.ViewportDisposed);

    public void CancelInteractionFromPointerCaptureLost() =>
        ViewportInput.Terminate(EditorPointerEventKind.CaptureLost);

    public void CancelInteractionFromNativePointer(string reason) =>
        ViewportInput.Terminate(EditorPointerEventKind.Cancel);

    void CancelActiveInput(string reason) =>
        ViewportInput.Terminate(EditorPointerEventKind.Cancel);
}
