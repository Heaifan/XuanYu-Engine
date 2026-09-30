using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Pipeline;

namespace XuanYu.Render.Vulkan.Session;

public sealed partial class VulkanRenderSession
{
    VulkanGraphicsPipelineOwner? _terrainPipeline;

    void AttachTerrainPipeline(Vk vk)
    {
        // CREATE -> LOCAL OWNERSHIP; transfer is complete only after the session field is assigned.
        var pipeline = VulkanGraphicsPipelineOwner.CreateTerrain(vk, _deviceOwner, _clearFrame, _swapchainOwner, _log);
        TryTransferTerrainPipeline(
            pipeline,
            inject: value => _clearFrame.SetTerrainPipeline(value.Pipeline, value.Layout),
            transfer: value => _terrainPipeline = value,
            dispose: value => value.Dispose());
    }

    internal static bool TryTransferTerrainPipeline<T>(T? pipeline, Action<T> inject,
        Action<T> transfer, Action<T> dispose) where T : class
    {
        if (pipeline is null) return false;

        var sessionOwns = false;
        try
        {
            // INJECT may rerecord and throw; until transfer returns, the local path owns the pipeline.
            inject(pipeline);
            transfer(pipeline);
            sessionOwns = true;
            return true;
        }
        finally
        {
            if (!sessionOwns) dispose(pipeline);
        }
    }
}
