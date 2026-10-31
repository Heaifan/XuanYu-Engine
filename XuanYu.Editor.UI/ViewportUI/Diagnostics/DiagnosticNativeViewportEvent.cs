namespace XuanYu.Editor.UI;

internal enum DiagnosticNativeViewportPhase
{
    Entered,
    Moved,
    Exited,
    Clicked
}

internal sealed record DiagnosticNativeViewportEvent(
    VulkanNativeHost Host,
    DiagnosticNativeViewportPhase Phase,
    double X,
    double Y);
