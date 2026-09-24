using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using VkSemaphore = Silk.NET.Vulkan.Semaphore;

namespace XuanYu.Viewport.CompositionSpike;

unsafe sealed class VulkanCommandPool : IDisposable
{
    readonly Vk _api; readonly Device _device; readonly Queue _queue; readonly CommandPool _pool;
    public VulkanCommandPool(Vk api, Device device, Queue queue, uint family)
    { _api = api; _device = device; _queue = queue; var info = new CommandPoolCreateInfo { SType = StructureType.CommandPoolCreateInfo, QueueFamilyIndex = family, Flags = CommandPoolCreateFlags.ResetCommandBufferBit }; api.CreateCommandPool(device, in info, null, out _pool).Ensure(); }
    public void Clear(Image image, ImageLayout oldLayout, ImageLayout newLayout, VkSemaphore signal)
    {
        var alloc = new CommandBufferAllocateInfo { SType = StructureType.CommandBufferAllocateInfo, CommandPool = _pool, Level = CommandBufferLevel.Primary, CommandBufferCount = 1 };
        _api.AllocateCommandBuffers(_device, in alloc, out var command).Ensure();
        var begin = new CommandBufferBeginInfo { SType = StructureType.CommandBufferBeginInfo, Flags = CommandBufferUsageFlags.OneTimeSubmitBit }; _api.BeginCommandBuffer(command, in begin).Ensure();
        var barrier = new ImageMemoryBarrier { SType = StructureType.ImageMemoryBarrier, OldLayout = oldLayout, NewLayout = ImageLayout.TransferDstOptimal, SrcAccessMask = AccessFlags.None, DstAccessMask = AccessFlags.TransferWriteBit, Image = image, SubresourceRange = new ImageSubresourceRange(ImageAspectFlags.ColorBit, 0, 1, 0, 1) };
        _api.CmdPipelineBarrier(command, PipelineStageFlags.TopOfPipeBit, PipelineStageFlags.TransferBit, DependencyFlags.None, 0, null, 0, null, 1, in barrier);
        var color = new ClearColorValue(0.08f, 0.27f, 0.38f, 1f); _api.CmdClearColorImage(command, image, ImageLayout.TransferDstOptimal, in color, 1, in barrier.SubresourceRange);
        var present = new ImageMemoryBarrier { SType = StructureType.ImageMemoryBarrier, OldLayout = ImageLayout.TransferDstOptimal, NewLayout = newLayout, SrcAccessMask = AccessFlags.TransferWriteBit, DstAccessMask = AccessFlags.MemoryReadBit, Image = image, SubresourceRange = barrier.SubresourceRange };
        _api.CmdPipelineBarrier(command, PipelineStageFlags.TransferBit, PipelineStageFlags.BottomOfPipeBit, DependencyFlags.None, 0, null, 0, null, 1, in present); _api.EndCommandBuffer(command).Ensure();
        var submit = new SubmitInfo { SType = StructureType.SubmitInfo, CommandBufferCount = 1, PCommandBuffers = &command, SignalSemaphoreCount = 1, PSignalSemaphores = &signal }; _api.QueueSubmit(_queue, 1, in submit, default).Ensure(); _api.QueueWaitIdle(_queue).Ensure(); _api.FreeCommandBuffers(_device, _pool, 1, in command);
    }
    public void Dispose() => _api.DestroyCommandPool(_device, _pool, null);
}
