using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Pipeline;

internal sealed unsafe partial class VulkanGraphicsPipelineOwner
{
    static PipelineDepthStencilStateCreateInfo DepthState(bool depthTest = true, bool depthWrite = true) =>
        new()
        {
            SType = StructureType.PipelineDepthStencilStateCreateInfo,
            DepthTestEnable = depthTest,
            DepthWriteEnable = depthWrite,
            DepthCompareOp = CompareOp.LessOrEqual
        };
}
