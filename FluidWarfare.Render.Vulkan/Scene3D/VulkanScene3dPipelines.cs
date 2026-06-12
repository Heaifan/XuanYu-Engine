using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>
/// 创建 Grid (LineList) 和 Unit (TriangleList) Graphics Pipeline。
/// Vertex input 使用 VulkanScene3dVertex 的 Position + Color。
/// </summary>
public static unsafe class VulkanScene3dPipelines
{
    /// <summary>
    /// 创建两条 Pipeline。
    /// </summary>
    public static bool Create(Vk vk, Silk.NET.Vulkan.Device dev,
        RenderPass renderPass, PipelineLayout pipelineLayout,
        ShaderModule vertModule, ShaderModule fragModule,
        uint viewportWidth, uint viewportHeight,
        out Pipeline gridPipeline, out Pipeline unitPipeline,
        out string errorMessage)
    {
        gridPipeline = default;
        unitPipeline = default;
        errorMessage = string.Empty;

        var vertexBinding = new VertexInputBindingDescription
        {
            Binding = 0,
            Stride = 28,
            InputRate = VertexInputRate.Vertex
        };
        var vertexAttribs = new[]
        {
            new VertexInputAttributeDescription { Location = 0, Binding = 0, Format = Format.R32G32B32Sfloat, Offset = 0 },
            new VertexInputAttributeDescription { Location = 1, Binding = 0, Format = Format.R32G32B32A32Sfloat, Offset = 12 }
        };

        fixed (VertexInputAttributeDescription* pAttr = vertexAttribs)
        {
            var viCI = new PipelineVertexInputStateCreateInfo
            {
                SType = StructureType.PipelineVertexInputStateCreateInfo,
                VertexBindingDescriptionCount = 1,
                PVertexBindingDescriptions = &vertexBinding,
                VertexAttributeDescriptionCount = 2,
                PVertexAttributeDescriptions = pAttr
            };

            var viewport = new Viewport { X = 0, Y = 0, Width = viewportWidth, Height = viewportHeight, MinDepth = 0, MaxDepth = 1 };
            var scissor = new Rect2D(new Offset2D(0, 0), new Extent2D(viewportWidth, viewportHeight));
            var vsCI = new PipelineViewportStateCreateInfo
            {
                SType = StructureType.PipelineViewportStateCreateInfo,
                ViewportCount = 1, PViewports = &viewport,
                ScissorCount = 1, PScissors = &scissor
            };
            var rsCI = new PipelineRasterizationStateCreateInfo
            {
                SType = StructureType.PipelineRasterizationStateCreateInfo,
                DepthClampEnable = Vk.False, RasterizerDiscardEnable = Vk.False,
                PolygonMode = PolygonMode.Fill, CullMode = CullModeFlags.None,
                FrontFace = FrontFace.Clockwise,
                DepthBiasEnable = Vk.False, LineWidth = 1.0f
            };
            var dsCI = new PipelineDepthStencilStateCreateInfo
            {
                SType = StructureType.PipelineDepthStencilStateCreateInfo,
                DepthTestEnable = Vk.True,
                DepthWriteEnable = Vk.True,
                DepthCompareOp = CompareOp.Less,
                DepthBoundsTestEnable = Vk.False,
                StencilTestEnable = Vk.False
            };
            var msCI = new PipelineMultisampleStateCreateInfo
            {
                SType = StructureType.PipelineMultisampleStateCreateInfo,
                SampleShadingEnable = Vk.False, RasterizationSamples = SampleCountFlags.Count1Bit
            };
            var blendAtt = new PipelineColorBlendAttachmentState
            {
                BlendEnable = Vk.False,
                ColorWriteMask = ColorComponentFlags.RBit | ColorComponentFlags.GBit | ColorComponentFlags.BBit | ColorComponentFlags.ABit
            };
            var cbCI = new PipelineColorBlendStateCreateInfo
            {
                SType = StructureType.PipelineColorBlendStateCreateInfo,
                AttachmentCount = 1, PAttachments = &blendAtt
            };

            var stages = stackalloc PipelineShaderStageCreateInfo[2];
            var name0 = Marshal.StringToHGlobalAnsi("main");
            var name1 = Marshal.StringToHGlobalAnsi("main");
            stages[0] = new PipelineShaderStageCreateInfo { SType = StructureType.PipelineShaderStageCreateInfo, Stage = ShaderStageFlags.VertexBit, Module = vertModule, PName = (byte*)name0 };
            stages[1] = new PipelineShaderStageCreateInfo { SType = StructureType.PipelineShaderStageCreateInfo, Stage = ShaderStageFlags.FragmentBit, Module = fragModule, PName = (byte*)name1 };

            // Grid (LineList)
            var iaCI = new PipelineInputAssemblyStateCreateInfo
            {
                SType = StructureType.PipelineInputAssemblyStateCreateInfo,
                Topology = PrimitiveTopology.LineList,
                PrimitiveRestartEnable = Vk.False
            };
            var gpCI = new GraphicsPipelineCreateInfo
            {
                SType = StructureType.GraphicsPipelineCreateInfo,
                StageCount = 2, PStages = stages,
                PVertexInputState = &viCI, PInputAssemblyState = &iaCI,
                PViewportState = &vsCI, PRasterizationState = &rsCI,
                PMultisampleState = &msCI, PDepthStencilState = &dsCI,
                PColorBlendState = &cbCI,
                Layout = pipelineLayout, RenderPass = renderPass, Subpass = 0
            };
            var gridRes = vk.CreateGraphicsPipelines(dev, default, 1, &gpCI, null, out gridPipeline);
            if (gridRes != Result.Success)
            {
                Marshal.FreeHGlobal(name0); Marshal.FreeHGlobal(name1);
                errorMessage = $"Scene3D GridPipeline：vkCreateGraphicsPipelines 返回 {gridRes}。";
                return false;
            }

            // Unit (TriangleList)
            var iaCI2 = new PipelineInputAssemblyStateCreateInfo
            {
                SType = StructureType.PipelineInputAssemblyStateCreateInfo,
                Topology = PrimitiveTopology.TriangleList,
                PrimitiveRestartEnable = Vk.False
            };
            var gpCI2 = new GraphicsPipelineCreateInfo
            {
                SType = StructureType.GraphicsPipelineCreateInfo,
                StageCount = 2, PStages = stages,
                PVertexInputState = &viCI, PInputAssemblyState = &iaCI2,
                PViewportState = &vsCI, PRasterizationState = &rsCI,
                PMultisampleState = &msCI, PDepthStencilState = &dsCI,
                PColorBlendState = &cbCI,
                Layout = pipelineLayout, RenderPass = renderPass, Subpass = 0
            };
            var unitRes = vk.CreateGraphicsPipelines(dev, default, 1, &gpCI2, null, out unitPipeline);
            if (unitRes != Result.Success)
            {
                vk.DestroyPipeline(dev, gridPipeline, null);
                gridPipeline = default;
                Marshal.FreeHGlobal(name0); Marshal.FreeHGlobal(name1);
                errorMessage = $"Scene3D UnitPipeline：vkCreateGraphicsPipelines 返回 {unitRes}。";
                return false;
            }

            Marshal.FreeHGlobal(name0); Marshal.FreeHGlobal(name1);
        }

        return true;
    }
}
