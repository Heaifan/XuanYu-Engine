using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

public partial class DatasetLayerPanel
{
    Control? _dragHandle;
    IPointer? _dragPointer;
    string? _dragId;
    Point _dragStart;
    int? _dragTarget;
    bool _dragging;
    Border? _dragRow;
    Border? _dropRow;

    void DragHandle_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control { DataContext: MapDatasetRow row } handle ||
            e.GetCurrentPoint(handle).Properties.PointerUpdateKind != PointerUpdateKind.LeftButtonPressed) return;
        _dragHandle = handle; _dragPointer = e.Pointer; _dragId = row.Id; _dragStart = e.GetPosition(handle);
        _dragTarget = null; _dragging = false; _dragRow = VisualRow(handle); e.Pointer.Capture(handle);
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
            if (_dragRow is not null) _dragRow.Opacity = 0.55;
        }
        if (!_dragging || DataContext is not UiVm vm) return;
        _dragTarget = TargetAt(e.GetPosition(DatasetLayerList));
        SetDropRow(_dragTarget);
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
        if (_dragRow is not null) _dragRow.Opacity = 1.0;
        SetDropRow(null);
        _dragPointer?.Capture(null); _dragHandle = null; _dragPointer = null; _dragId = null; _dragTarget = null; _dragging = false;
        _dragRow = null;
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

    void SetDropRow(int? target)
    {
        if (_dropRow is not null) DropLine(_dropRow).IsVisible = false;
        _dropRow = target is int index ? RowAt(index) : null;
        if (_dropRow is not null) DropLine(_dropRow).IsVisible = true;
    }

    Border? RowAt(int index) => DatasetLayerList.ContainerFromIndex(index) is Control item
        ? item.GetVisualDescendants().OfType<Border>().FirstOrDefault(row => row.Name == "DatasetLayerRow") : null;

    static Border VisualRow(Control control) => control.GetVisualAncestors().OfType<Border>()
        .First(row => row.Name == "DatasetLayerRow");

    static Border DropLine(Border row) => row.GetVisualDescendants().OfType<Border>()
        .First(line => line.Name == "DropLine");
}
