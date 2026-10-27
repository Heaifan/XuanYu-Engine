using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Pipeline;

internal static class VulkanPipelineDepthState
{
    internal static PipelineDepthStencilStateCreateInfo Create(bool depthTest = true, bool depthWrite = true) =>
        new()
        {
            SType = StructureType.PipelineDepthStencilStateCreateInfo,
            DepthTestEnable = depthTest,
            DepthWriteEnable = depthWrite,
            DepthCompareOp = CompareOp.GreaterOrEqual
        };
}
