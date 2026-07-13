using System;
using System.Diagnostics;
using System.Threading;
using Silk.NET.Vulkan;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanPresentLoop
{
    public bool Stop()
    {
        var t = _thread;
        if (t is null) return true;
        Volatile.Write(ref _stopRequested, 1);
        if (!t.Join(2000))
        {
            Log(VulkanClearFrameLogFormatter.LoopStopTimedOut());
            return false;
        }
        _thread = null;
        Log(VulkanClearFrameLogFormatter.LoopStopped());
        return true;
    }

    public void Dispose()
    {
        if (!Stop()) return;
        DestroySync();
    }

    bool CreateSync()
    {
        var semInfo = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo };
        if (!Check(_vk.CreateSemaphore(_deviceOwner.LogicalDevice, &semInfo, null, out _imageAvailable), "CreateSemaphore(imageAvailable)"))
        {
            DestroySync();
            return false;
        }
        if (!Check(_vk.CreateSemaphore(_deviceOwner.LogicalDevice, &semInfo, null, out _renderFinished), "CreateSemaphore(renderFinished)"))
        {
            DestroySync();
            return false;
        }
        var fenceInfo = new FenceCreateInfo { SType = StructureType.FenceCreateInfo };
        if (!Check(_vk.CreateFence(_deviceOwner.LogicalDevice, &fenceInfo, null, out _fence), "CreateFence"))
        {
            DestroySync();
            return false;
        }
        _syncCreated = true;
        return true;
    }

    void DestroySync()
    {
        if (_imageAvailable.Handle != 0) _vk.DestroySemaphore(_deviceOwner.LogicalDevice, _imageAvailable, null);
        if (_renderFinished.Handle != 0) _vk.DestroySemaphore(_deviceOwner.LogicalDevice, _renderFinished, null);
        if (_fence.Handle != 0) _vk.DestroyFence(_deviceOwner.LogicalDevice, _fence, null);
        _imageAvailable = default;
        _renderFinished = default;
        _fence = default;
        _syncCreated = false;
    }

    bool Check(Result res, string op, bool allowSuboptimal = false)
    {
        if (res == Result.Success) return true;
        if (allowSuboptimal && res == Result.SuboptimalKhr) return true;
        Log(VulkanClearFrameLogFormatter.PresentError($"{op} 失败：{res}"));
        return false;
    }

    void Log(string message)
    {
        try
        {
            _log?.Invoke(message);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(VulkanClearFrameLogFormatter.LogFallback(ex.Message, message));
        }
    }
}
