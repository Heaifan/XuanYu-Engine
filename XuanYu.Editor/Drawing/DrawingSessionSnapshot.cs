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

public sealed record DrawingSession(DrawingSessionSnapshot Snapshot);
