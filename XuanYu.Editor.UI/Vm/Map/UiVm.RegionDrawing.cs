using XuanYu.Core.Gizmo;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly RegionDrawingState _regionDrawing = new();

    public bool IsRegionDrawingActive => IsRegionDrawingTool;
    public int RegionDrawingHitCount { get; private set; }
    public MapPoint? LastRegionDrawingHit { get; private set; }

    public bool RegionDrawingPointerPressed(double x, double y, ViewportState viewport)
    {
        if (!IsRegionDrawingTool) return false;
        if (!IsInsideViewport(x, y, viewport) || !TryPickRegionPoint(x, y, viewport, out var point)) return true;
        LastRegionDrawingHit = point;
        RegionDrawingHitCount++;
        FooterMessage = $"区域绘制地面命中：MapPoint=({point.X:0.##}, {point.Y:0.##})";
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

    static bool IsInsideViewport(double x, double y, ViewportState viewport) =>
        x >= viewport.LogicalX && y >= viewport.LogicalY &&
        x <= viewport.LogicalX + viewport.LogicalWidth &&
        y <= viewport.LogicalY + viewport.LogicalHeight;
}
