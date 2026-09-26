using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Pipeline;

namespace XuanYu.Render.Vulkan.Session;

public sealed partial class VulkanRenderSession
{
    VulkanGraphicsPipelineOwner? _terrainPipeline;

    void AttachTerrainPipeline(Vk vk)
    {
        var pipeline = VulkanGraphicsPipelineOwner.CreateTerrain(vk, _deviceOwner, _clearFrame, _swapchainOwner, _log);
        if (pipeline is null) return;
        _clearFrame.SetTerrainPipeline(pipeline.Pipeline, pipeline.Layout);
        _terrainPipeline = pipeline;
    }
}
