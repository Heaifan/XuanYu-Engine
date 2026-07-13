using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    INativeHostSurfaceBridge CreateBridge()
    {
        if (DataContext is UiVm { SurfaceBridgeFactory: { } factory })
        {
            LogBridgeFactorySource("应用注入（XuanYu.Editor.App）");
            return factory.Create(ReportVulkanMessage);
        }
        LogBridgeFactorySource("旧兼容回退（VulkanSurfaceBridgeProvider）");
        return VulkanSurfaceBridgeProvider.Create(ReportVulkanMessage);
    }

    void LogBridgeFactorySource(string source)
    {
        var message = $"【ARCH-A-R2】桥接工厂来源：{source}";
        Console.WriteLine(message); ReportVulkanMessage(message);
    }
}
