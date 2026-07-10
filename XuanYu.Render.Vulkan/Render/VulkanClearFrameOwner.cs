using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Swapchain;
using XuanYu.Render.Vulkan.Diagnostic;

namespace XuanYu.Render.Vulkan.Render;

// VK4-D 清屏 + VK5-B 三角形。VK5-D 职责边界：帧缓冲管理｜命令录制｜绘制｜管线注入。RZ-VK5-D-R1：RebuildFramebuffers 加 T+ 追踪。
public sealed unsafe class VulkanClearFrameOwner : IDisposable
{
    readonly Vk _vk; readonly VulkanDeviceOwner _deviceOwner; readonly VulkanSwapchainOwner _swapchainOwner; readonly Action<string>? _log;
    RenderPass _renderPass; CommandPool _commandPool; CommandBuffer[] _commandBuffers = []; Framebuffer[] _framebuffers = [];
    ImageView[] _views = []; Silk.NET.Vulkan.Pipeline _pipeline = default; Extent2D _extent; bool _disposed;

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
    public void SetPipeline(Silk.NET.Vulkan.Pipeline pipeline) { _pipeline = pipeline; if (_views.Length > 0) RecordCommandBuffers(_views); }
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
        _log?.Invoke(VulkanResizeTracer.Stage(0, "Framebuffer.Rebuild", "开始..."));
        DestroyFramebuffers();
        _extent = _swapchainOwner.Extent;
        _views = _swapchainOwner.ImageViews.ToArray();
        _framebuffers = new Framebuffer[_views.Length];
        for (var i = 0; i < _views.Length; i++) { fixed (ImageView* pView = &_views[i]) { var fbInfo = new FramebufferCreateInfo { SType = StructureType.FramebufferCreateInfo, RenderPass = _renderPass, AttachmentCount = 1, PAttachments = pView, Width = _extent.Width, Height = _extent.Height, Layers = 1 }; _vk.CreateFramebuffer(_deviceOwner.LogicalDevice, &fbInfo, null, out _framebuffers[i]); } }
        _log?.Invoke(VulkanResizeTracer.Stage(0, "Framebuffer.Rebuild完成", $"{_extent.Width}x{_extent.Height}；{_framebuffers.Length}张FB+重录CB"));
        RecordCommandBuffers(_views);
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
        var begin = new CommandBufferBeginInfo { SType = StructureType.CommandBufferBeginInfo };
        _vk.BeginCommandBuffer(cb, &begin);
        var clear = new ClearValue { Color = new ClearColorValue { Float32_0 = 0.25f, Float32_1 = 0.45f, Float32_2 = 0.70f, Float32_3 = 1.0f } };
        var rp = new RenderPassBeginInfo { SType = StructureType.RenderPassBeginInfo, RenderPass = _renderPass, Framebuffer = fb, RenderArea = new Rect2D { Offset = new Offset2D { X = 0, Y = 0 }, Extent = _extent }, ClearValueCount = 1, PClearValues = &clear };
        _vk.CmdBeginRenderPass(cb, &rp, SubpassContents.Inline);
        RecordDraw(cb);
        _vk.CmdEndRenderPass(cb);
        _vk.EndCommandBuffer(cb);
    }
    // 绘制（管线已注入时绑定并画固定三角形）。不含 VertexBuffer / Mesh。
    void RecordDraw(CommandBuffer cb)
    {
        if (_pipeline.Handle == 0) return;
        Viewport* pVp = stackalloc Viewport[1];
        pVp[0] = new Viewport { X = 0, Y = 0, Width = _extent.Width, Height = _extent.Height, MinDepth = 0, MaxDepth = 1 };
        Rect2D* pSc = stackalloc Rect2D[1];
        pSc[0] = new Rect2D { Offset = new Offset2D { X = 0, Y = 0 }, Extent = _extent };
        _vk.CmdBindPipeline(cb, PipelineBindPoint.Graphics, _pipeline);
        _vk.CmdSetViewport(cb, 0, 1, pVp);
        _vk.CmdSetScissor(cb, 0, 1, pSc);
        _vk.CmdDraw(cb, 3, 1, 0, 0);
    }
    void DestroyFramebuffers() { foreach (var f in _framebuffers) if (f.Handle != 0) _vk.DestroyFramebuffer(_deviceOwner.LogicalDevice, f, null); _framebuffers = []; }
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
