using XuanYu.Core.Gizmo;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly RegionDrawingState _regionDrawing = new();

    public bool IsRegionDrawingActive => IsRegionDrawingTool;

    public bool RegionDrawingPointerPressed(double x, double y, ViewportState viewport)
    {
        if (!IsRegionDrawingTool || !TryPickRegionPoint(x, y, viewport, out var point)) return false;
        if (!_regionDrawing.IsActive)
        {
            var layer = MapLayerRules.Find(MapSession.CurrentMap.Layers, MapSession.ActiveRegionLayerId);
            if (layer is not { Kind: MapLayerKind.Region }) return false;
            _regionDrawing.Start(layer.LayerId, "未命名区域", MapRegionKind.Generic);
        }

        if (_regionDrawing.IsCloseCandidate)
        {
            var draft = _regionDrawing.TakeDraftForClose();
            if (draft is null) return true;
            var result = MapSession.CreateRegion(draft);
            if (!result.IsSuccess)
            {
                FooterState = "状态：错误";
                FooterMessage = result.Error?.Message ?? "区域闭合失败";
                return true;
            }

            _regionDrawing.Cancel();
            FooterState = "状态：就绪";
            FooterMessage = "区域已创建";
            PublishSceneRenderSnapshot();
            return true;
        }

        _regionDrawing.AddVertex(point);
        PublishSceneRenderSnapshot();
        return true;
    }

    public bool RegionDrawingPointerMoved(double x, double y, ViewportState viewport)
    {
        if (!IsRegionDrawingTool || !_regionDrawing.IsActive ||
            !TryPickRegionPoint(x, y, viewport, out var point)) return false;
        var first = _regionDrawing.Draft!.Vertices[0];
        var projection = ViewProjectionState.Create(CurrentCamera(viewport.Revision), viewport);
        var firstScreen = projection.ProjectWorldPoint(new(
            first.X, first.Y, MapSession.CurrentMap.Surface.BaseHeightMeters));
        var distance = Math.Sqrt(Math.Pow(firstScreen.X - x, 2) + Math.Pow(firstScreen.Y - y, 2));
        _regionDrawing.UpdatePointer(point, distance <= ScaleGizmoScreenSize.CenterHitRadiusDip);
        PublishSceneRenderSnapshot();
        return true;
    }

    public bool CancelRegionDrawingFromEscape()
    {
        if (!_regionDrawing.IsActive && !IsRegionDrawingTool) return false;
        _regionDrawing.Cancel();
        if (IsRegionDrawingTool) SelectTool("选择");
        FooterMessage = "已取消区域绘制";
        FooterState = "状态：就绪";
        PublishSceneRenderSnapshot();
        return true;
    }

    bool TryPickRegionPoint(double x, double y, ViewportState viewport, out MapPoint point)
    {
        var projection = ViewProjectionState.Create(CurrentCamera(viewport.Revision), viewport);
        return MapSurfacePicker.TryPick(MapSession.CurrentMap, projection, x, y, out point);
    }
}
