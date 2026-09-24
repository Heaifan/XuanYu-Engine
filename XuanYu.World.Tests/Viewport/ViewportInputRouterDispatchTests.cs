using XuanYu.Editor.Input;

namespace XuanYu.World.Tests.Viewport;

public sealed class ViewportInputRouterDispatchTests
{
    [Fact]
    public void Observers_do_not_claim_and_wheel_is_dispatched_without_owner()
    {
        var observer = new Consumer(GestureOwner.SnapInteractionHelper, ViewportInputDispatchResult.Observed);
        var picker = new Consumer(GestureOwner.Picking, ViewportInputDispatchResult.Handled);
        var router = new ViewportInputRouter([observer, picker], new CaptureSpy());

        var result = router.Dispatch(Event(EditorPointerEventKind.Wheel));

        Assert.Equal(ViewportInputDispatchKind.Handled, result.Kind);
        Assert.Equal(GestureOwner.None, router.State.Owner);
        Assert.Equal([EditorPointerEventKind.Wheel], observer.Calls);
        Assert.Equal([EditorPointerEventKind.Wheel], picker.Calls);
    }

    [Fact]
    public void Active_owner_rejects_a_second_pointer_and_non_owner_cannot_take_over()
    {
        var camera = new Consumer(GestureOwner.Camera, ViewportInputDispatchResult.Captured);
        var gizmo = new Consumer(GestureOwner.Gizmo, ViewportInputDispatchResult.Captured);
        var router = new ViewportInputRouter([camera, gizmo], new CaptureSpy());

        router.Dispatch(Event(EditorPointerEventKind.Pressed));
        var result = router.Dispatch(Event(EditorPointerEventKind.Pressed, 2));

        Assert.Equal(ViewportInputDispatchKind.Ignored, result.Kind);
        Assert.Equal(GestureOwner.Camera, router.State.Owner);
        Assert.Empty(gizmo.Calls);
    }

    sealed class Consumer(GestureOwner owner, ViewportInputDispatchResult result) : IViewportInputConsumer
    {
        public GestureOwner Owner => owner;
        public List<EditorPointerEventKind> Calls { get; } = [];
        public ViewportInputDispatchResult Handle(EditorPointerEvent pointer, ViewportGestureState state)
        { Calls.Add(pointer.Kind); return result; }
    }

    sealed class CaptureSpy : IViewportPointerCaptureCoordinator
    {
        public void Capture(long pointerId, GestureOwner owner) { }
        public void Release(long pointerId, GestureOwner owner) { }
    }

    static EditorPointerEvent Event(EditorPointerEventKind kind, long id = 1) => new(
        kind, new(1, 2), EditorPointerButtons.None, EditorPointerModifiers.None,
        kind == EditorPointerEventKind.Wheel ? 1 : 0, id, new("test"), 1);
}
