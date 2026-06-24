using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Clear;

sealed unsafe class VulkanClearProbeRenderSubmitScope : IDisposable
{
    readonly Vk _vk;
    readonly Silk.NET.Vulkan.Device _device;
    CommandPool _cmdPool;
    CommandBuffer _cmdBuf;
    Silk.NET.Vulkan.Semaphore _semAvail, _semFin;
    Fence _fence;
    bool _hasPool, _hasSync;

    public VulkanClearProbeRenderSubmitScope(Vk vk, Silk.NET.Vulkan.Device dev) { _vk = vk; _device = dev; }
    public CommandBuffer CmdBuf => _cmdBuf;
    public Silk.NET.Vulkan.Semaphore SemAvail => _semAvail;
    public Silk.NET.Vulkan.Semaphore SemFin => _semFin;
    public Fence Fence => _fence;

    public bool CreateCommandPool(uint queueIndex)
    {
        var ci = new CommandPoolCreateInfo { SType = StructureType.CommandPoolCreateInfo, QueueFamilyIndex = queueIndex };
        if (_vk.CreateCommandPool(_device, &ci, null, out _cmdPool) != Result.Success) return false;
        _hasPool = true;

        var allocCI = new CommandBufferAllocateInfo { SType = StructureType.CommandBufferAllocateInfo, CommandPool = _cmdPool, Level = CommandBufferLevel.Primary, CommandBufferCount = 1 };
        return _vk.AllocateCommandBuffers(_device, &allocCI, out _cmdBuf) == Result.Success;
    }

    public bool CreateSync()
    {
        var semCI = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo };
        var fenceCI = new FenceCreateInfo { SType = StructureType.FenceCreateInfo, Flags = FenceCreateFlags.SignaledBit };
        if (_vk.CreateSemaphore(_device, &semCI, null, out _semAvail) != Result.Success ||
            _vk.CreateSemaphore(_device, &semCI, null, out _semFin) != Result.Success ||
            _vk.CreateFence(_device, &fenceCI, null, out _fence) != Result.Success)
            return false;
        _hasSync = true;
        return true;
    }

    public void Dispose()
    {
        if (_device.Handle == 0) return;
        if (_hasSync)
        {
            if (_semAvail.Handle != 0) _vk.DestroySemaphore(_device, _semAvail, null);
            if (_semFin.Handle != 0) _vk.DestroySemaphore(_device, _semFin, null);
            if (_fence.Handle != 0) _vk.DestroyFence(_device, _fence, null);
        }
        if (_hasPool) _vk.DestroyCommandPool(_device, _cmdPool, null);
    }
}
