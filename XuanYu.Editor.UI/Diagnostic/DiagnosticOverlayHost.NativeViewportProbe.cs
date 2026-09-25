using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    bool _nativeViewportProbeAttached;
    Point _lastProbePointer;

    void AttachNativeViewportProbe(Window window)
    {
        if (_nativeViewportProbeAttached) return;
        VulkanNativeHost.NativeViewportDiagnosticChanged += OnNativeViewportDiagnosticChanged;
        _nativeViewportProbeAttached = true;
    }

    void DetachNativeViewportProbe()
    {
        if (!_nativeViewportProbeAttached) return;
        VulkanNativeHost.NativeViewportDiagnosticChanged -= OnNativeViewportDiagnosticChanged;
        _nativeViewportProbeAttached = false;
    }

    void OnNativeViewportDiagnosticChanged(
        VulkanNativeHost host, DiagnosticNativeViewportEvent change)
    {
        if (!ProbeEnabled) return;
        if (change.Phase == DiagnosticNativeViewportPhase.Exited)
        {
            ClearNativeViewportOverride(host);
            return;
        }
        if (_floatingLayer is not null)
        {
            var scale = _topLevel?.RenderScaling ?? 1d;
            var screen = host.PointToScreen(new Point(change.X, change.Y));
            var layer = _floatingLayer.PointToScreen(default);
            _lastProbePointer = DiagnosticNativeCoordinateMapping.ToLayer(screen, layer, scale);
        }
        _nativeViewportHost = host;
        var target = DiagnosticNativeViewportTarget.Create(host);
        if (change.Phase == DiagnosticNativeViewportPhase.Clicked)
        {
            TrackProbe(target); return;
        }
        if (CurrentIsNativeViewport(host)) RepositionNativeViewportProbe(host);
        else SetProbeResult(target);
    }

    void RepositionNativeViewportProbe(VulkanNativeHost host)
    {
        if (!TryGetFloatingBounds(host, out var bounds)) return;
        PlaceToolWindow(bounds);
    }

    bool CurrentIsNativeViewport(VulkanNativeHost host) =>
        ReferenceEquals(_nativeViewportHost, host) &&
        ReferenceEquals(_probeResult?.DeepVisual, host);

    void ClearNativeViewportOverride(VulkanNativeHost host)
    {
        if (!ReferenceEquals(_nativeViewportHost, host) &&
            !ReferenceEquals(_probeResult?.DeepVisual, host)) return;
        _nativeViewportHost = null;
        if (!IsProbeLocked && _probeResult?.DeepVisual is VulkanNativeHost)
            SetProbeResult(null);
    }

    bool HasNativeViewportOverride => _nativeViewportHost is not null ||
        _probeResult?.DeepVisual is VulkanNativeHost;

    void RememberProbePointer(PointerEventArgs e)
    {
        if (_floatingLayer is not null) _lastProbePointer = e.GetPosition(_floatingLayer);
    }

    bool TryRepositionViewportProbe(Visual hit) => false;

    static bool IsViewportProbeTarget(Visual visual) =>
        visual is VulkanViewport or VulkanNativeHost;
}
