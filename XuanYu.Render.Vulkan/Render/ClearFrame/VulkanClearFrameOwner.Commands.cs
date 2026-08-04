using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    bool CreateFramebuffers()
    {
        _depthAttachment = VulkanDepthAttachment.Create(_vk, _deviceOwner, _extent);
        if (_depthAttachment is null) return false;
        for (var i = 0; i < _views.Length; i++)
        {
            var attachments = new[] { _views[i], _depthAttachment.View };
            fixed (ImageView* pAttachments = attachments)
            {
                var fbInfo = new FramebufferCreateInfo
                {
                    SType = StructureType.FramebufferCreateInfo,
                    RenderPass = _renderPass, AttachmentCount = 2, PAttachments = pAttachments,
                    Width = _extent.Width, Height = _extent.Height, Layers = 1
                };
                var result = _vk.CreateFramebuffer(_deviceOwner.LogicalDevice, &fbInfo, null, out _framebuffers[i]);
                if (!Ok(result, "CreateFramebuffer")) return false;
            }
        }
        return true;
    }

    bool RecordCommandBuffers(ImageView[] views)
    {
        _recordCommandDepth++;
        try
        {
            var old = _commandBuffers;
            var next = new CommandBuffer[views.Length];
            var alloc = new CommandBufferAllocateInfo
            {
                SType = StructureType.CommandBufferAllocateInfo,
                CommandPool = _commandPool,
                Level = CommandBufferLevel.Primary,
                CommandBufferCount = (uint)views.Length
            };
            fixed (CommandBuffer* p = next)
            {
                var result = _vk.AllocateCommandBuffers(_deviceOwner.LogicalDevice, &alloc, p);
                if (!Ok(result, "AllocateCommandBuffers")) return false;
                for (var i = 0; i < next.Length; i++)
                {
                    if (RecordOne(next[i], _framebuffers[i])) continue;
                    _vk.FreeCommandBuffers(_deviceOwner.LogicalDevice, _commandPool, (uint)next.Length, next);
                    return false;
                }
            }
            if (old.Length > 0) _vk.FreeCommandBuffers(_deviceOwner.LogicalDevice, _commandPool, (uint)old.Length, old);
            _commandBuffers = next;
            TraceRecordCommands(views.Length);
            return true;
        }
        finally
        {
            _recordCommandDepth--;
        }
    }

    bool RecordOne(CommandBuffer cb, Framebuffer fb)
    {
        var begin = new CommandBufferBeginInfo { SType = StructureType.CommandBufferBeginInfo };
        if (!Ok(_vk.BeginCommandBuffer(cb, &begin), "BeginCommandBuffer")) return false;
        ClearValue* clears = stackalloc ClearValue[2];
        // F5：ClearColor 仅作天空失败回退，改为浅蓝色，不再用灰色掩盖天空管线失败。
        clears[0] = new ClearValue { Color = new ClearColorValue { Float32_0 = 0.35f, Float32_1 = 0.55f, Float32_2 = 0.80f, Float32_3 = 1.0f } };
        clears[1] = new ClearValue { DepthStencil = new ClearDepthStencilValue { Depth = 1.0f, Stencil = 0 } };
        var rp = new RenderPassBeginInfo
        {
            SType = StructureType.RenderPassBeginInfo,
            RenderPass = _renderPass,
            Framebuffer = fb,
            RenderArea = new Rect2D { Offset = new Offset2D { X = 0, Y = 0 }, Extent = _extent },
            ClearValueCount = 2,
            PClearValues = clears
        };
        _vk.CmdBeginRenderPass(cb, &rp, SubpassContents.Inline);
        RecordDraw(cb);
        _vk.CmdEndRenderPass(cb);
        return Ok(_vk.EndCommandBuffer(cb), "EndCommandBuffer");
    }

}
