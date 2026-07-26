namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public void CancelInteractionFromEscape()
    {
        if (_editorState.InteractionSnapshot.HasCapture) CancelInteraction("Escape");
        else CancelCameraNavigation("Escape");
    }

    public void CancelInteractionFromWindowDeactivated() => CancelActiveInput("窗口失焦");

    public void CancelInteractionFromWindowClosing() => CancelActiveInput("窗口关闭");

    public void CancelInteractionFromHostDetach() => CancelActiveInput("NativeHost Detach");

    public void CancelInteractionFromPointerCaptureLost() => CancelActiveInput("PointerCaptureLost");

    public void CancelInteractionFromNativePointer(string reason) => CancelActiveInput(reason);

    void CancelActiveInput(string reason)
    {
        if (_editorState.InteractionSnapshot.HasCapture) CancelInteraction(reason);
        CancelCameraNavigation(reason);
    }
}
