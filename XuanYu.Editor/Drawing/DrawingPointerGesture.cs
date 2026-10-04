namespace XuanYu.Editor.Drawing;

// A session can remain Armed/Drawing while one pointer gesture is owned by Camera.
// One gesture has one live owner; the long-lived session does not own that capture.
public enum DrawingPointerGestureOwner
{
    None,
    Drawing,
    Camera
}

public sealed record DrawingPointerGesture(DrawingPointerGestureOwner Owner);
