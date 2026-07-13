using System;
using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    void BuildRenderPass()
    {
        var attachment = new AttachmentDescription
        {
            Format = _swapchainOwner.Format,
            Samples = SampleCountFlags.Count1Bit,
            LoadOp = AttachmentLoadOp.Clear,
            StoreOp = AttachmentStoreOp.Store,
            StencilLoadOp = AttachmentLoadOp.DontCare,
            StencilStoreOp = AttachmentStoreOp.DontCare,
            InitialLayout = ImageLayout.Undefined,
            FinalLayout = ImageLayout.PresentSrcKhr
        };
        var colorRef = new AttachmentReference { Attachment = 0, Layout = ImageLayout.ColorAttachmentOptimal };
        var subpass = new SubpassDescription
        {
            PipelineBindPoint = PipelineBindPoint.Graphics,
            ColorAttachmentCount = 1,
            PColorAttachments = &colorRef
        };
        var info = new RenderPassCreateInfo
        {
            SType = StructureType.RenderPassCreateInfo,
            AttachmentCount = 1,
            PAttachments = &attachment,
            SubpassCount = 1,
            PSubpasses = &subpass
        };
        var result = _vk.CreateRenderPass(_deviceOwner.LogicalDevice, &info, null, out _renderPass);
        if (!Ok(result, "CreateRenderPass")) throw new InvalidOperationException($"CreateRenderPass {result}");
    }

    void CreateCommandPool(int graphicsFamily)
    {
        var poolInfo = new CommandPoolCreateInfo
        {
            SType = StructureType.CommandPoolCreateInfo,
            QueueFamilyIndex = (uint)graphicsFamily
        };
        var result = _vk.CreateCommandPool(_deviceOwner.LogicalDevice, &poolInfo, null, out _commandPool);
        if (!Ok(result, "CreateCommandPool")) throw new InvalidOperationException($"CreateCommandPool {result}");
    }
}
