using Silk.NET.Vulkan;
using System.Text;
using XuanYu.Core.Diagnostics;
using XuanYu.Render.Vulkan.Render.Readback;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    void RebuildReadbackBuffers()
    {
        foreach (var buffer in _readbackBuffers) buffer.Dispose();
        _readbackBuffers = [];
        if (_offscreenTarget is null &&
            (!TerrainReplayOptions.Enabled || !_swapchainOwner!.ReadbackEnabled)) return;
        var size = (ulong)_extent.Width * _extent.Height * 4;
        var buffers = new VulkanReadbackBuffer[_offscreenTarget is null
            ? _swapchainOwner!.Images.Length : 1];
        for (var i = 0; i < buffers.Length; i++)
        {
            var buffer = VulkanReadbackBuffer.Create(_vk, _deviceOwner, size);
            if (buffer is null)
            {
                foreach (var created in buffers) created?.Dispose();
                Log("【VulkanReadback】Host-visible coherent buffer 创建失败；GPU PNG 不可用");
                return;
            }
            buffers[i] = buffer;
        }
        _readbackBuffers = buffers;
        Log($"【VulkanReadback】真实 Terrain RenderPass 读回缓冲已创建；图像数={buffers.Length}；尺寸={_extent.Width}x{_extent.Height}");
        _vk.GetPhysicalDeviceProperties(_deviceOwner.PhysicalDevice, out var properties);
        var device = Encoding.UTF8.GetString(properties.DeviceName, 256).TrimEnd('\0');
        ViewportProbe.LogGpuReadbackCapability(device,
            _offscreenTarget is null ? _swapchainOwner!.FormatName : _offscreenTarget.Format.ToString(),
            _offscreenTarget is not null || _swapchainOwner!.ReadbackEnabled);
    }

    public bool TryReadbackPresentedImage(uint imageIndex, out byte[] pixels)
    {
        pixels = [];
        if (imageIndex >= _readbackBuffers.Length) return false;
        pixels = _readbackBuffers[imageIndex].Read();
        return true;
    }

    void RecordReadback(CommandBuffer cb, int imageIndex)
    {
        if (imageIndex >= _readbackBuffers.Length) return;
        var image = _offscreenTarget?.Image ?? _swapchainOwner!.Images[imageIndex];
        var range = new ImageSubresourceRange { AspectMask = ImageAspectFlags.ColorBit,
            LevelCount = 1, LayerCount = 1 };
        var toTransfer = new ImageMemoryBarrier { SType = StructureType.ImageMemoryBarrier,
            SrcAccessMask = AccessFlags.ColorAttachmentWriteBit, DstAccessMask = AccessFlags.TransferReadBit,
            OldLayout = _offscreenTarget is null ? ImageLayout.PresentSrcKhr : ImageLayout.TransferSrcOptimal,
            NewLayout = ImageLayout.TransferSrcOptimal,
            SrcQueueFamilyIndex = Vk.QueueFamilyIgnored, DstQueueFamilyIndex = Vk.QueueFamilyIgnored,
            Image = image, SubresourceRange = range };
        _vk.CmdPipelineBarrier(cb, PipelineStageFlags.ColorAttachmentOutputBit,
            PipelineStageFlags.TransferBit, 0, 0, null, 0, null, 1, &toTransfer);
        var copy = new BufferImageCopy { ImageSubresource = new ImageSubresourceLayers {
            AspectMask = ImageAspectFlags.ColorBit, LayerCount = 1 },
            ImageExtent = new Extent3D(_extent.Width, _extent.Height, 1) };
        var buffer = _readbackBuffers[imageIndex].Buffer;
        _vk.CmdCopyImageToBuffer(cb, image, ImageLayout.TransferSrcOptimal, buffer, 1, &copy);
        if (_offscreenTarget is null)
        {
            var toPresent = new ImageMemoryBarrier { SType = StructureType.ImageMemoryBarrier,
                SrcAccessMask = AccessFlags.TransferReadBit, DstAccessMask = AccessFlags.MemoryReadBit,
                OldLayout = ImageLayout.TransferSrcOptimal, NewLayout = ImageLayout.PresentSrcKhr,
                SrcQueueFamilyIndex = Vk.QueueFamilyIgnored, DstQueueFamilyIndex = Vk.QueueFamilyIgnored,
                Image = image, SubresourceRange = range };
            _vk.CmdPipelineBarrier(cb, PipelineStageFlags.TransferBit,
                PipelineStageFlags.BottomOfPipeBit, 0, 0, null, 0, null, 1, &toPresent);
        }
    }
}
