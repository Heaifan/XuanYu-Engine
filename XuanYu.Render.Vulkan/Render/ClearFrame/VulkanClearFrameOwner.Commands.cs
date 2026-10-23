using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Render.Frame;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    bool CreateFramebuffers()
    {
        _depthAttachments = new VulkanDepthAttachment?[_views.Length];
        for (var i = 0; i < _views.Length; i++)
        {
            _depthAttachments[i] = VulkanDepthAttachment.Create(_vk, _deviceOwner, _extent);
            if (_depthAttachments[i] is not { } depth) return false;
            var attachments = new[] { _views[i], depth.View };
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
        var terrainRecordingStarted = false;
        var terrainRecordingCompleted = false;
        try
        {
            terrainRecordingStarted = BeginTerrainCommandBufferRecording();
            if (!_gridTraceLogged) ResetGridTrace();
            var old = _commandBuffers;
            var next = new CommandBuffer[views.Length * _frameSlotCount];
            var alloc = new CommandBufferAllocateInfo
            {
                SType = StructureType.CommandBufferAllocateInfo,
                CommandPool = _commandPool,
                Level = CommandBufferLevel.Primary,
                CommandBufferCount = (uint)next.Length
            };
            fixed (CommandBuffer* p = next)
            {
                var result = _vk.AllocateCommandBuffers(_deviceOwner.LogicalDevice, &alloc, p);
                if (!Ok(result, "AllocateCommandBuffers")) return false;
                for (var slot = 0; slot < _frameSlotCount; slot++)
                for (var i = 0; i < views.Length; i++)
                {
                    var commandIndex = CommandBufferIndex(slot, i);
                    if (RecordOne(next[commandIndex], _framebuffers[i], i)) continue;
                    _vk.FreeCommandBuffers(_deviceOwner.LogicalDevice, _commandPool, (uint)next.Length, next);
                    return false;
                }
            }
            if (old.Length > 0) _vk.FreeCommandBuffers(_deviceOwner.LogicalDevice, _commandPool, (uint)old.Length, old);
            _commandBuffers = next;
            if (terrainRecordingStarted) { CompleteTerrainCommandBufferRecording(); terrainRecordingCompleted = true; }
            _commandBufferGeneration++;
            TraceRecordCommands(next.Length);
            return true;
        }
        finally
        {
            if (terrainRecordingStarted && !terrainRecordingCompleted) CancelTerrainCommandBufferRecording();
            _recordCommandDepth--;
        }
    }

}
