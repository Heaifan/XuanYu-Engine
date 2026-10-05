using XuanYu.Core.Space;
using XuanYu.Editor.Drawing;
using XuanYu.Editor.MapDocument;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly RoadDrawingState _roadDrawing = new();
    RoadDrawingController? _roadDrawingController;
    RoadDrawingController RoadDrawingController =>
        _roadDrawingController ??= new RoadDrawingController(MapSession);
    public bool IsRoadDrawingDraftActive => _roadDrawing.IsActive;
    public int RoadDrawingDraftPointCount => RoadDrawingController.PointCount;
    public string RoadDrawingDraftStatus => !IsRoadDrawingDraftActive ? "尚未开始绘制" : RoadDrawingDraftPointCount < 2 ? "至少需要 2 个节点" : "可以完成";
    public int RoadContentCount => MapSession.CurrentMap.Roads.IsDefault ? 0 : MapSession.CurrentMap.Roads.Length;
    public bool RoadDrawingPointerPressed(double x, double y, ViewportState viewport)
    {
        if (!IsRoadDrawingTool) return false;
        if (!IsInsideViewport(x, y, viewport)) return true;
        if (!TryPickMapPoint(x, y, viewport, out var point)) return true;
        if (!_roadDrawing.IsActive)
        {
            if (SelectedDataset is not { Type: "road", Status: "正常", IsLocked: false } target)
                return true;
            var metadata = new RoadDrawingMetadata(
                MapDatasetLayerIdProjection.Project(target.Id),
                MapObjectNameAllocator.Road(MapSession.CurrentMap, "道路"), "generic", target.Id);
            if (!RoadDrawingController.Begin(metadata)) return true;
            _roadDrawing.ProjectFrom(RoadDrawingController.Session, RoadDrawingController.Metadata);
        }
        var result = RoadDrawingController.AcceptPoint(point);
        if (result.Disposition == DrawingInputDisposition.Accepted)
        {
            _roadDrawing.ProjectFrom(RoadDrawingController.Session, RoadDrawingController.Metadata);
            RaiseRoadDrawingBindings();
        }
        PublishSceneRenderSnapshot(); return true;
    }
    public bool RoadDrawingPointerMoved(double x, double y, ViewportState viewport)
    {
        if (!IsRoadDrawingTool || !RoadDrawingController.IsActive || !TryPickMapPoint(x, y, viewport, out var point)) return false;
        RoadDrawingController.UpdatePreview(point,
            new(point, DrawingSnapKind.Surface, "surface", null, null, true));
        _roadDrawing.ProjectFrom(RoadDrawingController.Session, RoadDrawingController.Metadata);
        PublishSceneRenderSnapshot(); return true;
    }
    public bool CommitRoadDrawingFromEnter() { if (!IsRoadDrawingTool || !RoadDrawingController.IsActive) return false; return CloseRoadDraft(); }
    public bool CommitDrawingFromEnter() => IsRoadDrawingTool ? CommitRoadDrawingFromEnter() : CommitRegionDrawingFromEnter();
    public bool CancelRoadDrawingFromEscape()
    {
        if (!RoadDrawingController.IsActive && !IsRoadDrawingTool) return false;
        RoadDrawingController.Cancel(); _roadDrawing.Clear(); RaiseRoadDrawingBindings(); EndDrawingTransaction(); if (IsRoadDrawingTool) SelectTool("选择");
        FooterMessage = "已取消道路绘制"; FooterState = "状态：就绪"; LogRoadDrawingCanceled(); PublishSceneRenderSnapshot(); return true;
    }
}
