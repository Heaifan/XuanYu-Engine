using Avalonia.Controls;
using Avalonia.Platform;

namespace XuanYu.Editor.UI;

public sealed class VulkanNativeHost : NativeControlHost
{
    nint _hwnd;
    VulkanClearSession? _session;

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        _hwnd = Win32ViewportHost.CreateChild(parent.Handle);
        StartSession();
        return new PlatformHandle(_hwnd, "HWND");
    }

    protected override void DestroyNativeControlCore(IPlatformHandle control)
    {
        _session?.Dispose();
        Log("Vulkan 释放完成", "中央视口 Vulkan Clear Probe 已释放。");
        _session = null;
        Win32ViewportHost.Destroy(_hwnd);
        _hwnd = 0;
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        if (_hwnd == 0 || e.NewSize.Width < 1 || e.NewSize.Height < 1) return;
        Win32ViewportHost.Resize(_hwnd, (int)e.NewSize.Width, (int)e.NewSize.Height);
        _session?.Resize((uint)e.NewSize.Width, (uint)e.NewSize.Height);
    }

    void StartSession()
    {
        var size = Bounds.Size;
        Log("Vulkan 初始化开始", "中央视口开始创建 Vulkan Clear Probe。");
        _session = VulkanClearSession.TryCreate(_hwnd, (uint)Math.Max(1, size.Width), (uint)Math.Max(1, size.Height), Log);
        if (_session?.IsReady == true) Owner()?.HideFallback(); else Owner()?.SetFallback("Vulkan 初始化失败，已显示占位视口。");
    }

    void Log(string message, string detail) =>
        (DataContext as UiVm)?.LogVulkanLifecycle(message, detail);

    VulkanViewport? Owner()
    {
        var p = Parent;
        while (p is not null and not VulkanViewport) p = p.Parent;
        return p as VulkanViewport;
    }
}
