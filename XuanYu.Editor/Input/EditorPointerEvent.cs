namespace XuanYu.Editor.Input;

public readonly record struct EditorPointerPosition(double X, double Y);

public readonly record struct ViewportPointerSource(string Id);

// Position is always viewport logical/DIP space. Native physical pixels are
// converted once by NativePointerEventAdapter at the source boundary.
// DpiScale records that boundary conversion; consumers must not convert again.
// WheelDelta is normalized to Avalonia-style wheel notches (120 native units = 1).
public readonly record struct EditorPointerEvent(
    EditorPointerEventKind Kind,
    EditorPointerPosition Position,
    EditorPointerButtons Buttons,
    EditorPointerModifiers Modifiers,
    double WheelDelta,
    long PointerId,
    ViewportPointerSource SourceSurface,
    double DpiScale);
