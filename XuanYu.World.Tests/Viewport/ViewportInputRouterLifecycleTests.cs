using XuanYu.Editor.Input;

namespace XuanYu.World.Tests.Viewport;

public sealed class ViewportInputRouterLifecycleTests
{
    [Fact]
    public void Press_claims_one_owner_and_active_move_stays_with_that_owner()
    {
        var camera = new Consumer(GestureOwner.Camera, ViewportInputDispatchResult.Captured);
        var gizmo = new Consumer(GestureOwner.Gizmo, ViewportInputDispatchResult.Handled);
        var router = Create(camera, gizmo);

        Assert.Equal(ViewportInputDispatchKind.Captured, router.Dispatch(Event(EditorPointerEventKind.Pressed)).Kind);
        Assert.Equal(GestureOwner.Camera, router.State.Owner);
        router.Dispatch(Event(EditorPointerEventKind.Move));

        Assert.Equal(2, camera.Calls.Count);
        Assert.Empty(gizmo.Calls);
    }

    [Fact]
    public void Release_releases_capture_and_returns_to_idle()
    {
        var capture = new CaptureSpy();
        var camera = new Consumer(GestureOwner.Camera, ViewportInputDispatchResult.Captured);
        var router = Create(capture, camera);

        router.Dispatch(Event(EditorPointerEventKind.Pressed));
        var result = router.Dispatch(Event(EditorPointerEventKind.Released));

        Assert.Equal(ViewportInputDispatchKind.Released, result.Kind);
        Assert.Equal(ViewportGestureState.Idle, router.State);
        Assert.Equal(new[] { "capture:Camera", "release:Camera" }, capture.Events);
    }

    [Fact]
    public void Cancel_and_capture_loss_are_terminal_and_next_press_can_claim()
    {
        var camera = new Consumer(GestureOwner.Camera, ViewportInputDispatchResult.Handled);
        var router = Create(camera);

        router.Dispatch(Event(EditorPointerEventKind.Pressed));
        Assert.Equal(ViewportInputDispatchKind.Cancelled, router.Dispatch(Event(EditorPointerEventKind.Cancel)).Kind);
        router.Dispatch(Event(EditorPointerEventKind.Pressed, 2));
        router.Dispatch(Event(EditorPointerEventKind.CaptureLost, 2));

        Assert.Equal(4, camera.Calls.Count);
        Assert.Equal(ViewportGestureState.Idle, router.State);
    }

    static ViewportInputRouter Create(params object[] items) => new(
        items.OfType<Consumer>(), items.OfType<CaptureSpy>().SingleOrDefault() ?? new CaptureSpy());

    static EditorPointerEvent Event(EditorPointerEventKind kind, long id = 1) => new(
        kind, new(10, 20), EditorPointerButtons.Left, EditorPointerModifiers.None,
        0, id, new("test"), 1);

    sealed class Consumer(GestureOwner owner, ViewportInputDispatchResult result) : IViewportInputConsumer
    {
        public GestureOwner Owner => owner;
        public List<EditorPointerEventKind> Calls { get; } = [];
        public ViewportInputDispatchResult Handle(EditorPointerEvent pointer, ViewportGestureState state)
        { Calls.Add(pointer.Kind); return result; }
    }

    sealed class CaptureSpy : IViewportPointerCaptureCoordinator
    {
        public List<string> Events { get; } = [];
        public void Capture(long pointerId, GestureOwner owner) => Events.Add($"capture:{owner}");
        public void Release(long pointerId, GestureOwner owner) => Events.Add($"release:{owner}");
    }
}
