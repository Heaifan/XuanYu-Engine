namespace XuanYu.Editor.UI;

internal enum DiagnosticNativeViewportPhase
{
    Entered,
    Moved,
    Exited
}

internal sealed record DiagnosticNativeViewportEvent(
    VulkanNativeHost Host,
    DiagnosticNativeViewportPhase Phase,
    double X,
    double Y);
