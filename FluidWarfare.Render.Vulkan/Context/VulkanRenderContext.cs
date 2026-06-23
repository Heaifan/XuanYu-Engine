using System.Diagnostics;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Context;

public sealed unsafe class VulkanRenderContext : IDisposable
{
    Vk? _vk;
    Silk.NET.Vulkan.Instance _instance;
    Silk.NET.Vulkan.Device _device;
    Silk.NET.Vulkan.PhysicalDevice _physicalDevice;
    Queue _graphicsQueue;
    uint _graphicsQueueFamilyIndex;
    SurfaceKHR _surface;
    bool _initialized;
    string _lastErrorMessage = "";
    nint _fnDestroySurfaceKHR, _fnCreateWin32SurfaceKHR, _fnGetCaps, _fnGetFmts, _fnGetModes;
    nint _fnCreateSC, _fnDestroySC, _fnGetSCImages, _fnAcquire, _fnPresent;
    VulkanRenderContextSetup _setup = new();
    VulkanRenderContextSelector _selector = new();

    public string PhysicalDeviceName { get; private set; } = "未知";
    public string ApiVersionText { get; private set; } = "未知";
    public uint SwapchainImageCount { get; private set; }
    public bool IsInitialized => _initialized;

    public bool Initialize(nint hinstance, nint hwnd, out string message)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            _vk = Vk.GetApi(); _setup.SetVk(_vk);
            if (!_setup.CreateInstance(out _instance, out var v)) { return Fail("Instance 创建失败。", out message); }
            ApiVersionText = v; _setup.SetInstance(_instance);
            _fnDestroySurfaceKHR = _setup.GetInstanceProc("vkDestroySurfaceKHR");
            _fnCreateWin32SurfaceKHR = _setup.GetInstanceProc("vkCreateWin32SurfaceKHR");
            _fnGetCaps = _setup.GetInstanceProc("vkGetPhysicalDeviceSurfaceCapabilitiesKHR");
            _fnGetFmts = _setup.GetInstanceProc("vkGetPhysicalDeviceSurfaceFormatsKHR");
            _fnGetModes = _setup.GetInstanceProc("vkGetPhysicalDeviceSurfacePresentModesKHR");
            if (_fnDestroySurfaceKHR == 0) { return Fail("无法加载 Instance 扩展。", out message); }
            if (!_selector.Select(_vk, _instance, out _physicalDevice, out _graphicsQueueFamilyIndex, out var n))
                { return Fail("未找到物理设备。", out message); }
            PhysicalDeviceName = n;
            if (!_setup.CreateDevice(_physicalDevice, _graphicsQueueFamilyIndex, out _device))
                { return Fail("Device 创建失败。", out message); }
            _fnCreateSC = _setup.GetDeviceProc(_device, "vkCreateSwapchainKHR");
            _fnDestroySC = _setup.GetDeviceProc(_device, "vkDestroySwapchainKHR");
            _fnGetSCImages = _setup.GetDeviceProc(_device, "vkGetSwapchainImagesKHR");
            _fnAcquire = _setup.GetDeviceProc(_device, "vkAcquireNextImageKHR");
            _fnPresent = _setup.GetDeviceProc(_device, "vkQueuePresentKHR");
            _vk.GetDeviceQueue(_device, _graphicsQueueFamilyIndex, 0, out _graphicsQueue);
            if (!_setup.CreateSurface(_instance, _fnCreateWin32SurfaceKHR, hinstance, hwnd, out _surface))
                { return Fail("Surface 创建失败。", out message); }

            var legacy = new VulkanRenderContextLegacy();
            if (!legacy.TryCreateDeviceResources(_vk, _device, _physicalDevice, _instance, _surface,
                    _graphicsQueueFamilyIndex, _fnCreateSC, _fnDestroySC, _fnGetSCImages, _fnAcquire, _fnPresent,
                    _fnGetCaps, _fnGetFmts, _fnGetModes))
            { _lastErrorMessage = legacy.LastErrorMessage; message = _lastErrorMessage; return false; }
            _initialized = true;
            sw.Stop();
            message = $"全部创建成功，用时 {sw.Elapsed.TotalMilliseconds:F2} ms，Swapchain 图像数：{SwapchainImageCount}。";
            return true;
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        { sw.Stop(); _lastErrorMessage = $"初始化失败：{ex.Message}"; message = _lastErrorMessage; Cleanup(); return false; }
    }

    bool Fail(string msg, out string m) { _lastErrorMessage = msg; m = msg; return false; }

    public bool RenderFrame()
    {
        if (!_initialized || _vk is null) return false;
        return false;
    }

    public void Shutdown() => Cleanup();
    public void Dispose() => Shutdown();

    void Cleanup()
    {
        if (_vk is null) return;
        if (_device.Handle != 0) { _vk.DeviceWaitIdle(_device); _vk.DestroyDevice(_device, null); }
        if (_surface.Handle != 0 && _fnDestroySurfaceKHR != 0)
            Marshal.GetDelegateForFunctionPointer<DestroySurfacePtr>(_fnDestroySurfaceKHR)(_instance, _surface, null);
        if (_instance.Handle != 0) _vk.DestroyInstance(_instance, null);
        _initialized = false;
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)] delegate void DestroySurfacePtr(Silk.NET.Vulkan.Instance i, SurfaceKHR s, AllocationCallbacks* a);
}
