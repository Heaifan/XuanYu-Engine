namespace XuanYu.Editor.Input;

public sealed class ViewportInputCaptureCoordinator : IViewportPointerCaptureCoordinator
{
    public GestureOwner Owner { get; private set; }
    public long PointerId { get; private set; }
    public bool IsCaptured => Owner != GestureOwner.None;

    public void Capture(long pointerId, GestureOwner owner)
    {
        if (IsCaptured) throw new InvalidOperationException("视口输入捕获已由其他 Owner 持有。");
        PointerId = pointerId;
        Owner = owner;
    }

    public void Release(long pointerId, GestureOwner owner)
    {
        if (!IsCaptured || PointerId != pointerId || Owner != owner) return;
        PointerId = 0;
        Owner = GestureOwner.None;
    }
}
