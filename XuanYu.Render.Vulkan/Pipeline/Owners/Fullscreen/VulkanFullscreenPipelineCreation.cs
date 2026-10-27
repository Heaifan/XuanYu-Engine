using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Pipeline.Contracts;
using XuanYu.Render.Vulkan.Render;

namespace XuanYu.Render.Vulkan.Pipeline;

internal static unsafe class VulkanFullscreenPipelineCreation
{
    internal static bool TryCreate(Vk vk, VulkanDeviceOwner deviceOwner,
        VulkanClearFrameOwner clearFrame, ShaderModule vert, ShaderModule frag,
        PipelineLayout layout, bool depthTest, PrimitiveTopology topology,
        out Silk.NET.Vulkan.Pipeline pipeline)
    {
        var entry = System.Text.Encoding.ASCII.GetBytes("main\0");
        pipeline = default;
        fixed (byte* pName = entry)
        {
            var stages = VulkanFullscreenPipelineState.ShaderStages(vert, frag, pName);
            PipelineShaderStageCreateInfo* pStages = stackalloc PipelineShaderStageCreateInfo[2];
            pStages[0] = stages[0];
            pStages[1] = stages[1];
            var binding = VulkanStaticModelVertexInput.Binding();
            VertexInputAttributeDescription* attrs = stackalloc VertexInputAttributeDescription[3];
            VulkanStaticModelVertexInput.FillAttributes(attrs);
            var vertexInput = VulkanStaticModelVertexInput.Create(&binding, attrs);
            var inputAssembly = new PipelineInputAssemblyStateCreateInfo
            {
                SType = StructureType.PipelineInputAssemblyStateCreateInfo,
                Topology = topology
            };
            var viewportState = VulkanGraphicsPipelineStates.DynamicViewport();
            DynamicState* pDynamic = stackalloc DynamicState[2];
            pDynamic[0] = DynamicState.Viewport;
            pDynamic[1] = DynamicState.Scissor;
            var dynamicState = VulkanGraphicsPipelineStates.DynamicViewportScissor(pDynamic);
            var raster = VulkanGraphicsPipelineStates.Rasterization();
            var multisample = VulkanGraphicsPipelineStates.Multisample();
            var depth = new PipelineDepthStencilStateCreateInfo
            {
                SType = StructureType.PipelineDepthStencilStateCreateInfo,
                DepthTestEnable = depthTest, DepthWriteEnable = false,
                DepthCompareOp = CompareOp.GreaterOrEqual
            };
            var blendAttach = VulkanGraphicsPipelineStates.AlphaBlendAttachment();
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
                return false;
            }
        }
        return true;
    }

}
