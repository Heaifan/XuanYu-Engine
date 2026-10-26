using System.Text;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Render;
using XuanYu.Render.Vulkan.Swapchain;

namespace XuanYu.Render.Vulkan.Pipeline;

internal sealed unsafe partial class VulkanGraphicsPipelineOwner
{
    internal static VulkanGraphicsPipelineOwner? CreateMapLabel(Vk vk, VulkanDeviceOwner device,
        VulkanClearFrameOwner clear, VulkanSwapchainOwner swapchain, DescriptorSetLayout descriptorLayout,
        Action<string>? log)
    {
        var vert = VulkanShaderModuleOwner.Create(vk, device, ShaderBytecodeEditorMapLabelVert.Code);
        var frag = VulkanShaderModuleOwner.Create(vk, device, ShaderBytecodeEditorMapLabelFrag.Code);
        if (vert.Handle == 0 || frag.Handle == 0) return FailLabel(vk, device, vert, frag, log);
        var layouts = stackalloc DescriptorSetLayout[1]; layouts[0] = descriptorLayout;
        var range = new PushConstantRange { StageFlags = ShaderStageFlags.VertexBit | ShaderStageFlags.FragmentBit,
            Offset = 0, Size = VulkanScenePushConstants.SizeInBytes };
        var layoutInfo = new PipelineLayoutCreateInfo { SType = StructureType.PipelineLayoutCreateInfo,
            SetLayoutCount = 1, PSetLayouts = layouts, PushConstantRangeCount = 1, PPushConstantRanges = &range };
        if (vk.CreatePipelineLayout(device.LogicalDevice, &layoutInfo, null, out var layout) != Result.Success)
            return FailLabel(vk, device, vert, frag, log);
        var pipeline = CreateLabelGraphics(vk, device, clear, vert, frag, layout, log);
        VulkanShaderModuleOwner.Destroy(vk, device, vert); VulkanShaderModuleOwner.Destroy(vk, device, frag);
        if (pipeline.Handle == 0) { vk.DestroyPipelineLayout(device.LogicalDevice, layout, null); return null; }
        return new(vk, device, layout, pipeline, log);
    }

    static VulkanGraphicsPipelineOwner? FailLabel(Vk vk, VulkanDeviceOwner device, ShaderModule vert,
        ShaderModule frag, Action<string>? log)
    {
        VulkanShaderModuleOwner.Destroy(vk, device, vert); VulkanShaderModuleOwner.Destroy(vk, device, frag);
        log?.Invoke(VulkanPipelineLogFormatter.Failed("Map Label ShaderModule 创建失败")); return null;
    }

    static Silk.NET.Vulkan.Pipeline CreateLabelGraphics(Vk vk, VulkanDeviceOwner device,
        VulkanClearFrameOwner clear, ShaderModule vert, ShaderModule frag, PipelineLayout layout,
        Action<string>? log)
    {
        var name = Encoding.ASCII.GetBytes("main\0");
        fixed (byte* pName = name)
        {
            var stages = stackalloc PipelineShaderStageCreateInfo[2];
            stages[0] = new() { SType = StructureType.PipelineShaderStageCreateInfo, Stage = ShaderStageFlags.VertexBit, Module = vert, PName = pName };
            stages[1] = new() { SType = StructureType.PipelineShaderStageCreateInfo, Stage = ShaderStageFlags.FragmentBit, Module = frag, PName = pName };
            var input = new PipelineVertexInputStateCreateInfo { SType = StructureType.PipelineVertexInputStateCreateInfo };
            var assembly = new PipelineInputAssemblyStateCreateInfo { SType = StructureType.PipelineInputAssemblyStateCreateInfo, Topology = PrimitiveTopology.TriangleList };
            var viewport = new PipelineViewportStateCreateInfo { SType = StructureType.PipelineViewportStateCreateInfo, ViewportCount = 1, ScissorCount = 1 };
            DynamicState* dynamicItems = stackalloc DynamicState[2] { DynamicState.Viewport, DynamicState.Scissor };
            var dynamic = new PipelineDynamicStateCreateInfo { SType = StructureType.PipelineDynamicStateCreateInfo, DynamicStateCount = 2, PDynamicStates = dynamicItems };
            var raster = new PipelineRasterizationStateCreateInfo { SType = StructureType.PipelineRasterizationStateCreateInfo, PolygonMode = PolygonMode.Fill, CullMode = CullModeFlags.None, FrontFace = FrontFace.Clockwise, LineWidth = 1 };
            var multisample = new PipelineMultisampleStateCreateInfo { SType = StructureType.PipelineMultisampleStateCreateInfo, RasterizationSamples = SampleCountFlags.Count1Bit };
            var depth = DepthState(false, false);
            var blend = new PipelineColorBlendAttachmentState { ColorWriteMask = ColorComponentFlags.RBit | ColorComponentFlags.GBit | ColorComponentFlags.BBit | ColorComponentFlags.ABit, BlendEnable = true, SrcColorBlendFactor = BlendFactor.SrcAlpha, DstColorBlendFactor = BlendFactor.OneMinusSrcAlpha, ColorBlendOp = BlendOp.Add, SrcAlphaBlendFactor = BlendFactor.One, DstAlphaBlendFactor = BlendFactor.OneMinusSrcAlpha, AlphaBlendOp = BlendOp.Add };
            var color = new PipelineColorBlendStateCreateInfo { SType = StructureType.PipelineColorBlendStateCreateInfo, AttachmentCount = 1, PAttachments = &blend };
            var info = new GraphicsPipelineCreateInfo { SType = StructureType.GraphicsPipelineCreateInfo, StageCount = 2, PStages = stages, PVertexInputState = &input, PInputAssemblyState = &assembly, PViewportState = &viewport, PRasterizationState = &raster, PMultisampleState = &multisample, PDepthStencilState = &depth, PColorBlendState = &color, PDynamicState = &dynamic, Layout = layout, RenderPass = clear.RenderPass, Subpass = 0 };
            if (vk.CreateGraphicsPipelines(device.LogicalDevice, default, 1, &info, null, out var pipeline) == Result.Success) return pipeline;
        }
        log?.Invoke(VulkanPipelineLogFormatter.Failed("Map Label GraphicsPipeline 创建失败")); return default;
    }
}
