using System.Runtime.InteropServices;

namespace XuanYu.Engine.Editor.Windows.Panels.Viewport.NativeHost.Input.Pointer;

/// <summary>Win32 鼠标捕获管理。封装 SetCapture / ReleaseCapture 和捕获状态。</summary>
sealed class NativeViewportMouseCapture
{
    bool _captured;

    public bool IsCaptured => _captured;

    public void Capture(nint hwnd)
    {
        if (hwnd == 0) return;
        SetCapture(hwnd);
        _captured = true;
    }

    public void Release()
    {
        ReleaseCapture();
        _captured = false;
    }

    public void ClearState() => _captured = false;

    [DllImport("user32.dll", EntryPoint = "SetCapture")]
    static extern nint SetCapture(nint hwnd);

    [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool ReleaseCapture();
}
