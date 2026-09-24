using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.World.Tests.Viewport.InputLifecycle;

sealed class LifecycleProbe : IViewportGestureConsumer, IViewportPointerCaptureCoordinator
{
    public readonly List<string> Order = [];
    public ViewportGestureContext? CanceledContext { get; private set; }
    public ViewportCancellationReason? CancelReason { get; private set; }
    public int BeginCount { get; private set; }
    public int CommitCount { get; private set; }
    public int CancelCount { get; private set; }

    public void Begin(ViewportGestureContext context)
    {
        BeginCount++;
        Order.Add("Begin");
    }

    public void Update(ViewportGestureContext context) => Order.Add("Update");

    public void Capture(long pointerId, GestureOwner owner) => Order.Add("Capture");
    public void Release(long pointerId, GestureOwner owner) => Order.Add("ReleaseCapture");

    public void Commit(ViewportGestureContext context)
    {
        CommitCount++;
        Order.Add("Commit");
    }

    public void Cancel(ViewportCancellationContext context)
    {
        CancelCount++;
        CanceledContext = context.Gesture;
        CancelReason = context.Reason;
        Order.Add("Cancel");
    }
}
