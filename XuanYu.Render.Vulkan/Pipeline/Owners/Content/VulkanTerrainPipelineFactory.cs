using System.Text;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Pipeline.Contracts;
using XuanYu.Render.Vulkan.Render;

namespace XuanYu.Render.Vulkan.Pipeline;

internal static unsafe class VulkanTerrainPipelineFactory
{
    internal static VulkanGraphicsPipelineOwner? Create(Vk vk, VulkanDeviceOwner device,
        VulkanClearFrameOwner clear, Action<string>? log)
    {
        var vert = VulkanShaderModuleFactory.Create(vk, device, ShaderBytecodeTerrainVert.Code);
        var frag = VulkanShaderModuleFactory.Create(vk, device, ShaderBytecodeTerrainFrag.Code);
        if (vert.Handle == 0 || frag.Handle == 0) return Fail(vk, device, vert, frag, log, "Terrain ShaderModule 创建失败");
        var range = new PushConstantRange { StageFlags = ShaderStageFlags.VertexBit, Size = VulkanScenePushConstants.SizeInBytes };
        var layoutInfo = new PipelineLayoutCreateInfo { SType = StructureType.PipelineLayoutCreateInfo, PushConstantRangeCount = 1, PPushConstantRanges = &range };
        if (vk.CreatePipelineLayout(device.LogicalDevice, &layoutInfo, null, out var layout) != Result.Success)
            return Fail(vk, device, vert, frag, log, "Terrain PipelineLayout 创建失败");
        var entry = Encoding.ASCII.GetBytes("main\0"); Silk.NET.Vulkan.Pipeline pipeline;
        fixed (byte* pName = entry)
        {
            var stages = stackalloc PipelineShaderStageCreateInfo[2];
            stages[0] = new() { SType = StructureType.PipelineShaderStageCreateInfo, Stage = ShaderStageFlags.VertexBit, Module = vert, PName = pName };
            stages[1] = new() { SType = StructureType.PipelineShaderStageCreateInfo, Stage = ShaderStageFlags.FragmentBit, Module = frag, PName = pName };
            var binding = VulkanStaticModelVertexInput.Binding(); var attrs = stackalloc VertexInputAttributeDescription[3];
            VulkanStaticModelVertexInput.FillAttributes(attrs); var input = VulkanStaticModelVertexInput.Create(&binding, attrs);
            var assembly = new PipelineInputAssemblyStateCreateInfo { SType = StructureType.PipelineInputAssemblyStateCreateInfo, Topology = PrimitiveTopology.TriangleList };
            var viewport = VulkanGraphicsPipelineStates.DynamicViewport();
            var dynamic = stackalloc DynamicState[2]; dynamic[0] = DynamicState.Viewport; dynamic[1] = DynamicState.Scissor;
            var dynamicState = VulkanGraphicsPipelineStates.DynamicViewportScissor(dynamic);
            var raster = VulkanGraphicsPipelineStates.Rasterization();
            var multi = VulkanGraphicsPipelineStates.Multisample();
            var depth = new PipelineDepthStencilStateCreateInfo { SType = StructureType.PipelineDepthStencilStateCreateInfo, DepthTestEnable = true, DepthWriteEnable = true, DepthCompareOp = CompareOp.GreaterOrEqual };
            var blend = new PipelineColorBlendAttachmentState { ColorWriteMask = ColorComponentFlags.RBit | ColorComponentFlags.GBit | ColorComponentFlags.BBit | ColorComponentFlags.ABit };
            var color = new PipelineColorBlendStateCreateInfo { SType = StructureType.PipelineColorBlendStateCreateInfo, AttachmentCount = 1, PAttachments = &blend };
            var info = new GraphicsPipelineCreateInfo { SType = StructureType.GraphicsPipelineCreateInfo, StageCount = 2, PStages = stages, PVertexInputState = &input, PInputAssemblyState = &assembly, PViewportState = &viewport, PRasterizationState = &raster, PMultisampleState = &multi, PDepthStencilState = &depth, PColorBlendState = &color, PDynamicState = &dynamicState, Layout = layout, RenderPass = clear.RenderPass };
            if (vk.CreateGraphicsPipelines(device.LogicalDevice, default, 1, &info, null, out pipeline) != Result.Success)
            { vk.DestroyPipelineLayout(device.LogicalDevice, layout, null); return Fail(vk, device, vert, frag, log, "Terrain GraphicsPipeline 创建失败"); }
        }
        VulkanShaderModuleFactory.Destroy(vk, device, vert); VulkanShaderModuleFactory.Destroy(vk, device, frag);
        var owner = new VulkanGraphicsPipelineOwner(vk, device, layout, pipeline, log);
        return VulkanPipelineCreationCompletion.Complete(owner, log, owner.Dispose,
            PipelineLogFormatter.GraphicsPipelineCreated());
    }

    static VulkanGraphicsPipelineOwner? Fail(Vk vk, VulkanDeviceOwner d, ShaderModule v, ShaderModule f, Action<string>? log, string message)
    { VulkanShaderModuleFactory.Destroy(vk, d, v); VulkanShaderModuleFactory.Destroy(vk, d, f); log?.Invoke(message); return null; }
}
