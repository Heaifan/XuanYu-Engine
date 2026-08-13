using System.Collections.Immutable;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    MapGeometryPreview? DisplayGeometry() => _selectedMapGeometry is { } selection
        ? new(selection, GeometryPoints(selection)) : null;

    ImmutableArray<MapPoint> GeometryPoints(MapGeometrySelection selection) => selection.Kind switch
    {
        MapGeometryFeatureKind.Region => MapSession.CurrentMap.Regions.First(r => r.RegionId.ToString() == selection.FeatureId).Vertices,
        MapGeometryFeatureKind.Road => MapSession.CurrentMap.Roads.First(r => r.RoadId.ToString() == selection.FeatureId).Points,
        _ => [MapSession.CurrentMap.Markers.First(r => r.MarkerId.ToString() == selection.FeatureId).Position]
    };

    void RefreshMapGeometryDisplay()
    {
        if (_selectedMapGeometry is not { } selection) return;
        var exists = selection.Kind == MapGeometryFeatureKind.Region
            ? MapSession.CurrentMap.Regions.Any(r => r.RegionId.ToString() == selection.FeatureId)
            : selection.Kind == MapGeometryFeatureKind.Road
            ? MapSession.CurrentMap.Roads.Any(r => r.RoadId.ToString() == selection.FeatureId) && MapGeometryHitTester.IsEditable(MapSession.CurrentMap, selection)
            : MapSession.CurrentMap.Markers.Any(r => r.MarkerId.ToString() == selection.FeatureId) && MapGeometryHitTester.IsEditable(MapSession.CurrentMap, selection);
        if (!exists) { _selectedMapGeometry = null; _selectedMapGeometryVertexIndex = -1; _mapGeometryPreview = null; RaiseMapGeometryBindings(); return; }
        _mapGeometryPreview = DisplayGeometry();
    }

    static MapRegionId MapRegionIdFrom(MapGeometrySelection selection) => MapRegionId.TryParse(selection.FeatureId, out var id) ? id : default;
    static MapRoadId MapRoadIdFrom(MapGeometrySelection selection) => MapRoadId.TryParse(selection.FeatureId, out var id) ? id : default;
    static MapMarkerId MapMarkerIdFrom(MapGeometrySelection selection) => MapMarkerId.TryParse(selection.FeatureId, out var id) ? id : default;
}
