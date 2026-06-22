using FluidWarfare.Render.Vulkan.Scene3D.GroundCursor;
using FluidWarfare.Render.Vulkan.Scene3D.Session.Swapchain;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

/// <summary>
/// Vulkan 核心句柄 + 函数指针 + 帧常量。
/// </summary>
unsafe partial class VulkanScene3dSession
{
    // ─── 会话级持久资源 ─────────────────────────────────────────
    private Vk? _vk;
    private Silk.NET.Vulkan.Instance _instance;
    private SurfaceKHR _surface;
    private Silk.NET.Vulkan.Device _device;
    private Silk.NET.Vulkan.PhysicalDevice _physicalDevice;
    private uint _queueIndex;
    private Queue _queue;

    // Shader & Pipeline
    private ShaderModule _vertModule, _fragModule;
    private PipelineLayout _pipelineLayout;
    private Pipeline _gridPipeline, _unitPipeline;

    // Vertex Buffers
    private Silk.NET.Vulkan.Buffer _gridBuffer, _unitBuffer;
    private DeviceMemory _gridMemory, _unitMemory;
    private int _gridVertexCount;
    private int _unitVertexCount;

    // Ground Cursor Buffer
    private Silk.NET.Vulkan.Buffer _cursorBuffer;
    private DeviceMemory _cursorMemory;
    private int _cursorVertexCount;
    private bool _cursorBufOk;
    private readonly VulkanGroundCursorState _cursorState = new();

    // UnitDrawData 缓存
    private VulkanScene3dUnitDrawInfo[] _cachedUnitDraws = [];
    private int _transformRevision;

    // Function pointers
    private nint _fnDestroySurface, _fnDestroySwapchain, _fnCreateSwapchain;
    private nint _fnGetSwapchainImages, _fnAcquireNextImage, _fnQueuePresent;
    private nint _fnGetCaps, _fnGetFormats, _fnGetModes;

    // Swapchain 函数集合
    private VulkanScene3dSwapchainFunctions? _swapchainFunctions;

    // Fence 超时常量
    private const ulong FrameFenceTimeoutNanoseconds = 500_000_000;
    private const ulong AcquireImageTimeoutNanoseconds = 100_000_000;
    private const int MaxConsecutiveAcquireTimeouts = 10;
}
