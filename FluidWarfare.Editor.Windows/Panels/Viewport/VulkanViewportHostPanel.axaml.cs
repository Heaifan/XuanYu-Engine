using Avalonia.Controls;
using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost;

namespace FluidWarfare.Editor.Windows.Panels.Viewport;

public sealed partial class VulkanViewportHostPanel : UserControl
{
    private TextBlock? _hostInfoText;
    private TextBlock? _nativeHostInfoText;
    private WindowsVulkanViewportHostControl? _nativeHostControl;
    private TextBlock? _surfaceInfoText;

    public VulkanViewportHostPanel()
    {
        InitializeComponent();
        _hostInfoText = this.FindControl<TextBlock>("HostInfoText");
        _nativeHostInfoText = this.FindControl<TextBlock>("NativeHostInfoText");
        _nativeHostControl = this.FindControl<WindowsVulkanViewportHostControl>("NativeHostControl");
        _surfaceInfoText = this.FindControl<TextBlock>("SurfaceInfoText");
    }

    /// <summary>
    /// 显示 Vulkan 视口宿主状态信息。
    /// 不创建 Vulkan 对象，不获取窗口句柄，不写日志。
    /// </summary>
    public void ShowHostInfo(VulkanViewportHostInfo hostInfo)
    {
        if (_hostInfoText is not null)
        {
            _hostInfoText.Text = hostInfo.Message;
        }
    }

    /// <summary>
    /// 返回 Vulkan Surface 所需的原生宿主句柄信息。
    /// 只读取独立 Windows 子窗口宿主，不使用主窗口句柄冒充视口 Surface。
    /// </summary>
    public VulkanViewportNativeHostInfo GetNativeHostInfo()
    {
        if (!OperatingSystem.IsWindows())
        {
            return new VulkanViewportNativeHostInfo(
                "非 Windows",
                false,
                "当前平台不支持 Windows Vulkan Surface 创建。",
                0,
                0);
        }

        var hostInfo = GetWindowsNativeHostInfo();
        if (!hostInfo.HasWindowHandle)
        {
            return new VulkanViewportNativeHostInfo(
                hostInfo.PlatformText,
                false,
                hostInfo.Message,
                0,
                hostInfo.InstanceHandle);
        }

        return new VulkanViewportNativeHostInfo(
            "Windows",
            true,
            "已获取 Windows Vulkan 视口原生子窗口句柄。",
            hostInfo.WindowHandle,
            hostInfo.InstanceHandle);
    }

    /// <summary>
    /// 返回 Windows 原生子窗口宿主状态，并刷新面板显示。
    /// </summary>
    public WindowsVulkanViewportHostInfo GetWindowsNativeHostInfo()
    {
        var hostInfo = _nativeHostControl?.GetHostInfo()
            ?? WindowsVulkanViewportHostInfo.NotCreated;

        ShowNativeHostInfo(hostInfo);
        return hostInfo;
    }

    /// <summary>
    /// 显示 Windows 原生子窗口宿主状态。
    /// 不创建 Vulkan 对象，不写日志。
    /// </summary>
    public void ShowNativeHostInfo(WindowsVulkanViewportHostInfo hostInfo)
    {
        if (_nativeHostInfoText is not null)
        {
            _nativeHostInfoText.Text = hostInfo.HasWindowHandle
                ? $"{hostInfo.Message} 平台：{hostInfo.PlatformText}。"
                : hostInfo.Message;
        }
    }

    /// <summary>
    /// 显示 Vulkan Surface 创建状态。
    /// 不创建 Vulkan 对象，不写日志。
    /// </summary>
    public void ShowSurfaceInfo(string statusText)
    {
        if (_surfaceInfoText is not null)
        {
            _surfaceInfoText.Text = statusText;
        }
    }
}
