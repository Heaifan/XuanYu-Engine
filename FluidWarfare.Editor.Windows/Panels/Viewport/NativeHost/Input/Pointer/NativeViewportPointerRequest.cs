namespace FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Input.Pointer;

/// <summary>从 Win32 wParam/lParam 解析的原生指针输入数据。</summary>
sealed record NativeViewportPointerRequest(
    NativeViewportPointerAction Action,
    int X,
    int Y,
    int ButtonCode,
    int WheelDelta,
    int ModifierFlags)
{
    public static NativeViewportPointerRequest FromMove(nint lParam) => new(
        NativeViewportPointerAction.Move,
        (short)(lParam.ToInt64() & 0xFFFF),
        (short)((lParam.ToInt64() >> 16) & 0xFFFF),
        0, 0, 0);

    public static NativeViewportPointerRequest FromLeave() => new(
        NativeViewportPointerAction.Leave, 0, 0, 0, 0, 0);

    public static NativeViewportPointerRequest FromWheel(nint wParam, nint lParam) => new(
        NativeViewportPointerAction.Wheel,
        (short)(lParam.ToInt64() & 0xFFFF),
        (short)((lParam.ToInt64() >> 16) & 0xFFFF),
        0,
        (short)((wParam.ToInt64() >> 16) & 0xFFFF),
        (int)wParam & 0xFFFF);

    public static NativeViewportPointerRequest FromButtonDown(NativeViewportPointerAction action, nint lParam, int buttonCode) => new(
        action,
        (short)(lParam.ToInt64() & 0xFFFF),
        (short)((lParam.ToInt64() >> 16) & 0xFFFF),
        buttonCode, 0, 0);

    public static NativeViewportPointerRequest FromButtonUp(NativeViewportPointerAction action, nint lParam, int buttonCode) => new(
        action,
        (short)(lParam.ToInt64() & 0xFFFF),
        (short)((lParam.ToInt64() >> 16) & 0xFFFF),
        buttonCode, 0, 0);
}
