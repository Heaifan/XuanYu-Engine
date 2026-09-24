using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    Popup? _nativeCardPopup;
    Popup? _nativeHighlightPopup;
    void ShowNativeProbeOverlay(Border? highlight, Border card, Rect bounds)
    {
        if (_floatingLayer is null) return;
        _nativeCardPopup ??= CreatePopup(); AddPopup(_nativeCardPopup);
        _nativeCardPopup.Child = card;
        _nativeCardPopup.PlacementTarget = _floatingLayer;
        var passThrough = _floatingLayer.Parent as Control;
        _nativeCardPopup.OverlayInputPassThroughElement = passThrough;
        _nativeCardPopup.IsOpen = true;
        if (highlight is not null)
        {
            _nativeHighlightPopup ??= CreatePopup(); AddPopup(_nativeHighlightPopup);
            _nativeHighlightPopup.IsHitTestVisible = false;
            _nativeHighlightPopup.Child = highlight;
            _nativeHighlightPopup.PlacementTarget = _floatingLayer;
            _nativeHighlightPopup.OverlayInputPassThroughElement = passThrough;
            _nativeHighlightPopup.IsOpen = true;
        }
        if (TopLevel.GetTopLevel(this) is Window window)
            _lastNativeWindowProbe = DiagnosticNativeWindowProbe.Capture(_nativeCardPopup, window);
        PositionNativeProbe(bounds);
    }

    Popup CreatePopup() => new()
    {
        Placement = PlacementMode.AnchorAndGravity,
        PlacementAnchor = PopupAnchor.TopLeft,
        PlacementGravity = PopupGravity.BottomRight,
        PlacementConstraintAdjustment = PopupPositionerConstraintAdjustment.None,
        IsLightDismissEnabled = false,
        TakesFocusFromNativeControl = false,
        ShouldUseOverlayLayer = false,
    };

    void AddPopup(Popup popup)
    { if (popup.Parent is null) PopupOwner.Children.Add(popup); }

    void PositionNativeProbe(Rect target)
    {
        if (_nativeCardPopup is not null)
            _nativeCardPopup.PlacementRect = new Rect(CardLeft, CardTop, 0, 0);
        if (_nativeHighlightPopup is not null)
            _nativeHighlightPopup.PlacementRect = new Rect(target.X, target.Y, 0, 0);
    }

    static bool IsViewportDiagnosticTarget(DiagnosticProbeResult? result) =>
        result?.DebugId == "XYE.VIEWPORT" || result?.DeepVisual is VulkanViewport or VulkanNativeHost;

    void CloseNativeProbeOverlay()
    {
        if (_nativeCardPopup is not null) _nativeCardPopup.IsOpen = false;
        if (_nativeHighlightPopup is not null) _nativeHighlightPopup.IsOpen = false;
        if (_nativeCardPopup?.Parent is Panel) PopupOwner.Children.Remove(_nativeCardPopup);
        if (_nativeHighlightPopup?.Parent is Panel) PopupOwner.Children.Remove(_nativeHighlightPopup);
    }
}
