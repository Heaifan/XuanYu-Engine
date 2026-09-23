using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    Popup? _nativeCardPopup;
    Popup? _nativeHighlightPopup;
    Border? _cardAnchor;
    Border? _highlightAnchor;

    void ShowNativeProbeOverlay(Border highlight, Border card, Rect bounds)
    {
        if (_floatingLayer is null) return;
        _cardAnchor ??= CreateAnchor(); _highlightAnchor ??= CreateAnchor();
        AddAnchor(_cardAnchor); AddAnchor(_highlightAnchor);
        _nativeCardPopup ??= CreatePopup(); _nativeHighlightPopup ??= CreatePopup();
        AddPopup(_nativeCardPopup); AddPopup(_nativeHighlightPopup);
        _nativeCardPopup.Child = card; _nativeHighlightPopup.Child = highlight;
        _nativeCardPopup.PlacementTarget = _cardAnchor;
        _nativeHighlightPopup.PlacementTarget = _highlightAnchor;
        _nativeCardPopup.IsOpen = true; _nativeHighlightPopup.IsOpen = true;
        PositionNativeProbe(bounds);
    }

    Popup CreatePopup() => new()
    {
        Placement = PlacementMode.TopEdgeAlignedLeft,
        IsLightDismissEnabled = false,
        TakesFocusFromNativeControl = false,
        ShouldUseOverlayLayer = false,
    };

    void AddPopup(Popup popup)
    { if (popup.Parent is null) PopupOwner.Children.Add(popup); }

    Border CreateAnchor() => new() { Width = 1, Height = 1, IsHitTestVisible = false };

    void AddAnchor(Border anchor)
    {
        if (anchor.Parent is null) _floatingLayer?.Children.Add(anchor);
    }

    void PositionNativeProbe(Rect target)
    {
        if (_cardAnchor is not null) { Canvas.SetLeft(_cardAnchor, CardLeft); Canvas.SetTop(_cardAnchor, CardTop); }
        if (_highlightAnchor is not null)
        { Canvas.SetLeft(_highlightAnchor, target.X); Canvas.SetTop(_highlightAnchor, target.Y); }
    }

    void CloseNativeProbeOverlay()
    {
        if (_nativeCardPopup is not null) _nativeCardPopup.IsOpen = false;
        if (_nativeHighlightPopup is not null) _nativeHighlightPopup.IsOpen = false;
        if (_nativeCardPopup?.Parent is Panel) PopupOwner.Children.Remove(_nativeCardPopup);
        if (_nativeHighlightPopup?.Parent is Panel) PopupOwner.Children.Remove(_nativeHighlightPopup);
        if (_cardAnchor is not null) _floatingLayer?.Children.Remove(_cardAnchor);
        if (_highlightAnchor is not null) _floatingLayer?.Children.Remove(_highlightAnchor);
    }
}
