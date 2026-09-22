using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    double CardLeft { get; set; } = 12;
    double CardTop { get; set; } = 12;
    Point _lastDragPoint;
    bool _dragging;

    void AttachCardDrag(Control card)
    {
        var surface = card switch
        {
            DiagnosticFloatingCard floating => floating.DragSurface,
            DiagnosticDetailPanel detail => detail.DragSurface,
            _ => card
        };
        surface.PointerPressed += CardPointerPressed;
        surface.PointerMoved += CardPointerMoved;
        surface.PointerReleased += CardPointerReleased;
        surface.PointerCaptureLost += CardPointerCaptureLost;
    }

    void CardPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Source is Button) return;
        if (e.Source is not Control card || !e.GetCurrentPoint(card).Properties.IsLeftButtonPressed) return;
        _dragging = true; _lastDragPoint = e.GetPosition(card); e.Pointer.Capture(card); e.Handled = true;
    }

    void CardPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_dragging || e.Source is not Control card) return;
        var point = e.GetPosition(card); MoveCard(point - _lastDragPoint); _lastDragPoint = point; e.Handled = true;
    }

    void CardPointerReleased(object? sender, PointerReleasedEventArgs e) => EndCardDrag(e.Pointer);
    void CardPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e) => EndCardDrag(null);

    void EndCardDrag(IPointer? pointer)
    {
        if (!_dragging) return; _dragging = false; pointer?.Capture(null);
    }

    void MoveCard(Vector delta)
    {
        CardLeft = Math.Clamp(CardLeft + delta.X, 0, Math.Max(0, Bounds.Width - 300));
        CardTop = Math.Clamp(CardTop + delta.Y, 0, Math.Max(0, Bounds.Height - 180));
        if (_probePopup is not null)
        {
            _probePopup.HorizontalOffset = CardLeft;
            _probePopup.VerticalOffset = CardTop;
        }
        if (_probeCard?.Parent is OverlayLayer)
            _probeCard.Margin = new Thickness(CardLeft, CardTop, 0, 0);
    }
}
