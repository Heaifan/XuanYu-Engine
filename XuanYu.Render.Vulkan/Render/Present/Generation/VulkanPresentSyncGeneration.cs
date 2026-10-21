using Silk.NET.Vulkan;
using Semaphore = Silk.NET.Vulkan.Semaphore;
using LogicalDevice = Silk.NET.Vulkan.Device;

namespace XuanYu.Render.Vulkan.Render.Generation;

public sealed unsafe class VulkanPresentSyncGeneration : IDisposable
{
    readonly Vk _vk;
    readonly LogicalDevice _device;
    readonly Semaphore[] _renderFinished;
    Fence _presentFence;
    bool _disposed;

    VulkanPresentSyncGeneration(Vk vk, LogicalDevice device, uint generation,
        SwapchainKHR swapchain, Semaphore[] semaphores, Fence fence)
    { _vk = vk; _device = device; Generation = generation; Swapchain = swapchain;
      _renderFinished = semaphores; _presentFence = fence; }

    public uint Generation { get; }
    public SwapchainKHR Swapchain { get; }
    public uint ImageCount => (uint)_renderFinished.Length;
    public Fence PresentFence => _presentFence;

    public static bool TryCreate(Vk vk, LogicalDevice device, uint generation, SwapchainKHR swapchain,
        uint imageCount, out VulkanPresentSyncGeneration? result)
    {
        result = null;
        var semaphores = new Semaphore[imageCount];
        var info = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo };
        for (var i = 0; i < semaphores.Length; i++)
            if (vk.CreateSemaphore(device, &info, null, out semaphores[i]) != Result.Success)
            { DestroySemaphores(vk, device, semaphores); return false; }
        var fenceInfo = new FenceCreateInfo { SType = StructureType.FenceCreateInfo };
        if (vk.CreateFence(device, &fenceInfo, null, out var fence) != Result.Success)
        { DestroySemaphores(vk, device, semaphores); return false; }
        result = new VulkanPresentSyncGeneration(vk, device, generation, swapchain, semaphores, fence);
        return true;
    }

    public bool TryGetImageSemaphore(uint imageIndex, SwapchainKHR swapchain, out Semaphore semaphore)
    {
        semaphore = default;
        if (_disposed || swapchain.Handle != Swapchain.Handle || imageIndex >= ImageCount) return false;
        semaphore = _renderFinished[imageIndex];
        return true;
    }

    public Result ResetPresentFence() { var fence = _presentFence; return _vk.ResetFences(_device, 1, &fence); }
    public Result WaitPresentFence(ulong timeout) { var fence = _presentFence; return _vk.WaitForFences(_device, 1, &fence, true, timeout); }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        DestroySemaphores(_vk, _device, _renderFinished);
        if (_presentFence.Handle != 0) _vk.DestroyFence(_device, _presentFence, null);
        _presentFence = default;
    }

    static void DestroySemaphores(Vk vk, LogicalDevice device, Semaphore[] semaphores)
    { foreach (var semaphore in semaphores) if (semaphore.Handle != 0) vk.DestroySemaphore(device, semaphore, null); }
}
