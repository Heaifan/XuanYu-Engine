using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Render;
using XuanYu.Render.Vulkan.Swapchain;

namespace XuanYu.Render.Vulkan.Pipeline;

// WORLD-D-R1：天空专用管线。与主管线共用 Shader、顶点输入与 RenderPass，
// 仅深度状态不同（DepthTest=Off、DepthWrite=Off），保证全屏背景不写深度、
// 不遮挡网格/模型/Bounds/Gizmo；初始化创建一次，随 Session 释放。
internal sealed unsafe partial class VulkanGraphicsPipelineOwner
{
    internal static VulkanGraphicsPipelineOwner? CreateSky(Vk vk, VulkanDeviceOwner deviceOwner,
        VulkanClearFrameOwner clearFrame, VulkanSwapchainOwner swapchain, Action<string>? log)
    {
        var vert = VulkanShaderModuleOwner.Create(vk, deviceOwner, ShaderBytecodeVert.Code);
        var frag = VulkanShaderModuleOwner.Create(vk, deviceOwner, ShaderBytecodeFrag.Code);
        if (vert.Handle == 0 || frag.Handle == 0)
        {
            VulkanShaderModuleOwner.Destroy(vk, deviceOwner, vert);
            VulkanShaderModuleOwner.Destroy(vk, deviceOwner, frag);
            log?.Invoke(VulkanPipelineLogFormatter.Failed("天空 ShaderModule 创建失败"));
            return null;
        }
        var range = new PushConstantRange { StageFlags = ShaderStageFlags.VertexBit, Offset = 0, Size = VulkanScenePushConstants.SizeInBytes };
        var layoutInfo = new PipelineLayoutCreateInfo { SType = StructureType.PipelineLayoutCreateInfo, SetLayoutCount = 0, PushConstantRangeCount = 1, PPushConstantRanges = &range };
        if (vk.CreatePipelineLayout(deviceOwner.LogicalDevice, &layoutInfo, null, out var layout) != Result.Success)
        {
            VulkanShaderModuleOwner.Destroy(vk, deviceOwner, vert);
            VulkanShaderModuleOwner.Destroy(vk, deviceOwner, frag);
            log?.Invoke(VulkanPipelineLogFormatter.Failed("天空 CreatePipelineLayout 失败"));
            return null;
        }
        var entry = System.Text.Encoding.ASCII.GetBytes("main\0");
        Silk.NET.Vulkan.Pipeline pipeline = default;
        fixed (byte* pName = entry)
        {
            var vertStage = new PipelineShaderStageCreateInfo { SType = StructureType.PipelineShaderStageCreateInfo, Stage = ShaderStageFlags.VertexBit, Module = vert, PName = pName };
            var fragStage = new PipelineShaderStageCreateInfo { SType = StructureType.PipelineShaderStageCreateInfo, Stage = ShaderStageFlags.FragmentBit, Module = frag, PName = pName };
            PipelineShaderStageCreateInfo* pStages = stackalloc PipelineShaderStageCreateInfo[2];
            pStages[0] = vertStage;
            pStages[1] = fragStage;
            var binding = StaticModelVertexBinding();
            VertexInputAttributeDescription* attrs = stackalloc VertexInputAttributeDescription[3];
            FillStaticModelAttributes(attrs);
            var vertexInput = StaticModelVertexInput(&binding, attrs);
            var inputAssembly = new PipelineInputAssemblyStateCreateInfo { SType = StructureType.PipelineInputAssemblyStateCreateInfo, Topology = PrimitiveTopology.TriangleList };
            var viewportState = new PipelineViewportStateCreateInfo { SType = StructureType.PipelineViewportStateCreateInfo, ViewportCount = 1, ScissorCount = 1 };
            DynamicState* pDynamic = stackalloc DynamicState[2];
            pDynamic[0] = DynamicState.Viewport;
            pDynamic[1] = DynamicState.Scissor;
            var dynamicState = new PipelineDynamicStateCreateInfo { SType = StructureType.PipelineDynamicStateCreateInfo, DynamicStateCount = 2, PDynamicStates = pDynamic };
            var raster = new PipelineRasterizationStateCreateInfo { SType = StructureType.PipelineRasterizationStateCreateInfo, PolygonMode = PolygonMode.Fill, CullMode = CullModeFlags.None, FrontFace = FrontFace.Clockwise, LineWidth = 1.0f };
            var multisample = new PipelineMultisampleStateCreateInfo { SType = StructureType.PipelineMultisampleStateCreateInfo, RasterizationSamples = SampleCountFlags.Count1Bit };
            var depth = new PipelineDepthStencilStateCreateInfo { SType = StructureType.PipelineDepthStencilStateCreateInfo, DepthTestEnable = false, DepthWriteEnable = false };
            var blendAttach = new PipelineColorBlendAttachmentState { ColorWriteMask = ColorComponentFlags.RBit | ColorComponentFlags.GBit | ColorComponentFlags.BBit | ColorComponentFlags.ABit, BlendEnable = false };
            var colorBlend = new PipelineColorBlendStateCreateInfo { SType = StructureType.PipelineColorBlendStateCreateInfo, AttachmentCount = 1, PAttachments = &blendAttach, LogicOpEnable = false };
            var pipelineInfo = new GraphicsPipelineCreateInfo { SType = StructureType.GraphicsPipelineCreateInfo, StageCount = 2, PStages = pStages, PVertexInputState = &vertexInput, PInputAssemblyState = &inputAssembly, PViewportState = &viewportState, PRasterizationState = &raster, PMultisampleState = &multisample, PDepthStencilState = &depth, PColorBlendState = &colorBlend, PDynamicState = &dynamicState, Layout = layout, RenderPass = clearFrame.RenderPass, Subpass = 0 };
            if (vk.CreateGraphicsPipelines(deviceOwner.LogicalDevice, default, 1, &pipelineInfo, null, out pipeline) != Result.Success)
            {
                vk.DestroyPipelineLayout(deviceOwner.LogicalDevice, layout, null);
                VulkanShaderModuleOwner.Destroy(vk, deviceOwner, vert);
                VulkanShaderModuleOwner.Destroy(vk, deviceOwner, frag);
                log?.Invoke(VulkanPipelineLogFormatter.Failed("天空 CreateGraphicsPipelines 失败"));
                return null;
            }
        }
        VulkanShaderModuleOwner.Destroy(vk, deviceOwner, vert);
        VulkanShaderModuleOwner.Destroy(vk, deviceOwner, frag);
        log?.Invoke(VulkanPipelineLogFormatter.SkyCreated());
        return new VulkanGraphicsPipelineOwner(vk, deviceOwner, layout, pipeline, log);
    }
}
