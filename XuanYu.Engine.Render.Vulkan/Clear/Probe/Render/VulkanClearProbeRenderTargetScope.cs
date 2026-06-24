using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Clear;

sealed unsafe class VulkanClearProbeRenderTargetScope : IDisposable
{
    readonly Vk _vk;
    readonly Silk.NET.Vulkan.Device _device;
    SwapchainKHR _swapchain;
    ImageView[] _imageViews = [];
    RenderPass _renderPass;
    Framebuffer[] _framebuffers = [];
    nint _fnDestroySwapchain;
    bool _hasSwapchain, _hasRenderPass;
    Format _format;
    Extent2D _extent;
    public VulkanClearProbeRenderTargetScope(Vk vk, Silk.NET.Vulkan.Device dev) { _vk = vk; _device = dev; }
    public SwapchainKHR Swapchain => _swapchain;
    public RenderPass RenderPass => _renderPass;
    public Framebuffer[] Framebuffers => _framebuffers;
    public bool CreateSwapchain(SurfaceKHR surf, Format fmt, ColorSpaceKHR cs, SurfaceCapabilitiesKHR caps,
        Extent2D extent, uint imageCount, PresentModeKHR mode, nint fnCreate, nint fnDestroy, nint fnGetImages)
    {
        _format = fmt; _extent = extent; _fnDestroySwapchain = fnDestroy;
        var ci = new SwapchainCreateInfoKHR
        {
            SType = StructureType.SwapchainCreateInfoKhr, Surface = surf,
            MinImageCount = imageCount, ImageFormat = fmt, ImageColorSpace = cs,
            ImageExtent = extent, ImageArrayLayers = 1,
            ImageUsage = ImageUsageFlags.ColorAttachmentBit,
            ImageSharingMode = SharingMode.Exclusive,
            PreTransform = caps.CurrentTransform,
            CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
            PresentMode = mode, Clipped = Vk.True
        };
        var fn = Marshal.GetDelegateForFunctionPointer<CreateSwapchainPtr>(fnCreate);
        SwapchainKHR sc;
        if (fn(_device, &ci, null, &sc) != Result.Success) return false;
        _swapchain = sc;
        _hasSwapchain = true;

        var fnImg = Marshal.GetDelegateForFunctionPointer<GetSwapchainImagesPtr>(fnGetImages);
        uint count = 0;
        fnImg(_device, _swapchain, &count, null);
        if (count == 0) return false;
        var images = new Image[count];
        fixed (Image* ip = images) fnImg(_device, _swapchain, &count, ip);

        _imageViews = new ImageView[count];
        for (var i = 0; i < count; i++)
        {
            var ivCI = new ImageViewCreateInfo
            {
                SType = StructureType.ImageViewCreateInfo, Image = images[i],
                ViewType = ImageViewType.Type2D, Format = fmt,
                Components = new ComponentMapping { R = ComponentSwizzle.Identity, G = ComponentSwizzle.Identity, B = ComponentSwizzle.Identity, A = ComponentSwizzle.Identity },
                SubresourceRange = new ImageSubresourceRange { AspectMask = ImageAspectFlags.ColorBit, BaseMipLevel = 0, LevelCount = 1, BaseArrayLayer = 0, LayerCount = 1 }
            };
            if (_vk.CreateImageView(_device, &ivCI, null, out _imageViews[i]) != Result.Success) return false;
        }
        return true;
    }
    public bool CreateRenderPass()
    {
        var att = new AttachmentDescription
        {
            Format = _format, Samples = SampleCountFlags.Count1Bit,
            LoadOp = AttachmentLoadOp.Clear, StoreOp = AttachmentStoreOp.Store,
            StencilLoadOp = AttachmentLoadOp.DontCare, StencilStoreOp = AttachmentStoreOp.DontCare,
            InitialLayout = ImageLayout.Undefined, FinalLayout = ImageLayout.PresentSrcKhr
        };
        var ref_ = new AttachmentReference { Attachment = 0, Layout = ImageLayout.ColorAttachmentOptimal };
        var sp = new SubpassDescription { PipelineBindPoint = PipelineBindPoint.Graphics, ColorAttachmentCount = 1, PColorAttachments = &ref_ };
        var ci = new RenderPassCreateInfo { SType = StructureType.RenderPassCreateInfo, AttachmentCount = 1, PAttachments = &att, SubpassCount = 1, PSubpasses = &sp };
        if (_vk.CreateRenderPass(_device, &ci, null, out _renderPass) != Result.Success) return false;
        _hasRenderPass = true;

        _framebuffers = new Framebuffer[_imageViews.Length];
        for (var i = 0; i < _imageViews.Length; i++)
        {
            var a = stackalloc[] { _imageViews[i] };
            var fbCI = new FramebufferCreateInfo { SType = StructureType.FramebufferCreateInfo, RenderPass = _renderPass, AttachmentCount = 1, PAttachments = a, Width = _extent.Width, Height = _extent.Height, Layers = 1 };
            if (_vk.CreateFramebuffer(_device, &fbCI, null, out _framebuffers[i]) != Result.Success) return false;
        }
        return true;
    }
    public void Dispose()
    {
        if (_device.Handle == 0) return;
        foreach (var fb in _framebuffers) if (fb.Handle != 0) _vk.DestroyFramebuffer(_device, fb, null);
        if (_hasRenderPass && _renderPass.Handle != 0) _vk.DestroyRenderPass(_device, _renderPass, null);
        foreach (var iv in _imageViews) if (iv.Handle != 0) _vk.DestroyImageView(_device, iv, null);
        if (_hasSwapchain && _fnDestroySwapchain != 0)
            Marshal.GetDelegateForFunctionPointer<DestroySwapchainPtr>(_fnDestroySwapchain)(_device, _swapchain, null);
    }
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] delegate Result CreateSwapchainPtr(Silk.NET.Vulkan.Device d, SwapchainCreateInfoKHR* ci, AllocationCallbacks* a, SwapchainKHR* sc);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] delegate void DestroySwapchainPtr(Silk.NET.Vulkan.Device d, SwapchainKHR sc, AllocationCallbacks* a);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] delegate Result GetSwapchainImagesPtr(Silk.NET.Vulkan.Device d, SwapchainKHR sc, uint* c, Image* imgs);
}
