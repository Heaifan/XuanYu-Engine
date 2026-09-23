using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    Popup? _nativeCardPopup;
    Popup? _nativeHighlightPopup;
    void ShowNativeProbeOverlay(Border highlight, Border card, Rect bounds)
    {
        if (_floatingLayer is null) return;
        _nativeCardPopup ??= CreatePopup(); _nativeHighlightPopup ??= CreatePopup();
        _nativeHighlightPopup.IsHitTestVisible = false;
        AddPopup(_nativeCardPopup); AddPopup(_nativeHighlightPopup);
        _nativeCardPopup.Child = card; _nativeHighlightPopup.Child = highlight;
        _nativeCardPopup.PlacementTarget = _floatingLayer;
        _nativeHighlightPopup.PlacementTarget = _floatingLayer;
        var passThrough = _floatingLayer.Parent as Control;
        _nativeCardPopup.OverlayInputPassThroughElement = passThrough;
        _nativeHighlightPopup.OverlayInputPassThroughElement = passThrough;
        _nativeCardPopup.IsOpen = true; _nativeHighlightPopup.IsOpen = true;
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

    void CloseNativeProbeOverlay()
    {
        if (_nativeCardPopup is not null) _nativeCardPopup.IsOpen = false;
        if (_nativeHighlightPopup is not null) _nativeHighlightPopup.IsOpen = false;
        if (_nativeCardPopup?.Parent is Panel) PopupOwner.Children.Remove(_nativeCardPopup);
        if (_nativeHighlightPopup?.Parent is Panel) PopupOwner.Children.Remove(_nativeHighlightPopup);
    }
}
