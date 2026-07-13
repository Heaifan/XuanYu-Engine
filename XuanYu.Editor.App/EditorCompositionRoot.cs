using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan;

namespace XuanYu.Editor.App;

public static class EditorCompositionRoot
{
    public static INativeHostSurfaceBridgeFactory CreateSurfaceBridgeFactory() =>
        new VulkanNativeHostSurfaceBridgeFactory();
}
