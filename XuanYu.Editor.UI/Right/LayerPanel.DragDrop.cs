using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using XuanYu.World.Map;
namespace XuanYu.Editor.UI;
// MAP-A-R2-D4-F3：区域图层拖动（code-behind 只处理指针/Drop；手柄按下 ≥4 DIP 启动；仅区域行接受；一次交给 UiVm）。
public partial class LayerPanel
{
    MapLayerRowViewModel? _dragCandidate;
    PointerPressedEventArgs? _dragPressedArgs;
    Point _dragStart;
    void DragHandle_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control { DataContext: MapLayerRowViewModel row } handle || !row.IsRegion) return;
        if (e.GetCurrentPoint(handle).Properties.PointerUpdateKind != PointerUpdateKind.LeftButtonPressed) return;
        _dragCandidate = row; _dragPressedArgs = e; _dragStart = e.GetPosition(handle);
        handle.PointerMoved += DragCandidate_PointerMoved;
        handle.PointerReleased += DragCandidate_PointerReleased;
        e.Handled = true;
    }
    async void DragCandidate_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_dragCandidate is null || _dragPressedArgs is null || sender is not Control handle) return;
        var p = e.GetPosition(handle);
        var dx = p.X - _dragStart.X;
        var dy = p.Y - _dragStart.Y;
        if ((dx * dx) + (dy * dy) < 16.0) return; // 4 DIP 阈值
        handle.PointerMoved -= DragCandidate_PointerMoved;
        handle.PointerReleased -= DragCandidate_PointerReleased;
        var item = new DataTransferItem();
        item.SetText(_dragCandidate.LayerId.Value);
        var data = new DataTransfer();
        data.Add(item);
        _dragCandidate = null;
        try
        {
            await DragDrop.DoDragDropAsync(_dragPressedArgs, data, DragDropEffects.Move);
        }
        finally
        {
            _dragPressedArgs = null;
            if (DataContext is UiVm vm) vm.SetDropTarget(null); // 拖动结束才清理插入线
        }
    }
    void DragCandidate_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is Control handle)
        {
            handle.PointerMoved -= DragCandidate_PointerMoved;
            handle.PointerReleased -= DragCandidate_PointerReleased;
        }
        _dragCandidate = null;
    }
    void LayerList_DragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = DragDropEffects.None;
        e.Handled = true;
        if (DataContext is not UiVm vm || !TryGetDragLayerId(e, out var fromId)) return;
        if (MapLayerId.TryParse(fromId, out var from) && TryGetDropTarget(e, out var targetIndex))
        {
            e.DragEffects = DragDropEffects.Move;
            vm.SetDropTarget(targetIndex);
        }
        else vm.SetDropTarget(null);
    }
    void LayerList_Drop(object? sender, DragEventArgs e)
    {
        e.Handled = true;
        if (DataContext is not UiVm vm) return;
        vm.SetDropTarget(null);
        if (TryGetDragLayerId(e, out var fromId) && TryGetDropTarget(e, out var targetIndex))
            vm.CommitLayerDrag(fromId, targetIndex);
    }
    static bool TryGetDragLayerId(DragEventArgs e, out string layerId)
    {
        layerId = "";
        if (e.DataTransfer is not { } data) return false;
        foreach (var item in data.GetItems(DataFormat.Text))
            if (item.TryGetRaw(DataFormat.Text) is string text && !string.IsNullOrEmpty(text)) { layerId = text; return true; }
        return false;
    }
    // 指针所在行必须是区域图层（系统层/空白不接受）；targetIndex=该区域层位置（插入其前）。
    bool TryGetDropTarget(DragEventArgs e, out int targetIndex)
    {
        targetIndex = -1;
        var pos = e.GetPosition(LayerList);
        for (var i = 0; i < LayerList.ItemCount; i++)
        {
            var container = LayerList.ContainerFromIndex(i);
            if (container is null || !container.Bounds.Contains(pos)) continue;
            if (container.DataContext is not MapLayerRowViewModel { IsRegion: true } row) return false;
            targetIndex = (DataContext as UiVm)?.RegionPositionOf(row.LayerId) ?? -1;
            return targetIndex >= 0;
        }
        return false;
    }
}
