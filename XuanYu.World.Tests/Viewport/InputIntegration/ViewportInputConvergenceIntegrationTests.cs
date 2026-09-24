using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.World.Tests.Viewport.InputIntegration;

public sealed class ViewportInputConvergenceIntegrationTests
{
    [Fact]
    public void Native_and_Avalonia_events_enter_the_same_router_contract()
    {
        var consumer = new Consumer();
        var router = new ViewportInputRouter([consumer], new CaptureSpy());
        var native = Event(EditorPointerEventKind.Pressed, new("native"));
        var avalonia = native with { SourceSurface = new("avalonia") };

        Assert.Equal(ViewportInputDispatchKind.Captured, router.Dispatch(native).Kind);
        router.Dispatch(Event(EditorPointerEventKind.Released, native.SourceSurface));
        Assert.Equal(ViewportInputDispatchKind.Captured, router.Dispatch(avalonia).Kind);
        Assert.Equal(GestureOwner.Camera, router.State.Owner);
    }

    [Theory]
    [InlineData(EditorPointerEventKind.Cancel, ViewportCancellationReason.ExplicitCancel)]
    [InlineData(EditorPointerEventKind.CaptureLost, ViewportCancellationReason.CaptureLost)]
    [InlineData(EditorPointerEventKind.FocusLost, ViewportCancellationReason.FocusLost)]
    [InlineData(EditorPointerEventKind.WindowDeactivated, ViewportCancellationReason.WindowDeactivated)]
    public void Every_terminal_input_uses_one_cancel_pipeline(
        EditorPointerEventKind kind, ViewportCancellationReason reason)
    {
        var capture = new CaptureSpy();
        var consumer = new Consumer();
        var router = new ViewportInputRouter([consumer], capture);

        router.Dispatch(Event(EditorPointerEventKind.Pressed));
        Assert.Equal(ViewportInputDispatchKind.Cancelled, router.Dispatch(Event(kind)).Kind);
        Assert.Equal(ViewportGestureState.Idle, router.State);
        Assert.Equal(1, capture.ReleaseCount);
        Assert.Equal(reason, consumer.LastReason);
        Assert.Equal(ViewportInputDispatchKind.Ignored, router.Dispatch(Event(kind)).Kind);
    }

    [Fact]
    public void Cancel_then_next_gesture_has_no_stale_owner_or_capture()
    {
        var capture = new CaptureSpy();
        var router = new ViewportInputRouter([new Consumer()], capture);

        router.Dispatch(Event(EditorPointerEventKind.Pressed, new("one")));
        router.Dispatch(Event(EditorPointerEventKind.Cancel, new("one")));
        router.Dispatch(Event(EditorPointerEventKind.Pressed, new("two")));

        Assert.Equal(GestureOwner.Camera, router.State.Owner);
        Assert.Equal(2, capture.CaptureCount);
        Assert.Equal(1, capture.ReleaseCount);
    }

    static EditorPointerEvent Event(EditorPointerEventKind kind, ViewportPointerSource? source = null) => new(
        kind, new(10, 20), EditorPointerButtons.Left, EditorPointerModifiers.None,
        0, 1, source ?? new("test"), 1);

    sealed class Consumer : IViewportInputConsumer
    {
        public GestureOwner Owner => GestureOwner.Camera;
        public ViewportCancellationReason? LastReason { get; private set; }
        public void Begin(ViewportGestureContext context) { }
        public void Update(ViewportGestureContext context) { }
        public void Commit(ViewportGestureContext context) { }
        public void Cancel(ViewportCancellationContext context) => LastReason = context.Reason;
        public ViewportInputDispatchResult Handle(EditorPointerEvent pointer, ViewportGestureState state)
        {
            if (pointer.Kind is EditorPointerEventKind.Cancel or EditorPointerEventKind.CaptureLost
                or EditorPointerEventKind.FocusLost or EditorPointerEventKind.WindowDeactivated)
                LastReason = pointer.Kind switch
                {
                    EditorPointerEventKind.CaptureLost => ViewportCancellationReason.CaptureLost,
                    EditorPointerEventKind.FocusLost => ViewportCancellationReason.FocusLost,
                    EditorPointerEventKind.WindowDeactivated => ViewportCancellationReason.WindowDeactivated,
                    _ => ViewportCancellationReason.ExplicitCancel,
                };
            return pointer.Kind == EditorPointerEventKind.Pressed
                ? ViewportInputDispatchResult.Captured : ViewportInputDispatchResult.Handled;
        }
    }

    sealed class CaptureSpy : IViewportPointerCaptureCoordinator
    {
        public int CaptureCount { get; private set; }
        public int ReleaseCount { get; private set; }
        public void Capture(long pointerId, GestureOwner owner) => CaptureCount++;
        public void Release(long pointerId, GestureOwner owner) => ReleaseCount++;
    }
}
