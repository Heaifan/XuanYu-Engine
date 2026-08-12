using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    MapGeometrySelection? _selectedMapGeometry;
    MapGeometryDrag? _mapGeometryDrag;
    MapGeometryPreview? _mapGeometryPreview;
    readonly RegionVertexSnapState _regionVertexSnap = new();

    public bool IsMapGeometryDragActive => _mapGeometryDrag is not null;
    public string SelectedMapGeometryText => _selectedMapGeometry is not { } selection ? "未选择几何" :
        selection.Kind == MapGeometryFeatureKind.Region ? "已选择区域" : "已选择道路";
    public MapGeometryPreview? MapGeometryPreview => _mapGeometryPreview;

    public bool TryBeginMapGeometryPointer(double x, double y, ViewportState viewport)
    {
        if (!IsRegionEditMode || !IsSelectTool || IsRegionDrawingDraftActive || IsRoadDrawingDraftActive ||
            !IsInsideViewport(x, y, viewport)) return false;
        var projection = ViewProjectionState.Create(CurrentCamera(viewport.Revision), viewport);
        if (_selectedMapGeometry is { } selected && MapGeometryHitTester.TryHitVertex(
                MapSession.CurrentMap, selected, projection, x, y, 10, MapSession.CurrentMap.Surface.BaseHeightMeters, out var index))
        {
            var points = GeometryPoints(selected);
            _mapGeometryDrag = new(selected, index, points);
            _regionVertexSnap.Clear();
            _mapGeometryPreview = new(selected, points);
            FooterState = "状态：捕获中";
            FooterMessage = "顶点拖动预览中。释放鼠标提交，按 Esc 取消。";
            PublishSceneRenderSnapshot();
            return true;
        }
        if (!MapGeometryHitTester.TryHitFeature(MapSession.CurrentMap, projection, x, y,
                MapSession.CurrentMap.Surface.BaseHeightMeters, out var hit))
        {
            ClearMapGeometrySelection();
            return false;
        }
        _selectedMapGeometry = hit.Selection;
        _mapGeometryPreview = new(hit.Selection, GeometryPoints(hit.Selection));
        RaiseMapGeometryBindings();
        PublishSceneRenderSnapshot();
        return true;
    }

    public bool PreviewMapGeometryPointer(double x, double y, ViewportState viewport)
    {
        if (_mapGeometryDrag is not { } drag || !TryPickRegionPoint(x, y, viewport, out var point)) return false;
        if (drag.Selection.Kind == MapGeometryFeatureKind.Region)
            point = ResolveRegionVertexSnap(drag.Selection, point, x, y, viewport);
        var points = drag.OriginalPoints.SetItem(drag.VertexIndex, point);
        _mapGeometryPreview = new(drag.Selection, points);
        FooterMessage = $"顶点预览：({point.X:0.##}, {point.Y:0.##})";
        PublishSceneRenderSnapshot();
        return true;
    }

    public bool CommitMapGeometryPointer(double x, double y, ViewportState viewport)
    {
        if (_mapGeometryDrag is null) return false;
        PreviewMapGeometryPointer(x, y, viewport);
        var drag = _mapGeometryDrag.Value;
        var points = _mapGeometryPreview?.Points ?? drag.OriginalPoints;
        var result = drag.Selection.Kind == MapGeometryFeatureKind.Region
            ? MapSession.EditRegionVertices(MapRegionIdFrom(drag.Selection), points)
            : MapSession.EditRoadPoints(MapRoadIdFrom(drag.Selection), points);
        _mapGeometryDrag = null;
        _regionVertexSnap.Clear();
        _mapGeometryPreview = DisplayGeometry();
        FooterState = "状态：就绪";
        FooterMessage = result.IsSuccess ? "顶点编辑已提交。" : result.Error?.Message ?? "顶点编辑失败。";
        PublishSceneRenderSnapshot();
        return true;
    }

    public bool CancelMapGeometryPointer(string reason)
    {
        if (_mapGeometryDrag is null) return false;
        _mapGeometryDrag = null;
        _regionVertexSnap.Clear();
        _mapGeometryPreview = DisplayGeometry();
        FooterState = "状态：就绪"; FooterMessage = $"顶点编辑已取消：{reason}";
        PublishSceneRenderSnapshot();
        return true;
    }

    void ClearMapGeometrySelection()
    {
        if (_selectedMapGeometry is null) return;
        _selectedMapGeometry = null; RaiseMapGeometryBindings(); PublishSceneRenderSnapshot();
    }

    void RaiseMapGeometryBindings()
    {
        OnPropertyChanged(nameof(SelectedMapGeometryText)); OnPropertyChanged(nameof(IsMapGeometryDragActive));
    }
}
