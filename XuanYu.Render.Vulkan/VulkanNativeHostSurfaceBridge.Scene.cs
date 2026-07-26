using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan;

public sealed partial class VulkanNativeHostSurfaceBridge
{
    void OnRenderProjectionChanged(RenderProjectionResult projection)
    {
        _renderSession?.UpdateRenderProjection(projection);
    }
}
