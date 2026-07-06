using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace XuanYu.Editor.UI;

public sealed unsafe partial class VulkanClearSession : IDisposable
{
    readonly Vk _vk;
    readonly Action<string, string> _log;
    Instance _instance;
    Device _device;
    PhysicalDevice _physicalDevice;
    SurfaceKHR _surface;
    SwapchainKHR _swapchain;
    KhrSurface? _khrSurface;
    KhrWin32Surface? _khrWin32Surface;
    KhrSwapchain? _khrSwapchain;
    uint _queueFamily;
    uint _width, _height;
    bool _disposed;

    VulkanClearSession(nint hwnd, uint width, uint height, Action<string, string> log)
    {
        _vk = Vk.GetApi();
        _log = log;
        _width = width; _height = height;
        CreateInstance();
        CreateSurface(hwnd);
        PickDevice();
        CreateDevice();
        CreateSwapchain();
        IsReady = true;
        _log("Vulkan 初始化成功", "Instance / Surface / Device / Swapchain 已创建。");
    }

    public bool IsReady { get; private set; }

    public static VulkanClearSession? TryCreate(nint hwnd, uint width, uint height, Action<string, string> log)
    {
        try { return new VulkanClearSession(hwnd, width, height, log); }
        catch (Exception ex) { log("Vulkan 初始化失败", ex.Message); return null; }
    }

    public void Resize(uint width, uint height)
    {
        if (!IsReady || width == 0 || height == 0 || (width == _width && height == _height)) return;
        _width = width; _height = height;
        try { DestroySwapchain(); CreateSwapchain(); _log("Swapchain 重建成功", $"{width} x {height}"); }
        catch (Exception ex) { _log("Swapchain 重建失败", ex.Message); }
    }

    void CreateInstance()
    {
        var appBytes = System.Text.Encoding.UTF8.GetBytes("XuanYu Engine\0");
        var surfaceBytes = System.Text.Encoding.UTF8.GetBytes("VK_KHR_surface\0");
        var win32Bytes = System.Text.Encoding.UTF8.GetBytes("VK_KHR_win32_surface\0");
        fixed (byte* app = appBytes)
        fixed (byte* surface = surfaceBytes)
        fixed (byte* win32 = win32Bytes)
        {
            var appInfo = new ApplicationInfo { SType = StructureType.ApplicationInfo, PApplicationName = app, PEngineName = app, ApiVersion = Vk.Version12 };
            byte** ext = stackalloc byte*[2]; ext[0] = surface; ext[1] = win32;
            var create = new InstanceCreateInfo { SType = StructureType.InstanceCreateInfo, PApplicationInfo = &appInfo, EnabledExtensionCount = 2, PpEnabledExtensionNames = ext };
            Check(_vk.CreateInstance(&create, null, out _instance), "创建 Vulkan Instance 失败");
        }
        _vk.TryGetInstanceExtension(_instance, out _khrSurface);
        _vk.TryGetInstanceExtension(_instance, out _khrWin32Surface);
    }

    void CreateSurface(nint hwnd)
    {
        if (_khrWin32Surface is null) throw new InvalidOperationException("缺少 VK_KHR_win32_surface");
        var info = new Win32SurfaceCreateInfoKHR { SType = StructureType.Win32SurfaceCreateInfoKhr, Hwnd = hwnd, Hinstance = Win32ViewportHost.ModuleHandle };
        Check(_khrWin32Surface.CreateWin32Surface(_instance, &info, null, out _surface), "创建 Vulkan Surface 失败");
    }

    static void Check(Result result, string message)
    {
        if (result != Result.Success) throw new InvalidOperationException($"{message}: {result}");
    }
}
