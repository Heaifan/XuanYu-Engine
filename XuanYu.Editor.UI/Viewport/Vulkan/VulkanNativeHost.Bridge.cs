using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    INativeHostSurfaceBridge CreateBridge()
    {
        if (DataContext is UiVm { SurfaceBridgeFactory: { } factory } vm)
        {
            LogBridgeFactorySource("应用注入");
            return factory.Create(ReportVulkanMessage, vm.SceneSnapshotSource);
        }
        LogBridgeFactorySource("缺少应用注入，已拒绝旧回退路径");
        throw new InvalidOperationException("NativeHost Surface Bridge factory 未由应用组装层注入。");
    }

    void LogBridgeFactorySource(string source)
    {
        var message = $"桥接工厂来源：{source}；应用=玄域编辑器";
        var console = $"{DateTime.Now:HH:mm:ss} 【信息】【Vulkan桥接】{message}";
        Console.WriteLine(console); ReportVulkanMessage(message);
    }
}
