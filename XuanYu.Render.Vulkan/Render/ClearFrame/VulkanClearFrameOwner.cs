using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Diagnostic;
using XuanYu.Render.Vulkan.Render.StaticModels;
using XuanYu.Render.Vulkan.Render.VectorOverlay;
using XuanYu.Render.Vulkan.Benchmark;
using XuanYu.Render.Vulkan.Render.Readback;
using XuanYu.Render.Vulkan.Swapchain;
namespace XuanYu.Render.Vulkan.Render;
public sealed unsafe partial class VulkanClearFrameOwner : IDisposable
{
    readonly Vk _vk;
    readonly VulkanDeviceOwner _deviceOwner;
    readonly VulkanSwapchainOwner? _swapchainOwner;
    readonly VulkanOffscreenColorTarget? _offscreenTarget;
    readonly Action<string>? _log;
    RenderPass _renderPass;
    CommandPool _commandPool;
    CommandBuffer[] _commandBuffers = [];
    Framebuffer[] _framebuffers = [];
    VulkanReadbackBuffer[] _readbackBuffers = [];
    VulkanGpuTimestampQuery? _timestampQuery;
    ImageView[] _views = []; VulkanDepthAttachment? _depthAttachment;
    Silk.NET.Vulkan.Pipeline _pipeline = default;
    PipelineLayout _pipelineLayout = default;
    readonly VulkanFrameState _frameState = new();
    readonly VulkanGpuResourceState _gpuResourceState = new();
    readonly VulkanStaticModelCache _staticModels;
    readonly VulkanVectorOverlayCache _vectorOverlays;
    VulkanStaticModelBuffer? _proceduralVertexBuffer;
    Extent2D _extent;
    int _recordCommandDepth;
    int _recordCommandTraceCount;
    int _lastLoggedCommandEntityCount = -1;
    int _lastLoggedCommandViewCount = -1;
    bool _disposed;

    RenderProjection _renderProjection => _frameState.Projection;
    bool _hasRenderProjection => _frameState.HasProjection;

    public VulkanClearFrameOwner(Vk vk, VulkanDeviceOwner deviceOwner, VulkanSwapchainOwner swapchainOwner, int graphicsFamily, Action<string>? log)
    {
        _vk = vk; _deviceOwner = deviceOwner; _swapchainOwner = swapchainOwner; _log = log;
        _staticModels = new VulkanStaticModelCache(vk, deviceOwner, log);
        _vectorOverlays = new VulkanVectorOverlayCache(vk, deviceOwner, log);
        _proceduralVertexBuffer = VulkanStaticModelBuffer.Create(vk, deviceOwner,
            new VulkanStaticModelVertex[RenderDrawPlan.RotateGizmoVertexCount],
            BufferUsageFlags.VertexBufferBit, out _);
        Initialize(graphicsFamily);
    }

    void Initialize(int graphicsFamily)
    {
        try
        {
            BuildRenderPass();
            CreateCommandPool(graphicsFamily);
            InitializeMapLabelTextures();
            if (!RebuildFramebuffers()) throw new InvalidOperationException("Framebuffer 创建失败");
            Log(VulkanClearFrameLogFormatter.Created());
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    public bool RebuildFramebuffers(uint generation = 0, bool force = false)
    {
        var nextExtent = _offscreenTarget?.Extent ?? _swapchainOwner!.Extent;
        if (!force && _framebuffers.Length > 0 &&
            _extent.Width == nextExtent.Width && _extent.Height == nextExtent.Height)
        {
            Log(VulkanClearFrameLogFormatter.Skipped($"同尺寸跳过帧缓冲重建（{_extent.Width}x{_extent.Height}）"));
            return true;
        }
        _log?.Invoke(VulkanResizeTracer.Stage(generation, "帧缓冲重建开始", "开始"));
        DestroyFramebuffers();
        _extent = nextExtent;
        _views = _offscreenTarget is null
            ? _swapchainOwner!.ImageViews.ToArray()
            : [_offscreenTarget.View];
        RebuildReadbackBuffers();
        _framebuffers = new Framebuffer[_views.Length];
        if (!CreateFramebuffers()) return false;
        _framebufferGeneration++;
        _log?.Invoke(VulkanResizeTracer.Stage(generation, "帧缓冲重建完成", $"物理尺寸={_extent.Width}x{_extent.Height}；帧缓冲={_framebuffers.Length} 张；命令缓冲已重录"));
        return RecordCommandBuffers(_views);
    }

}
