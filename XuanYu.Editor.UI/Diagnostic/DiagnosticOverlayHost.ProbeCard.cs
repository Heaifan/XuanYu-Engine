using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    void ShowPreviewCard()
    {
        _probePopup = new Popup { Child = _probeCard, PlacementTarget = this,
            Placement = PlacementMode.BottomEdgeAlignedLeft, HorizontalOffset = CardLeft,
            VerticalOffset = CardTop, ShouldUseOverlayLayer = true, IsLightDismissEnabled = false };
        PopupOwner.Children.Add(_probePopup); _probePopup.IsOpen = true;
    }

    void ShowLockedCard()
    {
        _overlayLayer = OverlayLayer.GetOverlayLayer(this);
        if (_overlayLayer is null || _probeCard is null) return;
        _probeCard.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
        _probeCard.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
        _probeCard.Margin = new Thickness(CardLeft, CardTop, 0, 0);
        _overlayLayer.Children.Add(_probeCard);
    }

    void OpenDetails(DiagnosticElementSnapshot snapshot, Control? target)
    {
        if (_probeCard is null) return;
        var panel = new DiagnosticDetailPanel(snapshot,
            text => target is null ? Task.CompletedTask : _clipboard.SetTextAsync(target, text),
            () => { UnlockProbe(); SetProbeResult(null); });
        AttachCardDrag(panel); _probeCard.Child = panel;
    }

    void ClearProbeVisuals()
    {
        ProbeOwner.Children.Clear();
        if (_probePopup is not null) { _probePopup.IsOpen = false; PopupOwner.Children.Remove(_probePopup); }
        if (_overlayLayer is not null && _probeCard is not null)
            _overlayLayer.Children.Remove(_probeCard);
        _probePopup = null; _probeHighlight = null; _probeCard = null;
        _overlayLayer = null;
    }
}
