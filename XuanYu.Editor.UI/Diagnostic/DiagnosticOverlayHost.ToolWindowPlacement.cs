using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using XuanYu.Editor.UI.Diagnostic;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    void PlaceToolWindow(Rect target)
    {
        if (_cardPlacementMode != DiagnosticCardPlacementMode.Auto ||
            _toolWindow is null || _floatingLayer is null) return;
        var size = _toolWindow.Bounds.Size;
        if (size.Width <= 0 || size.Height <= 0) size = _toolCard?.DesiredSize ?? new Size(330, 180);
        var area = new Rect(12, 12, Math.Max(0, _floatingLayer.Bounds.Width - 24),
            Math.Max(0, _floatingLayer.Bounds.Height - 24));
        var kind = IsViewportDiagnosticTarget(_probeResult)
            ? DiagnosticPlacementTargetKind.Viewport : DiagnosticPlacementTargetKind.SmallControl;
        var placement = DiagnosticPlacementPolicy.Place(new(kind, target, _lastProbePointer, size, area, 12));
        CardLeft = placement.CardBounds.X; CardTop = placement.CardBounds.Y;
        ApplyToolPosition();
    }

    void ApplyToolPosition()
    {
        if (_toolWindow is null || _floatingLayer is null) return;
        var origin = _floatingLayer.PointToScreen(default);
        var scale = _topLevel?.RenderScaling ?? 1d;
        _toolWindow.Position = new PixelPoint((int)Math.Round(origin.X + CardLeft * scale),
            (int)Math.Round(origin.Y + CardTop * scale));
    }

    void BeginToolDrag()
    {
        _cardPlacementMode = DiagnosticCardPlacementMode.Manual;
    }

    void EndToolDrag() { }

    void OpenToolDetails()
    {
        if (_toolWindow is null) return;
        var result = IsProbeLocked ? _lockedProbeResult : _probeResult;
        if (result is null) return;
        var snapshot = IsProbeLocked ? TrackedSnapshot ?? DiagnosticElementSnapshot.Capture(result) :
            DiagnosticElementSnapshot.Capture(result);
        var panel = new DiagnosticDetailPanel(snapshot, CopyToolText, CloseToolWindow);
        _toolWindow.ReplaceContent(panel);
        _toolWindow.SetDragSurface(panel.DragSurface, BeginToolDrag, EndToolDrag);
    }
}
