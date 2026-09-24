using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.World.Tests.Viewport.InputLifecycle;

public sealed class ViewportGestureLifecycleTests
{
    static readonly EditorPointerEvent Input = new(
        EditorPointerEventKind.Pressed, new(12, 34), EditorPointerButtons.Left,
        EditorPointerModifiers.Control, 0, 7, new("main"), 1.25);

    [Theory]
    [InlineData(ViewportCancellationReason.Escape)]
    [InlineData(ViewportCancellationReason.CaptureLost)]
    [InlineData(ViewportCancellationReason.FocusLost)]
    [InlineData(ViewportCancellationReason.WindowDeactivated)]
    [InlineData(ViewportCancellationReason.ToolChanged)]
    [InlineData(ViewportCancellationReason.ModeChanged)]
    [InlineData(ViewportCancellationReason.ViewportDisposed)]
    [InlineData(ViewportCancellationReason.PlatformInterrupted)]
    [InlineData(ViewportCancellationReason.ExplicitCancel)]
    public void Cancel_reasons_share_one_contract(ViewportCancellationReason reason)
    {
        var probe = new LifecycleProbe();
        var lifecycle = NewLifecycle(probe);
        lifecycle.Begin(Context());

        var result = lifecycle.Cancel(reason);

        Assert.Equal(ViewportGestureLifecycleState.Idle, result.State);
        Assert.Equal(ViewportGestureTerminalKind.Canceled, result.Terminal);
        Assert.Equal(reason, probe.CancelReason);
    }

    [Fact]
    public void Cancel_notifies_with_context_before_release_and_cleanup()
    {
        var probe = new LifecycleProbe();
        var lifecycle = NewLifecycle(probe);
        var context = Context();
        lifecycle.Begin(context);

        var result = lifecycle.Cancel(ViewportCancellationReason.CaptureLost);

        Assert.Equal(context, probe.CanceledContext);
        Assert.Equal(new[] { "Capture", "Begin", "Cancel", "ReleaseCapture", "ClearTemporary" }, probe.Order);
        Assert.True(result.CaptureReleased);
        Assert.True(result.TemporaryStateCleared);
        Assert.Null(lifecycle.Current);
    }

    [Fact]
    public void Repeated_cancel_and_late_commit_are_no_ops()
    {
        var probe = new LifecycleProbe();
        var lifecycle = NewLifecycle(probe);
        lifecycle.Begin(Context());

        var first = lifecycle.Cancel(ViewportCancellationReason.ExplicitCancel);
        var second = lifecycle.Cancel(ViewportCancellationReason.ExplicitCancel);
        var lateCommit = lifecycle.Commit();

        Assert.Equal(ViewportGestureTerminalKind.Canceled, first.Terminal);
        Assert.Equal(ViewportGestureTerminalKind.None, second.Terminal);
        Assert.Equal(ViewportGestureTerminalKind.None, lateCommit.Terminal);
        Assert.Equal(1, probe.CancelCount);
        Assert.Equal(0, probe.CommitCount);
    }

    [Fact]
    public void Update_replaces_current_input_without_changing_owner()
    {
        var probe = new LifecycleProbe();
        var lifecycle = NewLifecycle(probe);
        lifecycle.Begin(Context());
        var moved = Input with { Kind = EditorPointerEventKind.Move, Position = new(20, 30) };

        Assert.True(lifecycle.Update(moved));
        Assert.Equal(GestureOwner.Gizmo, lifecycle.Current!.Owner);
        Assert.Equal(moved, lifecycle.Current.Input);
    }

    static ViewportGestureLifecycle NewLifecycle(LifecycleProbe probe) =>
        new(probe, probe, context => probe.Order.Add("ClearTemporary"));

    static ViewportGestureContext Context(long pointerId = 7) =>
        new("TransformDrag", GestureOwner.Gizmo, pointerId,
            ViewportGestureCapture.Pointer, Input with { PointerId = pointerId });
}
