using XYUI.Avalonia.Controls;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

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
        _mapContextMenu.ContextName = DisplayName(vm.MapSession.CurrentMap, hit);
        var items = MapGeometryContextMenuSpec.Build(vm.MapSession.CurrentMap, hit)
            .Select(item => MenuItem(vm, item, hit)).ToArray();
        _mapContextMenu.Menu = new XYMenu(items);
        _mapContextMenu.OpenAt(this, new Avalonia.Point(x, y));
    }

    static string DisplayName(MapDefinition map, MapGeometryContextHit hit) => hit.Selection.Kind switch
    {
        MapGeometryFeatureKind.Region => map.Regions.FirstOrDefault(x => x.RegionId.ToString() == hit.Selection.FeatureId)?.DisplayName ?? "区域",
        MapGeometryFeatureKind.Road => map.Roads.FirstOrDefault(x => x.RoadId.ToString() == hit.Selection.FeatureId)?.DisplayName ?? "道路",
        MapGeometryFeatureKind.Marker => map.Markers.FirstOrDefault(x => x.MarkerId.ToString() == hit.Selection.FeatureId)?.DisplayName ?? "标记",
        _ => "地图对象"
    };

    XYMenuItem MenuItem(UiVm vm, MapGeometryContextMenuItem item, MapGeometryContextHit hit) => new()
    {
        Id = item.Id, Label = item.Label, IsEnabled = item.IsEnabled,
        Action = () => vm.ExecuteMapGeometryContext(item.Id, hit, _mapContextX, _mapContextY, vm.CurrentViewport)
    };
}
