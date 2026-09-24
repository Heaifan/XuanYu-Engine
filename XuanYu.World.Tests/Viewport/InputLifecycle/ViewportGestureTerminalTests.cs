using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.World.Tests.Viewport.InputLifecycle;

public sealed class ViewportGestureTerminalTests
{
    static readonly EditorPointerEvent Input = new(
        EditorPointerEventKind.Pressed, new(12, 34), EditorPointerButtons.Left,
        EditorPointerModifiers.Control, 0, 7, new("main"), 1.25);

    [Fact]
    public void Commit_is_distinct_from_cancel_and_allows_next_begin()
    {
        var probe = new LifecycleProbe();
        var lifecycle = NewLifecycle(probe);

        lifecycle.Begin(Context());
        var committed = lifecycle.Commit();
        var beganAgain = lifecycle.Begin(Context(8));

        Assert.Equal(ViewportGestureTerminalKind.Committed, committed.Terminal);
        Assert.Equal(1, probe.CommitCount);
        Assert.True(beganAgain);
        Assert.Equal(2, probe.BeginCount);
        Assert.Equal(ViewportGestureLifecycleState.Active, lifecycle.State);
    }

    [Fact]
    public void Cancel_without_owner_is_safe_and_does_not_release()
    {
        var probe = new LifecycleProbe();
        var lifecycle = NewLifecycle(probe);

        var result = lifecycle.Cancel(ViewportCancellationReason.ExplicitCancel);

        Assert.Equal(ViewportGestureTerminalKind.None, result.Terminal);
        Assert.False(result.CaptureReleased);
        Assert.Empty(probe.Order);
    }

    static ViewportGestureLifecycle NewLifecycle(LifecycleProbe probe) =>
        new(probe, probe, context => probe.Order.Add("ClearTemporary"));

    static ViewportGestureContext Context(long pointerId = 7) =>
        new("TransformDrag", GestureOwner.Gizmo, pointerId,
            ViewportGestureCapture.Pointer, Input with { PointerId = pointerId });
}
