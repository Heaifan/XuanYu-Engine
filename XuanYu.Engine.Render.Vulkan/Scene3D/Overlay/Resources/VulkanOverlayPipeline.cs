using System.Runtime.InteropServices;
using Silk.NET.Vulkan;
using FluidWarfare.Render.Vulkan.Shaders;

namespace FluidWarfare.Render.Vulkan.Scene3D.Overlay;

/// <summary>Overlay Graphics Pipeline。TriangleList，Depth=off，Blend=on，Cull=off。</summary>
public static unsafe partial class VulkanOverlayPipeline
{
    public const uint VertexStride = 24;

    public static bool Create(Vk vk, Silk.NET.Vulkan.Device dev, RenderPass renderPass, PipelineLayout pipelineLayout,
        ShaderModule vertModule, ShaderModule fragModule, uint viewportWidth, uint viewportHeight,
        out Pipeline pipeline, out string errorMessage)
    {
        pipeline = default; errorMessage = string.Empty;
        var vb = new VertexInputBindingDescription { Binding = 0, Stride = VertexStride, InputRate = VertexInputRate.Vertex };
        var va = new VertexInputAttributeDescription[] {
            new() { Location = 0, Binding = 0, Format = Format.R32G32Sfloat, Offset = 0 },
            new() { Location = 1, Binding = 0, Format = Format.R32G32B32A32Sfloat, Offset = 8 } };
        fixed (VertexInputAttributeDescription* pa = va)
        {
            var viCI = new PipelineVertexInputStateCreateInfo { SType = StructureType.PipelineVertexInputStateCreateInfo, VertexBindingDescriptionCount = 1, PVertexBindingDescriptions = &vb, VertexAttributeDescriptionCount = 2, PVertexAttributeDescriptions = pa };
            var vp = new Viewport { X = 0, Y = 0, Width = viewportWidth, Height = viewportHeight, MinDepth = 0, MaxDepth = 1 };
            var scissor = new Rect2D(new Offset2D(0, 0), new Extent2D(viewportWidth, viewportHeight));
            var vsCI = new PipelineViewportStateCreateInfo { SType = StructureType.PipelineViewportStateCreateInfo, ViewportCount = 1, PViewports = &vp, ScissorCount = 1, PScissors = &scissor };
            var rsCI = new PipelineRasterizationStateCreateInfo { SType = StructureType.PipelineRasterizationStateCreateInfo, DepthClampEnable = Vk.False, RasterizerDiscardEnable = Vk.False, PolygonMode = PolygonMode.Fill, CullMode = CullModeFlags.None, FrontFace = FrontFace.Clockwise, DepthBiasEnable = Vk.False, LineWidth = 1.0f };
            var dsCI = new PipelineDepthStencilStateCreateInfo { SType = StructureType.PipelineDepthStencilStateCreateInfo, DepthTestEnable = Vk.False, DepthWriteEnable = Vk.False, DepthCompareOp = CompareOp.Always, DepthBoundsTestEnable = Vk.False, StencilTestEnable = Vk.False };
            var msCI = new PipelineMultisampleStateCreateInfo { SType = StructureType.PipelineMultisampleStateCreateInfo, SampleShadingEnable = Vk.False, RasterizationSamples = SampleCountFlags.Count1Bit };
            var ba = new PipelineColorBlendAttachmentState { BlendEnable = Vk.True, SrcColorBlendFactor = BlendFactor.SrcAlpha, DstColorBlendFactor = BlendFactor.OneMinusSrcAlpha, ColorBlendOp = BlendOp.Add, SrcAlphaBlendFactor = BlendFactor.One, DstAlphaBlendFactor = BlendFactor.Zero, AlphaBlendOp = BlendOp.Add, ColorWriteMask = ColorComponentFlags.RBit | ColorComponentFlags.GBit | ColorComponentFlags.BBit | ColorComponentFlags.ABit };
            var cbCI = new PipelineColorBlendStateCreateInfo { SType = StructureType.PipelineColorBlendStateCreateInfo, AttachmentCount = 1, PAttachments = &ba };
            var stages = stackalloc PipelineShaderStageCreateInfo[2];
            var n0 = Marshal.StringToHGlobalAnsi("main"); var n1 = Marshal.StringToHGlobalAnsi("main");
            stages[0] = new PipelineShaderStageCreateInfo { SType = StructureType.PipelineShaderStageCreateInfo, Stage = ShaderStageFlags.VertexBit, Module = vertModule, PName = (byte*)n0 };
            stages[1] = new PipelineShaderStageCreateInfo { SType = StructureType.PipelineShaderStageCreateInfo, Stage = ShaderStageFlags.FragmentBit, Module = fragModule, PName = (byte*)n1 };
            var iaCI = new PipelineInputAssemblyStateCreateInfo { SType = StructureType.PipelineInputAssemblyStateCreateInfo, Topology = PrimitiveTopology.TriangleList, PrimitiveRestartEnable = Vk.False };
            var gpCI = new GraphicsPipelineCreateInfo { SType = StructureType.GraphicsPipelineCreateInfo, StageCount = 2, PStages = stages, PVertexInputState = &viCI, PInputAssemblyState = &iaCI, PViewportState = &vsCI, PRasterizationState = &rsCI, PMultisampleState = &msCI, PDepthStencilState = &dsCI, PColorBlendState = &cbCI, Layout = pipelineLayout, RenderPass = renderPass, Subpass = 0 };
            var res = vk.CreateGraphicsPipelines(dev, default, 1, &gpCI, null, out pipeline);
            if (res != Result.Success) { Marshal.FreeHGlobal(n0); Marshal.FreeHGlobal(n1); errorMessage = $"Overlay Pipeline 创建失败：{res}。"; return false; }
            Marshal.FreeHGlobal(n0); Marshal.FreeHGlobal(n1);
        }
        return true;
    }
}
