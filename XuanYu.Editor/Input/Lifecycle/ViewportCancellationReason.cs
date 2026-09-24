namespace XuanYu.Editor.Input.Lifecycle;

public enum ViewportCancellationReason
{
    Escape,
    CaptureLost,
    FocusLost,
    WindowDeactivated,
    ToolChanged,
    ModeChanged,
    ViewportDisposed,
    PlatformInterrupted,
    ExplicitCancel,
}
