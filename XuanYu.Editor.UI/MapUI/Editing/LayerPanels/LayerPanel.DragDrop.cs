using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

// F2：仅六点手柄捕获 Pointer；不进入全局 DragDrop，不替换 ItemsSource。
public partial class LayerPanel
{
    MapLayerRowViewModel? _dragRow;
    Control? _dragHandle;
    IPointer? _dragPointer;
    Point _dragStart;
    int _dragSourceIndex = -1;
    int? _dragTarget;
    bool _dragging;

    void DragHandle_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control { DataContext: MapLayerRowViewModel row } handle || !row.IsDragEnabled) return;
        if (e.GetCurrentPoint(handle).Properties.PointerUpdateKind != PointerUpdateKind.LeftButtonPressed) return;
        _dragRow = row; _dragHandle = handle; _dragPointer = e.Pointer;
        _dragStart = e.GetPosition(handle); _dragSourceIndex = (DataContext as UiVm)?.RegionPositionOf(row.LayerId) ?? -1;
        if (_dragSourceIndex < 0) return;
        _dragTarget = null; _dragging = false;
        e.Pointer.Capture(handle); handle.PointerMoved += DragHandle_PointerMoved;
        handle.PointerReleased += DragHandle_PointerReleased; handle.PointerCaptureLost += DragHandle_PointerCaptureLost;
        e.Handled = true;
    }

    void DragHandle_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_dragging && _dragRow is not null && sender is Control handle)
        {
            var p = e.GetPosition(handle); var dx = p.X - _dragStart.X; var dy = p.Y - _dragStart.Y;
            if ((dx * dx) + (dy * dy) < 16.0) return;
            _dragging = true;
        }
        if (!_dragging || _dragSourceIndex < 0 || DataContext is not UiVm vm) return;
        _dragTarget = TargetAt(e.GetPosition(LayerList)); vm.SetDropTarget(_dragTarget);
    }

    void DragHandle_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_dragging && _dragRow is { } row && _dragTarget is { } target && DataContext is UiVm vm)
            vm.CommitLayerDrag(row.LayerId.Value, target);
        EndLayerDrag();
    }

    void DragHandle_PointerCaptureLost(object? sender, PointerCaptureLostEventArgs e) => EndLayerDrag();

    void LayerPanel_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Escape || !_dragging) return;
        EndLayerDrag(); e.Handled = true;
    }

    void EndLayerDrag()
    {
        if (_dragHandle is { } handle)
        {
            handle.PointerMoved -= DragHandle_PointerMoved;
            handle.PointerReleased -= DragHandle_PointerReleased;
            handle.PointerCaptureLost -= DragHandle_PointerCaptureLost;
        }
        if (DataContext is UiVm vm) vm.SetDropTarget(null);
        if (_dragPointer is { } pointer) pointer.Capture(null);
        _dragRow = null; _dragHandle = null; _dragPointer = null;
        _dragTarget = null; _dragSourceIndex = -1; _dragging = false;
    }

    int? TargetAt(Point position)
    {
        var candidates = new List<(int Index, double Center)>();
        for (var i = 0; i < LayerList.ItemCount; i++)
        {
            var container = LayerList.ContainerFromIndex(i);
            if (container is null || container.DataContext is not MapLayerRowViewModel { IsRegion: true } row) continue;
            if (container.TranslatePoint(new Point(), LayerList) is not { } origin) continue;
            candidates.Add(((DataContext as UiVm)?.RegionPositionOf(row.LayerId) ?? -1,
                origin.Y + container.Bounds.Height / 2));
        }
        foreach (var candidate in candidates.OrderBy(c => c.Center))
            if (position.Y <= candidate.Center) return candidate.Index;
        return candidates.Count == 0 ? null : candidates[^1].Index;
    }
}
