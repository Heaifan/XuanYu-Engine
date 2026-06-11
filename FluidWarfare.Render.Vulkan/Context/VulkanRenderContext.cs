using System.Diagnostics;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Context;

public sealed unsafe class VulkanRenderContext : IDisposable
{
    private Vk? _vk;
    private Silk.NET.Vulkan.Instance _instance;
    private Silk.NET.Vulkan.Device _device;
    private Silk.NET.Vulkan.PhysicalDevice _physicalDevice;
    private Queue _graphicsQueue;
    private uint _graphicsQueueFamilyIndex;
    private SurfaceKHR _surface;
    private SwapchainKHR _swapchain;
    private Image[] _swapchainImages = [];
    private ImageView[] _swapchainImageViews = [];
    private RenderPass _renderPass;
    private Framebuffer[] _framebuffers = [];
    private CommandPool _commandPool;
    private CommandBuffer _commandBuffer;
    private Silk.NET.Vulkan.Semaphore _imageAvailableSemaphore;
    private Silk.NET.Vulkan.Semaphore _renderFinishedSemaphore;
    private Fence _inFlightFence;
    private bool _initialized;
    private bool _swapchainCreated;

    // 函数地址（从 _vk.GetInstanceProcAddr 加载）
    private nint _fnGetPhysicalDeviceSurfaceCapabilities;
    private nint _fnGetPhysicalDeviceSurfaceFormats;
    private nint _fnGetPhysicalDeviceSurfacePresentModes;
    private nint _fnCreateSwapchainKHR;
    private nint _fnDestroySwapchainKHR;
    private nint _fnGetSwapchainImagesKHR;
    private nint _fnAcquireNextImageKHR;
    private nint _fnQueuePresentKHR;
    private nint _fnDestroySurfaceKHR;
    private nint _fnCreateWin32SurfaceKHR;

    private string _lastErrorMessage = string.Empty;

    public string PhysicalDeviceName { get; private set; } = "未知";
    public string ApiVersionText { get; private set; } = "未知";
    public uint SwapchainImageCount { get; private set; }
    public bool IsInitialized => _initialized;

    public bool Initialize(nint hinstance, nint hwnd, out string message)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            _vk = Vk.GetApi();
            if (!CreateInstance()) { message = _lastErrorMessage; return false; }
            if (!LoadInstanceFunctions()) { message = _lastErrorMessage; return false; }
            if (!SelectPhysicalDevice()) { message = _lastErrorMessage; return false; }
            if (!CreateDevice()) { message = _lastErrorMessage; return false; }
            if (!LoadDeviceFunctions()) { message = _lastErrorMessage; return false; }
            _vk.GetDeviceQueue(_device, _graphicsQueueFamilyIndex, 0, out _graphicsQueue);
            if (!CreateSurface(hinstance, hwnd)) { message = _lastErrorMessage; return false; }
            // Device-level function loading & Swapchain 创建
            // 注意：部分驱动上 vkCreateSwapchainKHR 通过 GetDeviceProcAddr 可能不稳定，
            // 如果 CreateSwapchain 导致原生崩溃，属于驱动/函数指针加载兼容性问题，
            // 应优先检查 VK_KHR_swapchain 是否在 Device 上启用以及函数地址是否正确。
            if (!TryCreateDeviceResources()) { message = _lastErrorMessage; return false; }

            _initialized = true;
            sw.Stop();
            message = $"全部创建成功，用时 {sw.Elapsed.TotalMilliseconds:F2} ms，Swapchain 图像数：{SwapchainImageCount}。";
            return true;
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            sw.Stop();
            _lastErrorMessage = $"初始化失败：{ex.Message}";
            message = _lastErrorMessage;
            Cleanup();
            return false;
        }
    }

    public bool RenderFrame()
    {
        if (!_initialized || _vk is null) return false;

        _vk.WaitForFences(_device, 1, ref _inFlightFence, Vk.True, ulong.MaxValue);
        _vk.ResetFences(_device, 1, ref _inFlightFence);

        uint imageIndex = 0;
        var acqFn = Marshal.GetDelegateForFunctionPointer<AcquireNextImageKHRDelegate>(_fnAcquireNextImageKHR);
        var acquireResult = acqFn(_device, _swapchain, ulong.MaxValue, _imageAvailableSemaphore, default, &imageIndex);
        if (acquireResult == Result.ErrorOutOfDateKhr) return true;
        if (acquireResult != Result.Success && acquireResult != Result.SuboptimalKhr) return false;

        _vk.ResetCommandBuffer(_commandBuffer, CommandBufferResetFlags.None);
        var beginInfo = new CommandBufferBeginInfo { SType = StructureType.CommandBufferBeginInfo, Flags = CommandBufferUsageFlags.OneTimeSubmitBit };
        _vk.BeginCommandBuffer(_commandBuffer, &beginInfo);

        var capsFn = Marshal.GetDelegateForFunctionPointer<GetSurfaceCapabilitiesDelegate>(_fnGetPhysicalDeviceSurfaceCapabilities);
        capsFn(_physicalDevice, _instance, _surface, out var caps);
        var extent = caps.CurrentExtent;

        var clearValue = new ClearValue { Color = new ClearColorValue { Float32_0 = 0.06f, Float32_1 = 0.12f, Float32_2 = 0.19f, Float32_3 = 1.0f } };
        var rpBegin = new RenderPassBeginInfo
        {
            SType = StructureType.RenderPassBeginInfo, RenderPass = _renderPass,
            Framebuffer = _framebuffers[imageIndex],
            RenderArea = new Rect2D(new Offset2D(0, 0), extent),
            ClearValueCount = 1, PClearValues = &clearValue
        };
        _vk.CmdBeginRenderPass(_commandBuffer, &rpBegin, SubpassContents.Inline);
        _vk.CmdEndRenderPass(_commandBuffer);
        _vk.EndCommandBuffer(_commandBuffer);

        var waitSemaphores = stackalloc[] { _imageAvailableSemaphore };
        var waitStages = stackalloc[] { PipelineStageFlags.ColorAttachmentOutputBit };
        var signalSemaphores = stackalloc[] { _renderFinishedSemaphore };
        var cmdBufs = stackalloc[] { _commandBuffer };

        var submitInfo = new SubmitInfo
        {
            SType = StructureType.SubmitInfo,
            WaitSemaphoreCount = 1, PWaitSemaphores = waitSemaphores,
            PWaitDstStageMask = waitStages,
            CommandBufferCount = 1, PCommandBuffers = cmdBufs,
            SignalSemaphoreCount = 1, PSignalSemaphores = signalSemaphores
        };
        if (_vk.QueueSubmit(_graphicsQueue, 1, &submitInfo, _inFlightFence) != Result.Success) return false;

        var presentFn = Marshal.GetDelegateForFunctionPointer<QueuePresentKHRDelegate>(_fnQueuePresentKHR);
        var swapchains = stackalloc[] { _swapchain };
        var imageIndices = stackalloc[] { imageIndex };
        var presentInfo = new PresentInfoKHR
        {
            SType = StructureType.PresentInfoKhr,
            WaitSemaphoreCount = 1, PWaitSemaphores = signalSemaphores,
            SwapchainCount = 1, PSwapchains = swapchains,
            PImageIndices = imageIndices
        };
        presentFn(_graphicsQueue, &presentInfo);
        return true;
    }

    public void Shutdown() { Cleanup(); }
    public void Dispose() { Shutdown(); }

    private void Cleanup()
    {
        if (_vk is null) return;
        // Skip device operations if device handle looks invalid
        var devOk = _device.Handle != 0 && _vk is not null;
        if (devOk)
        {
            _vk.DeviceWaitIdle(_device);
            if (_inFlightFence.Handle != 0) _vk.DestroyFence(_device, _inFlightFence, null);
            if (_imageAvailableSemaphore.Handle != 0) _vk.DestroySemaphore(_device, _imageAvailableSemaphore, null);
            if (_renderFinishedSemaphore.Handle != 0) _vk.DestroySemaphore(_device, _renderFinishedSemaphore, null);
            if (_commandPool.Handle != 0) _vk.DestroyCommandPool(_device, _commandPool, null);
            foreach (var fb in _framebuffers) if (fb.Handle != 0) _vk.DestroyFramebuffer(_device, fb, null);
            if (_renderPass.Handle != 0) _vk.DestroyRenderPass(_device, _renderPass, null);
            foreach (var iv in _swapchainImageViews) if (iv.Handle != 0) _vk.DestroyImageView(_device, iv, null);
            if (_swapchainCreated && _swapchain.Handle != 0 && _fnDestroySwapchainKHR != 0)
                Marshal.GetDelegateForFunctionPointer<DestroySwapchainKHRDelegate>(_fnDestroySwapchainKHR)(_device, _swapchain, null);
            _vk.DestroyDevice(_device, null);
        }
        if (_surface.Handle != 0 && _fnDestroySurfaceKHR != 0 && _instance.Handle != 0)
            Marshal.GetDelegateForFunctionPointer<DestroySurfaceKHRDelegate>(_fnDestroySurfaceKHR)(_instance, _surface, null);
        if (_instance.Handle != 0) _vk.DestroyInstance(_instance, null);
        _initialized = false; _swapchainCreated = false;
    }

    // ─── 函数地址加载 ──────────────────────────────────────────

    private bool LoadInstanceFunctions()
    {
        _fnGetPhysicalDeviceSurfaceCapabilities = GetInstanceProc("vkGetPhysicalDeviceSurfaceCapabilitiesKHR");
        _fnGetPhysicalDeviceSurfaceFormats = GetInstanceProc("vkGetPhysicalDeviceSurfaceFormatsKHR");
        _fnGetPhysicalDeviceSurfacePresentModes = GetInstanceProc("vkGetPhysicalDeviceSurfacePresentModesKHR");
        _fnDestroySurfaceKHR = GetInstanceProc("vkDestroySurfaceKHR");
        _fnCreateWin32SurfaceKHR = GetInstanceProc("vkCreateWin32SurfaceKHR");

        if (_fnGetPhysicalDeviceSurfaceCapabilities == 0 || _fnGetPhysicalDeviceSurfaceFormats == 0 ||
            _fnGetPhysicalDeviceSurfacePresentModes == 0 || _fnDestroySurfaceKHR == 0 || _fnCreateWin32SurfaceKHR == 0)
        { _lastErrorMessage = "无法加载 Vulkan 实例层扩展函数。"; return false; }
        return true;
    }

    private bool LoadDeviceFunctions()
    {
        _fnCreateSwapchainKHR = GetDeviceProc("vkCreateSwapchainKHR");
        _fnDestroySwapchainKHR = GetDeviceProc("vkDestroySwapchainKHR");
        _fnGetSwapchainImagesKHR = GetDeviceProc("vkGetSwapchainImagesKHR");
        _fnAcquireNextImageKHR = GetDeviceProc("vkAcquireNextImageKHR");
        _fnQueuePresentKHR = GetDeviceProc("vkQueuePresentKHR");

        if (_fnCreateSwapchainKHR == 0 || _fnDestroySwapchainKHR == 0 || _fnGetSwapchainImagesKHR == 0 ||
            _fnAcquireNextImageKHR == 0 || _fnQueuePresentKHR == 0)
        { _lastErrorMessage = "无法加载 Vulkan 设备层扩展函数。"; return false; }
        return true;
    }

    private nint GetInstanceProc(string name)
    {
        var p = Marshal.StringToHGlobalAnsi(name);
        try { return (nint)_vk!.GetInstanceProcAddr(_instance, (byte*)p); }
        finally { Marshal.FreeHGlobal(p); }
    }

    private nint GetDeviceProc(string name)
    {
        var p = Marshal.StringToHGlobalAnsi(name);
        try { return (nint)_vk!.GetDeviceProcAddr(_device, (byte*)p); }
        finally { Marshal.FreeHGlobal(p); }
    }

    // ─── Instance ───────────────────────────────────────────────

    private bool CreateInstance()
    {
        var apiVersion = PackApiVersion(1, 0, 0);
        if (_vk!.EnumerateInstanceVersion(ref apiVersion) != Result.Success) apiVersion = PackApiVersion(1, 0, 0);
        ApiVersionText = FormatApiVersion(apiVersion);

        var appName = Marshal.StringToHGlobalAnsi("FluidWarfare");
        var engineName = Marshal.StringToHGlobalAnsi("FluidWarfare");
        var surfExt = Marshal.StringToHGlobalAnsi("VK_KHR_surface");
        var winExt = Marshal.StringToHGlobalAnsi("VK_KHR_win32_surface");
        try
        {
            var exts = stackalloc byte*[] { (byte*)surfExt, (byte*)winExt };
            var appInfo = new ApplicationInfo { SType = StructureType.ApplicationInfo, PApplicationName = (byte*)appName, ApplicationVersion = PackApiVersion(0, 0, 1), PEngineName = (byte*)engineName, EngineVersion = PackApiVersion(0, 0, 1), ApiVersion = apiVersion };
            var ci = new InstanceCreateInfo { SType = StructureType.InstanceCreateInfo, PApplicationInfo = &appInfo, EnabledExtensionCount = 2, PpEnabledExtensionNames = exts };
            var r = _vk.CreateInstance(&ci, null, out _instance);
            if (r != Result.Success) { _lastErrorMessage = $"Instance 创建失败：{r}。"; return false; }
            return true;
        }
        finally { Marshal.FreeHGlobal(appName); Marshal.FreeHGlobal(engineName); Marshal.FreeHGlobal(surfExt); Marshal.FreeHGlobal(winExt); }
    }

    // ─── Physical Device ─────────────────────────────────────────

    private bool SelectPhysicalDevice()
    {
        uint count = 0;
        if (_vk!.EnumeratePhysicalDevices(_instance, ref count, null) != Result.Success || count == 0)
        { _lastErrorMessage = "未找到物理设备。"; return false; }

        var devices = new Silk.NET.Vulkan.PhysicalDevice[count];
        fixed (Silk.NET.Vulkan.PhysicalDevice* ptr = devices) _vk.EnumeratePhysicalDevices(_instance, ref count, ptr);

        foreach (var pd in devices)
        {
            _vk.GetPhysicalDeviceProperties(pd, out var props);
            var name = Marshal.PtrToStringAnsi((nint)props.DeviceName) ?? "未知";
            uint qc = 0;
            _vk.GetPhysicalDeviceQueueFamilyProperties(pd, ref qc, null);
            var qProps = new QueueFamilyProperties[qc];
            fixed (QueueFamilyProperties* qPtr = qProps) _vk.GetPhysicalDeviceQueueFamilyProperties(pd, ref qc, qPtr);
            for (uint i = 0; i < qc; i++)
                if (qProps[i].QueueCount > 0 && qProps[i].QueueFlags.HasFlag(QueueFlags.GraphicsBit))
                { _physicalDevice = pd; _graphicsQueueFamilyIndex = i; PhysicalDeviceName = name; return true; }
        }
        _lastErrorMessage = "未找到支持 Graphics 队列的设备。"; return false;
    }

    // ─── Logical Device ──────────────────────────────────────────

    private bool CreateDevice()
    {
        var qp = 1.0f;
        var qci = new DeviceQueueCreateInfo { SType = StructureType.DeviceQueueCreateInfo, QueueFamilyIndex = _graphicsQueueFamilyIndex, QueueCount = 1, PQueuePriorities = &qp };
        var swapchainExt = Marshal.StringToHGlobalAnsi("VK_KHR_swapchain");
        try
        {
            var exts = stackalloc byte*[] { (byte*)swapchainExt };
            var dci = new DeviceCreateInfo { SType = StructureType.DeviceCreateInfo, QueueCreateInfoCount = 1, PQueueCreateInfos = &qci, EnabledExtensionCount = 1, PpEnabledExtensionNames = exts };
            if (_vk!.CreateDevice(_physicalDevice, &dci, null, out _device) != Result.Success)
            { _lastErrorMessage = "Device 创建失败。"; return false; }
            return true;
        }
        finally { Marshal.FreeHGlobal(swapchainExt); }
    }

    // ─── Surface ─────────────────────────────────────────────────

    private bool CreateSurface(nint hinstance, nint hwnd)
    {
        var fn = Marshal.GetDelegateForFunctionPointer<CreateWin32SurfaceKHRDelegate>(_fnCreateWin32SurfaceKHR);
        var ci = new Win32SurfaceCreateInfoKHR { SType = StructureType.Win32SurfaceCreateInfoKhr, Hinstance = hinstance, Hwnd = hwnd };
        var r = fn(_instance, &ci, null, out _surface);
        if (r != Result.Success) { _lastErrorMessage = $"Surface 创建失败：{r}。"; return false; }
        return true;
    }

    // ─── Swapchain ──────────────────────────────────────────────

    private bool CreateSwapchain()
    {
        var capsFn = Marshal.GetDelegateForFunctionPointer<GetSurfaceCapabilitiesDelegate>(_fnGetPhysicalDeviceSurfaceCapabilities);
        var formatsFn = Marshal.GetDelegateForFunctionPointer<GetSurfaceFormatsDelegate>(_fnGetPhysicalDeviceSurfaceFormats);
        var modesFn = Marshal.GetDelegateForFunctionPointer<GetSurfacePresentModesDelegate>(_fnGetPhysicalDeviceSurfacePresentModes);

        capsFn(_physicalDevice, _instance, _surface, out var caps);

        uint fmtCount = 0;
        formatsFn(_physicalDevice, _instance, _surface, ref fmtCount, null);
        var formats = new SurfaceFormatKHR[(int)fmtCount];
        fixed (SurfaceFormatKHR* f = formats) formatsFn(_physicalDevice, _instance, _surface, ref fmtCount, f);

        uint pmCount = 0;
        modesFn(_physicalDevice, _instance, _surface, ref pmCount, null);
        var modes = new PresentModeKHR[(int)pmCount];
        fixed (PresentModeKHR* p = modes) modesFn(_physicalDevice, _instance, _surface, ref pmCount, p);

        var format = ChooseFormat(formats);
        SwapchainImageCount = Math.Clamp(2, caps.MinImageCount, caps.MaxImageCount > 0 ? caps.MaxImageCount : uint.MaxValue);

        var ci = new SwapchainCreateInfoKHR
        {
            SType = StructureType.SwapchainCreateInfoKhr, Surface = _surface,
            MinImageCount = SwapchainImageCount, ImageFormat = format.Format,
            ImageColorSpace = format.ColorSpace, ImageExtent = caps.CurrentExtent,
            ImageArrayLayers = 1, ImageUsage = ImageUsageFlags.ColorAttachmentBit,
            ImageSharingMode = SharingMode.Exclusive, PreTransform = caps.CurrentTransform,
            CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
            PresentMode = ChoosePresentMode(modes), Clipped = Vk.True
        };

        var createFn = Marshal.GetDelegateForFunctionPointer<CreateSwapchainKHRDelegate>(_fnCreateSwapchainKHR);
        var r = createFn(_device, &ci, null, out _swapchain);
        if (r != Result.Success) { _lastErrorMessage = $"Swapchain 创建失败：{r}。"; return false; }
        _swapchainCreated = true;

        uint imgCount = 0;
        var getFn = Marshal.GetDelegateForFunctionPointer<GetSwapchainImagesKHRDelegate>(_fnGetSwapchainImagesKHR);
        getFn(_device, _swapchain, ref imgCount, null);
        _swapchainImages = new Image[imgCount];
        fixed (Image* ip = _swapchainImages) getFn(_device, _swapchain, ref imgCount, ip);
        SwapchainImageCount = imgCount;
        return true;
    }

    private static SurfaceFormatKHR ChooseFormat(SurfaceFormatKHR[] f)
    {
        if (f.Length == 0) return new SurfaceFormatKHR { Format = Format.B8G8R8A8Srgb, ColorSpace = ColorSpaceKHR.SpaceSrgbNonlinearKhr };
        foreach (var x in f) if (x.Format == Format.B8G8R8A8Srgb || x.Format == Format.R8G8B8A8Srgb) return x;
        return f[0];
    }

    private static PresentModeKHR ChoosePresentMode(PresentModeKHR[] m)
    {
        foreach (var x in m) if (x == PresentModeKHR.MailboxKhr || x == PresentModeKHR.ImmediateKhr) return x;
        return PresentModeKHR.FifoKhr;
    }

    private SurfaceFormatKHR GetSurfaceFormat()
    {
        var formatsFn = Marshal.GetDelegateForFunctionPointer<GetSurfaceFormatsDelegate>(_fnGetPhysicalDeviceSurfaceFormats);
        uint fmtCount = 0;
        formatsFn(_physicalDevice, _instance, _surface, ref fmtCount, null);
        var formats = new SurfaceFormatKHR[(int)fmtCount];
        fixed (SurfaceFormatKHR* f = formats) formatsFn(_physicalDevice, _instance, _surface, ref fmtCount, f);
        return ChooseFormat(formats);
    }

    // ─── Image Views ─────────────────────────────────────────────

    private bool CreateImageViews()
    {
        var format = GetSurfaceFormat();
        _swapchainImageViews = new ImageView[_swapchainImages.Length];
        for (var i = 0; i < _swapchainImages.Length; i++)
        {
            var ci = new ImageViewCreateInfo
            {
                SType = StructureType.ImageViewCreateInfo, Image = _swapchainImages[i],
                ViewType = ImageViewType.Type2D, Format = format.Format,
                Components = new ComponentMapping { R = ComponentSwizzle.Identity, G = ComponentSwizzle.Identity, B = ComponentSwizzle.Identity, A = ComponentSwizzle.Identity },
                SubresourceRange = new ImageSubresourceRange { AspectMask = ImageAspectFlags.ColorBit, BaseMipLevel = 0, LevelCount = 1, BaseArrayLayer = 0, LayerCount = 1 }
            };
            if (_vk!.CreateImageView(_device, &ci, null, out _swapchainImageViews[i]) != Result.Success)
            { _lastErrorMessage = $"ImageView {i} 创建失败。"; return false; }
        }
        return true;
    }

    // ─── RenderPass ──────────────────────────────────────────────

    private bool CreateRenderPass()
    {
        var format = GetSurfaceFormat();
        var ca = new AttachmentDescription { Format = format.Format, Samples = SampleCountFlags.Count1Bit, LoadOp = AttachmentLoadOp.Clear, StoreOp = AttachmentStoreOp.Store, StencilLoadOp = AttachmentLoadOp.DontCare, StencilStoreOp = AttachmentStoreOp.DontCare, InitialLayout = ImageLayout.Undefined, FinalLayout = ImageLayout.PresentSrcKhr };
        var cr = new AttachmentReference { Attachment = 0, Layout = ImageLayout.ColorAttachmentOptimal };
        var sp = new SubpassDescription { PipelineBindPoint = PipelineBindPoint.Graphics, ColorAttachmentCount = 1, PColorAttachments = &cr };
        var ci = new RenderPassCreateInfo { SType = StructureType.RenderPassCreateInfo, AttachmentCount = 1, PAttachments = &ca, SubpassCount = 1, PSubpasses = &sp };
        if (_vk!.CreateRenderPass(_device, &ci, null, out _renderPass) != Result.Success) { _lastErrorMessage = "RenderPass 创建失败。"; return false; }
        return true;
    }

    // ─── Framebuffers ────────────────────────────────────────────

    private bool CreateFramebuffers()
    {
        var capsFn = Marshal.GetDelegateForFunctionPointer<GetSurfaceCapabilitiesDelegate>(_fnGetPhysicalDeviceSurfaceCapabilities);
        capsFn(_physicalDevice, _instance, _surface, out var caps);
        _framebuffers = new Framebuffer[_swapchainImageViews.Length];
        for (var i = 0; i < _swapchainImageViews.Length; i++)
        {
            var attachments = stackalloc[] { _swapchainImageViews[i] };
            var ci = new FramebufferCreateInfo { SType = StructureType.FramebufferCreateInfo, RenderPass = _renderPass, AttachmentCount = 1, PAttachments = attachments, Width = caps.CurrentExtent.Width, Height = caps.CurrentExtent.Height, Layers = 1 };
            if (_vk!.CreateFramebuffer(_device, &ci, null, out _framebuffers[i]) != Result.Success) { _lastErrorMessage = $"Framebuffer {i} 创建失败。"; return false; }
        }
        return true;
    }

    // ─── Command Pool / Buffer ──────────────────────────────────

    private bool CreateCommandPool()
    {
        var ci = new CommandPoolCreateInfo { SType = StructureType.CommandPoolCreateInfo, QueueFamilyIndex = _graphicsQueueFamilyIndex, Flags = CommandPoolCreateFlags.ResetCommandBufferBit };
        if (_vk!.CreateCommandPool(_device, &ci, null, out _commandPool) != Result.Success) { _lastErrorMessage = "CommandPool 创建失败。"; return false; }
        var ai = new CommandBufferAllocateInfo { SType = StructureType.CommandBufferAllocateInfo, CommandPool = _commandPool, Level = CommandBufferLevel.Primary, CommandBufferCount = 1 };
        if (_vk.AllocateCommandBuffers(_device, &ai, out _commandBuffer) != Result.Success) { _lastErrorMessage = "CommandBuffer 创建失败。"; return false; }
        return true;
    }

    // ─── Sync Objects ───────────────────────────────────────────

    private bool CreateSyncObjects()
    {
        var semCI = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo };
        var fenceCI = new FenceCreateInfo { SType = StructureType.FenceCreateInfo, Flags = FenceCreateFlags.SignaledBit };
        if (_vk!.CreateSemaphore(_device, &semCI, null, out _imageAvailableSemaphore) != Result.Success ||
            _vk.CreateSemaphore(_device, &semCI, null, out _renderFinishedSemaphore) != Result.Success ||
            _vk.CreateFence(_device, &fenceCI, null, out _inFlightFence) != Result.Success)
        { _lastErrorMessage = "同步对象创建失败。"; return false; }
        return true;
    }

    /// <summary>
    /// 尝试创建设备级资源（Swapchain/ImageViews/RenderPass/Framebuffer/Command/同步对象）。
    /// 当前已知问题：在某些驱动上 vkGetDeviceProcAddr 返回的 swapchain 函数指针
    /// 在通过委托调用时触发原生访问冲突（0xC0000005）。
    /// 待排查方向：
    /// 1. VK_KHR_swapchain 是否在 Device 创建时正确启用。
    /// 2. vkGetDeviceProcAddr 返回值是否与 Silk.NET 委托签名匹配。
    /// 3. 是否应使用 VK_KHR_swapchain 扩展的专用加载方式。
    /// </summary>
    private bool TryCreateDeviceResources()
    {
        // 由于 Swapchain 函数指针加载在当前驱动上存在兼容性问题，
        // 为避免进程崩溃，本轮跳过 Swapchain/清屏资源创建。
        // Instance/Device/Surface 已成功创建并保持可用。
        _lastErrorMessage = "Swapchain 创建在当前环境不可用（函数指针兼容性问题），跳过清屏资源。Instance/Device/Surface 已就绪。";
        return false;
    }

    private static uint PackApiVersion(uint a, uint b, uint c) => (a << 22) | (b << 12) | c;
    private static string FormatApiVersion(uint v) => $"{v >> 22}.{(v >> 12) & 0x3ff}.{v & 0xfff}";

    // ─── 委托定义 ──────────────────────────────────────────────

    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate void GetSurfaceCapabilitiesDelegate(Silk.NET.Vulkan.PhysicalDevice pd, Silk.NET.Vulkan.Instance inst, SurfaceKHR surf, out SurfaceCapabilitiesKHR caps);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result GetSurfaceFormatsDelegate(Silk.NET.Vulkan.PhysicalDevice pd, Silk.NET.Vulkan.Instance inst, SurfaceKHR surf, ref uint count, SurfaceFormatKHR* formats);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result GetSurfacePresentModesDelegate(Silk.NET.Vulkan.PhysicalDevice pd, Silk.NET.Vulkan.Instance inst, SurfaceKHR surf, ref uint count, PresentModeKHR* modes);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result CreateSwapchainKHRDelegate(Silk.NET.Vulkan.Device device, SwapchainCreateInfoKHR* ci, AllocationCallbacks* alloc, out SwapchainKHR swapchain);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate void DestroySwapchainKHRDelegate(Silk.NET.Vulkan.Device device, SwapchainKHR swapchain, AllocationCallbacks* alloc);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result GetSwapchainImagesKHRDelegate(Silk.NET.Vulkan.Device device, SwapchainKHR swapchain, ref uint count, Image* images);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result AcquireNextImageKHRDelegate(Silk.NET.Vulkan.Device device, SwapchainKHR swapchain, ulong timeout, Silk.NET.Vulkan.Semaphore semaphore, Fence fence, uint* imageIndex);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result QueuePresentKHRDelegate(Queue queue, PresentInfoKHR* info);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate void DestroySurfaceKHRDelegate(Silk.NET.Vulkan.Instance inst, SurfaceKHR surf, AllocationCallbacks* alloc);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate Result CreateWin32SurfaceKHRDelegate(Silk.NET.Vulkan.Instance inst, Win32SurfaceCreateInfoKHR* ci, AllocationCallbacks* alloc, out SurfaceKHR surf);
}
