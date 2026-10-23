using System;
using System.Threading;
using System.Diagnostics;
using XuanYu.Core.Diagnostics;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Swapchain;
using XuanYu.Render.Vulkan.Render.Frame;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace XuanYu.Render.Vulkan.Render;

// VK-LIFE-1：Present 泵必须确认停止成功后，才允许释放同步对象。
public sealed unsafe partial class VulkanPresentLoop : IDisposable
{
    readonly Vk _vk;
    readonly VulkanDeviceOwner _deviceOwner;
    readonly VulkanSwapchainOwner _swapchainOwner;
    readonly VulkanClearFrameOwner _clearFrame;
    readonly Func<string, bool>? _onOutOfDate;
    readonly Action<string>? _onFatal;
    readonly Action<string>? _log;
    readonly Action<VulkanFenceWaitObservation>? _onFenceWait;
    int _framesInFlight;
    VulkanFrameContext[] _frames = [];
    int[] _imageOwnerSlots = [];
    uint[] _imageOwnerGenerations = [];
    int _nextFrameSlot;
    Thread? _thread;
    int _stopRequested;
    bool _syncCreated;
    bool _firstPresentLogged;
    const ulong AcquireTimeoutNs = 1_000_000_000;
    const ulong FenceTimeoutNs = 1_000_000_000;

    public VulkanPresentLoop(Vk vk, VulkanDeviceOwner deviceOwner, VulkanSwapchainOwner swapchainOwner,
        VulkanClearFrameOwner clearFrame, Func<string, bool>? onOutOfDate, Action<string>? onFatal,
        Action<string>? log, int framesInFlight = 1,
        Action<VulkanFenceWaitObservation>? onFenceWait = null)
    {
        if (framesInFlight is < 1 or > 2) throw new ArgumentOutOfRangeException(nameof(framesInFlight));
        _framesInFlight = framesInFlight;
        _vk = vk;
        _deviceOwner = deviceOwner;
        _swapchainOwner = swapchainOwner;
        _clearFrame = clearFrame;
        _onOutOfDate = onOutOfDate;
        _onFatal = onFatal;
        _log = log;
        _onFenceWait = onFenceWait;
    }

    public bool Start()
    {
        if (_thread is not null) return true;
        if (!_syncCreated && !CreateSync()) return false;
        Volatile.Write(ref _stopRequested, 0);
        _thread = new Thread(Run) { IsBackground = true, Name = "VulkanPresent" };
        _thread.Start();
        PublishLoopStarted();
        Log(VulkanClearFrameLogFormatter.LoopStarted());
        return true;
    }

    void Run()
    {
        try { RunFrames(); }
        catch (Exception ex) { Fatal($"Present 线程异常：{ex.Message}"); }
    }

}
