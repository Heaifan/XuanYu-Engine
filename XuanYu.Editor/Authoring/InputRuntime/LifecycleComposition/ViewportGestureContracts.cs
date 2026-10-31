using XuanYu.Editor.Input;

namespace XuanYu.Editor.Input.Lifecycle;

public enum ViewportGestureLifecycleState { Idle, Active }

public enum ViewportGestureTerminalKind { None, Committed, Canceled }

public enum ViewportGestureCapture { None, Pointer }

public sealed record ViewportGestureContext(
    string Gesture,
    GestureOwner Owner,
    long PointerId,
    ViewportGestureCapture Capture,
    EditorPointerEvent Input);

public sealed record ViewportCancellationContext(
    ViewportGestureContext Gesture,
    ViewportCancellationReason Reason);

public sealed record ViewportGestureLifecycleResult(
    ViewportGestureLifecycleState State,
    ViewportGestureTerminalKind Terminal,
    bool ConsumerNotified,
    bool CaptureReleased,
    bool TemporaryStateCleared)
{
    public static ViewportGestureLifecycleResult NoOp { get; } = new(
        ViewportGestureLifecycleState.Idle,
        ViewportGestureTerminalKind.None, false, false, false);
}

public interface IViewportGestureConsumer
{
    void Begin(ViewportGestureContext context);
    void Update(ViewportGestureContext context);
    void Commit(ViewportGestureContext context);
    void Cancel(ViewportCancellationContext context);
}
