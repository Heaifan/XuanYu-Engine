using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Diagnostic;
using XuanYu.Render.Vulkan.Render.StaticModels;
using XuanYu.Render.Vulkan.Render.VectorOverlay;
using XuanYu.Render.Vulkan.Benchmark;
using XuanYu.Render.Vulkan.Render.Readback;
using XuanYu.Render.Vulkan.Swapchain;
using XuanYu.Render.Vulkan.Render.Frame;
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
    ImageView[] _views = []; VulkanDepthAttachment?[] _depthAttachments = [];
    Silk.NET.Vulkan.Pipeline _pipeline = default;
    PipelineLayout _pipelineLayout = default;
    readonly VulkanFrameState _frameState = new();
    readonly VulkanGpuResourceState _gpuResourceState = new();
    readonly VulkanStaticModelCache _staticModels;
    readonly VulkanVectorOverlayCache _vectorOverlays;
    VulkanStaticModelBuffer? _proceduralVertexBuffer;
    Extent2D _extent;
    int _recordCommandDepth;
    int _frameSlotCount;
    VulkanFrameContext? _activeFrameContext;
    readonly VulkanFrameResourceRetirement _resourceRetirement = new(2);
    int _recordCommandTraceCount;
    int _lastLoggedCommandEntityCount = -1;
    int _lastLoggedCommandViewCount = -1;
    bool _disposed;

    RenderProjection _renderProjection => _frameState.Projection;
    bool _hasRenderProjection => _frameState.HasProjection;

    public VulkanClearFrameOwner(Vk vk, VulkanDeviceOwner deviceOwner, VulkanSwapchainOwner swapchainOwner, int graphicsFamily, Action<string>? log)
        : this(vk, deviceOwner, swapchainOwner, graphicsFamily, log, 1) { }

    public VulkanClearFrameOwner(Vk vk, VulkanDeviceOwner deviceOwner, VulkanSwapchainOwner swapchainOwner,
        int graphicsFamily, Action<string>? log, int frameSlotCount)
    {
        if (frameSlotCount is < 1 or > 2) throw new ArgumentOutOfRangeException(nameof(frameSlotCount));
        _frameSlotCount = frameSlotCount;
        _vk = vk; _deviceOwner = deviceOwner; _swapchainOwner = swapchainOwner; _log = log;
        _staticModels = new VulkanStaticModelCache(vk, deviceOwner, log, RetireGpuResource);
        _vectorOverlays = new VulkanVectorOverlayCache(vk, deviceOwner, log, RetireGpuResource);
        _proceduralVertexBuffer = VulkanStaticModelBuffer.Create(vk, deviceOwner,
            new VulkanStaticModelVertex[RenderDrawPlan.RotateGizmoVertexCount],
            BufferUsageFlags.VertexBufferBit, out _);
        Initialize(graphicsFamily);
    }

}
