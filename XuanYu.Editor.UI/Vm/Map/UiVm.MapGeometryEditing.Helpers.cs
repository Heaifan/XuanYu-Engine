using System.Collections.Immutable;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    MapGeometryPreview? DisplayGeometry() => _selectedMapGeometry is { } selection
        ? new(selection, GeometryPoints(selection)) : null;

    ImmutableArray<MapPoint> GeometryPoints(MapGeometrySelection selection) =>
        selection.Kind == MapGeometryFeatureKind.Region
            ? MapSession.CurrentMap.Regions.First(r => r.RegionId.ToString() == selection.FeatureId).Vertices
            : MapSession.CurrentMap.Roads.First(r => r.RoadId.ToString() == selection.FeatureId).Points;

    void RefreshMapGeometryDisplay()
    {
        if (_selectedMapGeometry is not { } selection) return;
        var exists = selection.Kind == MapGeometryFeatureKind.Region
            ? MapSession.CurrentMap.Regions.Any(r => r.RegionId.ToString() == selection.FeatureId)
            : MapSession.CurrentMap.Roads.Any(r => r.RoadId.ToString() == selection.FeatureId);
        if (!exists)
        {
            _selectedMapGeometry = null; _mapGeometryPreview = null; RaiseMapGeometryBindings(); return;
        }
        _mapGeometryPreview = DisplayGeometry();
    }

    static MapRegionId MapRegionIdFrom(MapGeometrySelection selection) =>
        MapRegionId.TryParse(selection.FeatureId, out var id) ? id : default;

    static MapRoadId MapRoadIdFrom(MapGeometrySelection selection) =>
        MapRoadId.TryParse(selection.FeatureId, out var id) ? id : default;
}

readonly record struct MapGeometryDrag(
    MapGeometrySelection Selection, int VertexIndex, ImmutableArray<MapPoint> OriginalPoints);
