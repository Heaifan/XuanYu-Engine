using Silk.NET.Vulkan;
using Semaphore = Silk.NET.Vulkan.Semaphore;
using LogicalDevice = Silk.NET.Vulkan.Device;

namespace XuanYu.Render.Vulkan.Render.Generation;

public sealed unsafe class VulkanPresentSyncGeneration : IDisposable
{
    readonly Vk _vk;
    readonly LogicalDevice _device;
    readonly Semaphore[] _renderFinished;
    readonly Fence[] _presentFences;
    bool _disposed;

    VulkanPresentSyncGeneration(Vk vk, LogicalDevice device, uint generation,
        SwapchainKHR swapchain, Semaphore[] semaphores, Fence[] fences)
    { _vk = vk; _device = device; Generation = generation; Swapchain = swapchain;
      _renderFinished = semaphores; _presentFences = fences; }

    public uint Generation { get; }
    public SwapchainKHR Swapchain { get; }
    public uint ImageCount => (uint)_renderFinished.Length;
    public Fence GetPresentFence(uint imageIndex) => _presentFences[imageIndex];

    public static bool TryCreate(Vk vk, LogicalDevice device, uint generation, SwapchainKHR swapchain,
        uint imageCount, out VulkanPresentSyncGeneration? result)
    {
        result = null;
        var semaphores = new Semaphore[imageCount];
        var info = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo };
        for (var i = 0; i < semaphores.Length; i++)
            if (vk.CreateSemaphore(device, &info, null, out semaphores[i]) != Result.Success)
            { DestroySemaphores(vk, device, semaphores); return false; }
        var fences = new Fence[imageCount];
        var fenceInfo = new FenceCreateInfo { SType = StructureType.FenceCreateInfo };
        for (var i = 0; i < fences.Length; i++)
            if (vk.CreateFence(device, &fenceInfo, null, out fences[i]) != Result.Success)
            { DestroyFences(vk, device, fences); DestroySemaphores(vk, device, semaphores); return false; }
        result = new VulkanPresentSyncGeneration(vk, device, generation, swapchain, semaphores, fences);
        return true;
    }

    public bool TryGetImageSemaphore(uint imageIndex, SwapchainKHR swapchain, out Semaphore semaphore)
    {
        semaphore = default;
        if (_disposed || swapchain.Handle != Swapchain.Handle || imageIndex >= ImageCount) return false;
        semaphore = _renderFinished[imageIndex];
        return true;
    }

    public Result ResetPresentFence(uint imageIndex)
    { var fence = _presentFences[imageIndex]; return _vk.ResetFences(_device, 1, &fence); }
    public Result WaitPresentFence(uint imageIndex, ulong timeout)
    { var fence = _presentFences[imageIndex]; return _vk.WaitForFences(_device, 1, &fence, true, timeout); }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        DestroySemaphores(_vk, _device, _renderFinished);
        DestroyFences(_vk, _device, _presentFences);
    }

    static void DestroySemaphores(Vk vk, LogicalDevice device, Semaphore[] semaphores)
    { foreach (var semaphore in semaphores) if (semaphore.Handle != 0) vk.DestroySemaphore(device, semaphore, null); }
    static void DestroyFences(Vk vk, LogicalDevice device, Fence[] fences)
    { foreach (var fence in fences) if (fence.Handle != 0) vk.DestroyFence(device, fence, null); }
}
