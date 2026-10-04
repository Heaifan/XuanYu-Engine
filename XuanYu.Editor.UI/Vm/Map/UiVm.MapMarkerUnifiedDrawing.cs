using XuanYu.Editor.Drawing;
using XuanYu.Editor.MapEditing;
using XuanYu.Core.Space;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    MarkerDrawingController? _unifiedMarkerDrawing;
    MarkerDrawingController UnifiedMarkerDrawing =>
        _unifiedMarkerDrawing ??= new MarkerDrawingController(MapSession);
    public bool IsUnifiedMarkerDrawingActive => UnifiedMarkerDrawing.IsActive;
    public DrawingSession? UnifiedDrawingSession => UnifiedMarkerDrawing.Session;
    public DrawingPreviewSnapshot? MarkerDrawingPreview => UnifiedMarkerDrawing.Preview;
    public DrawingSnapCandidate? MarkerDrawingSnapCandidate => UnifiedMarkerDrawing.SnapCandidate;

    public bool BeginMarkerDrawing() => IsMarkerPlacementTool && UnifiedMarkerDrawing.Begin();

    public bool MarkerDrawingPointerMoved(double x, double y, ViewportState viewport)
    {
        if (!IsMarkerPlacementTool || !IsUnifiedMarkerDrawingActive) return false;
        if (!double.IsFinite(x) || !double.IsFinite(y)) return true;
        if (!TryPickMapPoint(x, y, viewport, out var point)) return true;
        UnifiedMarkerDrawing.UpdatePreview(point,
            new(point, DrawingSnapKind.Surface, "surface", null, null, true));
        PublishSceneRenderSnapshot();
        return true;
    }

    public bool MarkerDrawingPointerReleased(double x, double y, ViewportState viewport)
    {
        if (!IsMarkerPlacementTool || !IsUnifiedMarkerDrawingActive) return false;
        if (!double.IsFinite(x) || !double.IsFinite(y)) return true;
        if (!TryPickMapPoint(x, y, viewport, out var point)) return true;
        var result = UnifiedMarkerDrawing.Commit(point);
        if (!result.IsSuccess)
        {
            FooterState = "状态：错误";
            FooterMessage = result.ErrorMessage ?? "地图标记创建失败。";
            return true;
        }
        SelectMapGeometry(new(MapGeometryFeatureKind.Marker, result.Marker!.MarkerId.ToString()));
        SelectTool("选择", logTool: false);
        FooterState = "状态：就绪"; FooterMessage = "地图标记已创建并选中。";
        PublishSceneRenderSnapshot();
        return true;
    }

    public bool CancelMarkerDrawing(string reason = "取消")
    {
        if (!IsUnifiedMarkerDrawingActive) return false;
        UnifiedMarkerDrawing.Cancel();
        FooterState = "状态：就绪"; FooterMessage = $"地图标记绘制已取消：{reason}";
        PublishSceneRenderSnapshot();
        return true;
    }
}
