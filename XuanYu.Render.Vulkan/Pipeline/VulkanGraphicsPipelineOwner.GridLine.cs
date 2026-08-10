using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Render;
using XuanYu.Render.Vulkan.Swapchain;

namespace XuanYu.Render.Vulkan.Pipeline;

// GRID-RW-1-CORR2：参考网格专用 Empty-input procedural LineList 管线。
// 不再复用 CreateFullscreenPass（其声明 StaticModel VertexBinding/Attributes，与程序化顶点矛盾）。
// Depth Bias 向相机偏移（负 ConstantFactor），消除与 MapGround 共面 Z=BaseHeight 的深度竞争；
// DepthTest=On(LessOrEqual)、DepthWrite=Off、AlphaBlend=On。
internal sealed unsafe partial class VulkanGraphicsPipelineOwner
{
    internal static VulkanGraphicsPipelineOwner? CreateReferenceGridLinePass(Vk vk,
        VulkanDeviceOwner deviceOwner, VulkanClearFrameOwner clearFrame, VulkanSwapchainOwner swapchain,
        PhysicalDevice physicalDevice, uint[] vertCode, uint[] fragCode, Action<string>? log)
    {
        var props = new PhysicalDeviceProperties();
        vk.GetPhysicalDeviceProperties(physicalDevice, &props);
        if (props.Limits.MaxPushConstantsSize < VulkanClearFrameOwner.ReferenceGridPushSize)
        {
            log?.Invoke(VulkanPipelineLogFormatter.Failed($"参考网格管线：设备 maxPushConstantsSize={props.Limits.MaxPushConstantsSize} < {VulkanClearFrameOwner.ReferenceGridPushSize}，Pass 禁用"));
            return null;
        }

        var vert = VulkanShaderModuleOwner.Create(vk, deviceOwner, vertCode);
        var frag = VulkanShaderModuleOwner.Create(vk, deviceOwner, fragCode);
        if (vert.Handle == 0 || frag.Handle == 0)
        {
            VulkanShaderModuleOwner.Destroy(vk, deviceOwner, vert);
            VulkanShaderModuleOwner.Destroy(vk, deviceOwner, frag);
            log?.Invoke(VulkanPipelineLogFormatter.Failed("参考网格管线 ShaderModule 创建失败"));
            return null;
        }
        var range = new PushConstantRange
        {
            StageFlags = ShaderStageFlags.VertexBit | ShaderStageFlags.FragmentBit,
            Offset = 0, Size = VulkanClearFrameOwner.ReferenceGridPushSize
        };
        var layoutInfo = new PipelineLayoutCreateInfo { SType = StructureType.PipelineLayoutCreateInfo, SetLayoutCount = 0, PushConstantRangeCount = 1, PPushConstantRanges = &range };
        if (vk.CreatePipelineLayout(deviceOwner.LogicalDevice, &layoutInfo, null, out var layout) != Result.Success)
        {
            VulkanShaderModuleOwner.Destroy(vk, deviceOwner, vert);
            VulkanShaderModuleOwner.Destroy(vk, deviceOwner, frag);
            log?.Invoke(VulkanPipelineLogFormatter.Failed("参考网格管线 CreatePipelineLayout 失败"));
            return null;
        }
        var entry = System.Text.Encoding.ASCII.GetBytes("main\0");
        Silk.NET.Vulkan.Pipeline pipeline = default;
        fixed (byte* pName = entry)
        {
            var vertStage = new PipelineShaderStageCreateInfo { SType = StructureType.PipelineShaderStageCreateInfo, Stage = ShaderStageFlags.VertexBit, Module = vert, PName = pName };
            var fragStage = new PipelineShaderStageCreateInfo { SType = StructureType.PipelineShaderStageCreateInfo, Stage = ShaderStageFlags.FragmentBit, Module = frag, PName = pName };
            PipelineShaderStageCreateInfo* pStages = stackalloc PipelineShaderStageCreateInfo[2];
            pStages[0] = vertStage; pStages[1] = fragStage;
            // Empty-input：顶点完全由 gl_VertexIndex 程序化生成，无 VertexBinding/Attributes。
            var vertexInput = new PipelineVertexInputStateCreateInfo { SType = StructureType.PipelineVertexInputStateCreateInfo, VertexBindingDescriptionCount = 0, VertexAttributeDescriptionCount = 0 };
            var inputAssembly = new PipelineInputAssemblyStateCreateInfo { SType = StructureType.PipelineInputAssemblyStateCreateInfo, Topology = PrimitiveTopology.LineList };
            var viewportState = new PipelineViewportStateCreateInfo { SType = StructureType.PipelineViewportStateCreateInfo, ViewportCount = 1, ScissorCount = 1 };
            DynamicState* pDynamic = stackalloc DynamicState[2] { DynamicState.Viewport, DynamicState.Scissor };
            var dynamicState = new PipelineDynamicStateCreateInfo { SType = StructureType.PipelineDynamicStateCreateInfo, DynamicStateCount = 2, PDynamicStates = pDynamic };
            var raster = new PipelineRasterizationStateCreateInfo
            {
                SType = StructureType.PipelineRasterizationStateCreateInfo, PolygonMode = PolygonMode.Fill,
                CullMode = CullModeFlags.None, FrontFace = FrontFace.Clockwise, LineWidth = 1.0f,
                DepthBiasEnable = true, DepthBiasConstantFactor = -4.0f, DepthBiasSlopeFactor = 0.0f, DepthBiasClamp = 0.0f
            };
            var multisample = new PipelineMultisampleStateCreateInfo { SType = StructureType.PipelineMultisampleStateCreateInfo, RasterizationSamples = SampleCountFlags.Count1Bit };
            var depth = new PipelineDepthStencilStateCreateInfo
            {
                SType = StructureType.PipelineDepthStencilStateCreateInfo, DepthTestEnable = true, DepthWriteEnable = false,
                DepthCompareOp = CompareOp.LessOrEqual
            };
            var blendAttach = new PipelineColorBlendAttachmentState { ColorWriteMask = ColorComponentFlags.RBit | ColorComponentFlags.GBit | ColorComponentFlags.BBit | ColorComponentFlags.ABit, BlendEnable = true, SrcColorBlendFactor = BlendFactor.SrcAlpha, DstColorBlendFactor = BlendFactor.OneMinusSrcAlpha, ColorBlendOp = BlendOp.Add, SrcAlphaBlendFactor = BlendFactor.One, DstAlphaBlendFactor = BlendFactor.OneMinusSrcAlpha, AlphaBlendOp = BlendOp.Add };
            var colorBlend = new PipelineColorBlendStateCreateInfo { SType = StructureType.PipelineColorBlendStateCreateInfo, AttachmentCount = 1, PAttachments = &blendAttach, LogicOpEnable = false };
            var pipelineInfo = new GraphicsPipelineCreateInfo
            {
                SType = StructureType.GraphicsPipelineCreateInfo, StageCount = 2, PStages = pStages,
                PVertexInputState = &vertexInput, PInputAssemblyState = &inputAssembly,
                PViewportState = &viewportState, PRasterizationState = &raster,
                PMultisampleState = &multisample, PDepthStencilState = &depth,
                PColorBlendState = &colorBlend, PDynamicState = &dynamicState,
                Layout = layout, RenderPass = clearFrame.RenderPass, Subpass = 0,
            };
            if (vk.CreateGraphicsPipelines(deviceOwner.LogicalDevice, default, 1, &pipelineInfo, null, out pipeline) != Result.Success)
            {
                vk.DestroyPipelineLayout(deviceOwner.LogicalDevice, layout, null);
                VulkanShaderModuleOwner.Destroy(vk, deviceOwner, vert);
                VulkanShaderModuleOwner.Destroy(vk, deviceOwner, frag);
                log?.Invoke(VulkanPipelineLogFormatter.Failed("参考网格管线 CreateGraphicsPipelines 失败"));
                return null;
            }
        }
        VulkanShaderModuleOwner.Destroy(vk, deviceOwner, vert);
        VulkanShaderModuleOwner.Destroy(vk, deviceOwner, frag);
        log?.Invoke(VulkanPipelineLogFormatter.GridCreated());
        return new VulkanGraphicsPipelineOwner(vk, deviceOwner, layout, pipeline, log);
    }
}
