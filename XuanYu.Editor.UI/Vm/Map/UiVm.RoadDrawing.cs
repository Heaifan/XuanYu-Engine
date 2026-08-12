using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly RoadDrawingState _roadDrawing = new();
    public bool IsRoadDrawingDraftActive => _roadDrawing.IsActive;
    public int RoadDrawingDraftPointCount => _roadDrawing.Draft?.Points.Length ?? 0;
    public string RoadDrawingDraftStatus => !IsRoadDrawingDraftActive ? "尚未开始绘制" : RoadDrawingDraftPointCount < 2 ? "至少需要 2 个节点" : "可以完成";
    public int RoadContentCount => MapSession.CurrentMap.Roads.IsDefault ? 0 : MapSession.CurrentMap.Roads.Length;
    public bool RoadDrawingPointerPressed(double x, double y, ViewportState viewport)
    {
        if (!IsRoadDrawingTool) return false;
        if (!IsInsideViewport(x, y, viewport)) return true;
        if (!TryPickRegionPoint(x, y, viewport, out var point)) return true;
        if (!_roadDrawing.IsActive)
        {
            var layer = MapLayerRules.Find(MapSession.CurrentMap.Layers, MapSession.ActiveRegionLayerId);
            if (layer is not { Kind: MapLayerKind.Region }) return true;
            _roadDrawing.Start(layer.LayerId, "未命名道路", "generic");
        }
        _roadDrawing.AddVertex(point); RaiseRoadDrawingBindings(); PublishSceneRenderSnapshot(); return true;
    }
    public bool RoadDrawingPointerMoved(double x, double y, ViewportState viewport)
    {
        if (!IsRoadDrawingTool || !_roadDrawing.IsActive || !TryPickRegionPoint(x, y, viewport, out var point)) return false;
        _roadDrawing.UpdatePointer(point); PublishSceneRenderSnapshot(); return true;
    }
    public bool CommitRoadDrawingFromEnter() { if (!IsRoadDrawingTool || !_roadDrawing.IsActive) return false; return CloseRoadDraft(); }
    public bool CommitDrawingFromEnter() => IsRoadDrawingTool ? CommitRoadDrawingFromEnter() : CommitRegionDrawingFromEnter();
    public bool CancelRoadDrawingFromEscape()
    {
        if (!_roadDrawing.IsActive && !IsRoadDrawingTool) return false;
        _roadDrawing.Cancel(); RaiseRoadDrawingBindings(); if (IsRoadDrawingTool) SelectTool("选择");
        FooterMessage = "已取消道路绘制"; FooterState = "状态：就绪"; LogRoadDrawingCanceled(); PublishSceneRenderSnapshot(); return true;
    }
}
