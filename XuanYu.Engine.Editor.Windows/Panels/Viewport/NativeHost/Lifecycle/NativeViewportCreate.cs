using System.ComponentModel;
using System.Runtime.InteropServices;
using Avalonia.Platform;
using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Win32;

namespace FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Lifecycle;

/// <summary>NativeHost 子窗口创建。</summary>
sealed class NativeViewportCreate
{
    const int WsChild = 0x40000000;
    const int WsVisible = 0x10000000;
    const int WsClipChildren = 0x02000000;
    const int WsClipSiblings = 0x04000000;
    const int WindowStyle = WsChild | WsVisible | WsClipChildren | WsClipSiblings;

    public static NativeViewportLifecycleResult TryCreate(IPlatformHandle parent,
        Win32ViewportWindowClass.WndProc wndProc)
    {
        var instanceHandle = Win32ViewportModuleHandle.GetModuleHandle(null);
        if (instanceHandle == 0)
            return new NativeViewportLifecycleResult(0, 0, false, "无法获取当前进程模块句柄。");

        Win32ViewportWindowClass.EnsureRegistered(instanceHandle, wndProc);

        var windowHandle = CreateWindowEx(0, Win32ViewportWindowClass.WindowClassName,
            "FluidWarfare Vulkan Viewport", WindowStyle,
            0, 0, 1, 1, parent.Handle, 0, instanceHandle, 0);

        if (windowHandle == 0)
            return new NativeViewportLifecycleResult(0, instanceHandle, false,
                $"CreateWindowEx 失败：{new Win32Exception(Marshal.GetLastWin32Error()).Message}");

        return new NativeViewportLifecycleResult(windowHandle, instanceHandle, true, null);
    }

    [DllImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern nint CreateWindowEx(int exStyle, string className, string windowName,
        int style, int x, int y, int w, int h, nint parent, nint menu, nint instance, nint param);
}
