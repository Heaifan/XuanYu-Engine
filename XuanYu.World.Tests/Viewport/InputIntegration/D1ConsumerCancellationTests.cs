using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Consumers;
using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.World.Tests.Viewport.InputIntegration;

public sealed class D1ConsumerCancellationTests
{
    [Theory]
    [InlineData(EditorPointerEventKind.Cancel)]
    [InlineData(EditorPointerEventKind.CaptureLost)]
    [InlineData(EditorPointerEventKind.FocusLost)]
    [InlineData(EditorPointerEventKind.WindowDeactivated)]
    public void Every_platform_cancel_source_cancels_exactly_once(EditorPointerEventKind kind)
    {
        var camera = new Probe();
        var router = NewRouter(camera);

        router.Dispatch(Event(EditorPointerEventKind.Pressed));
        router.Dispatch(Event(kind));
        router.Dispatch(Event(kind));

        Assert.Equal(1, camera.CancelCalls);
        Assert.Equal(ViewportGestureState.Idle, router.State);
    }

    [Fact]
    public void Cancel_does_not_commit_and_next_gesture_starts_cleanly()
    {
        var camera = new Probe();
        var router = NewRouter(camera);

        router.Dispatch(Event(EditorPointerEventKind.Pressed));
        router.Dispatch(Event(EditorPointerEventKind.Cancel));
        router.Dispatch(Event(EditorPointerEventKind.Pressed, 2));
        router.Dispatch(Event(EditorPointerEventKind.Released, 2));

        Assert.Equal(2, camera.BeginCalls);
        Assert.Equal(1, camera.CancelCalls);
        Assert.Equal(1, camera.CommitCalls);
    }

    static ViewportInputRouter NewRouter(Probe camera) => new(
        ViewportD1ConsumerSet.Create(new GizmoViewportInputConsumer(new Probe(false)),
            new CameraViewportInputConsumer(camera),
            new PickingViewportInputConsumer(new Probe(false))), new CaptureSpy());

    static EditorPointerEvent Event(EditorPointerEventKind kind, long id = 1) => new(
        kind, new(10, 20), EditorPointerButtons.Middle, EditorPointerModifiers.None,
        0, id, new("test"), 1);

    sealed class Probe(bool claims = true) : IViewportD1ConsumerHandler
    {
        public int BeginCalls { get; private set; }
        public int CommitCalls { get; private set; }
        public int CancelCalls { get; private set; }
        public bool CanClaim(EditorPointerEvent pointer) => claims && pointer.Kind == EditorPointerEventKind.Pressed;
        public void Begin(ViewportGestureContext context) => BeginCalls++;
        public void Update(ViewportGestureContext context) { }
        public void Commit(ViewportGestureContext context) => CommitCalls++;
        public void Cancel(ViewportCancellationContext context) => CancelCalls++;
    }

    sealed class CaptureSpy : IViewportPointerCaptureCoordinator
    {
        public void Capture(long pointerId, GestureOwner owner) { }
        public void Release(long pointerId, GestureOwner owner) { }
    }
}
