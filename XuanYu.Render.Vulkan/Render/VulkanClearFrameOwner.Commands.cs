using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    bool CreateFramebuffers()
    {
        for (var i = 0; i < _views.Length; i++)
        {
            fixed (ImageView* pView = &_views[i])
            {
                var fbInfo = new FramebufferCreateInfo
                {
                    SType = StructureType.FramebufferCreateInfo,
                    RenderPass = _renderPass,
                    AttachmentCount = 1,
                    PAttachments = pView,
                    Width = _extent.Width,
                    Height = _extent.Height,
                    Layers = 1
                };
                var result = _vk.CreateFramebuffer(_deviceOwner.LogicalDevice, &fbInfo, null, out _framebuffers[i]);
                if (!Ok(result, "CreateFramebuffer")) return false;
            }
        }
        return true;
    }

    bool RecordCommandBuffers(ImageView[] views)
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
        return true;
    }

    bool RecordOne(CommandBuffer cb, Framebuffer fb)
    {
        var begin = new CommandBufferBeginInfo { SType = StructureType.CommandBufferBeginInfo };
        if (!Ok(_vk.BeginCommandBuffer(cb, &begin), "BeginCommandBuffer")) return false;
        var clear = new ClearValue { Color = new ClearColorValue { Float32_0 = 0.25f, Float32_1 = 0.45f, Float32_2 = 0.70f, Float32_3 = 1.0f } };
        var rp = new RenderPassBeginInfo
        {
            SType = StructureType.RenderPassBeginInfo,
            RenderPass = _renderPass,
            Framebuffer = fb,
            RenderArea = new Rect2D { Offset = new Offset2D { X = 0, Y = 0 }, Extent = _extent },
            ClearValueCount = 1,
            PClearValues = &clear
        };
        _vk.CmdBeginRenderPass(cb, &rp, SubpassContents.Inline);
        RecordDraw(cb);
        _vk.CmdEndRenderPass(cb);
        return Ok(_vk.EndCommandBuffer(cb), "EndCommandBuffer");
    }

}
