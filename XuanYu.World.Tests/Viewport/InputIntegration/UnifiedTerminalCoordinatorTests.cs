using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.World.Tests.Viewport.InputIntegration;

public sealed partial class UnifiedTerminalCoordinatorTests
{
    [Theory]
    [InlineData(EditorPointerEventKind.Escape)]
    [InlineData(EditorPointerEventKind.CaptureLost)]
    [InlineData(EditorPointerEventKind.FocusLost)]
    [InlineData(EditorPointerEventKind.WindowDeactivated)]
    [InlineData(EditorPointerEventKind.Cancel)]
    [InlineData(EditorPointerEventKind.ViewportDisposed)]
    [InlineData(EditorPointerEventKind.ToolChanged)]
    [InlineData(EditorPointerEventKind.ModeChanged)]
    public void Terminal_event_closes_router_and_notifies_once(
        EditorPointerEventKind terminal)
    {
        var consumer = new ConsumerProbe();
        var capture = new CaptureProbe();
        var notifications = new List<EditorPointerEventKind>();
        var composition = new ViewportInputComposition(
            [consumer], capture, notifications.Add);

        composition.Dispatch(Event(EditorPointerEventKind.Pressed, 7));
        composition.Dispatch(Event(terminal, 7));
        composition.Dispatch(Event(terminal, 7));

        Assert.Equal(ViewportGestureState.Idle, composition.Router.State);
        Assert.Equal(1, consumer.CancelCount);
        Assert.Equal(1, capture.ReleaseCount);
        Assert.Equal([terminal], notifications);
    }

    [Fact]
    public void Idle_terminal_is_forwarded_once_until_a_new_gesture_begins()
    {
        var notifications = new List<EditorPointerEventKind>();
        var composition = new ViewportInputComposition(
            [new ConsumerProbe()], new CaptureProbe(), notifications.Add);

        composition.Dispatch(Event(EditorPointerEventKind.WindowDeactivated, 1));
        composition.Dispatch(Event(EditorPointerEventKind.WindowDeactivated, 1));
        composition.Dispatch(Event(EditorPointerEventKind.Pressed, 2));
        composition.Dispatch(Event(EditorPointerEventKind.CaptureLost, 2));

        Assert.Equal([
            EditorPointerEventKind.WindowDeactivated,
            EditorPointerEventKind.CaptureLost], notifications);
    }

    [Fact]
    public void Terminal_callback_reentry_does_not_cancel_consumer_twice()
    {
        var consumer = new ConsumerProbe();
        ViewportInputComposition? composition = null;
        composition = new ViewportInputComposition(
            [consumer], new CaptureProbe(), _ =>
                composition!.Terminate(EditorPointerEventKind.Cancel));

        composition.Dispatch(Event(EditorPointerEventKind.Pressed, 3));
        composition.Dispatch(Event(EditorPointerEventKind.Escape, 3));

        Assert.Equal(1, consumer.CancelCount);
        Assert.Equal(ViewportGestureState.Idle, composition.Router.State);
    }

    sealed class ConsumerProbe : IViewportInputConsumer
    {
        public GestureOwner Owner => GestureOwner.Gizmo;
        public int CancelCount { get; private set; }
        public bool CanBegin(EditorPointerEvent p, ViewportGestureState s) =>
            p.Kind == EditorPointerEventKind.Pressed;
        public ViewportInputDispatchResult Handle(
            EditorPointerEvent p, ViewportGestureState s) =>
            p.Kind == EditorPointerEventKind.Pressed
                ? ViewportInputDispatchResult.Captured
                : ViewportInputDispatchResult.Handled;
        public void Begin(ViewportGestureContext c) { }
        public void Update(ViewportGestureContext c) { }
        public void Commit(ViewportGestureContext c) { }
        public void Cancel(ViewportCancellationContext c) => CancelCount++;
    }

    sealed class CaptureProbe : IViewportPointerCaptureCoordinator
    {
        public int ReleaseCount { get; private set; }
        public void Capture(long id, GestureOwner owner) { }
        public void Release(long id, GestureOwner owner) => ReleaseCount++;
    }

    static EditorPointerEvent Event(EditorPointerEventKind kind, long id) => new(
        kind, new(1, 2), EditorPointerButtons.Left, EditorPointerModifiers.None,
        0, id, new("test"), 1);
}
