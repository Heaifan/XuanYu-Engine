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
        if (!Check(_vk.CreateSemaphore(_deviceOwner.LogicalDevice, &semInfo, null, out _renderFinished), "CreateSemaphore(renderFinished)"))
        { DestroySync(); return false; }
        var fenceInfo = new FenceCreateInfo { SType = StructureType.FenceCreateInfo };
        if (!Check(_vk.CreateFence(_deviceOwner.LogicalDevice, &fenceInfo, null, out _fence), "CreateFence"))
        { DestroySync(); return false; }
        if (!_deviceOwner.PresentFenceEnabled || !Check(
            _vk.CreateFence(_deviceOwner.LogicalDevice, &fenceInfo, null, out _presentFence), "CreateFence(present)"))
        { DestroySync(); return false; }
        _syncCreated = true;
        return true;
    }

    void DestroySync()
    {
        if (!CanDestroySync) return;
        if (_imageAvailable.Handle != 0) _vk.DestroySemaphore(_deviceOwner.LogicalDevice, _imageAvailable, null);
        if (_renderFinished.Handle != 0) _vk.DestroySemaphore(_deviceOwner.LogicalDevice, _renderFinished, null);
        if (_fence.Handle != 0) _vk.DestroyFence(_deviceOwner.LogicalDevice, _fence, null);
        if (_presentFence.Handle != 0) _vk.DestroyFence(_deviceOwner.LogicalDevice, _presentFence, null);
        _imageAvailable = default; _renderFinished = default;
        _fence = default; _presentFence = default; _syncCreated = false;
    }
}
