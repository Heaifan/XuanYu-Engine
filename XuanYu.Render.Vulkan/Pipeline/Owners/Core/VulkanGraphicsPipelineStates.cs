using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Pipeline;

internal static unsafe class VulkanGraphicsPipelineStates
{
    internal static PipelineRasterizationStateCreateInfo Rasterization() => new()
    {
        SType = StructureType.PipelineRasterizationStateCreateInfo,
        PolygonMode = PolygonMode.Fill,
        CullMode = CullModeFlags.None,
        FrontFace = FrontFace.Clockwise,
        LineWidth = 1.0f
    };

    internal static PipelineMultisampleStateCreateInfo Multisample() => new()
    {
        SType = StructureType.PipelineMultisampleStateCreateInfo,
        RasterizationSamples = SampleCountFlags.Count1Bit
    };

    internal static PipelineViewportStateCreateInfo DynamicViewport() => new()
    {
        SType = StructureType.PipelineViewportStateCreateInfo,
        ViewportCount = 1,
        ScissorCount = 1
    };

    internal static PipelineDynamicStateCreateInfo DynamicViewportScissor(DynamicState* states) => new()
    {
        SType = StructureType.PipelineDynamicStateCreateInfo,
        DynamicStateCount = 2,
        PDynamicStates = states
    };

    internal static PipelineColorBlendAttachmentState AlphaBlendAttachment() => new()
    {
        ColorWriteMask = ColorComponentFlags.RBit | ColorComponentFlags.GBit |
            ColorComponentFlags.BBit | ColorComponentFlags.ABit,
        BlendEnable = true,
        SrcColorBlendFactor = BlendFactor.SrcAlpha,
        DstColorBlendFactor = BlendFactor.OneMinusSrcAlpha,
        ColorBlendOp = BlendOp.Add,
        SrcAlphaBlendFactor = BlendFactor.One,
        DstAlphaBlendFactor = BlendFactor.OneMinusSrcAlpha,
        AlphaBlendOp = BlendOp.Add
    };
}
