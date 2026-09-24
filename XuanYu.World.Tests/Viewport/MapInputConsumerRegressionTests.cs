using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Map;

namespace XuanYu.World.Tests.Viewport;

public sealed class MapInputConsumerRegressionTests
{
    [Fact]
    public void Consecutive_map_gestures_do_not_inherit_cancelled_state_or_duplicate_commit()
    {
        var backend = new MapEditingInputBackend((_, _) => true);
        var router = new ViewportInputRouter(
            [new MapGeometryInputConsumer(backend)], new CaptureSpy());

        router.Dispatch(Event(EditorPointerEventKind.Pressed, 1));
        router.Dispatch(Event(EditorPointerEventKind.Cancel, 1));
        router.Dispatch(Event(EditorPointerEventKind.Pressed, 2));
        router.Dispatch(Event(EditorPointerEventKind.Move, 2));
        router.Dispatch(Event(EditorPointerEventKind.Released, 2));

        Assert.Equal(1, backend.CancelCount);
        Assert.Equal(1, backend.CommitCount);
        Assert.Equal(ViewportGestureState.Idle, router.State);
        Assert.False(backend.Snapshot.HasPreview || backend.Snapshot.HasTemporaryGeometry);
    }

    sealed class CaptureSpy : IViewportPointerCaptureCoordinator
    {
        public void Capture(long pointerId, GestureOwner owner) { }
        public void Release(long pointerId, GestureOwner owner) { }
    }

    static EditorPointerEvent Event(EditorPointerEventKind kind, long id) => new(
        kind, new(1, 2), EditorPointerButtons.Left, EditorPointerModifiers.None, 0, id, new("test"), 1);
}
