namespace FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Input;

/// <summary>Win32 原生指针消息解析与分发。只处理消息翻译，不含编辑器业务。</summary>
sealed class NativeViewportPointerMessages
{
    // Win32 消息常量
    public const uint WmLButtonDown = 0x0201;
    public const uint WmLButtonUp = 0x0202;
    public const uint WmMButtonDown = 0x0207;
    public const uint WmMButtonUp = 0x0208;
    public const uint WmMouseMove = 0x0200;
    public const uint WmMouseLeave = 0x02A3;
    public const uint WmMouseWheel = 0x020A;
    public const uint WmCaptureChanged = 0x0215;

    public const int VkMButton = 0x04;

    public bool IsPointerMessage(uint msg) => msg switch
    {
        WmLButtonDown or WmLButtonUp or WmMButtonDown or WmMButtonUp
            or WmMouseMove or WmMouseLeave or WmMouseWheel or WmCaptureChanged => true,
        _ => false
    };

    /// <summary>解析指针消息，返回请求数据。返回 null 表示不是指针消息。</summary>
    public NativeViewportPointerRequest? Parse(uint msg, nint wParam, nint lParam)
    {
        return msg switch
        {
            WmMButtonDown => NativeViewportPointerRequest.FromButtonDown(
                NativeViewportPointerAction.MiddleDown, lParam, VkMButton),
            WmMButtonUp => NativeViewportPointerRequest.FromButtonUp(
                NativeViewportPointerAction.MiddleUp, lParam, VkMButton),
            WmMouseMove => NativeViewportPointerRequest.FromMove(lParam),
            WmMouseLeave => NativeViewportPointerRequest.FromLeave(),
            WmMouseWheel => NativeViewportPointerRequest.FromWheel(wParam, lParam),
            WmLButtonDown => NativeViewportPointerRequest.FromButtonDown(
                NativeViewportPointerAction.LeftDown, lParam, 1),
            WmLButtonUp => NativeViewportPointerRequest.FromButtonUp(
                NativeViewportPointerAction.LeftUp, lParam, 1),
            WmCaptureChanged => new NativeViewportPointerRequest(
                NativeViewportPointerAction.CaptureChanged, 0, 0, 0, 0, 0),
            _ => null
        };
    }
}
