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
        LogBridgeFactorySource("缺少应用注入，已拒绝旧 fallback");
        throw new InvalidOperationException("NativeHost Surface Bridge factory 未由应用组装层注入。");
    }

    void LogBridgeFactorySource(string source)
    {
        var message = $"【ARCH-A-R2】桥接工厂来源：{source}";
        Console.WriteLine(message); ReportVulkanMessage(message);
    }
}
