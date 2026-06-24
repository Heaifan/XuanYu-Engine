using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>
/// GraphicsPipeline 创建辅助：状态构建 + CreateGraphicsPipelines 调用。
/// 被 VulkanScene3dPipelines.Create 编排器调用。
/// </summary>
public static unsafe partial class VulkanScene3dPipelines
{
    internal static PipelineRasterizationStateCreateInfo BuildRasterizerState() => new()
    {
        SType = StructureType.PipelineRasterizationStateCreateInfo,
        DepthClampEnable = Vk.False, RasterizerDiscardEnable = Vk.False,
        PolygonMode = PolygonMode.Fill, CullMode = CullModeFlags.None,
        FrontFace = FrontFace.Clockwise,
        DepthBiasEnable = Vk.False, LineWidth = 1.0f
    };

    internal static PipelineDepthStencilStateCreateInfo BuildDepthStencilState() => new()
    {
        SType = StructureType.PipelineDepthStencilStateCreateInfo,
        DepthTestEnable = Vk.True, DepthWriteEnable = Vk.True,
        DepthCompareOp = CompareOp.Less,
        DepthBoundsTestEnable = Vk.False, StencilTestEnable = Vk.False
    };

    internal static PipelineMultisampleStateCreateInfo BuildMultisampleState() => new()
    {
        SType = StructureType.PipelineMultisampleStateCreateInfo,
        SampleShadingEnable = Vk.False, RasterizationSamples = SampleCountFlags.Count1Bit
    };

    internal static bool TryCreate(Vk vk, Silk.NET.Vulkan.Device dev,
        RenderPass renderPass, PipelineLayout pipelineLayout,
        PipelineVertexInputStateCreateInfo* vi,
        PipelineViewportStateCreateInfo* vp,
        PipelineRasterizationStateCreateInfo* rs,
        PipelineDepthStencilStateCreateInfo* ds,
        PipelineMultisampleStateCreateInfo* ms,
        PipelineColorBlendStateCreateInfo* cb,
        PipelineShaderStageCreateInfo* stages,
        PrimitiveTopology topology,
        out Pipeline pipeline, out string error)
    {
        var ia = new PipelineInputAssemblyStateCreateInfo
        {
            SType = StructureType.PipelineInputAssemblyStateCreateInfo,
            Topology = topology, PrimitiveRestartEnable = Vk.False
        };
        var ci = new GraphicsPipelineCreateInfo
        {
            SType = StructureType.GraphicsPipelineCreateInfo,
            StageCount = 2, PStages = stages,
            PVertexInputState = vi, PInputAssemblyState = &ia,
            PViewportState = vp, PRasterizationState = rs,
            PMultisampleState = ms, PDepthStencilState = ds,
            PColorBlendState = cb,
            Layout = pipelineLayout, RenderPass = renderPass, Subpass = 0
        };
        var result = vk.CreateGraphicsPipelines(dev, default, 1, &ci, null, out pipeline);
        if (result != Result.Success)
        {
            error = $"Scene3D Pipeline (Topology={topology})：vkCreateGraphicsPipelines 返回 {result}。";
            return false;
        }
        error = string.Empty;
        return true;
    }
}
