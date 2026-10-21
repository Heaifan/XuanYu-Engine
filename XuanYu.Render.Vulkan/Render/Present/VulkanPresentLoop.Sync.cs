using System;
using XuanYu.Render.Vulkan.Render.Generation;
using Silk.NET.Vulkan;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanPresentLoop
{
    bool CreateSync()
    {
        var semInfo = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo };
        if (!Check(_vk.CreateSemaphore(_deviceOwner.LogicalDevice, &semInfo, null, out _imageAvailable), "CreateSemaphore(imageAvailable)"))
        { DestroySync(); return false; }
        var fenceInfo = new FenceCreateInfo { SType = StructureType.FenceCreateInfo };
        if (!Check(_vk.CreateFence(_deviceOwner.LogicalDevice, &fenceInfo, null, out _fence), "CreateFence(graphics)"))
        { DestroySync(); return false; }
        _syncCreated = true;
        if (!EnsurePresentGeneration()) { DestroySync(); return false; }
        return true;
    }

    bool EnsurePresentGeneration()
    {
        var generation = _swapchainOwner.ResourceGeneration;
        var swapchain = _swapchainOwner.Swapchain;
        if (_syncGeneration is { } current && current.Generation == generation &&
            current.Swapchain.Handle == swapchain.Handle) return true;
        if (!VulkanPresentGenerationPolicy.CanRetire(!_graphicsPending, !_presentPending,
                _presentUnknown, _deviceLost)) return false;
        if (!VulkanPresentSyncGeneration.TryCreate(_vk, _deviceOwner.LogicalDevice,
                generation, swapchain, (uint)_swapchainOwner.Images.Length, out var next)) return false;
        _syncGeneration?.Dispose();
        var created = next ?? throw new InvalidOperationException("PresentSync generation creation returned no owner.");
        _syncGeneration = created;
        _presentUnknown = false;
        _presentPending = false;
        _presentGeneration = generation;
        Log($"PresentSync generation created: generation={generation}; imageCount={created.ImageCount}; swapchain={swapchain.Handle}");
        return true;
    }

    void DestroySync()
    {
        if (!CanDestroySync) return;
        if (_imageAvailable.Handle != 0) _vk.DestroySemaphore(_deviceOwner.LogicalDevice, _imageAvailable, null);
        if (_fence.Handle != 0) _vk.DestroyFence(_deviceOwner.LogicalDevice, _fence, null);
        _syncGeneration?.Dispose();
        _syncGeneration = null;
        _imageAvailable = default;
        _fence = default;
        _syncCreated = false;
    }
}
