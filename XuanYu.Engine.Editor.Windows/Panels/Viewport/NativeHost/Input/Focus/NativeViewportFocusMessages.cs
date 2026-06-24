using System.Runtime.InteropServices;

namespace XuanYu.Engine.Editor.Windows.Panels.Viewport.NativeHost.Input.Focus;

/// <summary>Win32 焦点消息处理。含 SetFocus P/Invoke 和焦点消息识别。</summary>
sealed class NativeViewportFocusMessages
{
    public const uint WmKillFocus = 0x0008;

    public bool IsKillFocus(uint msg) => msg == WmKillFocus;

    public void SetFocusTo(nint hwnd)
    {
        if (hwnd != 0) SetFocus(hwnd);
    }

    [DllImport("user32.dll")]
    static extern nint SetFocus(nint hwnd);
}
