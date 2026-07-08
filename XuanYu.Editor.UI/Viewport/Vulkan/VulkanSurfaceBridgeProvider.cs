using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan;

namespace XuanYu.Editor.UI;

// VK3-C2 组合根：把渲染层具体桥接实现装配给 UI 生命周期宿主。
// UI 宿主（VulkanNativeHost）只认 INativeHostSurfaceBridge 契约，
// 不直接依赖 Render.Vulkan 具体类，保持 Editor.UI → Abstractions 的依赖方向。
public static class VulkanSurfaceBridgeProvider
{
    public static INativeHostSurfaceBridge Create(Action<string> log) =>
        new VulkanNativeHostSurfaceBridge(log);
}
