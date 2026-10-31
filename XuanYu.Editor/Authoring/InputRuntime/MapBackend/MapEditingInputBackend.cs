using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.Editor.Input.Map;

public sealed class MapEditingInputBackend(
    Func<EditorPointerEvent, ViewportGestureState, bool> canBegin,
    Action<ViewportGestureContext>? resolveSnap = null) : IMapEditingInputBackend
{
    readonly Func<EditorPointerEvent, ViewportGestureState, bool> _canBegin = canBegin;
    readonly Action<ViewportGestureContext>? _resolveSnap = resolveSnap;
    public MapEditingInputSnapshot Snapshot { get; private set; } = MapEditingInputSnapshot.Idle;
    public int CommitCount { get; private set; }
    public int CancelCount { get; private set; }
    public ViewportCancellationReason? LastCancelReason { get; private set; }
    public bool CanBegin(EditorPointerEvent pointer, ViewportGestureState state) =>
        _canBegin(pointer, state);
    public void Begin(ViewportGestureContext context) => Snapshot = new(
        MapEditingInputPhase.Preview, MapEditingInputTerminal.None, true, false, true, true, true);
    public void Update(ViewportGestureContext context)
    {
        if (Snapshot.Phase != MapEditingInputPhase.Preview) return;
        _resolveSnap?.Invoke(context);
        Snapshot = Snapshot with { HasSnapCandidate = _resolveSnap is not null };
    }
    public void Commit(ViewportGestureContext context)
    {
        if (Snapshot.Phase != MapEditingInputPhase.Preview) return;
        CommitCount++;
        Snapshot = MapEditingInputSnapshot.Idle with { LastTerminal = MapEditingInputTerminal.Committed };
    }
    public void Cancel(ViewportCancellationContext context)
    {
        if (Snapshot.Phase != MapEditingInputPhase.Preview) return;
        CancelCount++;
        LastCancelReason = context.Reason;
        Snapshot = MapEditingInputSnapshot.Idle with { LastTerminal = MapEditingInputTerminal.Canceled };
    }
}
