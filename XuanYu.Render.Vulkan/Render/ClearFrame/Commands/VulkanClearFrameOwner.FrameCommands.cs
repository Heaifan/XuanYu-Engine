using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Render.Frame;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    int CommandBufferIndex(int frameSlot, int imageIndex) =>
        VulkanFrameInFlightPolicy.CommandBufferIndex(frameSlot, checked((uint)imageIndex),
            _framebuffers.Length, _frameSlotCount);

    internal void BeginPresentFrame(VulkanFrameContext frame)
    {
        _resourceRetirement.Complete(frame.FrameIndex);
        _activeFrameContext = frame;
        _mapSurface.UpdatePatch(CurrentViewProjectionState(), frame.FrameIndex);
    }

    internal bool PreparePresentCommandBuffer(int frameSlot, uint imageIndex) =>
        RecordPresentCommandBuffer(frameSlot, imageIndex);

    internal void CompleteFrameResources(VulkanFrameContext frame) =>
        _resourceRetirement.Complete(frame.FrameIndex);

    internal bool ReconfigureFrameSlots(int frameCount, bool allFramesDrained)
    {
        if (!allFramesDrained || frameCount is < 1 or > 2) return false;
        _resourceRetirement.Complete(0);
        _resourceRetirement.Complete(1);
        if (_commandBuffers.Length > 0)
            _vk.FreeCommandBuffers(_deviceOwner.LogicalDevice, _commandPool,
                (uint)_commandBuffers.Length, _commandBuffers);
        _commandBuffers = [];
        _frameSlotCount = frameCount;
        _activeFrameContext = null;
        if (!_mapSurface.ReconfigureFrames(frameCount)) return false;
        return _views.Length == 0 || RecordCommandBuffers(_views);
    }

    internal void TrackFrameResource(IDisposable resource)
    {
        if (_activeFrameContext is { } frame)
            _resourceRetirement.Track(frame.FrameIndex, resource);
    }

    void RetireGpuResource(IDisposable resource) => _resourceRetirement.Retire(resource);

    void RetireTerrainGpuResource(IDisposable resource, Action? onDisposed) =>
        _resourceRetirement.Retire(resource, onDisposed);

    internal CommandBuffer GetPresentCommandBuffer(int frameSlot, uint imageIndex)
    {
        if (frameSlot < 0 || frameSlot >= _frameSlotCount || imageIndex >= _framebuffers.Length)
            throw new ArgumentOutOfRangeException(nameof(imageIndex));
        return _commandBuffers[CommandBufferIndex(frameSlot, checked((int)imageIndex))];
    }

    internal bool RecordPresentCommandBuffer(int frameSlot, uint imageIndex)
    {
        if (frameSlot < 0 || frameSlot >= _frameSlotCount || imageIndex >= _framebuffers.Length)
            return false;
        var index = CommandBufferIndex(frameSlot, checked((int)imageIndex));
        if (!Ok(_vk.ResetCommandBuffer(_commandBuffers[index], 0), "ResetCommandBuffer")) return false;
        if (!RecordOne(_commandBuffers[index], _framebuffers[imageIndex], checked((int)imageIndex)))
            return false;
        return true;
    }

    bool RecordOne(CommandBuffer cb, Framebuffer fb, int imageIndex)
    {
        var begin = new CommandBufferBeginInfo { SType = StructureType.CommandBufferBeginInfo };
        if (!Ok(_vk.BeginCommandBuffer(cb, &begin), "BeginCommandBuffer")) return false;
        ClearValue* clears = stackalloc ClearValue[2];
        clears[0] = new ClearValue { Color = new ClearColorValue
            { Float32_0 = 0.35f, Float32_1 = 0.55f, Float32_2 = 0.80f, Float32_3 = 1.0f } };
        clears[1] = new ClearValue { DepthStencil = new ClearDepthStencilValue { Depth = 0.0f } };
        var rp = new RenderPassBeginInfo { SType = StructureType.RenderPassBeginInfo,
            RenderPass = _renderPass, Framebuffer = fb,
            RenderArea = new Rect2D { Extent = _extent }, ClearValueCount = 2,
            PClearValues = clears };
        _timestampQuery?.RecordStart(cb);
        _vk.CmdBeginRenderPass(cb, &rp, SubpassContents.Inline);
        RecordDraw(cb);
        _vk.CmdEndRenderPass(cb);
        _timestampQuery?.RecordEnd(cb);
        RecordReadback(cb, imageIndex);
        return Ok(_vk.EndCommandBuffer(cb), "EndCommandBuffer");
    }
}
