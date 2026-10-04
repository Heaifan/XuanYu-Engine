using XuanYu.World.Map;

namespace XuanYu.Editor.Drawing;

public sealed record DrawingSessionSnapshot(
    DrawingState State,
    DrawingPrimitiveKind PrimitiveKind,
    int PointCount,
    bool CanUndo,
    bool CanRedo,
    bool CanComplete,
    bool CanCancel,
    bool CanClose,
    SnapState SnapState,
    string? BlockedReason,
    DrawingInputResult? LastInputResult);

public sealed record DrawingSession
{
    readonly DrawingSessionSnapshot? _snapshotOverride;

    public DrawingSession(DrawingSessionSnapshot snapshot)
    {
        _snapshotOverride = snapshot;
        PrimitiveKind = snapshot.PrimitiveKind;
        ToolIdentity = "snapshot"; DomainIdentity = "snapshot";
        Draft = new PointDraft();
    }

    public DrawingSession(DrawingPrimitiveKind primitiveKind, string toolIdentity, string domainIdentity)
    {
        PrimitiveKind = primitiveKind;
        ToolIdentity = toolIdentity; DomainIdentity = domainIdentity;
        Draft = primitiveKind == DrawingPrimitiveKind.Point ? new PointDraft() :
            throw new NotSupportedException("W2 currently migrates Point only.");
    }

    public DrawingPrimitiveKind PrimitiveKind { get; }
    public string ToolIdentity { get; }
    public string DomainIdentity { get; }
    public PointDraft Draft { get; }
    public DrawingState State { get; private set; } = DrawingState.Armed;
    public DrawingPreviewSnapshot? Preview { get; private set; }
    public DrawingSnapCandidate? SnapCandidate { get; private set; }
    public bool IsActive => State is DrawingState.Armed or DrawingState.Drawing or DrawingState.Committing;
    public DrawingSessionSnapshot Snapshot => _snapshotOverride ?? new(State, PrimitiveKind,
        Draft.PointCount, false, false, State == DrawingState.Committing, IsActive, IsActive,
        SnapCandidate is null ? SnapState.Inactive : SnapState.Active, null, null);

    public void UpdatePreview(MapPoint cursor, DrawingSnapCandidate? candidate)
    {
        if (!IsActive) return;
        SnapCandidate = candidate;
        var valid = candidate is { IsValid: true };
        Preview = new(cursor, null, new(false),
            valid ? DrawingPreviewHoverState.Acceptable : DrawingPreviewHoverState.Rejected, valid);
        State = DrawingState.Drawing;
    }

    public DrawingInputResult AcceptInput(MapPoint point)
    {
        if (!IsActive) return DrawingInputResult.Blocked("NoActiveSession", "绘制会话不可用。", RecoveryPolicy.Abort);
        var validation = DrawingValidation.Validate(PrimitiveKind, [point]);
        if (!validation.IsValid) return DrawingInputResult.Rejected(
            validation.FailureCategory.ToString(), "输入点无效。");
        Draft.Add(point);
        State = DrawingCompletion.Evaluate(PrimitiveKind, Draft.Points).CanComplete
            ? DrawingState.Committing : DrawingState.Drawing;
        return DrawingInputResult.Accepted("PointAccepted");
    }

    public DrawingCommitRequest? Complete()
    {
        if (State != DrawingState.Committing) return null;
        State = DrawingState.Completed;
        return new(PrimitiveKind, Draft.Points.ToArray());
    }

    public void Cancel() { State = DrawingState.Cancelled; ClearTransient(); }

    public void Terminate()
    {
        if (State is not DrawingState.Completed and not DrawingState.Cancelled) State = DrawingState.Aborted;
        ClearTransient();
    }

    void ClearTransient() { Draft.Clear(); Preview = null; SnapCandidate = null; }
}
