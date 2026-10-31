namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    UiVm? _mapContextVm;

    void OnMapContextDataContextChanged(object? sender, EventArgs e)
    {
        if (_mapContextVm is not null) _mapContextVm.MapGeometryContextRequested -= OnMapGeometryContextRequested;
        _mapContextVm = DataContext as UiVm;
        if (_mapContextVm is not null) _mapContextVm.MapGeometryContextRequested += OnMapGeometryContextRequested;
    }

    void OnMapGeometryContextRequested(MapGeometryContextRequest request)
    {
        if (DataContext is UiVm vm) ShowMapGeometryContextMenu(vm, request.Hit, request.X, request.Y);
    }
}
