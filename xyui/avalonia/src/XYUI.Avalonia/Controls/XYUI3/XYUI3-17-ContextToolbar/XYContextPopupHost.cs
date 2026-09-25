using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;

namespace XYUI.Avalonia.Controls;

internal sealed class XYContextPopupHost
{
    readonly Canvas _surface = new() { ClipToBounds = false };
    readonly Popup _popup;
    Control? _anchor;
    Border? _root;
    IReadOnlyList<Border> _children = [];

    internal XYContextPopupHost()
    {
        _popup = new Popup
        {
            Placement = PlacementMode.BottomEdgeAlignedLeft,
            IsLightDismissEnabled = true,
            ShouldUseOverlayLayer = false,
            PlacementConstraintAdjustment = PopupPositionerConstraintAdjustment.SlideX | PopupPositionerConstraintAdjustment.FlipY,
            Child = _surface
        };
    }

    internal Popup Popup => _popup;
    internal Canvas Surface => _surface;
    internal bool IsOpen => _popup.IsOpen;
    internal void Attach(Control anchor, Border root, IReadOnlyList<Border> children)
    {
        _anchor = anchor; _root = root; _children = children;
        _surface.Children.Clear(); _surface.Children.Add(root);
        foreach (var child in children) _surface.Children.Add(child);
        _popup.PlacementTarget = anchor;
    }

    internal void Open()
    {
        if (_anchor is null) return;
        _popup.PlacementTarget = _anchor; _popup.IsVisible = true; _popup.IsOpen = true; RefreshBounds();
    }

    internal void Close()
    {
        _popup.IsOpen = false; _popup.IsVisible = false; _anchor = null;
    }

    internal void RefreshPlacement() { if (_anchor is not null) _popup.PlacementTarget = _anchor; }

    internal void RefreshBounds()
    {
        if (_root is null) return;
        var infinite = new Size(double.PositiveInfinity, double.PositiveInfinity);
        _root.Measure(infinite);
        foreach (var child in _children) child.Measure(infinite);
        var visible = _children.Where(x => x.IsVisible).ToArray();
        var rootSize = _root.DesiredSize;
        var width = rootSize.Width + (visible.Length == 0 ? 0 : 132 + visible.Max(x => x.DesiredSize.Width));
        var height = Math.Max(rootSize.Height, visible.Length == 0 ? 0 : visible.Max(x => (double.IsNaN(Canvas.GetTop(x)) ? 0 : Canvas.GetTop(x)) + x.DesiredSize.Height));
        var nextWidth = Math.Max(128, width); var nextHeight = Math.Max(32, height);
        if (double.IsNaN(_surface.Width) || double.IsNaN(_surface.Height) || Math.Abs(_surface.Width - nextWidth) > 0.1 || Math.Abs(_surface.Height - nextHeight) > 0.1)
        { _surface.Width = nextWidth; _surface.Height = nextHeight; var bounds = new Rect(new Size(nextWidth, nextHeight)); _surface.Measure(bounds.Size); _surface.Arrange(bounds); }
        Canvas.SetLeft(_root, 0); Canvas.SetTop(_root, 0);
    }

    internal void SetChildPosition(Border child, double x, double y) { Canvas.SetLeft(child, x); Canvas.SetTop(child, y); }
}
