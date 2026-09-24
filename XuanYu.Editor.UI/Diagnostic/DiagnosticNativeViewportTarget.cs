using Avalonia;
using Avalonia.Controls;

namespace XuanYu.Editor.UI;

internal static class DiagnosticNativeViewportTarget
{
    public static DiagnosticProbeResult Create(VulkanNativeHost host) => new(
        host, host, "XYE.VIEWPORT", "N/A", "VulkanViewport",
        host.Name ?? "N/A", "N/A", "XYE.VIEWPORT",
        host.IsEffectivelyVisible, host.IsEffectivelyEnabled,
        new Rect(default, host.Bounds.Size), DiagnosticProbeMode.Semantic);
}
