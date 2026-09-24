using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Consumers;
using XuanYu.Editor.Input.Lifecycle;
using XuanYu.Editor.Input.Map;

namespace XuanYu.World.Tests.Viewport.InputIntegration;

public sealed class ConsumerLifecycleIntegrationTests
{
    [Fact] public void D1_cancel_then_D2_begin_has_one_clean_owner() { var d1 = new CameraHandler(); var d2 = new Backend { Eligible = false }; var r = Router(new CameraViewportInputConsumer(d1), new RegionInputConsumer(d2)); r.Dispatch(Event(EditorPointerEventKind.Pressed, 1)); r.Dispatch(Event(EditorPointerEventKind.Escape, 1)); d1.Eligible = false; d2.Eligible = true; r.Dispatch(Event(EditorPointerEventKind.Pressed, 2)); Assert.Equal(GestureOwner.Region, r.State.Owner); Assert.Equal(1, d1.CancelCount); }
    [Fact] public void D2_cancel_then_D1_begin_has_one_clean_owner() { var d2 = new Backend(); var d1 = new CameraHandler(false); var r = Router(new RegionInputConsumer(d2), new CameraViewportInputConsumer(d1)); r.Dispatch(Event(EditorPointerEventKind.Pressed, 1)); r.Dispatch(Event(EditorPointerEventKind.CaptureLost, 1)); d2.Eligible = false; d1.Eligible = true; r.Dispatch(Event(EditorPointerEventKind.Pressed, 2)); Assert.Equal(GestureOwner.Camera, r.State.Owner); Assert.Equal(1, d2.CancelCount); }
    [Theory]
    [InlineData(EditorPointerEventKind.CaptureLost)]
    [InlineData(EditorPointerEventKind.FocusLost)]
    [InlineData(EditorPointerEventKind.WindowDeactivated)]
    [InlineData(EditorPointerEventKind.ToolChanged)]
    [InlineData(EditorPointerEventKind.ModeChanged)]
    [InlineData(EditorPointerEventKind.ViewportDisposed)]
    public void Any_global_cancel_releases_capture_once(EditorPointerEventKind kind)
    { var backend = new Backend(); var capture = new CaptureSpy(); var r = new ViewportInputRouter([new RegionInputConsumer(backend)], capture); r.Dispatch(Event(EditorPointerEventKind.Pressed, 1)); r.Dispatch(Event(kind, 2)); r.Dispatch(Event(kind, 3)); Assert.Equal(ViewportGestureState.Idle, r.State); Assert.Equal(1, backend.CancelCount); Assert.Equal(1, capture.ReleaseCount); Assert.Equal(0, backend.CommitCount); }
    [Fact] public void Consecutive_cross_consumer_gestures_do_not_leak_state()
    { var first = new Backend(); var second = new Backend { Eligible = false }; var r = Router(new RegionInputConsumer(first), new RoadInputConsumer(second)); r.Dispatch(Event(EditorPointerEventKind.Pressed, 1)); r.Dispatch(Event(EditorPointerEventKind.Released, 1)); first.Eligible = false; second.Eligible = true; r.Dispatch(Event(EditorPointerEventKind.Pressed, 2)); r.Dispatch(Event(EditorPointerEventKind.Released, 2)); Assert.Equal(1, first.CommitCount); Assert.Equal(1, second.CommitCount); Assert.Equal(ViewportGestureState.Idle, r.State); }
    [Fact] public void CaptureLost_allows_another_consumer_to_begin()
    { var first = new Backend(); var second = new Backend { Eligible = false }; var r = Router(new RegionInputConsumer(first), new RoadInputConsumer(second)); r.Dispatch(Event(EditorPointerEventKind.Pressed, 1)); r.Dispatch(Event(EditorPointerEventKind.CaptureLost, 1)); first.Eligible = false; second.Eligible = true; r.Dispatch(Event(EditorPointerEventKind.Pressed, 2)); Assert.Equal(GestureOwner.Road, r.State.Owner); Assert.Equal(1, first.CancelCount); }

    static ViewportInputRouter Router(params IViewportInputConsumer[] c) => new(c, new CaptureSpy());
    static EditorPointerEvent Event(EditorPointerEventKind kind, long id) => new(kind, new(1, 2), EditorPointerButtons.Left, EditorPointerModifiers.None, 0, id, new("test"), 1);
    sealed class CaptureSpy : IViewportPointerCaptureCoordinator { public int ReleaseCount { get; private set; } public void Capture(long id, GestureOwner owner) { } public void Release(long id, GestureOwner owner) => ReleaseCount++; }
    sealed class CameraHandler(bool eligible = true) : IViewportD1ConsumerHandler { public bool Eligible { get; set; } = eligible; public int CancelCount { get; private set; } public bool CanClaim(EditorPointerEvent p) => Eligible; public void Begin(ViewportGestureContext c) { } public void Update(ViewportGestureContext c) { } public void Commit(ViewportGestureContext c) { } public void Cancel(ViewportCancellationContext c) => CancelCount++; }
    sealed class Backend : IMapEditingInputBackend { public bool Eligible { get; set; } = true; public int CancelCount { get; private set; } public int CommitCount { get; private set; } public bool CanBegin(EditorPointerEvent p, ViewportGestureState s) => Eligible; public void Begin(ViewportGestureContext c) { } public void Update(ViewportGestureContext c) { } public void Commit(ViewportGestureContext c) => CommitCount++; public void Cancel(ViewportCancellationContext c) => CancelCount++; }
}
