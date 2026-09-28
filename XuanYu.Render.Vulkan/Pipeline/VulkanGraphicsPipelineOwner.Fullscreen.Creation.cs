using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Render;

namespace XuanYu.Render.Vulkan.Pipeline;

internal sealed unsafe partial class VulkanGraphicsPipelineOwner
{
    static bool TryCreateFullscreenPipeline(Vk vk, VulkanDeviceOwner deviceOwner,
        VulkanClearFrameOwner clearFrame, ShaderModule vert, ShaderModule frag,
        PipelineLayout layout, bool depthTest, PrimitiveTopology topology,
        Action<string>? log, out Silk.NET.Vulkan.Pipeline pipeline)
    {
        var entry = System.Text.Encoding.ASCII.GetBytes("main\0");
        pipeline = default;
        fixed (byte* pName = entry)
        {
            var stages = CreateFullscreenStages(vert, frag, pName);
            PipelineShaderStageCreateInfo* pStages = stackalloc PipelineShaderStageCreateInfo[2];
            pStages[0] = stages[0];
            pStages[1] = stages[1];
            var binding = StaticModelVertexBinding();
            VertexInputAttributeDescription* attrs = stackalloc VertexInputAttributeDescription[3];
            FillStaticModelAttributes(attrs);
            var vertexInput = StaticModelVertexInput(&binding, attrs);
            var inputAssembly = new PipelineInputAssemblyStateCreateInfo
            {
                SType = StructureType.PipelineInputAssemblyStateCreateInfo,
                Topology = topology
            };
            var viewportState = new PipelineViewportStateCreateInfo
            {
                SType = StructureType.PipelineViewportStateCreateInfo,
                ViewportCount = 1, ScissorCount = 1
            };
            DynamicState* pDynamic = stackalloc DynamicState[2];
            pDynamic[0] = DynamicState.Viewport;
            pDynamic[1] = DynamicState.Scissor;
            var dynamicState = new PipelineDynamicStateCreateInfo
            {
                SType = StructureType.PipelineDynamicStateCreateInfo,
                DynamicStateCount = 2, PDynamicStates = pDynamic
            };
            var raster = new PipelineRasterizationStateCreateInfo
            {
                SType = StructureType.PipelineRasterizationStateCreateInfo,
                PolygonMode = PolygonMode.Fill, CullMode = CullModeFlags.None,
                FrontFace = FrontFace.Clockwise, LineWidth = 1.0f
            };
            var multisample = new PipelineMultisampleStateCreateInfo
            {
                SType = StructureType.PipelineMultisampleStateCreateInfo,
                RasterizationSamples = SampleCountFlags.Count1Bit
            };
            var depth = new PipelineDepthStencilStateCreateInfo
            {
                SType = StructureType.PipelineDepthStencilStateCreateInfo,
                DepthTestEnable = depthTest, DepthWriteEnable = false,
                DepthCompareOp = CompareOp.GreaterOrEqual
            };
            var blendAttach = FullscreenBlendAttachment();
            var colorBlend = new PipelineColorBlendStateCreateInfo
            {
                SType = StructureType.PipelineColorBlendStateCreateInfo,
                AttachmentCount = 1, PAttachments = &blendAttach, LogicOpEnable = false
            };
            var pipelineInfo = new GraphicsPipelineCreateInfo
            {
                SType = StructureType.GraphicsPipelineCreateInfo,
                StageCount = 2, PStages = pStages,
                PVertexInputState = &vertexInput, PInputAssemblyState = &inputAssembly,
                PViewportState = &viewportState, PRasterizationState = &raster,
                PMultisampleState = &multisample, PDepthStencilState = &depth,
                PColorBlendState = &colorBlend, PDynamicState = &dynamicState,
                Layout = layout, RenderPass = clearFrame.RenderPass, Subpass = 0
            };
            if (vk.CreateGraphicsPipelines(deviceOwner.LogicalDevice, default, 1,
                    &pipelineInfo, null, out pipeline) != Result.Success)
            {
                log?.Invoke(VulkanPipelineLogFormatter.Failed("全屏 Pass CreateGraphicsPipelines 失败"));
                return false;
            }
        }
        return true;
    }

}
