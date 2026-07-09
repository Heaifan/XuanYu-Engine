using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Swapchain;

namespace XuanYu.Render.Vulkan.Render;

// VK4-D（D1+D2）：单色清屏资源持有者。RenderPass + CommandPool + CommandBuffer[]（每 Swapchain 图像一张）+ Framebuffer[]。Resize 只重建 Framebuffer + 重录 CommandBuffer。不碰 Surface/Instance/Swapchain 重建/Present 泵。
public sealed unsafe class VulkanClearFrameOwner : IDisposable
{
    readonly Vk _vk; readonly VulkanDeviceOwner _deviceOwner; readonly VulkanSwapchainOwner _swapchainOwner; readonly Action<string>? _log;
    RenderPass _renderPass; CommandPool _commandPool; CommandBuffer[] _commandBuffers = []; Framebuffer[] _framebuffers = []; Extent2D _extent; bool _disposed;

    public VulkanClearFrameOwner(Vk vk, VulkanDeviceOwner deviceOwner, VulkanSwapchainOwner swapchainOwner, int graphicsFamily, Action<string>? log)
    {
        _vk = vk; _deviceOwner = deviceOwner; _swapchainOwner = swapchainOwner; _log = log;
        BuildRenderPass();
        var poolInfo = new CommandPoolCreateInfo { SType = StructureType.CommandPoolCreateInfo, Flags = 0, QueueFamilyIndex = (uint)graphicsFamily };
        _vk.CreateCommandPool(_deviceOwner.LogicalDevice, &poolInfo, null, out _commandPool);
        RebuildFramebuffers();
        Log(_log, VulkanClearFrameLogFormatter.Created());
    }

    public CommandBuffer[] CommandBuffers => _commandBuffers;
    public Extent2D Extent => _extent;
    public RenderPass RenderPass => _renderPass;

    void BuildRenderPass()
    {
        var attachment = new AttachmentDescription { Format = _swapchainOwner.Format, Samples = SampleCountFlags.Count1Bit, LoadOp = AttachmentLoadOp.Clear, StoreOp = AttachmentStoreOp.Store, StencilLoadOp = AttachmentLoadOp.DontCare, StencilStoreOp = AttachmentStoreOp.DontCare, InitialLayout = ImageLayout.Undefined, FinalLayout = ImageLayout.PresentSrcKhr };
        var colorRef = new AttachmentReference { Attachment = 0, Layout = ImageLayout.ColorAttachmentOptimal };
        var subpass = new SubpassDescription { PipelineBindPoint = PipelineBindPoint.Graphics, ColorAttachmentCount = 1, PColorAttachments = &colorRef };
        var info = new RenderPassCreateInfo { SType = StructureType.RenderPassCreateInfo, AttachmentCount = 1, PAttachments = &attachment, SubpassCount = 1, PSubpasses = &subpass };
        _vk.CreateRenderPass(_deviceOwner.LogicalDevice, &info, null, out _renderPass);
    }

    public void RebuildFramebuffers()
    {
        DestroyFramebuffers();
        _extent = _swapchainOwner.Extent;
        var views = _swapchainOwner.ImageViews.ToArray();
        _framebuffers = new Framebuffer[views.Length];
        for (var i = 0; i < views.Length; i++)
        {
            fixed (ImageView* pView = &views[i])
            {
                var fbInfo = new FramebufferCreateInfo { SType = StructureType.FramebufferCreateInfo, RenderPass = _renderPass, AttachmentCount = 1, PAttachments = pView, Width = _extent.Width, Height = _extent.Height, Layers = 1 };
                _vk.CreateFramebuffer(_deviceOwner.LogicalDevice, &fbInfo, null, out _framebuffers[i]);
            }
        }
        RecordCommandBuffers(views);
    }

    void RecordCommandBuffers(ImageView[] views)
    {
        var old = _commandBuffers;
        _commandBuffers = new CommandBuffer[views.Length];
        var alloc = new CommandBufferAllocateInfo { SType = StructureType.CommandBufferAllocateInfo, CommandPool = _commandPool, Level = CommandBufferLevel.Primary, CommandBufferCount = (uint)views.Length };
        fixed (CommandBuffer* p = _commandBuffers)
        {
            _vk.AllocateCommandBuffers(_deviceOwner.LogicalDevice, &alloc, p);
            for (var i = 0; i < _commandBuffers.Length; i++) RecordOne(_commandBuffers[i], _framebuffers[i]);
        }
        if (old.Length > 0) _vk.FreeCommandBuffers(_deviceOwner.LogicalDevice, _commandPool, (uint)old.Length, old);
    }

    void RecordOne(CommandBuffer cb, Framebuffer fb)
    {
        var begin = new CommandBufferBeginInfo { SType = StructureType.CommandBufferBeginInfo, Flags = 0 };
        _vk.BeginCommandBuffer(cb, &begin);
        var clear = new ClearValue { Color = new ClearColorValue { Float32_0 = 0.25f, Float32_1 = 0.45f, Float32_2 = 0.70f, Float32_3 = 1.0f } };
        var rp = new RenderPassBeginInfo { SType = StructureType.RenderPassBeginInfo, RenderPass = _renderPass, Framebuffer = fb, RenderArea = new Rect2D { Offset = new Offset2D { X = 0, Y = 0 }, Extent = _extent }, ClearValueCount = 1, PClearValues = &clear };
        _vk.CmdBeginRenderPass(cb, &rp, SubpassContents.Inline);
        _vk.CmdEndRenderPass(cb);
        _vk.EndCommandBuffer(cb);
    }

    void DestroyFramebuffers()
    {
        foreach (var f in _framebuffers) if (f.Handle != 0) _vk.DestroyFramebuffer(_deviceOwner.LogicalDevice, f, null);
        _framebuffers = [];
    }

    public void Dispose()
    {
        if (_disposed) return; _disposed = true;
        DestroyFramebuffers();
        if (_commandBuffers.Length > 0) _vk.FreeCommandBuffers(_deviceOwner.LogicalDevice, _commandPool, (uint)_commandBuffers.Length, _commandBuffers);
        if (_commandPool.Handle != 0) _vk.DestroyCommandPool(_deviceOwner.LogicalDevice, _commandPool, null);
        if (_renderPass.Handle != 0) _vk.DestroyRenderPass(_deviceOwner.LogicalDevice, _renderPass, null);
        Log(_log, VulkanClearFrameLogFormatter.Disposed());
    }
    static void Log(Action<string>? log, string m) { log?.Invoke(m); }
}
