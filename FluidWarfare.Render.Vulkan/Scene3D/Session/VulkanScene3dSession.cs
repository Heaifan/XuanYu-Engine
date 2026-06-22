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

    // ─── 帧绘制 ───────────────────────────────────────────────────

    /// <summary>
    /// 使用当前相机姿态渲染一帧。
    /// </summary>
    public VulkanScene3dFrameResult RenderFrame(
        VulkanScene3dFrameReason reason,
        FluidWarfare.Render.Camera.SceneCameraPose cameraPose,
        ReadOnlySpan<VulkanScene3dUnitDrawInfo> unitDraws)
    {
        if (_status != VulkanScene3dSessionStatus.Active)
            return VulkanScene3dFrameResult.Failed(_frameIndex, reason,
                $"Session 状态不允许渲染：{_status}");

        if (_rendering)
            return VulkanScene3dFrameResult.Failed(_frameIndex, reason, "渲染进行中，跳过。");

        _rendering = true;
        try
        {
            var sw = Stopwatch.StartNew();
            return RenderFrameInternal(reason, cameraPose, unitDraws, sw);
        }
        finally
        {
            _rendering = false;
        }
    }

    /// <summary>
    /// 在 resize 后重建 swapchain 资源。
    /// 使用"创建新资源 → 事务切换 → 销毁旧资源"模式，避免旧 Swapchain 泄漏。
    /// </summary>
    public VulkanScene3dFrameResult Resize(
        uint newWidth, uint newHeight,
        FluidWarfare.Render.Camera.SceneCameraPose cameraPose,
        ReadOnlySpan<VulkanScene3dUnitDrawInfo> unitDraws)
    {
        if (_status != VulkanScene3dSessionStatus.Active)
            return FailFrame(VulkanScene3dFrameReason.Resize,
                $"Session 状态不允许 Resize（当前 {_status}，仅 Active 允许）。");

        if (_rendering)
            return VulkanScene3dFrameResult.Failed(_frameIndex, VulkanScene3dFrameReason.Resize, "渲染进行中，跳过 Resize。");

        // ZeroExtent：窗口最小化时忽略，不创建 0×0 Swapchain
        if (newWidth == 0 || newHeight == 0)
        {
            return VulkanScene3dFrameResult.Skipped(
                _frameIndex, VulkanScene3dFrameReason.Resize,
                _swapchainGeneration, _consecutiveAcquireTimeouts,
                null, $"Resize 忽略 0×0 尺寸（{newWidth}x{newHeight}），等待非零尺寸。");
        }

        _recreateRequested = false;

        _status = VulkanScene3dSessionStatus.RecreatingSwapchain;
        var sw = Stopwatch.StartNew();

        if (_swapchainFunctions is null)
            return FailFrame(VulkanScene3dFrameReason.Resize, "SwapchainFunctions 未初始化。");

        // 等待 GPU 安全点
        var idleResult = _vk!.DeviceWaitIdle(_device);
        if (idleResult != Result.Success)
        {
            DisposeResources();
            _status = VulkanScene3dSessionStatus.Failed;
            return VulkanScene3dFrameResult.Failed(_frameIndex,
                VulkanScene3dFrameReason.Resize, $"DeviceWaitIdle 失败：{idleResult}。");
        }

        // 保存旧资源引用
        var oldResources = _swapchainRes;
        var oldGridPipeline = _gridPipeline;
        var oldUnitPipeline = _unitPipeline;
        var oldGridPipeOk = _gridPipeOk;
        var oldUnitPipeOk = _unitPipeOk;
        var oldOverlayResources = _overlayResources;

        // 用旧 Swapchain 作为 OldSwapchain 创建新资源
        var oldSc = oldResources?.Swapchain ?? default;
        var createResult = VulkanScene3dSwapchainResources.TryCreate(
            _vk, _device, _physicalDevice, _surface,
            newWidth, newHeight, _queueIndex,
            _swapchainFunctions, oldSc);

        if (!createResult.IsSucceeded)
        {
            // 创建失败：旧 Swapchain 已被 retired，不能继续使用
            // 必须完整销毁 Session
            DisposeResources();
            _status = VulkanScene3dSessionStatus.Failed;
            sw.Stop();
            return VulkanScene3dFrameResult.Failed(_frameIndex,
                VulkanScene3dFrameReason.Resize, createResult.Message);
        }

        // 新资源成功：事务切换
        var newResources = createResult.Resources!;
        Pipeline newGridPipeline = default, newUnitPipeline = default;
        bool newGridPipeOk = false, newUnitPipeOk = false;
        Overlay.VulkanOverlayResources? newOverlayResources = null;

        try
        {
            // 创建新 Pipeline（与新 RenderPass 匹配）
            if (!VulkanScene3dPipelines.Create(_vk, _device,
                    newResources.RenderPass, _pipelineLayout,
                    _vertModule, _fragModule,
                    newResources.Extent.Width, newResources.Extent.Height,
                    out newGridPipeline, out newUnitPipeline, out var pipeErr))
            {
                newResources.Dispose();
                DisposeResources();
                _status = VulkanScene3dSessionStatus.Failed;
                return VulkanScene3dFrameResult.Failed(_frameIndex,
                    VulkanScene3dFrameReason.Resize, pipeErr);
            }
            newGridPipeOk = true;
            newUnitPipeOk = true;
            _pipelineCreateCount++;

            // Overlay Pipeline 使用静态 Viewport，并绑定新 RenderPass。Resize 必须同步重建，
            // 否则画面会按旧尺寸绘制，而 CPU HitTest 使用新尺寸，导致 Gizmo 看得见却点不中。
            newOverlayResources = Overlay.VulkanOverlayResources.TryCreate(
                _vk, _device, _physicalDevice,
                newResources.RenderPass,
                newResources.Extent.Width, newResources.Extent.Height,
                out var overlayResizeError);
            if (newOverlayResources is null)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[Overlay] Resize 后资源重建失败：{overlayResizeError}。场景继续运行，但导航 Overlay 暂停。");
            }
        }
        catch
        {
            newOverlayResources?.Dispose();
            newResources.Dispose();
            if (newGridPipeOk) _vk.DestroyPipeline(_device, newGridPipeline, null);
            if (newUnitPipeOk) _vk.DestroyPipeline(_device, newUnitPipeline, null);
            DisposeResources();
            _status = VulkanScene3dSessionStatus.Failed;
            throw;
        }

        // 全部成功 → 原子切换
        _swapchainRes = newResources;
        _gridPipeline = newGridPipeline;
        _unitPipeline = newUnitPipeline;
        _overlayResources = newOverlayResources;
        _gridPipeOk = true;
        _unitPipeOk = true;
        _swapchainGeneration++;
        _lastPresentedOverlaySnapshot = Overlay.PresentedNavigationOverlaySnapshot.Empty;
        _pendingOverlayLayout = null;

        // 安全释放旧资源
        if (oldGridPipeOk && oldGridPipeline.Handle != 0)
            _vk.DestroyPipeline(_device, oldGridPipeline, null);
        if (oldUnitPipeOk && oldUnitPipeline.Handle != 0)
            _vk.DestroyPipeline(_device, oldUnitPipeline, null);
        oldOverlayResources?.Dispose();
        oldResources?.Dispose();

        _status = VulkanScene3dSessionStatus.Active;
        _recreateRequested = false;
        _consecutiveAcquireTimeouts = 0;

        // 不变量：Resize 后应为 1 个活 Swapchain
        if (!VulkanScene3dSwapchainInvariant.IsActiveValid())
        {
            var diag = VulkanScene3dSwapchainInvariant.GetDiagnosticReport();
            DisposeResources();
            _status = VulkanScene3dSessionStatus.Failed;
            return VulkanScene3dFrameResult.Failed(_frameIndex,
                VulkanScene3dFrameReason.Resize, $"[严重]Resize 后不变量失败。\n{diag}");
        }

        sw.Stop();
        return RenderFrameInternal(VulkanScene3dFrameReason.Resize, cameraPose, unitDraws, sw);
    }

    // DisposeResources → VulkanScene3dSessionDisposeOrder.cs

    // ─── 内部渲染 ───────────────────────────────────────────────

    private VulkanScene3dFrameResult RenderFrameInternal(
        VulkanScene3dFrameReason reason,
        FluidWarfare.Render.Camera.SceneCameraPose cameraPose,
        ReadOnlySpan<VulkanScene3dUnitDrawInfo> unitDraws,
        Stopwatch sw)
    {
        if (_vk is null || _swapchainRes is null)
            return FailFrame(reason, "Session 未就绪。");

        _frameIndex++;
        var drawCalls = 0;

        int renderedUnitCount = 0;
        VulkanScene3dCommandRecorder.UnitDrawData[] unitDrawData = [];

        try
        {
            // 1. Wait for fence (有限等待：500ms)
            var fence = _swapchainRes.Fence;
            var waitResult = _vk.WaitForFences(_device, 1, ref fence, Vk.True, FrameFenceTimeoutNanoseconds);
            if (waitResult == Result.Timeout)
                return FailFrame(reason, $"GPU Fence 等待超时：{FrameFenceTimeoutNanoseconds / 1_000_000} ms。");
            if (waitResult != Result.Success)
                return FailFrame(reason, $"GPU Fence 等待失败：{waitResult}。");

            // 2. Acquire next image + 分类结果
            var (acquireResult, imgIndex) = AcquireNextImage(reason);
            if (acquireResult is not null)
                return acquireResult;

            // 4. Reset Fence（只有 Acquire 成功后才执行）
            var resetResult = _vk.ResetFences(_device, 1, ref fence);
            if (resetResult != Result.Success)
                return FailFrame(reason, $"GPU Fence 重置失败：{resetResult}。");

            // 5. Compute camera + build view-projection
            var aspect = _swapchainRes.Extent.Width / (float)_swapchainRes.Extent.Height;
            var vp = ComputeViewProjection(cameraPose, aspect);


            // 6-7. Sync + build unit draw data
            (unitDrawData, renderedUnitCount) = BuildUnitDrawData(vp, unitDraws);


            // 9. Build ground cursor data
            var cursorData = BuildGroundCursorData(vp);


            // 10. Build overlay geometry
            var (overlayVtxCount, overlayBuf, overlayPipe, overlayLayout) = BuildOverlay(cameraPose);


            if (!VulkanScene3dCommandRecorder.Record(_vk, _swapchainRes.CommandBuffer,
                    _swapchainRes.RenderPass, _swapchainRes.Framebuffers[imgIndex],
                    _swapchainRes.Extent,
                    _gridPipeline, _unitPipeline, _pipelineLayout,
                    vp, _gridBuffer, _gridVertexCount,
                    _unitBuffer, _unitVertexCount,
                    unitDrawData,
                    cursorData,
                    overlayBuf, overlayVtxCount,
                    overlayPipe, overlayLayout,
                    _swapchainRes.Extent.Width, _swapchainRes.Extent.Height,
                    out drawCalls, out var cmdErr))
                return FailFrame(reason, cmdErr);

            // 8. Submit
            if (SubmitFrame() != Result.Success)
                return FailFrame(reason, "QueueSubmit 失败。");


            // 9. Present
            var presentRes = PresentFrame(imgIndex);


            // 10. 分类处理 Present 结果
            var presentResult = ClassifyPresentResult(presentRes, reason);
            if (presentResult is not null)
                return presentResult; // 致命失败

            sw.Stop();

            // 11. 已成功 Present → 发布相机快照供 Picking 使用
            VulkanSceneRayBuilder.TryInvert(vp, out var invVp, out _);
            _lastPresentedSnapshot = new PresentedCameraSnapshot
            {
                CameraPose = cameraPose,
                ViewProjection = vp,
                InverseViewProjection = invVp ?? Array.Empty<double>(),
                ViewportWidth = (int)_swapchainRes.Extent.Width,
                ViewportHeight = (int)_swapchainRes.Extent.Height,
                FrameIndex = _frameIndex,
                CameraRevision = cameraPose.Revision
            };
            if (_pendingOverlayLayout is not null)
            {
                _lastPresentedOverlaySnapshot = new Overlay.PresentedNavigationOverlaySnapshot
                {
                    Layout = _pendingOverlayLayout,
                    ViewportWidth = (int)_swapchainRes.Extent.Width,
                    ViewportHeight = (int)_swapchainRes.Extent.Height,
                    PresentedFrameIndex = _frameIndex,
                    CameraRevision = cameraPose.Revision,
                    OverlayRevision = _overlayRevision
                };
            }
            else
            {
                _lastPresentedOverlaySnapshot = Overlay.PresentedNavigationOverlaySnapshot.Empty;
            }
            // 12. 判断是否需要标记重建请求
            var finalStatus = _recreateRequested
                ? VulkanScene3dFrameStatus.RecreateRequested
                : VulkanScene3dFrameStatus.Presented;

            return new VulkanScene3dFrameResult(
                true,
                $"Frame #{_frameIndex} | {reason} | " +
                $"Unit {renderedUnitCount} | DrawCall {drawCalls} | " +
                $"{sw.Elapsed.TotalMilliseconds:F2} ms",
                _frameIndex, reason,
                finalStatus, presentRes, null,
                _swapchainGeneration, _consecutiveAcquireTimeouts,
                (int)_swapchainRes.Extent.Width, (int)_swapchainRes.Extent.Height,
                renderedUnitCount, drawCalls,
                sw.Elapsed.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            _status = VulkanScene3dSessionStatus.Failed;
            return VulkanScene3dFrameResult.Failed(_frameIndex, reason,
                $"帧渲染异常：{ex.Message}");
        }
    }



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
