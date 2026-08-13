using System.Collections.Immutable;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public bool TryBeginMapGeometryVertexPointer(double x, double y, XuanYu.Core.Space.ViewportState viewport)
    {
        var canDrag = IsRegionDrawingTool || ((IsRoadAuthoringMode || IsMarkerAuthoringMode) && IsSelectTool);
        if (!IsRegionEditMode || !canDrag || IsRoadDrawingDraftActive ||
            !TryMapGeometryVertexHit(x, y, viewport, out var selection, out var index)) return false;
        var points = GeometryPoints(selection);
        _selectedMapGeometry = selection;
        _selectedMapGeometryVertexIndex = index;
        _mapGeometryDrag = new(selection, index, points);
        _regionVertexSnap.Clear();
        _mapGeometryPreview = new(selection, points);
        FooterState = "状态：捕获中"; FooterMessage = "顶点拖动预览中。释放鼠标提交，按 Esc 取消。";
        RaiseMapGeometryBindings(); PublishSceneRenderSnapshot(); return true;
    }

    bool TryMapGeometryVertexHover(double x, double y, XuanYu.Core.Space.ViewportState viewport)
    {
        if (!IsRegionEditMode || !IsRegionDrawingTool || IsMapGeometryDragActive) return false;
        if (!TryMapGeometryVertexHit(x, y, viewport, out var selection, out _)) return false;
        if (_selectedMapGeometry == selection) return true;
        _selectedMapGeometry = selection; _mapGeometryPreview = DisplayGeometry();
        RaiseMapGeometryBindings(); PublishSceneRenderSnapshot(); return true;
    }

    bool TryMapGeometryVertexHit(double x, double y, XuanYu.Core.Space.ViewportState viewport,
        out MapGeometrySelection selection, out int index)
    {
        var projection = XuanYu.Core.Space.ViewProjectionState.Create(CurrentCamera(viewport.Revision), viewport);
        var map = MapSession.CurrentMap; var height = map.Surface.BaseHeightMeters;
        if (_selectedMapGeometry is { } selected && MapGeometryHitTester.TryHitVertex(
                map, selected, projection, x, y, 10, height, out index)) { selection = selected; return true; }
        var features = IsRoadAuthoringMode
            ? map.Roads.Where(road => MapGeometryHitTester.IsEditable(map, road))
                .Select(road => new MapGeometrySelection(MapGeometryFeatureKind.Road, road.RoadId.ToString()))
            : IsMarkerAuthoringMode
            ? map.Markers.Where(marker => MapGeometryHitTester.IsEditable(map, new MapGeometrySelection(MapGeometryFeatureKind.Marker, marker.MarkerId.ToString())))
                .Select(marker => new MapGeometrySelection(MapGeometryFeatureKind.Marker, marker.MarkerId.ToString()))
            : map.Regions.Select(region => new MapGeometrySelection(MapGeometryFeatureKind.Region, region.RegionId.ToString()));
        foreach (var feature in features)
        {
            selection = feature;
            if (MapGeometryHitTester.TryHitVertex(map, selection, projection, x, y, 10, height, out index)) return true;
        }
        selection = default; index = -1; return false;
    }

    MapPoint ResolveGenericGeometrySnap(MapGeometrySelection selection, MapPoint raw,
        double x, double y, XuanYu.Core.Space.ViewportState viewport)
    {
        var projection = XuanYu.Core.Space.ViewProjectionState.Create(CurrentCamera(viewport.Revision), viewport);
        var source = new GeometryFeatureKey(
            selection.Kind == MapGeometryFeatureKind.Region ? GeometryFeatureKind.Region : selection.Kind == MapGeometryFeatureKind.Road ? GeometryFeatureKind.Road : GeometryFeatureKind.Marker,
            selection.FeatureId);
        // Legacy RegionSnapPipeline.Resolve contract remains documented; runtime now uses the generic pipeline.
        var result = GeometrySnapPipeline.Resolve(source, raw, new(x, y), MapSession.CurrentMap, projection,
            _geometrySnap, MapSession.QueryLocalGeometry, new RegionEdgeSnapSettings(8, 12));
        return result.ResolvedPoint;
    }
}

readonly record struct MapGeometryDrag(
    MapGeometrySelection Selection, int VertexIndex, ImmutableArray<MapPoint> OriginalPoints);
