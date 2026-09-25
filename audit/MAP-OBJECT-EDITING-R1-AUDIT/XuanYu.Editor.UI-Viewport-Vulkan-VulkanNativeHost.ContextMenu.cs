using XYUI.Avalonia.Controls;
using XuanYu.Editor.MapEditing;

namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    XYContextMenu? _mapContextMenu;
    MapGeometryContextHit _mapContextHit;
    double _mapContextX;
    double _mapContextY;

    void ShowMapGeometryContextMenu(UiVm vm, MapGeometryContextHit hit, double x, double y)
    {
        _mapContextHit = hit; _mapContextX = x; _mapContextY = y;
        _mapContextMenu ??= new XYContextMenu { ContextType = "地图对象" };
        _mapContextMenu.ContextName = hit.Selection.Kind switch
        {
            MapGeometryFeatureKind.Region => "区域",
            MapGeometryFeatureKind.Road => "道路",
            MapGeometryFeatureKind.Marker => "标记",
            _ => "地图对象"
        };
        var items = MapGeometryContextMenuSpec.Build(vm.MapSession.CurrentMap, hit)
            .Select(item => MenuItem(vm, item, hit)).ToArray();
        _mapContextMenu.Menu = new XYMenu(items);
        _mapContextMenu.Open(this);
    }

    XYMenuItem MenuItem(UiVm vm, MapGeometryContextMenuItem item, MapGeometryContextHit hit) => new()
    {
        Id = item.Id, Label = item.Label, IsEnabled = item.IsEnabled,
        Action = () => vm.ExecuteMapGeometryContext(item.Id, hit, _mapContextX, _mapContextY, vm.CurrentViewport)
    };
}
