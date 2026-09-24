using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Lifecycle;
using XuanYu.Editor.Input.Map;

namespace XuanYu.World.Tests.Viewport;

public sealed class MapInputConsumerContractTests
{
    [Theory]
    [InlineData(typeof(MapGeometryInputConsumer), GestureOwner.MapEdit)]
    [InlineData(typeof(RegionInputConsumer), GestureOwner.Region)]
    [InlineData(typeof(RoadInputConsumer), GestureOwner.Road)]
    [InlineData(typeof(MarkerInputConsumer), GestureOwner.Marker)]
    public void Consumer_exposes_frozen_owner(Type type, GestureOwner owner)
    {
        var consumer = Create(type, new Backend());
        Assert.Equal(owner, consumer.Owner);
        Assert.Equal(ViewportInputDispatchKind.Captured, consumer.Handle(Event(), Idle).Kind);
    }

    [Fact]
    public void Consumer_forwards_lifecycle_and_preserves_snap_as_helper()
    {
        var backend = new Backend { SnapHelperActive = true };
        IViewportInputConsumer consumer = new RoadInputConsumer(backend);
        var context = new ViewportGestureContext("Road", consumer.Owner, 1,
            ViewportGestureCapture.Pointer, Event());

        consumer.Handle(Event(), Idle);
        consumer.Begin(context);
        consumer.Handle(Event(EditorPointerEventKind.Move), Active(consumer.Owner));
        consumer.Commit(context);

        Assert.Equal(["begin", "update", "commit"], backend.Calls);
        Assert.True(backend.SnapHelperActive);
        Assert.NotEqual(GestureOwner.SnapInteractionHelper, consumer.Owner);
    }

    static IViewportInputConsumer Create(Type type, Backend backend) =>
        (IViewportInputConsumer)Activator.CreateInstance(type, backend)!;
    static ViewportGestureState Idle => ViewportGestureState.Idle;
    static ViewportGestureState Active(GestureOwner owner) =>
        new(ViewportGesturePhase.Active, owner, 1, true);
    static EditorPointerEvent Event(EditorPointerEventKind kind = EditorPointerEventKind.Pressed) => new(
        kind, new(1, 2), EditorPointerButtons.Left, EditorPointerModifiers.None, 0, 1, new("test"), 1);

    sealed class Backend : IMapEditingInputBackend
    {
        public List<string> Calls { get; } = [];
        public bool SnapHelperActive { get; set; }
        public bool CanBegin(EditorPointerEvent pointer, ViewportGestureState state) => true;
        public void Begin(ViewportGestureContext context) => Calls.Add("begin");
        public void Update(ViewportGestureContext context) => Calls.Add("update");
        public void Commit(ViewportGestureContext context) => Calls.Add("commit");
        public void Cancel(ViewportCancellationContext context) => Calls.Add("cancel");
    }
}
