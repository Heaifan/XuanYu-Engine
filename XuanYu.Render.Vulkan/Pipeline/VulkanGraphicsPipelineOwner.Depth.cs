using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Pipeline;

internal sealed unsafe partial class VulkanGraphicsPipelineOwner
{
    static PipelineDepthStencilStateCreateInfo DepthState() =>
        new()
        {
            SType = StructureType.PipelineDepthStencilStateCreateInfo,
            DepthTestEnable = true,
            DepthWriteEnable = true,
            DepthCompareOp = CompareOp.LessOrEqual
        };
}
