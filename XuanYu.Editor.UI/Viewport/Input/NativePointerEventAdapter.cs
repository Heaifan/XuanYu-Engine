using XuanYu.Editor.Input;

namespace XuanYu.Editor.UI;

public static class NativePointerEventAdapter
{
    public static EditorPointerEvent Convert(
        NativePointerMessage message, ViewportPointerSource source, double dpiScale)
    {
        var scale = dpiScale > 0 ? dpiScale : 1;
        return new(MapKind(message.Message),
            new(message.PhysicalX / scale, message.PhysicalY / scale),
            MapButtons(message.Buttons), MapModifiers(message),
            message.Message == NativePointerMessage.Wheel ? message.WheelDelta / 120d : 0,
            1, source, scale);
    }

    static EditorPointerEventKind MapKind(uint message) => message switch
    {
        NativePointerMessage.Move => EditorPointerEventKind.Move,
        NativePointerMessage.LeftDown or NativePointerMessage.RightDown => EditorPointerEventKind.Pressed,
        NativePointerMessage.LeftUp or NativePointerMessage.RightUp => EditorPointerEventKind.Released,
        NativePointerMessage.MiddleDown => EditorPointerEventKind.Pressed,
        NativePointerMessage.MiddleUp => EditorPointerEventKind.Released,
        NativePointerMessage.Wheel => EditorPointerEventKind.Wheel,
        NativePointerMessage.CaptureChanged => EditorPointerEventKind.CaptureLost,
        NativePointerMessage.KillFocus => EditorPointerEventKind.FocusLost,
        NativePointerMessage.CancelMode => EditorPointerEventKind.Cancel,
        _ => throw new ArgumentOutOfRangeException(nameof(message)),
    };

    static EditorPointerButtons MapButtons(int buttons) =>
        ((buttons & 1) != 0 ? EditorPointerButtons.Left : EditorPointerButtons.None) |
        ((buttons & 2) != 0 ? EditorPointerButtons.Right : EditorPointerButtons.None) |
        ((buttons & 16) != 0 ? EditorPointerButtons.Middle : EditorPointerButtons.None);

    static EditorPointerModifiers MapModifiers(NativePointerMessage message) =>
        (message.IsShiftDown ? EditorPointerModifiers.Shift : EditorPointerModifiers.None) |
        (message.IsControlDown ? EditorPointerModifiers.Control : EditorPointerModifiers.None) |
        (message.IsAltDown ? EditorPointerModifiers.Alt : EditorPointerModifiers.None);
}
