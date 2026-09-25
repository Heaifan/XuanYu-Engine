using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public bool TryGetMapGeometryContext(double x, double y, ViewportState viewport,
        out MapGeometryContextHit hit)
    {
        hit = MapGeometryContextHit.Empty;
        if (!IsRegionEditMode || !IsSelectTool || IsRegionDrawingDraftActive || IsRoadDrawingDraftActive) return false;
        if (!IsInsideViewport(x, y, viewport)) return false;
        var projection = ViewProjectionState.Create(CurrentCamera(viewport.Revision), viewport);
        if (!MapGeometryContextHitTester.TryHit(MapSession.CurrentMap, projection, x, y,
            MapSession.CurrentMap.Surface.BaseHeightMeters, out hit)) return false;
        SelectMapGeometry(hit.Selection);
        return true;
    }

    public bool ExecuteMapGeometryContext(string command, MapGeometryContextHit hit,
        double x, double y, ViewportState viewport)
    {
        var result = command switch
        {
            "delete-region" when hit.Selection.Kind == MapGeometryFeatureKind.Region =>
                MapSession.DeleteRegion(MapRegionIdFrom(hit.Selection)),
            "delete-road" when hit.Selection.Kind == MapGeometryFeatureKind.Road =>
                MapSession.DeleteRoad(MapRoadIdFrom(hit.Selection)),
            "delete-marker" when hit.Selection.Kind == MapGeometryFeatureKind.Marker =>
                MapSession.DeleteMarker(MapMarkerIdFrom(hit.Selection)),
            "delete-vertex" => DeleteContextVertex(hit),
            "add-vertex" => AddContextVertex(hit, x, y, viewport),
            "edit-region" or "edit-road" or "edit-marker" => EnterGeometryEditMode(),
            _ => XuanYu.Core.Results.EngineResult.Fail(XuanYu.Core.Results.EngineError.Create("UnknownContextCommand", "未知地图对象命令。"))
        };
        FooterMessage = result.IsSuccess ? "地图对象命令已执行。" : result.Error?.Message ?? "地图对象命令执行失败。";
        PublishSceneRenderSnapshot();
        return result.IsSuccess;
    }

    XuanYu.Core.Results.EngineResult DeleteContextVertex(MapGeometryContextHit hit) => hit.Selection.Kind switch
    {
        MapGeometryFeatureKind.Region => MapSession.DeleteRegionVertex(MapRegionIdFrom(hit.Selection), hit.VertexIndex),
        MapGeometryFeatureKind.Road => MapSession.DeleteRoadVertex(MapRoadIdFrom(hit.Selection), hit.VertexIndex),
        _ => XuanYu.Core.Results.EngineResult.Fail(XuanYu.Core.Results.EngineError.Create("InvalidVertex", "当前对象没有可删除的顶点。"))
    };

    XuanYu.Core.Results.EngineResult AddContextVertex(MapGeometryContextHit hit, double x, double y, ViewportState viewport)
    {
        if (!TryPickRegionPoint(x, y, viewport, out var point))
            return XuanYu.Core.Results.EngineResult.Fail(XuanYu.Core.Results.EngineError.Create("InvalidPointer", "无法取得地图坐标。"));
        return hit.Selection.Kind == MapGeometryFeatureKind.Region
            ? MapSession.InsertRegionVertex(MapRegionIdFrom(hit.Selection), hit.SegmentIndex, point)
            : MapSession.InsertRoadVertex(MapRoadIdFrom(hit.Selection), hit.SegmentIndex, point);
    }

    XuanYu.Core.Results.EngineResult EnterGeometryEditMode()
    {
        IsGeometryEditingActive = true;
        return XuanYu.Core.Results.EngineResult.Success();
    }
}
