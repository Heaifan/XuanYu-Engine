using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.Editor.Input;

public enum ViewportGesturePhase { Idle, Active }

public readonly record struct ViewportGestureState(
    ViewportGesturePhase Phase, GestureOwner Owner, long PointerId, bool IsCaptured)
{
    public bool IsActive => Phase == ViewportGesturePhase.Active;
    public static ViewportGestureState Idle => new(ViewportGesturePhase.Idle, GestureOwner.None, 0, false);

    public static ViewportGestureState From(ViewportGestureContext? context) => context is null
        ? Idle : new(ViewportGesturePhase.Active, context.Owner, context.PointerId,
            context.Capture == ViewportGestureCapture.Pointer);
}
