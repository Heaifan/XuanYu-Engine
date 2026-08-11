using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace XuanYu.Editor.UI;

public partial class DatasetLayerPanel
{
    Control? _dragHandle;
    IPointer? _dragPointer;
    string? _dragId;
    Point _dragStart;
    int? _dragTarget;
    bool _dragging;

    void DragHandle_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control { DataContext: MapDatasetRow row } handle ||
            e.GetCurrentPoint(handle).Properties.PointerUpdateKind != PointerUpdateKind.LeftButtonPressed) return;
        _dragHandle = handle; _dragPointer = e.Pointer; _dragId = row.Id; _dragStart = e.GetPosition(handle);
        _dragTarget = null; _dragging = false; e.Pointer.Capture(handle);
        handle.PointerMoved += DragHandle_PointerMoved;
        handle.PointerReleased += DragHandle_PointerReleased;
        handle.PointerCaptureLost += DragHandle_PointerCaptureLost;
        e.Handled = true;
    }

    void DragHandle_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (sender is not Control handle) return;
        var point = e.GetPosition(handle);
        if (!_dragging && Math.Pow(point.X - _dragStart.X, 2) + Math.Pow(point.Y - _dragStart.Y, 2) >= 16)
        {
            _dragging = true;
            if (DataContext is UiVm dragVm) dragVm.SetDatasetLayerDragging(_dragId);
        }
        if (!_dragging || DataContext is not UiVm vm) return;
        _dragTarget = TargetAt(e.GetPosition(DatasetLayerList));
        vm.SetDatasetLayerDropTarget(_dragTarget);
    }

    async void DragHandle_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_dragging && _dragId is { } id && _dragTarget is { } target && DataContext is UiVm vm)
            await vm.ReorderDatasetLayerAsync(id, target);
        EndDatasetDrag();
    }

    void DragHandle_PointerCaptureLost(object? sender, PointerCaptureLostEventArgs e) => EndDatasetDrag();

    void EndDatasetDrag()
    {
        if (_dragHandle is { } handle)
        {
            handle.PointerMoved -= DragHandle_PointerMoved; handle.PointerReleased -= DragHandle_PointerReleased;
            handle.PointerCaptureLost -= DragHandle_PointerCaptureLost;
        }
        if (DataContext is UiVm vm) { vm.SetDatasetLayerDropTarget(null); vm.SetDatasetLayerDragging(null); }
        _dragPointer?.Capture(null); _dragHandle = null; _dragPointer = null; _dragId = null; _dragTarget = null; _dragging = false;
    }

    int? TargetAt(Point position)
    {
        var items = new List<(int Index, double Center)>();
        for (var i = 0; i < DatasetLayerList.ItemCount; i++)
        {
            var item = DatasetLayerList.ContainerFromIndex(i);
            if (item?.TranslatePoint(new Point(), DatasetLayerList) is { } origin)
                items.Add((i, origin.Y + item.Bounds.Height / 2));
        }
        foreach (var item in items) if (position.Y <= item.Center) return item.Index;
        return items.Count == 0 ? null : items[^1].Index;
    }
}
