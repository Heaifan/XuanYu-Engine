using System;
using XuanYu.Render.Vulkan.Render.Generation;
using Silk.NET.Vulkan;
using Semaphore = Silk.NET.Vulkan.Semaphore;
using XuanYu.Render.Vulkan.Render.Frame;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanPresentLoop
{
    bool CreateSync()
    {
        var semInfo = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo };
        var fenceInfo = new FenceCreateInfo { SType = StructureType.FenceCreateInfo,
            Flags = FenceCreateFlags.SignaledBit };
        _frames = new VulkanFrameContext[_framesInFlight];
        for (var i = 0; i < _frames.Length; i++)
        {
            if (!Check(_vk.CreateSemaphore(_deviceOwner.LogicalDevice, &semInfo, null, out var available), "CreateSemaphore(imageAvailable)"))
            { DestroySync(); return false; }
            if (!Check(_vk.CreateFence(_deviceOwner.LogicalDevice, &fenceInfo, null, out var fence), "CreateFence(graphics)"))
            { _vk.DestroySemaphore(_deviceOwner.LogicalDevice, available, null); DestroySync(); return false; }
            _frames[i] = new VulkanFrameContext(i, available, fence);
        }
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
        var graphicsComplete = _frames.All(x => !x.GraphicsPending);
        var presentComplete = _presentPendingImages.All(x => !x);
        if (!VulkanPresentGenerationPolicy.CanRetire(graphicsComplete, presentComplete,
                _presentUnknown, _deviceLost)) return false;
        if (!VulkanPresentSyncGeneration.TryCreate(_vk, _deviceOwner.LogicalDevice,
                generation, swapchain, (uint)_swapchainOwner.Images.Length, out var next)) return false;
        _syncGeneration?.Dispose();
        var created = next ?? throw new InvalidOperationException("PresentSync generation creation returned no owner.");
        _syncGeneration = created;
        _presentUnknown = false;
        _presentPendingImages = new bool[created.ImageCount];
        _imageOwnerSlots = Enumerable.Repeat(-1, checked((int)created.ImageCount)).ToArray();
        _imageOwnerGenerations = new uint[created.ImageCount];
        _presentGeneration = generation;
        Log($"PresentSync generation created: generation={generation}; imageCount={created.ImageCount}; swapchain={swapchain.Handle}");
        return true;
    }

    void DestroySync()
    {
        if (!CanDestroySync) return;
        foreach (var frame in _frames)
        {
            if (frame.ImageAvailable.Handle != 0) _vk.DestroySemaphore(_deviceOwner.LogicalDevice, frame.ImageAvailable, null);
            if (frame.GraphicsFence.Handle != 0) _vk.DestroyFence(_deviceOwner.LogicalDevice, frame.GraphicsFence, null);
        }
        _frames = [];
        _syncGeneration?.Dispose();
        _syncGeneration = null;
        _syncCreated = false;
    }
}
