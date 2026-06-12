using System.Diagnostics;
using System.Runtime.InteropServices;
using FluidWarfare.Core.Math;
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
public sealed unsafe class VulkanScene3dSession : IDisposable
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

    // Validation
    private readonly VulkanValidationOptions _validationOptions = VulkanValidationOptions.FromEnvironment();
    private readonly VulkanValidationMessageStore _validationMessageStore = new();
    private Validation.VulkanDebugMessengerScope? _debugMessengerScope;

    // Success flags for session-level resources
    private bool _instOk, _surfOk, _devOk;
    private bool _vertModOk, _fragModOk, _layoutOk;
    private bool _gridPipeOk, _unitPipeOk;
    private bool _gridBufOk, _unitBufOk;

    // ─── 公共属性 ───────────────────────────────────────────────

    public VulkanScene3dSessionStatus Status => _status;
    public int FrameIndex => _frameIndex;
    public int InstanceCreateCount => _instanceCreateCount;
    public int DeviceCreateCount => _deviceCreateCount;
    public int PipelineCreateCount => _pipelineCreateCount;
    public int BufferCreateCount => _bufferCreateCount;
    public int SwapchainGeneration => _swapchainGeneration;
    public int GridVertexCount => _gridVertexCount;
    public int UnitVertexCount => _unitVertexCount;
    public string? SelectedEntityId => _selectedEntityId;
    public VulkanGroundCursorInfo GroundCursorInfo =>
        _cursorState.IsVisible
            ? new VulkanGroundCursorInfo(
                true, _cursorState.WorldPosition, _cursorState.Revision,
                _cursorVertexCount, _cursorState.IsVisible ? 1 : 0)
            : VulkanGroundCursorInfo.Hidden;

    /// <summary>实体 Transform 修订号（位置变化递增）。</summary>
    public int TransformRevision => _transformRevision;

    /// <summary>
    /// 设置当前选中实体。EntityId 无变化时不触发帧。
    /// </summary>
    public bool SetSelectedEntity(string? entityId)
    {
        if (_selectedEntityId == entityId)
            return false;
        _selectedEntityId = entityId;
        return true; // 触发帧重绘
    }

    // ─── 启动 ───────────────────────────────────────────────────

    /// <summary>
    /// 设置地面落点标记。相同坐标或相同隐藏状态返回 false（NoOp）。
    /// 调用方在返回 true 时应请求帧重绘。
    /// </summary>
    public bool SetGroundCursor(Vector3d? worldPosition)
    {
        if (_status != VulkanScene3dSessionStatus.Active)
            return false;
        return _cursorState.Set(worldPosition);
    }

    /// <summary>
    /// 更新一个实体的绘制位置。修改缓存的 UnitDrawData。
    /// 不创建 Instance/Device/Swapchain/Pipeline/VertexBuffer。
    /// </summary>
    /// <param name="entityId">实体 ID 字符串。</param>
    /// <param name="x">新 X 坐标。</param>
    /// <param name="y">新 Y 坐标。</param>
    /// <param name="z">新 Z 坐标。</param>
    /// <returns>是否实际变化（相同坐标返回 false）。</returns>
    public bool UpdateEntityPosition(string entityId, float x, float y, float z)
    {
        for (var i = 0; i < _cachedUnitDraws.Length; i++)
        {
            var draw = _cachedUnitDraws[i];
            if (draw.EntityId == entityId)
            {
                if (Math.Abs(draw.X - x) < 1e-6f &&
                    Math.Abs(draw.Y - y) < 1e-6f &&
                    Math.Abs(draw.Z - z) < 1e-6f)
                {
                    return false; // NoOp
                }

                _cachedUnitDraws[i] = new VulkanScene3dUnitDrawInfo(
                    entityId, x, y, z, draw.Scale);
                _transformRevision++;
                return true;
            }
        }

        return false; // EntityId not found in session
    }

    /// <summary>
    /// 启动 Scene3D 会话。创建会话级资源和 swapchain 级资源。
    /// </summary>
    public VulkanScene3dFrameResult Start(
        nint hinstance, nint hwnd,
        uint reqW, uint reqH,
        FluidWarfare.Render.Camera.SceneCameraState cameraState,
        ReadOnlySpan<VulkanScene3dVertex> gridVertices,
        ReadOnlySpan<VulkanScene3dVertex> unitVertices,
        ReadOnlySpan<VulkanScene3dUnitDrawInfo> unitDraws)
    {
        _status = VulkanScene3dSessionStatus.Starting;
        var sw = Stopwatch.StartNew();

        try
        {
            _vk = Vk.GetApi();
            if (hinstance == 0 || hwnd == 0)
                return FailFrame(VulkanScene3dFrameReason.SessionStart, "句柄不可用。");

            // Instance
            if (!CreateInstance()) return FailFrame(VulkanScene3dFrameReason.SessionStart, "Instance 创建失败。");
            _fnDestroySurface = LoadProc("vkDestroySurfaceKHR");

            // Surface
            if (!CreateSurface(hinstance, hwnd)) return FailFrame(VulkanScene3dFrameReason.SessionStart, "Surface 创建失败。");

            // Physical Device + Queue
            if (!SelectDevice()) return FailFrame(VulkanScene3dFrameReason.SessionStart, "未找到 Graphics+Present 队列。");

            // Device
            if (!CreateDevice()) return FailFrame(VulkanScene3dFrameReason.SessionStart, "Device 创建失败。");

            // Load function pointers
            LoadSessionFunctionPointers();

            // Shader Modules
            if (!CreateShaderModules()) return FailFrame(VulkanScene3dFrameReason.SessionStart, "Shader 创建失败。");

            // Pipeline Layout
            if (!CreatePipelineLayout()) return FailFrame(VulkanScene3dFrameReason.SessionStart, "PipelineLayout 创建失败。");

            // Pipelines (will be linked with render pass from SwapchainResources)
            // Actually pipelines need a render pass. Let's create them after swapchain.
            // For now, store render pass from swapchain resources after they're created.

            // Vertex Buffers
            if (!CreateVertexBuffers(gridVertices, unitVertices))
                return FailFrame(VulkanScene3dFrameReason.SessionStart, "VertexBuffer 创建失败。");

            // Ground Cursor Buffer (session-constant geometry, created once)
            if (!CreateGroundCursorBuffer())
                return FailFrame(VulkanScene3dFrameReason.SessionStart, "GroundCursor VertexBuffer 创建失败。");

            // Create SwapchainFunctions from loaded pointers
            var scFuncs = VulkanScene3dSwapchainFunctions.TryLoad(
                _fnCreateSwapchain, _fnDestroySwapchain, _fnGetSwapchainImages,
                _fnAcquireNextImage, _fnQueuePresent,
                _fnGetCaps, _fnGetFormats, _fnGetModes);
            if (scFuncs is null)
                return FailFrame(VulkanScene3dFrameReason.SessionStart, "Swapchain 函数指针加载不完整。");
            _swapchainFunctions = scFuncs;

            // Swapchain resources (includes RenderPass, Framebuffers, etc.)
            var createResult = VulkanScene3dSwapchainResources.TryCreate(
                _vk, _device, _physicalDevice, _surface,
                reqW, reqH, _queueIndex,
                _swapchainFunctions, default); // oldSwapchain = default (首次启动)
            if (!createResult.IsSucceeded)
                return FailFrame(VulkanScene3dFrameReason.SessionStart, createResult.Message);
            _swapchainRes = createResult.Resources!;
            _swapchainGeneration++;

            // Create pipelines now that we have a render pass
            if (!VulkanScene3dPipelines.Create(_vk, _device,
                    _swapchainRes.RenderPass, _pipelineLayout,
                    _vertModule, _fragModule,
                    _swapchainRes.Extent.Width, _swapchainRes.Extent.Height,
                    out _gridPipeline, out _unitPipeline, out var pipeErr))
                return FailFrame(VulkanScene3dFrameReason.SessionStart, pipeErr);
            _gridPipeOk = true; _unitPipeOk = true;
            _pipelineCreateCount++;

            _status = VulkanScene3dSessionStatus.Active;
            _recreateRequested = false;
            _consecutiveAcquireTimeouts = 0;

            // 不变量：活跃 Session 应有且仅有一个 Live Swapchain
            if (!VulkanScene3dSwapchainInvariant.IsActiveValid())
            {
                var diag = VulkanScene3dSwapchainInvariant.GetDiagnosticReport();
                DisposeResources();
                _status = VulkanScene3dSessionStatus.Failed;
                return VulkanScene3dFrameResult.Failed(0, VulkanScene3dFrameReason.SessionStart,
                    $"[严重]Swapchain 生命周期不变量启动校验失败。\n{diag}");
            }

            sw.Stop();

            // Render first frame
            return RenderFrameInternal(
                VulkanScene3dFrameReason.SessionStart, cameraState, unitDraws, sw);
        }
        catch (Exception ex)
        {
            _status = VulkanScene3dSessionStatus.Failed;
            return VulkanScene3dFrameResult.Failed(0, VulkanScene3dFrameReason.SessionStart,
                $"Session 启动异常：{ex.Message}");
        }
    }

    // ─── 帧绘制 ───────────────────────────────────────────────────

    /// <summary>
    /// 使用当前相机状态渲染一帧。
    /// </summary>
    public VulkanScene3dFrameResult RenderFrame(
        VulkanScene3dFrameReason reason,
        FluidWarfare.Render.Camera.SceneCameraState cameraState,
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
            return RenderFrameInternal(reason, cameraState, unitDraws, sw);
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
        FluidWarfare.Render.Camera.SceneCameraState cameraState,
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
        }
        catch
        {
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
        _gridPipeOk = true;
        _unitPipeOk = true;
        _swapchainGeneration++;

        // 安全释放旧资源
        if (oldGridPipeOk && oldGridPipeline.Handle != 0)
            _vk.DestroyPipeline(_device, oldGridPipeline, null);
        if (oldUnitPipeOk && oldUnitPipeline.Handle != 0)
            _vk.DestroyPipeline(_device, oldUnitPipeline, null);
        oldResources?.Dispose();

        _status = VulkanScene3dSessionStatus.Active;
        _recreateRequested = false;
        _consecutiveAcquireTimeouts = 0;

        // 不变量：Resize 后应为 1 个活 Swapchain
        if (!VulkanScene3dSwapchainInvariant.IsActiveValid())
        {
            var diag = VulkanScene3dSwapchainInvariant.GetDiagnosticReport();
            newResources.Dispose();
            DisposeResources();
            _status = VulkanScene3dSessionStatus.Failed;
            return VulkanScene3dFrameResult.Failed(_frameIndex,
                VulkanScene3dFrameReason.Resize, $"[严重]Resize 后不变量失败。\n{diag}");
        }

        sw.Stop();
        return RenderFrameInternal(VulkanScene3dFrameReason.Resize, cameraState, unitDraws, sw);
    }

    /// <summary>
    /// 完整释放所有会话级 Vulkan 资源（不含 Instance/Device/Surface 等）。
    /// 由 FallBackToVulkanClear 或 Resize 失败时调用。
    /// </summary>
    private void DisposeResources()
    {
        _swapchainRes?.Dispose();
        _swapchainRes = null;

        if (_vk is null || _device.Handle == 0) return;

        if (_unitPipeOk) { _vk.DestroyPipeline(_device, _unitPipeline, null); _unitPipeOk = false; }
        if (_gridPipeOk) { _vk.DestroyPipeline(_device, _gridPipeline, null); _gridPipeOk = false; }
        if (_layoutOk) { _vk.DestroyPipelineLayout(_device, _pipelineLayout, null); _layoutOk = false; }
        if (_fragModOk) { _vk.DestroyShaderModule(_device, _fragModule, null); _fragModOk = false; }
        if (_vertModOk) { _vk.DestroyShaderModule(_device, _vertModule, null); _vertModOk = false; }
        if (_unitBufOk)
        {
            if (_unitBuffer.Handle != 0) _vk.DestroyBuffer(_device, _unitBuffer, null);
            if (_unitMemory.Handle != 0) _vk.FreeMemory(_device, _unitMemory, null);
            _unitBufOk = false;
        }
        if (_gridBufOk)
        {
            if (_gridBuffer.Handle != 0) _vk.DestroyBuffer(_device, _gridBuffer, null);
            if (_gridMemory.Handle != 0) _vk.FreeMemory(_device, _gridMemory, null);
            _gridBufOk = false;
        }

        if (_cursorBufOk)
        {
            if (_cursorBuffer.Handle != 0) _vk.DestroyBuffer(_device, _cursorBuffer, null);
            if (_cursorMemory.Handle != 0) _vk.FreeMemory(_device, _cursorMemory, null);
            _cursorBufOk = false;
        }
    }

    // ─── 内部渲染 ───────────────────────────────────────────────

    private VulkanScene3dFrameResult RenderFrameInternal(
        VulkanScene3dFrameReason reason,
        FluidWarfare.Render.Camera.SceneCameraState cameraState,
        ReadOnlySpan<VulkanScene3dUnitDrawInfo> unitDraws,
        Stopwatch sw)
    {
        if (_vk is null || _swapchainRes is null)
            return FailFrame(reason, "Session 未就绪。");

        _frameIndex++;
        var drawCalls = 0;
        var renderedUnitCount = 0;

        try
        {
            // 1. Wait for fence (有限等待：500ms)
            var fence = _swapchainRes.Fence;
            var waitResult = _vk.WaitForFences(_device, 1, ref fence, Vk.True, FrameFenceTimeoutNanoseconds);
            if (waitResult == Result.Timeout)
                return FailFrame(reason, $"GPU Fence 等待超时：{FrameFenceTimeoutNanoseconds / 1_000_000} ms。");
            if (waitResult != Result.Success)
                return FailFrame(reason, $"GPU Fence 等待失败：{waitResult}。");

            // 2. Acquire next image (有限等待：100ms)
            uint imgIndex = 0;
            var acquireFn = Marshal.GetDelegateForFunctionPointer<AcquireNextImageFn>(_fnAcquireNextImage);
            var acqRes = acquireFn(_device, _swapchainRes.Swapchain, AcquireImageTimeoutNanoseconds,
                _swapchainRes.SemAvail, default, &imgIndex);

            // 3. 分类处理 Acquire 结果
            //    关键规则：Acquire 没成功时，不得 Reset Fence。
            var acquireResult = ClassifyAcquireResult(acqRes, reason);
            if (acquireResult is not null)
                return acquireResult; // 跳过或致命失败，不继续 Present

            // Acquire 成功（Success / Suboptimal），继续
            // Suboptimal 时标记重建请求
            if (acqRes == Result.SuboptimalKhr)
                _recreateRequested = true;

            _consecutiveAcquireTimeouts = 0; // 重置连续超时计数

            // 4. Reset Fence（只有 Acquire 成功后才执行）
            var resetResult = _vk.ResetFences(_device, 1, ref fence);
            if (resetResult != Result.Success)
                return FailFrame(reason, $"GPU Fence 重置失败：{resetResult}。");

            // 5. Compute camera
            var (dirX, dirY, dirZ) = FluidWarfare.Render.Camera.SceneCameraState.DefaultViewDirection();
            var (camX, camY, camZ) = cameraState.ComputePosition();
            var targetX = camX + dirX * cameraState.Distance;
            var targetY = camY + dirY * cameraState.Distance;
            var targetZ = camZ + dirZ * cameraState.Distance;

            var camWithTarget = new VulkanCameraInfo(
                camX, camY, camZ,
                targetX, targetY, targetZ,
                0, 1, 0,
                cameraState.FieldOfViewDegrees,
                cameraState.NearPlane,
                cameraState.FarPlane);

            var aspect = _swapchainRes.Extent.Width / (float)_swapchainRes.Extent.Height;
            var vp = VulkanCameraMatrices.ComputeVulkanMVP(camWithTarget, aspect);

            // 6. Sync cached unit draws from incoming data (first call or after resize)
            if (_cachedUnitDraws.Length != unitDraws.Length)
            {
                _cachedUnitDraws = unitDraws.ToArray();
            }

            // 7. Build per-object MVP + Tint from cached UnitDrawData
            var unitDrawData = new VulkanScene3dCommandRecorder.UnitDrawData[_cachedUnitDraws.Length];
            for (var i = 0; i < _cachedUnitDraws.Length; i++)
            {
                var draw = _cachedUnitDraws[i];
                var trans = VulkanCameraMatrices.CreateTranslation(draw.X, draw.Y, draw.Z);
                var scale = VulkanCameraMatrices.CreateScale(draw.Scale);
                var model = VulkanCameraMatrices.Mul(trans, scale);
                var mvp = VulkanCameraMatrices.Mul(vp, model);
                var tint = draw.EntityId == _selectedEntityId
                    ? VulkanScene3dPushConstants.SelectedTint
                    : VulkanScene3dPushConstants.NormalTint;
                unitDrawData[i] = new VulkanScene3dCommandRecorder.UnitDrawData(mvp, tint);
                renderedUnitCount++;
            }

            // 9. Build ground cursor data if visible
            VulkanScene3dCommandRecorder.GroundCursorDrawData? cursorData = null;
            if (_cursorState.IsVisible && _cursorState.WorldPosition is not null &&
                _cursorBufOk && _cursorBuffer.Handle != 0)
            {
                var pos = _cursorState.WorldPosition.Value;
                var ct = VulkanCameraMatrices.CreateTranslation(
                    (float)pos.X, (float)pos.Y + 0.02f, (float)pos.Z);
                var cursorMvp = VulkanCameraMatrices.Mul(vp, ct);
                cursorData = new VulkanScene3dCommandRecorder.GroundCursorDrawData(
                    _cursorBuffer, _cursorVertexCount, cursorMvp);
            }

            // 10. Record command buffer
            if (!VulkanScene3dCommandRecorder.Record(_vk, _swapchainRes.CommandBuffer,
                    _swapchainRes.RenderPass, _swapchainRes.Framebuffers[imgIndex],
                    _swapchainRes.Extent,
                    _gridPipeline, _unitPipeline, _pipelineLayout,
                    vp, _gridBuffer, _gridVertexCount,
                    _unitBuffer, _unitVertexCount,
                    unitDrawData,
                    cursorData,
                    out drawCalls, out var cmdErr))
                return FailFrame(reason, cmdErr);

            // 8. Submit
            _vk.GetDeviceQueue(_device, _queueIndex, 0, out _queue);
            var waitSem = stackalloc[] { _swapchainRes.SemAvail };
            var waitStage = stackalloc[] { PipelineStageFlags.ColorAttachmentOutputBit };
            var sigSem = stackalloc[] { _swapchainRes.SemFin };
            var cBufs = stackalloc[] { _swapchainRes.CommandBuffer };
            var submitInfo = new SubmitInfo
            {
                SType = StructureType.SubmitInfo,
                WaitSemaphoreCount = 1, PWaitSemaphores = waitSem,
                PWaitDstStageMask = waitStage,
                CommandBufferCount = 1, PCommandBuffers = cBufs,
                SignalSemaphoreCount = 1, PSignalSemaphores = sigSem
            };
            if (_vk.QueueSubmit(_queue, 1, &submitInfo, _swapchainRes.Fence) != Result.Success)
                return FailFrame(reason, "QueueSubmit 失败。");

            // 9. Present
            var presentFn = Marshal.GetDelegateForFunctionPointer<QueuePresentFn>(_fnQueuePresent);
            var scArr = stackalloc[] { _swapchainRes.Swapchain };
            var idxArr = stackalloc[] { imgIndex };
            var presentInfo = new PresentInfoKHR
            {
                SType = StructureType.PresentInfoKhr,
                WaitSemaphoreCount = 1, PWaitSemaphores = sigSem,
                SwapchainCount = 1, PSwapchains = scArr,
                PImageIndices = idxArr
            };
            var presentRes = presentFn(_queue, &presentInfo);

            // 10. 分类处理 Present 结果
            var presentResult = ClassifyPresentResult(presentRes, reason);
            if (presentResult is not null)
                return presentResult; // 致命失败

            sw.Stop();

            // 11. 判断是否需要标记重建请求
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

    /// <summary>
    /// 分类处理 AcquireNextImage 结果。
    /// 返回非 null 表示需要中断当前帧（跳过或失败）。
    /// </summary>
    private VulkanScene3dFrameResult? ClassifyAcquireResult(Result acqRes, VulkanScene3dFrameReason reason)
    {
        switch (acqRes)
        {
            case Result.Success:
            case Result.SuboptimalKhr:
                return null; // 继续

            case Result.Timeout:
                _consecutiveAcquireTimeouts++;
                if (_consecutiveAcquireTimeouts >= MaxConsecutiveAcquireTimeouts)
                {
                    _status = VulkanScene3dSessionStatus.Failed;
                    return VulkanScene3dFrameResult.Failed(_frameIndex, reason,
                        VulkanScene3dFrameStatus.Failed, acqRes, null,
                        _swapchainGeneration, _consecutiveAcquireTimeouts,
                        $"Acquire 连续超时 {MaxConsecutiveAcquireTimeouts} 次，Session 终止。");
                }
                // 跳过本帧
                return VulkanScene3dFrameResult.Skipped(
                    _frameIndex, reason,
                    _swapchainGeneration, _consecutiveAcquireTimeouts,
                    acqRes, $"Acquire 超时（{_consecutiveAcquireTimeouts}/{MaxConsecutiveAcquireTimeouts}），本帧跳过。");

            case Result.NotReady:
                // 与 Timeout 类似：跳过本帧
                return VulkanScene3dFrameResult.Skipped(
                    _frameIndex, reason,
                    _swapchainGeneration, _consecutiveAcquireTimeouts,
                    acqRes, "Acquire NotReady，本帧跳过。");

            case Result.ErrorOutOfDateKhr:
                _recreateRequested = true;
                return VulkanScene3dFrameResult.RecreateRequested(
                    _frameIndex, reason,
                    _swapchainGeneration, _consecutiveAcquireTimeouts,
                    acqRes, "Acquire 返回 OutOfDate，请求重建。");

            case Result.ErrorSurfaceLostKhr:
                _status = VulkanScene3dSessionStatus.Failed;
                return VulkanScene3dFrameResult.Failed(_frameIndex, reason,
                    VulkanScene3dFrameStatus.Failed, acqRes, VulkanScene3dSwapchainStage.SurfaceCapabilities,
                    _swapchainGeneration, _consecutiveAcquireTimeouts,
                    "[严重]Surface 已丢失，Acquire 返回 SurfaceLost。");

            case Result.ErrorDeviceLost:
                _status = VulkanScene3dSessionStatus.Failed;
                return VulkanScene3dFrameResult.Failed(_frameIndex, reason,
                    VulkanScene3dFrameStatus.Failed, acqRes, null,
                    _swapchainGeneration, _consecutiveAcquireTimeouts,
                    "[严重]Device 已丢失，Acquire 返回 DeviceLost。");

            default:
                _status = VulkanScene3dSessionStatus.Failed;
                return VulkanScene3dFrameResult.Failed(_frameIndex, reason,
                    VulkanScene3dFrameStatus.Failed, acqRes, VulkanScene3dSwapchainStage.GetSwapchainImages,
                    _swapchainGeneration, _consecutiveAcquireTimeouts,
                    $"Acquire 未预期错误：{acqRes}。");
        }
    }

    /// <summary>
    /// 分类处理 QueuePresent 结果。
    /// 返回非 null 表示需要中断当前帧（致命失败）。
    /// </summary>
    private VulkanScene3dFrameResult? ClassifyPresentResult(Result presentRes, VulkanScene3dFrameReason reason)
    {
        switch (presentRes)
        {
            case Result.Success:
            case Result.SuboptimalKhr:
                if (presentRes == Result.SuboptimalKhr)
                    _recreateRequested = true;
                return null; // 继续

            case Result.ErrorOutOfDateKhr:
                _recreateRequested = true;
                return VulkanScene3dFrameResult.RecreateRequested(
                    _frameIndex, reason,
                    _swapchainGeneration, _consecutiveAcquireTimeouts,
                    presentRes, "Present 返回 OutOfDate，请求重建。");

            case Result.ErrorSurfaceLostKhr:
                _status = VulkanScene3dSessionStatus.Failed;
                return VulkanScene3dFrameResult.Failed(_frameIndex, reason,
                    VulkanScene3dFrameStatus.Failed, presentRes, VulkanScene3dSwapchainStage.SurfaceCapabilities,
                    _swapchainGeneration, _consecutiveAcquireTimeouts,
                    "[严重]Surface 已丢失，Present 返回 SurfaceLost。");

            case Result.ErrorDeviceLost:
                _status = VulkanScene3dSessionStatus.Failed;
                return VulkanScene3dFrameResult.Failed(_frameIndex, reason,
                    VulkanScene3dFrameStatus.Failed, presentRes, null,
                    _swapchainGeneration, _consecutiveAcquireTimeouts,
                    "[严重]Device 已丢失，Present 返回 DeviceLost。");

            default:
                _status = VulkanScene3dSessionStatus.Failed;
                return VulkanScene3dFrameResult.Failed(_frameIndex, reason,
                    VulkanScene3dFrameStatus.Failed, presentRes, VulkanScene3dSwapchainStage.CreateSwapchain,
                    _swapchainGeneration, _consecutiveAcquireTimeouts,
                    $"Present 未预期错误：{presentRes}。");
        }
    }

    // ─── 创建辅助 ───────────────────────────────────────────────

    private bool CreateInstance()
    {
        var a = Marshal.StringToHGlobalAnsi("FluidWarfare");
        var eg = Marshal.StringToHGlobalAnsi("FluidWarfare");
        var surf = Marshal.StringToHGlobalAnsi("VK_KHR_surface");
        var win = Marshal.StringToHGlobalAnsi("VK_KHR_win32_surface");
        var debug = Marshal.StringToHGlobalAnsi("VK_EXT_debug_utils");
        var layer = Marshal.StringToHGlobalAnsi("VK_LAYER_KHRONOS_validation");
        try
        {
            var enableValidation = _validationOptions.IsRequested;
            var extCount = enableValidation ? 3u : 2u;
            var extPtrs = stackalloc byte*[3];
            extPtrs[0] = (byte*)surf;
            extPtrs[1] = (byte*)win;
            if (enableValidation) extPtrs[2] = (byte*)debug;

            var ai = new ApplicationInfo
            {
                SType = StructureType.ApplicationInfo,
                PApplicationName = (byte*)a, ApplicationVersion = 1,
                PEngineName = (byte*)eg, EngineVersion = 1,
                ApiVersion = (1 << 22) | (0 << 12) | 0
            };
            var ci = new InstanceCreateInfo
            {
                SType = StructureType.InstanceCreateInfo,
                PApplicationInfo = &ai,
                EnabledExtensionCount = extCount,
                PpEnabledExtensionNames = extPtrs
            };

            if (enableValidation)
            {
                var layerPtrs = stackalloc byte*[1];
                layerPtrs[0] = (byte*)layer;
                ci.EnabledLayerCount = 1;
                ci.PpEnabledLayerNames = layerPtrs;
            }

            var result = _vk!.CreateInstance(&ci, null, out _instance);
            if (result != Result.Success) return false;
            _instOk = true;
            _instanceCreateCount++;

            // Create Debug Messenger after Instance
            if (enableValidation)
            {
                try
                {
                    _debugMessengerScope = new VulkanDebugMessengerScope(_vk!, _instance, _validationMessageStore);
                }
                catch
                {
                    // Debug messenger creation failure is non-fatal
                }
            }

            return true;
        }
        finally { Marshal.FreeHGlobal(a); Marshal.FreeHGlobal(eg); Marshal.FreeHGlobal(surf); Marshal.FreeHGlobal(win); Marshal.FreeHGlobal(debug); Marshal.FreeHGlobal(layer); }
    }

    private bool CreateSurface(nint hinstance, nint hwnd)
    {
        var p = Marshal.StringToHGlobalAnsi("vkCreateWin32SurfaceKHR");
        try
        {
            var addr = (nint)_vk!.GetInstanceProcAddr(_instance, (byte*)p);
            if (addr == 0) return false;
            var fn = Marshal.GetDelegateForFunctionPointer<CreateWin32SurfaceFn>(addr);
            var ci = new Win32SurfaceCreateInfoKHR
            {
                SType = StructureType.Win32SurfaceCreateInfoKhr,
                Hinstance = hinstance, Hwnd = hwnd
            };
            fixed (SurfaceKHR* sp = &_surface)
            {
                if (fn(_instance, &ci, null, sp) == Result.Success)
                {
                    _surfOk = true;
                    return true;
                }
            }
            return false;
        }
        finally { Marshal.FreeHGlobal(p); }
    }

    private bool SelectDevice()
    {
        uint count = 0;
        if (_vk!.EnumeratePhysicalDevices(_instance, ref count, null) != Result.Success || count == 0)
            return false;

        var devices = new Silk.NET.Vulkan.PhysicalDevice[count];
        fixed (Silk.NET.Vulkan.PhysicalDevice* p = devices)
            _vk.EnumeratePhysicalDevices(_instance, ref count, p);

        var fnSupport = LoadProc("vkGetPhysicalDeviceSurfaceSupportKHR");
        if (fnSupport == 0) return false;
        var supportFn = Marshal.GetDelegateForFunctionPointer<SurfaceSupportFn>(fnSupport);

        foreach (var d in devices)
        {
            uint qc = 0;
            _vk.GetPhysicalDeviceQueueFamilyProperties(d, ref qc, null);
            var qProps = new QueueFamilyProperties[qc];
            fixed (QueueFamilyProperties* qp = qProps)
                _vk.GetPhysicalDeviceQueueFamilyProperties(d, ref qc, qp);

            for (uint i = 0; i < qc; i++)
            {
                if (qProps[i].QueueCount > 0 && qProps[i].QueueFlags.HasFlag(QueueFlags.GraphicsBit))
                {
                    int supported = 0;
                    supportFn(d, i, _surface, &supported);
                    if (supported != 0)
                    {
                        _physicalDevice = d;
                        _queueIndex = i;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool CreateDevice()
    {
        var qp = 1.0f;
        var qci = new DeviceQueueCreateInfo
        {
            SType = StructureType.DeviceQueueCreateInfo,
            QueueFamilyIndex = _queueIndex,
            QueueCount = 1,
            PQueuePriorities = &qp
        };
        var se = Marshal.StringToHGlobalAnsi("VK_KHR_swapchain");
        try
        {
            var exts = stackalloc byte*[] { (byte*)se };
            var dci = new DeviceCreateInfo
            {
                SType = StructureType.DeviceCreateInfo,
                QueueCreateInfoCount = 1,
                PQueueCreateInfos = &qci,
                EnabledExtensionCount = 1,
                PpEnabledExtensionNames = exts
            };
            var result = _vk!.CreateDevice(_physicalDevice, &dci, null, out _device);
            if (result == Result.Success)
            {
                _devOk = true;
                _deviceCreateCount++;
            }
            return result == Result.Success;
        }
        finally { Marshal.FreeHGlobal(se); }
    }

    private void LoadSessionFunctionPointers()
    {
        _fnDestroySurface = LoadProc("vkDestroySurfaceKHR");
        _fnGetCaps = LoadProc("vkGetPhysicalDeviceSurfaceCapabilitiesKHR");
        _fnGetFormats = LoadProc("vkGetPhysicalDeviceSurfaceFormatsKHR");
        _fnGetModes = LoadProc("vkGetPhysicalDeviceSurfacePresentModesKHR");
        _fnDestroySwapchain = LoadDeviceProc("vkDestroySwapchainKHR");
        _fnCreateSwapchain = LoadDeviceProc("vkCreateSwapchainKHR");
        _fnGetSwapchainImages = LoadDeviceProc("vkGetSwapchainImagesKHR");
        _fnAcquireNextImage = LoadDeviceProc("vkAcquireNextImageKHR");
        _fnQueuePresent = LoadDeviceProc("vkQueuePresentKHR");
    }

    private bool CreateShaderModules()
    {
        if (!VulkanScene3dShaderModules.Create(_vk!, _device,
                out _vertModule, out _fragModule, out var err))
            return false;
        _vertModOk = true;
        _fragModOk = true;
        return true;
    }

    private bool CreatePipelineLayout()
    {
        if (!VulkanScene3dPipelineLayout.Create(_vk!, _device, _physicalDevice,
                out _pipelineLayout, out var err))
            return false;
        _layoutOk = true;
        return true;
    }

    private bool CreateVertexBuffers(
        ReadOnlySpan<VulkanScene3dVertex> gridVertices,
        ReadOnlySpan<VulkanScene3dVertex> unitVertices)
    {
        if (!VulkanScene3dVertexBuffers.Create(_vk!, _physicalDevice, _device,
                gridVertices, unitVertices,
                out _gridBuffer, out _gridMemory,
                out _unitBuffer, out _unitMemory,
                out _gridVertexCount, out _unitVertexCount, out var err))
            return false;
        _gridBufOk = true;
        _unitBufOk = true;
        _bufferCreateCount++;
        return true;
    }

    private bool CreateGroundCursorBuffer()
    {
        var verts = VulkanGroundCursorGeometry.Create();
        if (!VulkanScene3dVertexBuffers.CreateCursor(_vk!, _physicalDevice, _device,
                verts, out _cursorBuffer, out _cursorMemory, out _cursorVertexCount, out var err))
            return false;
        _cursorBufOk = true;
        _bufferCreateCount++;
        return true;
    }

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

    private VulkanScene3dFrameResult FailFrame(VulkanScene3dFrameReason reason, string message)
    {
        DisposeResources();
        _status = VulkanScene3dSessionStatus.Failed;
        return VulkanScene3dFrameResult.Failed(_frameIndex, reason, message);
    }

    // ─── 释放 ─────────────────────────────────────────────────────

    public void Dispose()
    {
        _status = VulkanScene3dSessionStatus.Disposed;
        if (_vk is null) return;

        if (_devOk && _device.Handle != 0)
            try { _vk.DeviceWaitIdle(_device); } catch { }

        // Swapchain resources
        _swapchainRes?.Dispose();
        _swapchainRes = null;

        // Pipelines (depend on RenderPass in swapchainRes - destroyed above)
        if (_unitPipeOk && _device.Handle != 0)
            _vk.DestroyPipeline(_device, _unitPipeline, null);
        if (_gridPipeOk && _device.Handle != 0)
            _vk.DestroyPipeline(_device, _gridPipeline, null);

        // Pipeline Layout
        if (_layoutOk && _device.Handle != 0)
            _vk.DestroyPipelineLayout(_device, _pipelineLayout, null);

        // Shader Modules
        if (_fragModOk && _device.Handle != 0)
            _vk.DestroyShaderModule(_device, _fragModule, null);
        if (_vertModOk && _device.Handle != 0)
            _vk.DestroyShaderModule(_device, _vertModule, null);

        // Vertex Buffers
        if (_unitBufOk && _device.Handle != 0)
        {
            if (_unitBuffer.Handle != 0) _vk.DestroyBuffer(_device, _unitBuffer, null);
            if (_unitMemory.Handle != 0) _vk.FreeMemory(_device, _unitMemory, null);
        }
        if (_gridBufOk && _device.Handle != 0)
        {
            if (_gridBuffer.Handle != 0) _vk.DestroyBuffer(_device, _gridBuffer, null);
            if (_gridMemory.Handle != 0) _vk.FreeMemory(_device, _gridMemory, null);
        }

        if (_cursorBufOk && _device.Handle != 0)
        {
            if (_cursorBuffer.Handle != 0) _vk.DestroyBuffer(_device, _cursorBuffer, null);
            if (_cursorMemory.Handle != 0) _vk.FreeMemory(_device, _cursorMemory, null);
        }

        // Device
        if (_devOk && _device.Handle != 0)
            _vk.DestroyDevice(_device, null);

        // Surface
        if (_surfOk && _fnDestroySurface != 0)
        {
            var fn = Marshal.GetDelegateForFunctionPointer<DestroySurfaceFn>(_fnDestroySurface);
            fn(_instance, _surface, null);
        }

        // Debug Messenger (must precede Instance)
        if (_debugMessengerScope is not null)
        {
            _debugMessengerScope.Dispose();
            _debugMessengerScope = null;
        }

        // Instance
        if (_instOk)
            _vk.DestroyInstance(_instance, null);

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
