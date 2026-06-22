using System.Runtime.InteropServices;
using Avalonia;

namespace FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost;

/// <summary>NativeHost 状态信息创建与窗口尺寸同步。</summary>
internal static class NativeViewportHostInfoStatics
{
    const uint SwpNoZOrder = 0x0004;
    const uint SwpNoActivate = 0x0010;

    public static WindowsVulkanViewportHostInfo CreateHostInfo(
        nint windowHandle, nint instanceHandle, int width, int height) => new(
        WindowsVulkanViewportHostState.Created,
        $"Windows 原生子窗口已创建，HWND：{FormatHandle(windowHandle)}，尺寸：{width}x{height}。",
        "Windows", true, windowHandle, instanceHandle, width, height);

    public static WindowsVulkanViewportHostInfo CreateFailedHostInfo(
        string message, nint instanceHandle, int width, int height) => new(
        WindowsVulkanViewportHostState.Failed, message,
        "Windows", false, 0, instanceHandle, width, height);

    public static WindowsVulkanViewportHostInfo CreateUnsupportedPlatformInfo() => new(
        WindowsVulkanViewportHostState.UnsupportedPlatform,
        "当前平台不支持 Windows Vulkan 视口子窗口。",
        "非 Windows", false, 0, 0, 0, 0);

    public static WindowsVulkanViewportHostInfo CreateNoParentHandleInfo() => new(
        WindowsVulkanViewportHostState.Failed,
        "Avalonia 未提供可嵌入原生子窗口的父级句柄。",
        "Windows", false, 0, 0, 0, 0);

    public static void SyncWindowSize(nint windowHandle, int width, int height)
    {
        if (windowHandle == 0) return;
        SetWindowPos(windowHandle, 0, 0, 0,
            Math.Max(1, width), Math.Max(1, height),
            SwpNoZOrder | SwpNoActivate);
    }

    public static string FormatHandle(nint handle) => $"0x{handle.ToInt64():X16}";

    [DllImport("user32.dll", EntryPoint = "SetWindowPos", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool SetWindowPos(nint hwnd, nint after, int x, int y, int w, int h, uint flags);
}
