using Avalonia.Threading;

namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    // VK4-D-R2：后台 Present 泵日志必须回 UI 线程访问 DataContext / UiVm。
    void ReportVulkanMessage(string msg)
    {
        if (Dispatcher.UIThread.CheckAccess()) ReportVulkanMessageOnUiThread(msg);
        else Dispatcher.UIThread.Post(() => ReportVulkanMessageOnUiThread(msg));
    }

    void ReportVulkanMessageOnUiThread(string msg) =>
        ViewportNativeHostRoute.ReportVulkanBridge(DataContext as UiVm, msg);
}
