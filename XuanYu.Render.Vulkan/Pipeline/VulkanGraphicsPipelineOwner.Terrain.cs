using System.Text;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Render;
using XuanYu.Render.Vulkan.Swapchain;

namespace XuanYu.Render.Vulkan.Pipeline;

internal sealed unsafe partial class VulkanGraphicsPipelineOwner
{
    internal static VulkanGraphicsPipelineOwner? CreateTerrain(Vk vk, VulkanDeviceOwner device,
        VulkanClearFrameOwner clear, VulkanSwapchainOwner swap, Action<string>? log)
    {
        var vert = VulkanShaderModuleOwner.Create(vk, device, ShaderBytecodeTerrainVert.Code);
        var frag = VulkanShaderModuleOwner.Create(vk, device, ShaderBytecodeTerrainFrag.Code);
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
            var binding = StaticModelVertexBinding(); var attrs = stackalloc VertexInputAttributeDescription[3];
            FillStaticModelAttributes(attrs); var input = StaticModelVertexInput(&binding, attrs);
            var assembly = new PipelineInputAssemblyStateCreateInfo { SType = StructureType.PipelineInputAssemblyStateCreateInfo, Topology = PrimitiveTopology.TriangleList };
            var viewport = new PipelineViewportStateCreateInfo { SType = StructureType.PipelineViewportStateCreateInfo, ViewportCount = 1, ScissorCount = 1 };
            var dynamic = stackalloc DynamicState[2]; dynamic[0] = DynamicState.Viewport; dynamic[1] = DynamicState.Scissor;
            var dynamicState = new PipelineDynamicStateCreateInfo { SType = StructureType.PipelineDynamicStateCreateInfo, DynamicStateCount = 2, PDynamicStates = dynamic };
            var raster = new PipelineRasterizationStateCreateInfo { SType = StructureType.PipelineRasterizationStateCreateInfo, PolygonMode = PolygonMode.Fill, CullMode = CullModeFlags.None, FrontFace = FrontFace.Clockwise, LineWidth = 1 };
            var multi = new PipelineMultisampleStateCreateInfo { SType = StructureType.PipelineMultisampleStateCreateInfo, RasterizationSamples = SampleCountFlags.Count1Bit };
            var depth = new PipelineDepthStencilStateCreateInfo { SType = StructureType.PipelineDepthStencilStateCreateInfo, DepthTestEnable = true, DepthWriteEnable = true, DepthCompareOp = CompareOp.LessOrEqual };
            var blend = new PipelineColorBlendAttachmentState { ColorWriteMask = ColorComponentFlags.RBit | ColorComponentFlags.GBit | ColorComponentFlags.BBit | ColorComponentFlags.ABit };
            var color = new PipelineColorBlendStateCreateInfo { SType = StructureType.PipelineColorBlendStateCreateInfo, AttachmentCount = 1, PAttachments = &blend };
            var info = new GraphicsPipelineCreateInfo { SType = StructureType.GraphicsPipelineCreateInfo, StageCount = 2, PStages = stages, PVertexInputState = &input, PInputAssemblyState = &assembly, PViewportState = &viewport, PRasterizationState = &raster, PMultisampleState = &multi, PDepthStencilState = &depth, PColorBlendState = &color, PDynamicState = &dynamicState, Layout = layout, RenderPass = clear.RenderPass };
            if (vk.CreateGraphicsPipelines(device.LogicalDevice, default, 1, &info, null, out pipeline) != Result.Success)
            { vk.DestroyPipelineLayout(device.LogicalDevice, layout, null); return Fail(vk, device, vert, frag, log, "Terrain GraphicsPipeline 创建失败"); }
        }
        VulkanShaderModuleOwner.Destroy(vk, device, vert); VulkanShaderModuleOwner.Destroy(vk, device, frag);
        return new VulkanGraphicsPipelineOwner(vk, device, layout, pipeline, log);
    }

    static VulkanGraphicsPipelineOwner? Fail(Vk vk, VulkanDeviceOwner d, ShaderModule v, ShaderModule f, Action<string>? log, string message)
    { VulkanShaderModuleOwner.Destroy(vk, d, v); VulkanShaderModuleOwner.Destroy(vk, d, f); log?.Invoke(message); return null; }
}
