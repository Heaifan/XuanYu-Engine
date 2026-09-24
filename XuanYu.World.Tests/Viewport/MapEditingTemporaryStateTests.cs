using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Lifecycle;
using XuanYu.Editor.Input.Map;

namespace XuanYu.World.Tests.Viewport;

public sealed class MapEditingTemporaryStateTests
{
    [Fact]
    public void Update_is_preview_only_and_snap_runs_under_current_owner()
    {
        var snapCalls = 0;
        var backend = Create(() => snapCalls++);
        var context = Context(GestureOwner.Road);

        backend.Begin(context);
        backend.Update(context);

        Assert.Equal(MapEditingInputPhase.Preview, backend.Snapshot.Phase);
        Assert.True(backend.Snapshot.HasPreview);
        Assert.True(backend.Snapshot.HasSnapCandidate);
        Assert.Equal(1, snapCalls);
        Assert.Equal(0, backend.CommitCount);
    }

    [Fact]
    public void Commit_clears_temporary_state_and_cancel_never_commits()
    {
        var backend = Create();
        var context = Context(GestureOwner.Marker);

        backend.Begin(context);
        backend.Cancel(new(context, ViewportCancellationReason.CaptureLost));
        backend.Cancel(new(context, ViewportCancellationReason.Escape));

        Assert.Equal(0, backend.CommitCount);
        Assert.Equal(1, backend.CancelCount);
        Assert.Equal(MapEditingInputTerminal.Canceled, backend.Snapshot.LastTerminal);
        Assert.False(backend.Snapshot.HasPreview || backend.Snapshot.HasTransaction);

        backend.Begin(context);
        backend.Commit(context);
        Assert.Equal(1, backend.CommitCount);
        Assert.Equal(MapEditingInputTerminal.Committed, backend.Snapshot.LastTerminal);
        Assert.False(backend.Snapshot.HasSnapCandidate || backend.Snapshot.HasTemporaryGeometry);
    }

    static MapEditingInputBackend Create(Action? snap = null) => new(
        (_, _) => true, snap is null ? null : _ => snap());
    static ViewportGestureContext Context(GestureOwner owner) => new(
        "MapEditing", owner, 1, ViewportGestureCapture.Pointer, new(
            EditorPointerEventKind.Pressed, new(1, 2), EditorPointerButtons.Left,
            EditorPointerModifiers.None, 0, 1, new("test"), 1));
}
