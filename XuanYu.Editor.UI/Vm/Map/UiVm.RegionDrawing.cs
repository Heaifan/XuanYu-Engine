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
    public bool IsRegionDrawingCloseCandidate => _regionDrawing.IsCloseCandidate;
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
            LogRegionDrawingStarted();
        }
        if (_regionDrawing.IsCloseCandidate)
            return CloseRegionDraft();
        _regionDrawing.AddVertex(point);
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
        if (!projection.TryProjectWorldPoint(new(
                first.X, first.Y, MapSession.CurrentMap.Surface.BaseHeightMeters), out var firstScreen))
        {
            _regionDrawing.UpdatePointer(point, closeCandidate: false);
            PublishSceneRenderSnapshot();
            return true;
        }
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
        LogRegionDrawingCanceled();
        PublishSceneRenderSnapshot();
        return true;
    }

}
