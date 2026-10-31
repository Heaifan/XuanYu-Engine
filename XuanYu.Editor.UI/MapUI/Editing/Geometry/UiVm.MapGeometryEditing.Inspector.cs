using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public string InspectorFeatureNameText => _selectedMapGeometry switch
    {
        { Kind: MapGeometryFeatureKind.Road } selection when MapRoadId.TryParse(selection.FeatureId, out var roadId) => MapSession.CurrentMap.Roads.FirstOrDefault(item => item.RoadId == roadId)?.DisplayName ?? "",
        { Kind: MapGeometryFeatureKind.Region } selection when MapRegionId.TryParse(selection.FeatureId, out var regionId) => MapSession.CurrentMap.Regions.FirstOrDefault(item => item.RegionId == regionId)?.DisplayName ?? "",
        { Kind: MapGeometryFeatureKind.Marker } selection when MapMarkerId.TryParse(selection.FeatureId, out var markerId) => (MapSession.CurrentMap.Markers.IsDefault ? [] : MapSession.CurrentMap.Markers).FirstOrDefault(item => item.MarkerId == markerId)?.DisplayName ?? "",
        _ => ""
    };
    public string InspectorFeatureTypeText => _selectedMapGeometry?.Kind switch
    {
        MapGeometryFeatureKind.Region => "区域",
        MapGeometryFeatureKind.Road => "道路",
        MapGeometryFeatureKind.Marker => "点",
        _ => ""
    };

    public string InspectorFeatureIdText => _selectedMapGeometry?.FeatureId ?? "";

    public string InspectorFeaturePointCountText => _selectedMapGeometry is { } selection
        ? GeometryPoints(selection).Length.ToString()
        : "";

    public string InspectorFeatureClosedText => _selectedMapGeometry?.Kind switch
    {
        MapGeometryFeatureKind.Region => "是",
        MapGeometryFeatureKind.Road or MapGeometryFeatureKind.Marker => "否",
        _ => ""
    };

    public string InspectorFeatureStatusText => _selectedMapGeometry switch
    {
        { Kind: MapGeometryFeatureKind.Road } selection when MapRoadId.TryParse(selection.FeatureId, out var roadId) => Status(MapSession.CurrentMap.Roads.FirstOrDefault(item => item.RoadId == roadId)),
        { Kind: MapGeometryFeatureKind.Region } selection when MapRegionId.TryParse(selection.FeatureId, out var regionId) => Status(MapSession.CurrentMap.Regions.FirstOrDefault(item => item.RegionId == regionId)),
        { Kind: MapGeometryFeatureKind.Marker } selection when MapMarkerId.TryParse(selection.FeatureId, out var markerId) => Status((MapSession.CurrentMap.Markers.IsDefault ? [] : MapSession.CurrentMap.Markers).FirstOrDefault(item => item.MarkerId == markerId)),
        _ => ""
    };

    static string Status(MapRoad? feature) => feature is null ? "" : $"可见：{(feature.IsVisible ? "是" : "否")}；锁定：{(feature.IsLocked ? "是" : "否")}";
    static string Status(MapRegion? feature) => feature is null ? "" : $"可见：{(feature.IsVisible ? "是" : "否")}；锁定：{(feature.IsLocked ? "是" : "否")}";
    static string Status(MapMarker? feature) => feature is null ? "" : $"可见：{(feature.IsVisible ? "是" : "否")}；锁定：{(feature.IsLocked ? "是" : "否")}";
}
