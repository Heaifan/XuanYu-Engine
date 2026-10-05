using System.Collections.Immutable;
using XuanYu.Editor.Drawing;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public sealed class RegionDrawingController
{
    public DrawingSession? Session { get; private set; }
    public MapLayerId LayerId { get; private set; }
    public string DisplayName { get; private set; } = "";
    public MapRegionKind Kind { get; private set; }
    public SurfaceBinding SurfaceBinding { get; private set; } = SurfaceBinding.ReferencePlane;
    public MapPoint? Cursor { get; private set; }
    public bool IsCloseCandidate { get; private set; }
    public bool IsActive => Session?.IsActive == true;
    public int PointCount => Session?.PointCount ?? 0;
    public bool CanUndo => Session?.CanUndo == true;
    public bool CanRedo => Session?.CanRedo == true;
    public bool CanComplete => Session?.CanComplete == true;
    public MapRegionDraft? Draft => Session is null ? null : ToDraft();

    public void Begin(MapLayerId layerId, string displayName, MapRegionKind kind, SurfaceBinding binding)
    {
        Cancel();
        LayerId = layerId; DisplayName = displayName; Kind = kind; SurfaceBinding = binding;
        Session = new DrawingSession(DrawingPrimitiveKind.Polygon, "区域绘制", "Region");
    }

    public DrawingInputResult AcceptPoint(MapPoint point)
    {
        if (Session is null) return DrawingInputResult.Blocked(
            "NoActiveSession", "区域绘制会话不可用。", RecoveryPolicy.Abort);
        var result = Session.AcceptInput(point);
        if (double.IsFinite(point.X) && double.IsFinite(point.Y))
        {
            Cursor = point;
            IsCloseCandidate = false;
        }
        return result;
    }

    public void UpdatePreview(MapPoint cursor, DrawingSnapCandidate? candidate, bool closeCandidate)
    {
        if (Session is null) return;
        Cursor = cursor;
        IsCloseCandidate = closeCandidate && CanComplete;
        var previewCursor = IsCloseCandidate && Session.PolygonDraft is { PointCount: > 0 } draft
            ? draft.Points[0] : cursor;
        Session.UpdatePreview(previewCursor, candidate);
    }

    public DrawingCommitRequest? Complete() => Session?.Complete();
    public bool Undo() => MoveHistory(undo: true);
    public bool Redo() => MoveHistory(undo: false);

    public void Cancel()
    {
        Session?.Cancel();
        ClearTransient();
    }

    public void ClearAfterCommit()
    {
        Session?.Terminate();
        ClearTransient();
    }

    void ClearTransient()
    {
        Session = null; Cursor = null; IsCloseCandidate = false;
    }

    bool MoveHistory(bool undo)
    {
        var moved = undo ? Session?.Undo() == true : Session?.Redo() == true;
        if (moved) Cursor = Session!.PointCount == 0 ? null : Session.Draft.Points[^1];
        IsCloseCandidate = false;
        return moved;
    }

    MapRegionDraft ToDraft() => new(LayerId, DisplayName, Kind,
        Session!.Draft.Points.ToImmutableArray()) { SurfaceBinding = SurfaceBinding };
}
