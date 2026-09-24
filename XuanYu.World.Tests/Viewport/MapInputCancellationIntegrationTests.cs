using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Lifecycle;
using XuanYu.Editor.Input.Map;

namespace XuanYu.World.Tests.Viewport;

public sealed class MapInputCancellationIntegrationTests
{
    [Theory]
    [InlineData(EditorPointerEventKind.Escape, ViewportCancellationReason.Escape)]
    [InlineData(EditorPointerEventKind.CaptureLost, ViewportCancellationReason.CaptureLost)]
    [InlineData(EditorPointerEventKind.FocusLost, ViewportCancellationReason.FocusLost)]
    [InlineData(EditorPointerEventKind.WindowDeactivated, ViewportCancellationReason.WindowDeactivated)]
    [InlineData(EditorPointerEventKind.ToolChanged, ViewportCancellationReason.ToolChanged)]
    [InlineData(EditorPointerEventKind.ModeChanged, ViewportCancellationReason.ModeChanged)]
    [InlineData(EditorPointerEventKind.ViewportDisposed, ViewportCancellationReason.ViewportDisposed)]
    public void Every_map_cancel_source_clears_owner_capture_preview_and_transaction(
        EditorPointerEventKind kind, ViewportCancellationReason reason)
    {
        var backend = new MapEditingInputBackend((_, _) => true);
        var capture = new CaptureSpy();
        var router = new ViewportInputRouter([new RegionInputConsumer(backend)], capture);

        router.Dispatch(Event(EditorPointerEventKind.Pressed));
        var result = router.Dispatch(Event(kind, 2));

        Assert.Equal(ViewportInputDispatchKind.Cancelled, result.Kind);
        Assert.Equal(ViewportGestureState.Idle, router.State);
        Assert.Equal(1, capture.ReleaseCount);
        Assert.Equal(reason, backend.LastCancelReason);
        Assert.Equal(0, backend.CommitCount);
        Assert.False(backend.Snapshot.HasPreview || backend.Snapshot.HasTransaction);
    }

    sealed class CaptureSpy : IViewportPointerCaptureCoordinator
    {
        public int ReleaseCount { get; private set; }
        public void Capture(long pointerId, GestureOwner owner) { }
        public void Release(long pointerId, GestureOwner owner) => ReleaseCount++;
    }

    static EditorPointerEvent Event(EditorPointerEventKind kind, long id = 1) => new(
        kind, new(1, 2), EditorPointerButtons.Left, EditorPointerModifiers.None, 0, id, new("test"), 1);
}
