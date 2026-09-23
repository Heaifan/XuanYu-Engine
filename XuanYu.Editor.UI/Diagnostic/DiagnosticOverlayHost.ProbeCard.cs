using Avalonia;
using Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    void OpenDetails(DiagnosticElementSnapshot snapshot, Control? target)
    {
        if (_probeCard is null) return;
        var panel = new DiagnosticDetailPanel(snapshot,
            text => target is null ? Task.CompletedTask : _clipboard.SetTextAsync(target, text),
            () => { UnlockProbe(); SetProbeResult(null); });
        AttachCardDrag(panel); _probeCard.Child = panel; ClampCardToWindow();
    }

    void ClearProbeVisuals()
    {
        _previewTargetBounds = null;
        ProbeOwner.Children.Clear();
        if (_floatingLayer is not null)
        {
            if (_probeHighlight is not null) _floatingLayer.Children.Remove(_probeHighlight);
            if (_probeCard is not null) _floatingLayer.Children.Remove(_probeCard);
        }
        _probeHighlight = null; _probeCard = null;
    }
}
