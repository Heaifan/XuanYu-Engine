using XuanYu.Editor.Drawing;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public sealed record MarkerDrawingCommitResult(
    bool IsSuccess, MapMarker? Marker, string? ErrorCode, string? ErrorMessage)
{
    public static MarkerDrawingCommitResult Failed(string code, string message) =>
        new(false, null, code, message);
}

public sealed class MarkerDrawingAdapter(MapEditSession mapSession)
{
    readonly MapEditSession _mapSession = mapSession;

    public MarkerDrawingCommitResult Commit(DrawingCommitRequest request)
    {
        if (request.PrimitiveKind != DrawingPrimitiveKind.Point || request.Points.Count != 1)
            return MarkerDrawingCommitResult.Failed("InvalidMarkerDrawing", "Marker 需要一个点。");
        var layer = MapLayerRules.Find(_mapSession.CurrentMap.Layers, _mapSession.ActiveRegionLayerId);
        if (layer is not { Kind: MapLayerKind.Region })
            return MarkerDrawingCommitResult.Failed("InvalidMarkerLayer", "Marker 图层无效。");
        var marker = new MapMarker(MapMarkerId.New(), layer.LayerId,
            MapObjectNameAllocator.Marker(_mapSession.CurrentMap, "地图标记"), request.Points[0]);
        var result = _mapSession.CreateMarker(marker);
        return result.IsSuccess ? new(true, marker, null, null) :
            MarkerDrawingCommitResult.Failed(result.Error?.Code ?? "MarkerCommitFailed",
                result.Error?.Message ?? "Marker 创建失败。");
    }
}

public sealed class MarkerDrawingController
{
    readonly DrawingCoordinator _coordinator = new();
    readonly MarkerDrawingAdapter _adapter;

    public MarkerDrawingController(MapEditSession mapSession) => _adapter = new(mapSession);
    public bool IsActive => _coordinator.ActiveSession is { IsActive: true };
    public DrawingPreviewSnapshot? Preview => _coordinator.ActiveSession?.Preview;
    public DrawingSnapCandidate? SnapCandidate => _coordinator.ActiveSession?.SnapCandidate;
    public DrawingSession? Session => _coordinator.ActiveSession;

    public bool Begin()
    {
        if (IsActive) return true;
        _coordinator.Begin(DrawingPrimitiveKind.Point, "标记放置", "MapMarker");
        return true;
    }

    public void UpdatePreview(MapPoint point, DrawingSnapCandidate? candidate) =>
        _coordinator.UpdatePreview(point, candidate);

    public MarkerDrawingCommitResult Commit(MapPoint point)
    {
        if (!IsActive) return MarkerDrawingCommitResult.Failed("NoActiveSession", "Marker 会话不可用。");
        var input = _coordinator.AcceptInput(point);
        if (input.Disposition != DrawingInputDisposition.Accepted)
            return MarkerDrawingCommitResult.Failed(input.Code, input.UserMessage ?? "Marker 输入无效。");
        var request = _coordinator.Complete();
        if (request is null) return MarkerDrawingCommitResult.Failed("IncompleteMarker", "Marker 尚未完成。");
        var result = _adapter.Commit(request);
        if (result.IsSuccess) _coordinator.Terminate();
        return result;
    }

    public void Cancel() => _coordinator.Cancel();
}
