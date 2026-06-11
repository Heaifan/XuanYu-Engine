using Avalonia.Controls;

namespace FluidWarfare.Editor.Windows.Panels.Viewport;

public sealed partial class VulkanViewportHostPanel : UserControl
{
    private TextBlock? _hostInfoText;
    private TextBlock? _surfaceInfoText;

    public VulkanViewportHostPanel()
    {
        InitializeComponent();
        _hostInfoText = this.FindControl<TextBlock>("HostInfoText");
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
    /// 当前面板尚未创建独立 Windows 子窗口，因此不能使用主窗口句柄冒充视口 Surface。
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

        return new VulkanViewportNativeHostInfo(
            "Windows",
            false,
            "当前 Avalonia 宿主尚未提供可用于 Vulkan Surface 的独立 Windows 视口句柄。",
            0,
            0);
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
