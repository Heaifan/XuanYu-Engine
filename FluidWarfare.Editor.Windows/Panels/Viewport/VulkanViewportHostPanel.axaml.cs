using Avalonia.Controls;

namespace FluidWarfare.Editor.Windows.Panels.Viewport;

public sealed partial class VulkanViewportHostPanel : UserControl
{
    private TextBlock? _hostInfoText;

    public VulkanViewportHostPanel()
    {
        InitializeComponent();
        _hostInfoText = this.FindControl<TextBlock>("HostInfoText");
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
}
