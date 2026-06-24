using Avalonia;
using XuanYu.Engine.Editor.Windows.Panels.Viewport.NativeHost.Win32;

namespace XuanYu.Engine.Editor.Windows.Panels.Viewport.NativeHost.Lifecycle;

/// <summary>NativeHost 状态信息同步与大小变更处理。</summary>
sealed class NativeViewportHostSync
{
    int _width;
    int _height;
    WindowsVulkanViewportHostInfo _hostInfo = WindowsVulkanViewportHostInfo.NotCreated;

    public WindowsVulkanViewportHostInfo Current => _hostInfo;

    public bool Apply(nint windowHandle, nint instanceHandle, Rect bounds,
        out WindowsVulkanViewportHostInfo newHostInfo)
    {
        newHostInfo = _hostInfo;
        if (windowHandle == 0) return false;
        var w = Math.Max(1, (int)Math.Round(bounds.Width));
        var h = Math.Max(1, (int)Math.Round(bounds.Height));
        if (w < 1 || h < 1) return false;
        var changed = !_hostInfo.HasWindowHandle || _width != w || _height != h;
        NativeViewportHostInfoStatics.SyncWindowSize(windowHandle, w, h);
        _width = w; _height = h;
        _hostInfo = NativeViewportHostInfoStatics.CreateHostInfo(windowHandle, instanceHandle, _width, _height);
        newHostInfo = _hostInfo;
        return changed;
    }

    public void Reset() { _hostInfo = WindowsVulkanViewportHostInfo.NotCreated; _width = 0; _height = 0; }

    public void SetFailed(string message, nint instanceHandle)
    {
        _hostInfo = NativeViewportHostInfoStatics.CreateFailedHostInfo(message, instanceHandle, _width, _height);
    }
}
