using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan;

namespace XuanYu.Editor.UI;

// ARCH-A-R2：旧 UI 启动入口的兼容 fallback。
// 新 Editor.App 路径通过 INativeHostSurfaceBridgeFactory 注入，不走这里直接 new。
public static class VulkanSurfaceBridgeProvider
{
    public static INativeHostSurfaceBridge Create(Action<string> log) =>
        new VulkanNativeHostSurfaceBridge(log);
}
