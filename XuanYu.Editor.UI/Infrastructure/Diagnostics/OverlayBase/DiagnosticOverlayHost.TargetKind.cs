namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    static bool IsViewportDiagnosticTarget(DiagnosticProbeResult? result) =>
        result?.DebugId == "XYE.VIEWPORT" || result?.DeepVisual is VulkanViewport or VulkanNativeHost;
}
