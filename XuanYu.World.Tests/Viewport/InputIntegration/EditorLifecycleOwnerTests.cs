using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.World.Tests.Viewport.InputIntegration;

public sealed class EditorLifecycleOwnerTests
{
    [Theory]
    [InlineData(EditorPointerEventKind.ModeChanged)]
    [InlineData(EditorPointerEventKind.ToolChanged)]
    public void Lifecycle_terminal_releases_capture_and_clears_transient_state(
        EditorPointerEventKind terminal)
    {
        var consumer = new ProbeConsumer();
        var capture = new CaptureSpy();
        var cleared = new List<EditorPointerEventKind>();
        var composition = new ViewportInputComposition(
            [consumer], capture, cleared.Add);

        composition.Dispatch(Pointer(EditorPointerEventKind.Pressed, 3));
        var result = terminal == EditorPointerEventKind.ModeChanged
            ? composition.CancelForModeChange()
            : composition.CancelForToolChange();

        Assert.Equal(ViewportInputDispatchKind.Cancelled, result.Kind);
        Assert.Equal(1, consumer.CancelCount);
        Assert.Equal(1, capture.ReleaseCount);
        Assert.Equal([terminal], cleared);
        Assert.Equal(ViewportGestureState.Idle, composition.Router.State);
    }

    sealed class ProbeConsumer : IViewportInputConsumer
    {
        public GestureOwner Owner => GestureOwner.Gizmo;
        public int CancelCount { get; private set; }
        public bool CanBegin(EditorPointerEvent pointer, ViewportGestureState state) =>
            pointer.Kind == EditorPointerEventKind.Pressed;
        public ViewportInputDispatchResult Handle(
            EditorPointerEvent pointer,
            ViewportGestureState state) =>
            pointer.Kind == EditorPointerEventKind.Pressed
                ? ViewportInputDispatchResult.Captured
                : ViewportInputDispatchResult.Handled;
        public void Begin(ViewportGestureContext context) { }
        public void Update(ViewportGestureContext context) { }
        public void Commit(ViewportGestureContext context) { }
        public void Cancel(ViewportCancellationContext context) => CancelCount++;
    }

    sealed class CaptureSpy : IViewportPointerCaptureCoordinator
    {
        public int ReleaseCount { get; private set; }
        public void Capture(long pointerId, GestureOwner owner) { }
        public void Release(long pointerId, GestureOwner owner) => ReleaseCount++;
    }

    static EditorPointerEvent Pointer(EditorPointerEventKind kind, long id) => new(
        kind, new(1, 2), EditorPointerButtons.Left, EditorPointerModifiers.None,
        0, id, new("test"), 1);
}
