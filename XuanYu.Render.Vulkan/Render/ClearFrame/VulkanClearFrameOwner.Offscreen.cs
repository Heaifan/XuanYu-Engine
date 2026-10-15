using Silk.NET.Vulkan;
using System.Diagnostics;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    public byte[] RenderOffscreenAndReadback(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_offscreenTarget is null || _commandBuffers.Length != 1 || _readbackBuffers.Length != 1)
            throw new InvalidOperationException("离屏 RenderTarget 未就绪");
        var fenceInfo = new FenceCreateInfo { SType = StructureType.FenceCreateInfo };
        if (_vk.CreateFence(_deviceOwner.LogicalDevice, &fenceInfo, null, out var fence) != Result.Success)
            throw new InvalidOperationException("离屏渲染 Fence 创建失败");
        try
        {
            var command = _commandBuffers[0];
            var submit = new SubmitInfo { SType = StructureType.SubmitInfo,
                CommandBufferCount = 1, PCommandBuffers = &command };
            if (_vk.QueueSubmit(_deviceOwner.GraphicsQueue, 1, &submit, fence) != Result.Success)
                throw new InvalidOperationException("离屏渲染 QueueSubmit 失败");
            var timer = Stopwatch.StartNew();
            while (true)
            {
                var wait = _vk.WaitForFences(_deviceOwner.LogicalDevice, 1, &fence, true, 50_000_000);
                if (wait == Result.Success) break;
                if (wait != Result.Timeout) throw new InvalidOperationException($"离屏渲染 Fence 等待失败：{wait}");
                if (timer.Elapsed > TimeSpan.FromSeconds(30))
                {
                    _vk.DeviceWaitIdle(_deviceOwner.LogicalDevice);
                    throw new TimeoutException("离屏渲染 Fence 等待超时");
                }
            }
            cancellationToken.ThrowIfCancellationRequested();
            return _readbackBuffers[0].Read();
        }
        finally { _vk.DestroyFence(_deviceOwner.LogicalDevice, fence, null); }
    }
}
