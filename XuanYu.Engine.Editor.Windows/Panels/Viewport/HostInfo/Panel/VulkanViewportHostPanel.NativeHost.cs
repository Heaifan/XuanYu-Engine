using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost;
using XuanYu.Engine.Render.ViewportNavigation;

namespace FluidWarfare.Editor.Windows.Panels.Viewport;

/// <summary>Partial：原生宿主信息查询与状态更新。</summary>
sealed partial class VulkanViewportHostPanel
{
    public void ShowClearStatus(string t) { if (_clearStatusText is not null) _clearStatusText.Text = t; }

    public VulkanViewportNativeHostInfo GetNativeHostInfo()
    {
        if (!OperatingSystem.IsWindows())
            return new("非 Windows", false, "不支持 Windows Vulkan Surface。", 0, 0, 0, 0);
        var h = GetWindowsNativeHostInfo();
        return h.HasWindowHandle
            ? new("Windows", true, "已获取原生子窗口句柄。", h.WindowHandle, h.InstanceHandle, h.Width, h.Height)
            : new(h.PlatformText, false, h.Message, 0, h.InstanceHandle, h.Width, h.Height);
    }

    public WindowsVulkanViewportHostInfo GetWindowsNativeHostInfo()
    {
        var h = _nativeHostControl?.GetHostInfo() ?? WindowsVulkanViewportHostInfo.NotCreated;
        if (_nativeHostInfoText is not null)
            _nativeHostInfoText.Text = h.HasWindowHandle ? $"{h.Message} 平台：{h.PlatformText}。" : h.Message;
        return h;
    }

    void HandleHostInfoChanged(object? s, WindowsVulkanViewportHostInfo h)
    {
        if (_nativeHostInfoText is not null)
            _nativeHostInfoText.Text = h.HasWindowHandle ? $"{h.Message} 平台：{h.PlatformText}。" : h.Message;
        NativeHostInfoChanged?.Invoke(this, GetNativeHostInfo());
    }
}
