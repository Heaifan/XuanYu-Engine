using XuanYu.Render.Vulkan.Diagnostic;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    void Initialize(int graphicsFamily)
    {
        try
        {
            BuildRenderPass();
            CreateCommandPool(graphicsFamily);
            InitializeMapLabelTextures();
            if (!RebuildFramebuffers()) throw new InvalidOperationException("Framebuffer 创建失败");
            Log(VulkanClearFrameLogFormatter.Created());
        }
        catch
        {
            Dispose();
            throw;
        }
    }
}
