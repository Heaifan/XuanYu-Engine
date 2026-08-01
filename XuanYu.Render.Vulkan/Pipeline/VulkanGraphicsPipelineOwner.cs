using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Render;
using XuanYu.Render.Vulkan.Swapchain;

namespace XuanYu.Render.Vulkan.Pipeline;

// VK5-A：持有 PipelineLayout + GraphicsPipeline。ShaderModule 在创建期已释放（不持有）。Dispose 释放 Pipeline→Layout。
internal sealed unsafe class VulkanGraphicsPipelineOwner : IDisposable
{
    readonly Vk _vk;
    readonly VulkanDeviceOwner _deviceOwner;
    readonly Action<string>? _log;
    PipelineLayout _layout;
    Silk.NET.Vulkan.Pipeline _pipeline;
    public Silk.NET.Vulkan.Pipeline Pipeline => _pipeline;
    public PipelineLayout Layout => _layout;

    VulkanGraphicsPipelineOwner(Vk vk, VulkanDeviceOwner deviceOwner, PipelineLayout layout, Silk.NET.Vulkan.Pipeline pipeline, Action<string>? log)
    {
        _vk = vk; _deviceOwner = deviceOwner; _layout = layout; _pipeline = pipeline; _log = log;
    }

    internal static VulkanGraphicsPipelineOwner? Create(Vk vk, VulkanDeviceOwner deviceOwner,
        VulkanClearFrameOwner clearFrame, VulkanSwapchainOwner swapchain, Action<string>? log)
    {
        var vert = VulkanShaderModuleOwner.Create(vk, deviceOwner, ShaderBytecodeVert.Code);
        var frag = VulkanShaderModuleOwner.Create(vk, deviceOwner, ShaderBytecodeFrag.Code);
        if (vert.Handle == 0 || frag.Handle == 0)
        {
            VulkanShaderModuleOwner.Destroy(vk, deviceOwner, vert);
            VulkanShaderModuleOwner.Destroy(vk, deviceOwner, frag);
            log?.Invoke(VulkanPipelineLogFormatter.Failed("ShaderModule 创建失败"));
            return null;
        }
        log?.Invoke(VulkanPipelineLogFormatter.ShaderModuleCreated());
        var range = new PushConstantRange { StageFlags = ShaderStageFlags.VertexBit, Offset = 0, Size = VulkanScenePushConstants.SizeInBytes };
        var layoutInfo = new PipelineLayoutCreateInfo { SType = StructureType.PipelineLayoutCreateInfo, SetLayoutCount = 0, PushConstantRangeCount = 1, PPushConstantRanges = &range };
        var layoutResult = vk.CreatePipelineLayout(deviceOwner.LogicalDevice, &layoutInfo, null, out var layout);
        if (layoutResult != Result.Success)
        {
            VulkanShaderModuleOwner.Destroy(vk, deviceOwner, vert);
            VulkanShaderModuleOwner.Destroy(vk, deviceOwner, frag);
            log?.Invoke(VulkanPipelineLogFormatter.Failed($"CreatePipelineLayout {layoutResult}"));
            return null;
        }
        log?.Invoke(VulkanPipelineLogFormatter.PipelineLayoutCreated());
        var entry = System.Text.Encoding.ASCII.GetBytes("main\0");
        Silk.NET.Vulkan.Pipeline pipeline = default;
        fixed (byte* pName = entry)
        {
            var vertStage = new PipelineShaderStageCreateInfo { SType = StructureType.PipelineShaderStageCreateInfo, Stage = ShaderStageFlags.VertexBit, Module = vert, PName = pName };
            var fragStage = new PipelineShaderStageCreateInfo { SType = StructureType.PipelineShaderStageCreateInfo, Stage = ShaderStageFlags.FragmentBit, Module = frag, PName = pName };
            PipelineShaderStageCreateInfo* pStages = stackalloc PipelineShaderStageCreateInfo[2];
            pStages[0] = vertStage; pStages[1] = fragStage;
            var vertexInput = new PipelineVertexInputStateCreateInfo { SType = StructureType.PipelineVertexInputStateCreateInfo, VertexBindingDescriptionCount = 0, VertexAttributeDescriptionCount = 0 };
            var inputAssembly = new PipelineInputAssemblyStateCreateInfo { SType = StructureType.PipelineInputAssemblyStateCreateInfo, Topology = PrimitiveTopology.TriangleList };
            var viewportState = new PipelineViewportStateCreateInfo { SType = StructureType.PipelineViewportStateCreateInfo, ViewportCount = 1, ScissorCount = 1 };
            DynamicState* pDynamic = stackalloc DynamicState[2]; pDynamic[0] = DynamicState.Viewport; pDynamic[1] = DynamicState.Scissor;
            var dynamicState = new PipelineDynamicStateCreateInfo { SType = StructureType.PipelineDynamicStateCreateInfo, DynamicStateCount = 2, PDynamicStates = pDynamic };
            var raster = new PipelineRasterizationStateCreateInfo { SType = StructureType.PipelineRasterizationStateCreateInfo, PolygonMode = PolygonMode.Fill, CullMode = CullModeFlags.None, FrontFace = FrontFace.Clockwise, LineWidth = 1.0f };
            var multisample = new PipelineMultisampleStateCreateInfo { SType = StructureType.PipelineMultisampleStateCreateInfo, RasterizationSamples = SampleCountFlags.Count1Bit };
            var blendAttach = new PipelineColorBlendAttachmentState { ColorWriteMask = ColorComponentFlags.RBit | ColorComponentFlags.GBit | ColorComponentFlags.BBit | ColorComponentFlags.ABit, BlendEnable = true, SrcColorBlendFactor = BlendFactor.SrcAlpha, DstColorBlendFactor = BlendFactor.OneMinusSrcAlpha, ColorBlendOp = BlendOp.Add, SrcAlphaBlendFactor = BlendFactor.One, DstAlphaBlendFactor = BlendFactor.OneMinusSrcAlpha, AlphaBlendOp = BlendOp.Add };
            var colorBlend = new PipelineColorBlendStateCreateInfo { SType = StructureType.PipelineColorBlendStateCreateInfo, AttachmentCount = 1, PAttachments = &blendAttach, LogicOpEnable = false };
            var pipelineInfo = new GraphicsPipelineCreateInfo
            {
                SType = StructureType.GraphicsPipelineCreateInfo,
                StageCount = 2, PStages = pStages,
                PVertexInputState = &vertexInput, PInputAssemblyState = &inputAssembly,
                PViewportState = &viewportState, PRasterizationState = &raster,
                PMultisampleState = &multisample, PColorBlendState = &colorBlend, PDynamicState = &dynamicState,
                Layout = layout, RenderPass = clearFrame.RenderPass, Subpass = 0,
            };
            var res = vk.CreateGraphicsPipelines(deviceOwner.LogicalDevice, default, 1, &pipelineInfo, null, out pipeline);
            if (res != Result.Success)
            {
                log?.Invoke(VulkanPipelineLogFormatter.Failed($"CreateGraphicsPipelines {res}"));
                vk.DestroyPipelineLayout(deviceOwner.LogicalDevice, layout, null);
                VulkanShaderModuleOwner.Destroy(vk, deviceOwner, vert);
                VulkanShaderModuleOwner.Destroy(vk, deviceOwner, frag);
                return null;
            }
        }
        VulkanShaderModuleOwner.Destroy(vk, deviceOwner, vert);
        VulkanShaderModuleOwner.Destroy(vk, deviceOwner, frag);
        log?.Invoke(VulkanPipelineLogFormatter.GraphicsPipelineCreated());
        return new VulkanGraphicsPipelineOwner(vk, deviceOwner, layout, pipeline, log);
    }

    public void Dispose()
    {
        if (_pipeline.Handle == 0 && _layout.Handle == 0) return;
        if (_pipeline.Handle != 0) _vk.DestroyPipeline(_deviceOwner.LogicalDevice, _pipeline, null);
        if (_layout.Handle != 0) _vk.DestroyPipelineLayout(_deviceOwner.LogicalDevice, _layout, null);
        _pipeline = default; _layout = default;
        _log?.Invoke(VulkanPipelineLogFormatter.Disposed());
    }
}
