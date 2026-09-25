using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    bool TryGetFloatingBounds(Visual visual, out Rect bounds)
    {
        if (TopLevel.GetTopLevel(visual) is null) { bounds = default; return false; }
        if (visual is VulkanNativeHost && _floatingLayer is not null)
        {
            var nativeScale = _topLevel?.RenderScaling ?? 1d;
            var nativeOrigin = visual.PointToScreen(default);
            var nativeLayer = _floatingLayer.PointToScreen(default);
            var nativePoint = DiagnosticNativeCoordinateMapping.ToLayer(
                nativeOrigin, nativeLayer, nativeScale);
            bounds = new Rect(nativePoint, visual.Bounds.Size); return true;
        }
        if (_floatingLayer is not null && visual.TranslatePoint(default, _floatingLayer) is { } local)
        { bounds = new Rect(local, visual.Bounds.Size); return true; }
        if (_floatingLayer is null) { bounds = default; return false; }
        var scale = _topLevel?.RenderScaling ?? 1d;
        var layer = _floatingLayer.PointToScreen(default); var origin = visual.PointToScreen(default);
        var point = new Point((origin.X - layer.X) / scale, (origin.Y - layer.Y) / scale);
        bounds = new Rect(point, visual.Bounds.Size); return true;
    }

    void ClearProbeForRoot(TopLevel root)
    {
        var resultRoot = _probeResult is null ? null : TopLevel.GetTopLevel(_probeResult.DeepVisual);
        var lockedRoot = _lockedProbeResult is null ? null : TopLevel.GetTopLevel(_lockedProbeResult.DeepVisual);
        if (ReferenceEquals(resultRoot, root) && !IsProbeLocked) SetProbeResult(null);
        if (ReferenceEquals(lockedRoot, root)) ClearProbeHighlight();
    }

}
