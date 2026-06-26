using System.Diagnostics;
using System.Runtime.InteropServices;
using XuanYu.Engine.Editor.Windows.Shell.Diagnostics;

namespace XuanYu.Engine.Editor.Windows.Panels.Viewport.NativeHost.Input.Pointer;

/// <summary>Win32 鼠标捕获管理。封装 SetCapture / ReleaseCapture 和捕获状态。</summary>
sealed class NativeViewportMouseCapture
{
    bool _captured;
    nint _capturedHwnd;
    string _source = string.Empty;
    string _button = string.Empty;
    public bool IsCaptured => _captured;

    public void Capture(nint hwnd, string source, string button)
    {
        if (hwnd == 0)
        {
            Log("捕获开始", $"来源={source} 按钮={button} hwnd=0 跳过原因=hwnd无效");
            return;
        }
        var previousCapture = GetCapture();
        SetCapture(hwnd);
        _captured = true;
        _capturedHwnd = hwnd;
        _source = source;
        _button = button;
        Log("捕获开始", $"来源={source} 按钮={button} hwnd={hwnd:X} Win32当前捕获={previousCapture:X} 内部状态=已捕获");
    }

    public void Release(nint ownerHwnd, string reason)
    {
        var current = GetCapture();
        var actuallyReleased = ownerHwnd != 0 && current == ownerHwnd;
        var source = _source;
        var button = _button;
        if (actuallyReleased) ReleaseCapture();
        Log("释放", $"来源={source} 原因={reason} 按钮={button} 是否调用ReleaseCapture={actuallyReleased} Win32当前捕获={current:X} 内部状态={(_captured ? "已释放" : "未捕获")}");
        _captured = false;
        _capturedHwnd = 0;
        _source = string.Empty;
        _button = string.Empty;
    }

    public void ClearState(string reason)
    {
        var current = GetCapture();
        Log("CaptureChanged同步", $"原因={reason} Win32当前捕获={current:X} 来源={_source} 按钮={_button} 内部状态=已同步清除");
        _captured = false;
        _capturedHwnd = 0;
        _source = string.Empty;
        _button = string.Empty;
    }

    static void Log(string stage, string message)
    {
        var line = $"[鼠标捕获] 阶段={stage} {message}";
        if (Environment.GetEnvironmentVariable("FW_INPUT_TRACE") == "1") Debug.WriteLine(line);
        EditorProbe.Write("MouseCapture", stage, message);
    }

    [DllImport("user32.dll", EntryPoint = "SetCapture")] static extern nint SetCapture(nint hwnd);
    [DllImport("user32.dll", EntryPoint = "ReleaseCapture")][return: MarshalAs(UnmanagedType.Bool)] static extern bool ReleaseCapture();
    [DllImport("user32.dll", EntryPoint = "GetCapture")] static extern nint GetCapture();
}
