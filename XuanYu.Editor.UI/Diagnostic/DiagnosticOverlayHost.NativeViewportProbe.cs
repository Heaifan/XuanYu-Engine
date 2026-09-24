using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    readonly List<VulkanNativeHost> _nativeViewportProbeHosts = [];
    Point _lastProbePointer;

    void AttachNativeViewportProbe(Window window)
    {
        DetachNativeViewportProbe();
        foreach (var host in window.GetVisualDescendants().OfType<VulkanNativeHost>())
        {
            host.NativePointerMoved += OnNativeViewportPointerMoved;
            _nativeViewportProbeHosts.Add(host);
        }
    }

    void DetachNativeViewportProbe()
    {
        foreach (var host in _nativeViewportProbeHosts)
            host.NativePointerMoved -= OnNativeViewportPointerMoved;
        _nativeViewportProbeHosts.Clear();
    }

    void OnNativeViewportPointerMoved(VulkanNativeHost host, double x, double y)
    {
        if (!ProbeEnabled || _floatingLayer is null) return;
        _lastProbePointer = host.TranslatePoint(new Point(x, y), _floatingLayer) ?? _lastProbePointer;
        var target = host.GetVisualAncestors().OfType<VulkanViewport>().FirstOrDefault() as Visual ?? host;
        if (!TryRepositionViewportProbe(target)) ProbeHover(target, false);
    }

    void RememberProbePointer(PointerEventArgs e)
    {
        if (_floatingLayer is not null) _lastProbePointer = e.GetPosition(_floatingLayer);
    }

    bool TryRepositionViewportProbe(Visual hit)
    {
        if (IsProbeLocked || _previewTargetBounds is not { } target || !IsViewportProbeTarget(hit))
            return false;
        var current = _probeResult?.SemanticTarget ?? _probeResult?.DeepVisual;
        if (!ReferenceEquals(current, hit) && !ReferenceEquals(_probeResult?.DeepVisual, hit))
            return false;
        PlacePreviewCard(target);
        ClampCardToWindow();
        return true;
    }

    static bool IsViewportProbeTarget(Visual visual) =>
        visual is VulkanViewport or VulkanNativeHost;
}
