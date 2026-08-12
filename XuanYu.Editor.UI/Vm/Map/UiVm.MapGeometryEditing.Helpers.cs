using System.Collections.Immutable;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public bool TryBeginMapGeometryVertexPointer(double x, double y, XuanYu.Core.Space.ViewportState viewport)
    {
        if (!IsRegionEditMode || !IsRegionDrawingTool || !TryMapGeometryVertexHit(x, y, viewport, out var selection, out var index)) return false;
        var points = GeometryPoints(selection);
        _selectedMapGeometry = selection;
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
        foreach (var region in map.Regions)
        {
            selection = new(MapGeometryFeatureKind.Region, region.RegionId.ToString());
            if (MapGeometryHitTester.TryHitVertex(map, selection, projection, x, y, 10, height, out index)) return true;
        }
        selection = default; index = -1; return false;
    }

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

    MapPoint ResolveRegionVertexSnap(MapGeometrySelection selection, MapPoint raw,
        double x, double y, XuanYu.Core.Space.ViewportState viewport)
    {
        if (!MapRegionId.TryParse(selection.FeatureId, out var sourceId)) return raw;
        var projection = XuanYu.Core.Space.ViewProjectionState.Create(CurrentCamera(viewport.Revision), viewport);
        var result = RegionVertexSnapResolver.Resolve(sourceId, raw, x, y, MapSession.CurrentMap, projection,
            _regionVertexSnap, MapSession.QueryLocalRegions,
            id => MapSession.TryGetRegion(id, out var region) ? region : null,
            RegionVertexSnapSettings.Default);
        return result.ResolvedPoint;
    }
}

readonly record struct MapGeometryDrag(
    MapGeometrySelection Selection, int VertexIndex, ImmutableArray<MapPoint> OriginalPoints);
