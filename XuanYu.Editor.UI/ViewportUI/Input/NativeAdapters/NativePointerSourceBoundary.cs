namespace XuanYu.Editor.UI;

public static class NativePointerSourceBoundary
{
    public static bool IsPointerMessage(uint message) => message is
        NativePointerMessage.Move or NativePointerMessage.MouseLeave or
        NativePointerMessage.LeftDown or
        NativePointerMessage.LeftUp or NativePointerMessage.RightDown or
        NativePointerMessage.RightUp or NativePointerMessage.MiddleDown or
        NativePointerMessage.MiddleUp or NativePointerMessage.Wheel or
        NativePointerMessage.CaptureChanged or NativePointerMessage.KillFocus or
        NativePointerMessage.CancelMode;

    public static (int X, int Y) ToClientPoint(
        int screenX, int screenY, int clientOriginX, int clientOriginY) =>
        (screenX - clientOriginX, screenY - clientOriginY);
}
