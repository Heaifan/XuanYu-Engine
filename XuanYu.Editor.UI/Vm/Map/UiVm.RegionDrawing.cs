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
    public bool IsRegionDrawingDraftActive => _regionDrawing.IsActive;
    public int RegionDrawingDraftVertexCount => _regionDrawing.Draft?.Vertices.Length ?? 0;

    public bool RegionDrawingPointerPressed(double x, double y, ViewportState viewport)
    {
        if (!IsRegionDrawingTool) return false;
        if (!IsInsideViewport(x, y, viewport)) return true;
        if (!TryPickRegionPoint(x, y, viewport, out var point))
        {
            FooterMessage = $"区域绘制收到视口点击：地面拾取未命中 ({x:0.#}, {y:0.#})";
            return true;
        }
        LastRegionDrawingHit = point;
        RegionDrawingHitCount++;
        FooterMessage = $"区域绘制地面命中：MapPoint=({point.X:0.##}, {point.Y:0.##})";
        if (!_regionDrawing.IsActive)
        {
            var layer = MapLayerRules.Find(MapSession.CurrentMap.Layers, MapSession.ActiveRegionLayerId);
            if (layer is not { Kind: MapLayerKind.Region }) return true;
            _regionDrawing.Start(layer.LayerId, "未命名区域", MapRegionKind.Generic);
        }
        if (_regionDrawing.IsCloseCandidate)
            return CloseRegionDraft();
        var oldCount = _regionDrawing.Draft?.Vertices.Length ?? 0;
        _regionDrawing.AddVertex(point);
        F1ForensicTrace.Draft(this, oldCount, _regionDrawing.Draft?.Vertices.Length ?? 0,
            _regionDrawing.Cursor, _regionDrawing.IsActive);
        PublishSceneRenderSnapshot();
        return true;
    }

    public bool CommitRegionDrawingFromEnter()
    {
        if (!IsRegionDrawingTool || !_regionDrawing.IsActive) return false;
        return CloseRegionDraft();
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
        var ray = WorldRayFactory.FromViewportPoint(projection, x, y);
        var hit = MapSurfacePicker.TryPick(MapSession.CurrentMap, projection, x, y, out point);
        F1ForensicTrace.Picker(this, hit, ray, point);
        return hit;
    }

    static bool IsInsideViewport(double x, double y, ViewportState viewport) =>
        x >= viewport.LogicalX && y >= viewport.LogicalY &&
        x <= viewport.LogicalX + viewport.LogicalWidth &&
        y <= viewport.LogicalY + viewport.LogicalHeight;

    bool CloseRegionDraft()
    {
        var draft = _regionDrawing.TakeDraftForClose();
        if (draft is null) { FooterMessage = "区域至少需要三个顶点才能闭合。"; return true; }
        var result = MapSession.CreateRegion(draft);
        if (!result.IsSuccess) { FooterState = "状态：错误"; FooterMessage = result.Error?.Message ?? "区域闭合失败"; return true; }
        _regionDrawing.Cancel(); FooterState = "状态：就绪"; FooterMessage = "区域已创建"; PublishSceneRenderSnapshot(); return true;
    }
}
