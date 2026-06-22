using System.Diagnostics;
using System.Runtime.InteropServices;
using FluidWarfare.Core.Math;
using FluidWarfare.Render.Camera.Navigation;
using FluidWarfare.Render.ViewportNavigation;
using FluidWarfare.Render.Vulkan.Camera;
using FluidWarfare.Render.Vulkan.Scene3D.GroundCursor;
using FluidWarfare.Render.Vulkan.Scene3D.Session.Swapchain;
using FluidWarfare.Render.Vulkan.Validation;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

/// <summary>
/// 持久 Scene3D 渲染会话。
/// 会话级资源（Instance、Device、Shader、VertexBuffer）在整个生命周期保持。
/// Swapchain 级资源在 resize 时重建。
/// 每帧仅重录 CommandBuffer + Submit/Present。
/// </summary>
public sealed unsafe partial class VulkanScene3dSession : IDisposable
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

    // UnitDrawData 缓存（避免每帧重新分配）
    private VulkanScene3dUnitDrawInfo[] _cachedUnitDraws = [];
    private int _transformRevision;
    private DeviceMemory _cursorMemory;
    private int _cursorVertexCount;
    private bool _cursorBufOk;
    private readonly VulkanGroundCursorState _cursorState = new();

    // Function pointers (loaded once per session)
    private nint _fnDestroySurface;
    private nint _fnDestroySwapchain;
    private nint _fnCreateSwapchain;
    private nint _fnGetSwapchainImages;
    private nint _fnAcquireNextImage;
    private nint _fnQueuePresent;
    private nint _fnGetCaps;
    private nint _fnGetFormats;
    private nint _fnGetModes;

    // Swapchain 函数集合（从上述指针创建，验证完整性）
    private VulkanScene3dSwapchainFunctions? _swapchainFunctions;

    // Fence 超时常量
    private const ulong FrameFenceTimeoutNanoseconds = 500_000_000;
    private const ulong AcquireImageTimeoutNanoseconds = 100_000_000;
    private const int MaxConsecutiveAcquireTimeouts = 10;

    // ─── Swapchain 级资源 ───────────────────────────────────────
    private VulkanScene3dSwapchainResources? _swapchainRes;

    // ─── 运行时状态 ─────────────────────────────────────────────
    private VulkanScene3dSessionStatus _status = VulkanScene3dSessionStatus.Inactive;
    private int _frameIndex;
    private int _instanceCreateCount;
    private int _deviceCreateCount;
    private int _pipelineCreateCount;
    private int _bufferCreateCount;
    private int _swapchainGeneration;
    private int _consecutiveAcquireTimeouts;
    private string? _selectedEntityId;
    private bool _rendering; // 防重入
    private bool _recreateRequested; // Acquire/Present 返回 OutOfDate/Suboptimal 时标记
    private PresentedCameraSnapshot _lastPresentedSnapshot = PresentedCameraSnapshot.Empty;

    // Overlay
    private Overlay.VulkanOverlayResources? _overlayResources;
    private ViewportNavigation.ViewportNavigationElement _overlayHovered
        = ViewportNavigation.ViewportNavigationElement.None;
    private ViewportNavigation.ViewportNavigationElement _overlayActive
        = ViewportNavigation.ViewportNavigationElement.None;
    private Render.ViewportNavigation.ViewportNavigationLayout? _pendingOverlayLayout;
    private Overlay.PresentedNavigationOverlaySnapshot _lastPresentedOverlaySnapshot =
        Overlay.PresentedNavigationOverlaySnapshot.Empty;
    private int _overlayRevision;

    // Validation
    private readonly VulkanValidationOptions _validationOptions = VulkanValidationOptions.FromEnvironment();
    private readonly VulkanValidationMessageStore _validationMessageStore = new();
    private Validation.VulkanDebugMessengerScope? _debugMessengerScope;

    // Success flags for session-level resources
    private bool _instOk, _surfOk, _devOk;
    private bool _vertModOk, _fragModOk, _layoutOk;
    private bool _gridPipeOk, _unitPipeOk;
    private bool _gridBufOk, _unitBufOk;

    // ─── 保留字段（属性在 VulkanScene3dSession.Properties.cs）──
    private int _lastOverlayVertexCount;
    private Render.ViewportNavigation.ViewportNavigationLayout _lastOverlayLayout = null!;
    private Overlay.VulkanOverlayVertex[]? _pendingGizmoVerts;

    // Start → VulkanScene3dSessionStart.cs
    // RenderFrame / Resize / RenderFrameInternal → Session/Render/



    // Create* → VulkanScene3dSessionStart.cs / CreateInstance.cs / CreateSurface.cs / CreateDevice.cs / CreateResources.cs

    // ─── 辅助 ────────────────────────────────────────────────────

    private nint LoadProc(string name)
    {
        var p = Marshal.StringToHGlobalAnsi(name);
        try { return (nint)_vk!.GetInstanceProcAddr(_instance, (byte*)p); }
        finally { Marshal.FreeHGlobal(p); }
    }

    private nint LoadDeviceProc(string name)
    {
        var p = Marshal.StringToHGlobalAnsi(name);
        try { return (nint)_vk!.GetDeviceProcAddr(_device, (byte*)p); }
        finally { Marshal.FreeHGlobal(p); }
    }


    // ─── 释放 ─────────────────────────────────────────────────────

    public void Dispose()
    {
        _status = VulkanScene3dSessionStatus.Disposed;
        _lastPresentedSnapshot = PresentedCameraSnapshot.Empty;
        if (_vk is null) return;

        if (_devOk && _device.Handle != 0)
            try { _vk.DeviceWaitIdle(_device); } catch { }

        // 资源释放：一次性按顺序释放所有 Vulkan 资源
        DisposeSessionResources();
        ClearAllResourceFlags();

        // 不变量：Dispose 后应为 0 个活 Swapchain
        if (!VulkanScene3dSwapchainInvariant.IsDisposedValid())
        {
            var diag = VulkanScene3dSwapchainInvariant.GetDiagnosticReport();
            // 仅诊断日志，不抛异常（Dispose 中不应抛出）
            System.Diagnostics.Debug.WriteLine(
                $"[严重]Session Dispose 后 Swapchain 不变量失效。\n{diag}");
        }
    }

    // ─── 委托 ────────────────────────────────────────────────────

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result SurfaceSupportFn(Silk.NET.Vulkan.PhysicalDevice pd, uint qi, SurfaceKHR s, int* supported);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result CreateWin32SurfaceFn(Silk.NET.Vulkan.Instance i, Win32SurfaceCreateInfoKHR* ci, AllocationCallbacks* a, SurfaceKHR* s);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result AcquireNextImageFn(Silk.NET.Vulkan.Device d, SwapchainKHR sc, ulong t, Silk.NET.Vulkan.Semaphore s, Fence f, uint* i);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result QueuePresentFn(Queue q, PresentInfoKHR* p);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate void DestroySurfaceFn(Silk.NET.Vulkan.Instance i, SurfaceKHR s, AllocationCallbacks* a);
}
