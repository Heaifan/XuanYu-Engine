using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Pipeline.Contracts;
using XuanYu.Render.Vulkan.Render;

namespace XuanYu.Render.Vulkan.Pipeline;

internal static unsafe class VulkanScenePipelineFactory
{
    internal static VulkanGraphicsPipelineOwner? Create(Vk vk, VulkanDeviceOwner device,
        VulkanClearFrameOwner clear, Action<string>? log,
        bool depthTest = true, bool depthWrite = true, bool analyticStroke = false)
    {
        var vert = VulkanShaderModuleFactory.Create(vk, device, VulkanVectorOverlayShaderCode.Vertex(analyticStroke));
        var frag = VulkanShaderModuleFactory.Create(vk, device, VulkanVectorOverlayShaderCode.Fragment(analyticStroke));
        if (vert.Handle == 0 || frag.Handle == 0)
            return Fail(vk, device, vert, frag, log, "ShaderModule 创建失败");
        var range = new PushConstantRange
        {
            StageFlags = VulkanGraphicsPipelineOwner.PushConstantStages(analyticStroke),
            Size = VulkanScenePushConstants.SizeInBytes
        };
        var layoutInfo = new PipelineLayoutCreateInfo
        {
            SType = StructureType.PipelineLayoutCreateInfo,
            PushConstantRangeCount = 1,
            PPushConstantRanges = &range
        };
        if (vk.CreatePipelineLayout(device.LogicalDevice, &layoutInfo, null, out var layout) != Result.Success)
            return Fail(vk, device, vert, frag, log, "PipelineLayout 创建失败");
        var name = System.Text.Encoding.ASCII.GetBytes("main\0");
        Silk.NET.Vulkan.Pipeline pipeline = default;
        fixed (byte* pName = name)
        {
            var stages = stackalloc PipelineShaderStageCreateInfo[2];
            stages[0] = new() { SType = StructureType.PipelineShaderStageCreateInfo,
                Stage = ShaderStageFlags.VertexBit, Module = vert, PName = pName };
            stages[1] = new() { SType = StructureType.PipelineShaderStageCreateInfo,
                Stage = ShaderStageFlags.FragmentBit, Module = frag, PName = pName };
            var binding = VulkanStaticModelVertexInput.Binding();
            var attrs = stackalloc VertexInputAttributeDescription[3];
            VulkanStaticModelVertexInput.FillAttributes(attrs);
            var input = VulkanStaticModelVertexInput.Create(&binding, attrs);
            DynamicState* dynamic = stackalloc DynamicState[2] { DynamicState.Viewport, DynamicState.Scissor };
            var viewport = VulkanGraphicsPipelineStates.DynamicViewport();
            var raster = VulkanGraphicsPipelineStates.Rasterization();
            var multisample = VulkanGraphicsPipelineStates.Multisample();
            var dynamicState = VulkanGraphicsPipelineStates.DynamicViewportScissor(dynamic);
            var depth = VulkanPipelineDepthState.Create(depthTest, depthWrite);
            var blend = VulkanGraphicsPipelineStates.AlphaBlendAttachment();
            var color = new PipelineColorBlendStateCreateInfo { SType = StructureType.PipelineColorBlendStateCreateInfo,
                AttachmentCount = 1, PAttachments = &blend };
            var assembly = new PipelineInputAssemblyStateCreateInfo { SType = StructureType.PipelineInputAssemblyStateCreateInfo,
                Topology = PrimitiveTopology.TriangleList };
            var info = new GraphicsPipelineCreateInfo { SType = StructureType.GraphicsPipelineCreateInfo,
                StageCount = 2, PStages = stages, PVertexInputState = &input,
                PInputAssemblyState = &assembly, PViewportState = &viewport,
                PRasterizationState = &raster,
                PMultisampleState = &multisample,
                PDepthStencilState = &depth, PColorBlendState = &color,
                PDynamicState = &dynamicState,
                Layout = layout, RenderPass = clear.RenderPass };
            if (vk.CreateGraphicsPipelines(device.LogicalDevice, default, 1, &info, null, out pipeline) != Result.Success)
            {
                vk.DestroyPipelineLayout(device.LogicalDevice, layout, null);
                return Fail(vk, device, vert, frag, log, "GraphicsPipeline 创建失败");
            }
        }
        VulkanShaderModuleFactory.Destroy(vk, device, vert);
        VulkanShaderModuleFactory.Destroy(vk, device, frag);
        var owner = new VulkanGraphicsPipelineOwner(vk, device, layout, pipeline, log);
        return VulkanPipelineCreationCompletion.Complete(owner, log, owner.Dispose,
            PipelineLogFormatter.ShaderModuleCreated(),
            PipelineLogFormatter.PipelineLayoutCreated(),
            PipelineLogFormatter.GraphicsPipelineCreated());
    }

    static VulkanGraphicsPipelineOwner? Fail(Vk vk, VulkanDeviceOwner device,
        ShaderModule vert, ShaderModule frag, Action<string>? log, string message)
    {
        VulkanShaderModuleFactory.Destroy(vk, device, vert);
        VulkanShaderModuleFactory.Destroy(vk, device, frag);
        log?.Invoke(PipelineLogFormatter.Failed(message));
        return null;
    }
}
