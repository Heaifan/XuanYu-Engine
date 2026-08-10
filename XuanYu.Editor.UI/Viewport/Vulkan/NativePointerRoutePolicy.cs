namespace XuanYu.Editor.UI;

public enum NativePointerRoute
{
    None,
    LeftDown,
    MiddleDown,
    CameraPreview,
    RegionPreview,
    LeftPreview,
    LeftUp,
    MiddleUp,
    Wheel,
    CaptureChanged,
    KillFocus,
    CancelMode,
}

public static class NativePointerRoutePolicy
{
    public static NativePointerRoute Resolve(
        NativePointerMessage message, bool cameraActive, bool regionPreviewActive)
    {
        if (message.Message == NativePointerMessage.MiddleDown) return NativePointerRoute.MiddleDown;
        if (message.Message == NativePointerMessage.MiddleUp) return NativePointerRoute.MiddleUp;
        if (message.Message == NativePointerMessage.Move &&
            (cameraActive || message.IsMiddleButtonDown)) return NativePointerRoute.CameraPreview;
        if (message.Message == NativePointerMessage.Move && regionPreviewActive)
            return NativePointerRoute.RegionPreview;
        if (message.Message == NativePointerMessage.Move && message.IsLeftButtonDown)
            return NativePointerRoute.LeftPreview;
        return message.Message switch
        {
            NativePointerMessage.LeftDown => NativePointerRoute.LeftDown,
            NativePointerMessage.LeftUp => NativePointerRoute.LeftUp,
            NativePointerMessage.Wheel => NativePointerRoute.Wheel,
            NativePointerMessage.CaptureChanged => NativePointerRoute.CaptureChanged,
            NativePointerMessage.KillFocus => NativePointerRoute.KillFocus,
            NativePointerMessage.CancelMode => NativePointerRoute.CancelMode,
            _ => NativePointerRoute.None,
        };
    }
}
