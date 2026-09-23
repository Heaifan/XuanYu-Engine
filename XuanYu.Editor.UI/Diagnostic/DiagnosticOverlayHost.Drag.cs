using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

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
        if (e.Source is Visual source && source.GetVisualAncestors().Any(x => x is Button)) return;
        if (sender is not Control surface || !e.GetCurrentPoint(surface).Properties.IsLeftButtonPressed) return;
        _dragging = true; _lastDragPoint = e.GetPosition(surface); e.Pointer.Capture(surface); e.Handled = true;
    }

    void CardPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_dragging || sender is not Control surface) return;
        var point = e.GetPosition(surface); MoveCard(point - _lastDragPoint); _lastDragPoint = point; e.Handled = true;
    }

    void CardPointerReleased(object? sender, PointerReleasedEventArgs e) => EndCardDrag(e.Pointer);
    void CardPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e) => EndCardDrag(null);

    void EndCardDrag(IPointer? pointer)
    {
        if (!_dragging) return; _dragging = false; pointer?.Capture(null);
    }

    void MoveCard(Vector delta)
    {
        CardLeft += delta.X; CardTop += delta.Y; ClampCardToWindow();
    }

    void ClampCardToWindow()
    {
        if (_floatingLayer is null || _probeCard is null) return;
        var available = new Size(Math.Max(0, _floatingLayer.Bounds.Width - 24),
            Math.Max(0, _floatingLayer.Bounds.Height - 24));
        _probeCard.MaxWidth = available.Width; _probeCard.MaxHeight = available.Height;
        var size = _probeCard.Bounds.Size;
        var width = Math.Min(available.Width, size.Width > 0 ? size.Width : _probeCard.DesiredSize.Width);
        var height = Math.Min(available.Height, size.Height > 0 ? size.Height : _probeCard.DesiredSize.Height);
        var maxX = Math.Max(12, _floatingLayer.Bounds.Width - width - 12);
        var maxY = Math.Max(12, _floatingLayer.Bounds.Height - height - 12);
        CardLeft = Math.Clamp(CardLeft, 12, maxX); CardTop = Math.Clamp(CardTop, 12, maxY);
        Canvas.SetLeft(_probeCard, CardLeft); Canvas.SetTop(_probeCard, CardTop);
    }

    void PlacePreviewCard(Rect target)
    {
        if (_floatingLayer is null || _probeCard is null) return;
        var size = _probeCard.DesiredSize; var gap = 12d;
        var candidates = new[]
        {
            new Rect(target.Right + gap, target.Top, size.Width, size.Height),
            new Rect(target.Left - gap - size.Width, target.Top, size.Width, size.Height),
            new Rect(target.Left, target.Bottom + gap, size.Width, size.Height),
            new Rect(target.Left, target.Top - gap - size.Height, size.Width, size.Height)
        };
        var area = new Rect(12, 12, Math.Max(0, _floatingLayer.Bounds.Width - 24),
            Math.Max(0, _floatingLayer.Bounds.Height - 24));
        var choice = candidates.FirstOrDefault(x => area.Contains(x.Position) && area.Contains(x.BottomRight) && !x.Intersects(target));
        if (choice == default) choice = candidates[0];
        CardLeft = choice.X; CardTop = choice.Y;
    }
}
