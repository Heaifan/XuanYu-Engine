using XuanYu.Editor.Drawing;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public sealed class RoadDrawingController
{
    readonly DrawingCoordinator _coordinator = new();
    readonly RoadDrawingAdapter _adapter;
    RoadDrawingMetadata? _metadata;

    public RoadDrawingController(MapEditSession mapSession) => _adapter = new(mapSession);
    public DrawingSession? Session => _coordinator.ActiveSession;
    public RoadDrawingMetadata? Metadata => _metadata;
    public bool IsActive => Session is { IsActive: true };
    public int PointCount => Session?.PointCount ?? 0;
    public bool CanUndo => Session?.CanUndo == true;
    public bool CanRedo => Session?.CanRedo == true;
    public bool CanComplete => Session?.CanComplete == true;
    public DrawingPreviewSnapshot? Preview => Session?.Preview;
    public DrawingSnapCandidate? SnapCandidate => Session?.SnapCandidate;

    public bool Begin(RoadDrawingMetadata metadata)
    {
        if (IsActive) return false;
        _metadata = metadata;
        _coordinator.Begin(DrawingPrimitiveKind.Polyline, "道路绘制", "道路绘制");
        return true;
    }

    public DrawingInputResult AcceptPoint(MapPoint point) =>
        _coordinator.AcceptInput(point);

    public void UpdatePreview(MapPoint point, DrawingSnapCandidate? candidate) =>
        _coordinator.UpdatePreview(point, candidate);

    public bool Undo() => Session?.Undo() == true;
    public bool Redo() => Session?.Redo() == true;

    public RoadDrawingCommitResult Complete()
    {
        if (_metadata is not { } metadata || !IsActive)
            return RoadDrawingCommitResult.Failed("NoActiveSession", "道路绘制会话不可用。");
        var request = _coordinator.Complete();
        if (request is null)
            return RoadDrawingCommitResult.Failed("TooFewRoadPoints", "道路至少需要两个节点才能完成。");
        var result = _adapter.Commit(request, metadata);
        if (result.IsSuccess) _coordinator.Terminate();
        return result;
    }

    public bool Cancel()
    {
        _metadata = null;
        return _coordinator.Cancel();
    }
}
