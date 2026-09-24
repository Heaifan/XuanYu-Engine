using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.World.Tests.Viewport;

public sealed class ViewportInputRouterMapArbitrationTests
{
    [Fact]
    public void Router_selects_single_highest_priority_eligible_owner()
    {
        var low = new Consumer(GestureOwner.MapEdit, 1, true);
        var high = new Consumer(GestureOwner.Region, 2, true);
        var router = new ViewportInputRouter([low, high], new CaptureSpy());

        Assert.Equal(ViewportInputDispatchKind.Captured, router.Dispatch(Event()).Kind);
        Assert.Equal(GestureOwner.Region, router.State.Owner);
        Assert.Empty(low.Calls);
        Assert.Single(high.Calls);
    }

    [Fact]
    public void Ineligible_consumer_is_not_called_and_second_pointer_is_ignored()
    {
        var map = new Consumer(GestureOwner.MapEdit, 1, false);
        var road = new Consumer(GestureOwner.Road, 2, true);
        var router = new ViewportInputRouter([map, road], new CaptureSpy());

        router.Dispatch(Event());
        var result = router.Dispatch(Event(2));

        Assert.Equal(ViewportInputDispatchKind.Ignored, result.Kind);
        Assert.Empty(map.Calls);
        Assert.Single(road.Calls);
    }

    sealed class Consumer(GestureOwner owner, int priority, bool eligible) : IViewportInputConsumer
    {
        public GestureOwner Owner => owner;
        public int BeginPriority => priority;
        public List<long> Calls { get; } = [];
        public bool CanBegin(EditorPointerEvent pointer, ViewportGestureState state) => eligible;
        public ViewportInputDispatchResult Handle(EditorPointerEvent pointer, ViewportGestureState state)
        { Calls.Add(pointer.PointerId); return ViewportInputDispatchResult.Captured; }
        public void Begin(ViewportGestureContext context) { }
        public void Update(ViewportGestureContext context) { }
        public void Commit(ViewportGestureContext context) { }
        public void Cancel(ViewportCancellationContext context) { }
    }

    sealed class CaptureSpy : IViewportPointerCaptureCoordinator
    {
        public void Capture(long pointerId, GestureOwner owner) { }
        public void Release(long pointerId, GestureOwner owner) { }
    }

    static EditorPointerEvent Event(long id = 1) => new(
        EditorPointerEventKind.Pressed, new(1, 2), EditorPointerButtons.Left,
        EditorPointerModifiers.None, 0, id, new("test"), 1);
}
