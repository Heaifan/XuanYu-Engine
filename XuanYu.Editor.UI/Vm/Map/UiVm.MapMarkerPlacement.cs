using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public bool MarkerPlacementPointerPressed(double x, double y, ViewportState viewport)
    {
        if (!IsMarkerPlacementTool || !TryPickRegionPoint(x, y, viewport, out var point)) return IsMarkerPlacementTool;
        var layer = MapLayerRules.Find(MapSession.CurrentMap.Layers, MapSession.ActiveRegionLayerId);
        if (layer is not { Kind: MapLayerKind.Region }) return true;
        var marker = new MapMarker(MapMarkerId.New(), layer.LayerId, "地图标记", point);
        var result = MapSession.CreateMarker(marker);
        if (!result.IsSuccess) { FooterMessage = result.Error?.Message ?? "地图标记创建失败。"; return true; }
        SelectMapGeometry(new(MapGeometryFeatureKind.Marker, marker.MarkerId.ToString()));
        SelectTool("选择", logTool: false);
        FooterState = "状态：就绪"; FooterMessage = "地图标记已创建并选中。";
        PublishSceneRenderSnapshot();
        return true;
    }
}
