using XuanYu.World.Map;

namespace XuanYu.Editor.Drawing;

public sealed class DrawingSession
{
    readonly DrawingSessionSnapshot? _snapshotOverride;
    readonly List<IReadOnlyList<MapPoint>> _undo = [];
    readonly List<IReadOnlyList<MapPoint>> _redo = [];

    public DrawingSession(DrawingPrimitiveKind kind, string toolIdentity, string domainIdentity)
    {
        PrimitiveKind = kind; ToolIdentity = toolIdentity; DomainIdentity = domainIdentity;
        Draft = CreateDraft(kind);
    }

    public DrawingSession(DrawingSessionSnapshot snapshot) : this(snapshot.PrimitiveKind, "snapshot", "snapshot")
    {
        _snapshotOverride = snapshot;
        State = snapshot.State;
    }

    public DrawingPrimitiveKind PrimitiveKind { get; }
    public string ToolIdentity { get; }
    public string DomainIdentity { get; }
    public IDrawingDraft Draft { get; }
    public PointDraft? PointDraft => Draft as PointDraft;
    public PolylineDraft? PolylineDraft => Draft as PolylineDraft;
    public PolygonDraft? PolygonDraft => Draft as PolygonDraft;
    public int PointCount => Draft.PointCount;
    public DrawingState State { get; private set; } = DrawingState.Armed;
    public DrawingPreviewSnapshot? Preview { get; private set; }
    public DrawingSnapCandidate? SnapCandidate { get; private set; }
    public bool IsActive => State is DrawingState.Armed or DrawingState.Drawing or DrawingState.Committing;
    public bool CanUndo => IsActive && _undo.Count > 0;
    public bool CanRedo => IsActive && _redo.Count > 0;
    public bool CanComplete => IsActive && DrawingValidation.Validate(PrimitiveKind, Draft.Points).IsValid;

    public DrawingSessionSnapshot Snapshot => _snapshotOverride ?? new(State, PrimitiveKind, PointCount, CanUndo, CanRedo,
        CanComplete, IsActive, PrimitiveKind == DrawingPrimitiveKind.Polygon && CanComplete,
        SnapCandidate is null ? SnapState.Inactive : SnapState.Active, null, null);

    public void UpdatePreview(MapPoint cursor, DrawingSnapCandidate? candidate)
    {
        if (!IsActive) return;
        SnapCandidate = candidate;
        var valid = double.IsFinite(cursor.X) && double.IsFinite(cursor.Y) &&
            (candidate is null || candidate.IsValid) && (PointCount == 0 || Draft.Points[^1] != cursor);
        MapPoint? start = PointCount == 0 ? null : Draft.Points[^1];
        Preview = new(cursor, start is null ? null : new(start.Value, cursor),
            new(PrimitiveKind == DrawingPrimitiveKind.Polygon && PointCount >= 3 && cursor == Draft.Points[0]),
            valid ? DrawingPreviewHoverState.Acceptable : DrawingPreviewHoverState.Rejected, valid);
        State = DrawingState.Drawing;
    }

    public DrawingInputResult AcceptInput(MapPoint point)
    {
        if (!IsActive) return DrawingInputResult.Blocked("NoActiveSession", "绘制会话不可用。", RecoveryPolicy.Abort);
        var validation = DrawingValidation.Validate(PrimitiveKind, Draft.Points.Append(point).ToArray());
        if (!validation.IsValid && validation.FailureCategory != DrawingValidationFailureCategory.TooFewPoints)
            return DrawingInputResult.Rejected(validation.FailureCategory.ToString(), "输入点无效。");
        _undo.Add(Draft.Points.ToArray()); _redo.Clear(); Draft.AddPoint(point);
        State = PrimitiveKind == DrawingPrimitiveKind.Point ? DrawingState.Committing : DrawingState.Drawing;
        return DrawingInputResult.Accepted("PointAccepted");
    }

    public DrawingCommitRequest? Complete()
    {
        if (!CanComplete) return null;
        State = DrawingState.Completed;
        return new(PrimitiveKind, Draft.Points.ToArray());
    }

    public bool Undo() => Move(_undo, _redo);
    public bool Redo() => Move(_redo, _undo);
    public void Cancel() { State = DrawingState.Cancelled; Clear(); }

    public void Terminate()
    {
        if (State is not DrawingState.Completed and not DrawingState.Cancelled) State = DrawingState.Aborted;
        Clear();
    }

    bool Move(List<IReadOnlyList<MapPoint>> source, List<IReadOnlyList<MapPoint>> target)
    {
        if (!IsActive || source.Count == 0) return false;
        target.Add(Draft.Points.ToArray());
        var points = source[^1]; source.RemoveAt(source.Count - 1);
        Draft.Clear(); foreach (var point in points) Draft.AddPoint(point);
        return true;
    }

    void Clear() { Draft.Clear(); Preview = null; SnapCandidate = null; _undo.Clear(); _redo.Clear(); }

    static IDrawingDraft CreateDraft(DrawingPrimitiveKind kind) => kind switch
    {
        DrawingPrimitiveKind.Point => new PointDraft(), DrawingPrimitiveKind.Polyline => new PolylineDraft(),
        DrawingPrimitiveKind.Polygon => new PolygonDraft(), _ => throw new ArgumentOutOfRangeException(nameof(kind))
    };
}
