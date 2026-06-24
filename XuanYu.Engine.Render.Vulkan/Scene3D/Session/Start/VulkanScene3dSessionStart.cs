using System.Diagnostics;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session.Swapchain;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D.Session;

/// <summary>Start() 编排器：按顺序调用 Create* 步骤，失败时返回 FailFrame。</summary>
unsafe partial class VulkanScene3dSession
{
    public VulkanScene3dFrameResult Start(
        nint hinstance, nint hwnd,
        uint reqW, uint reqH,
        XuanYu.Engine.Render.Camera.SceneCameraPose cameraPose,
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

            if (!CreateInstance()) return FailFrame(VulkanScene3dFrameReason.SessionStart, "Instance 创建失败。");
            _fnDestroySurface = LoadProc("vkDestroySurfaceKHR");
            if (!CreateSurface(hinstance, hwnd)) return FailFrame(VulkanScene3dFrameReason.SessionStart, "Surface 创建失败。");
            if (!SelectDevice()) return FailFrame(VulkanScene3dFrameReason.SessionStart, "未找到 Graphics+Present 队列。");
            if (!CreateDevice()) return FailFrame(VulkanScene3dFrameReason.SessionStart, "Device 创建失败。");
            LoadSessionFunctionPointers();
            if (!CreateShaderModules()) return FailFrame(VulkanScene3dFrameReason.SessionStart, "Shader 创建失败。");
            if (!CreatePipelineLayout()) return FailFrame(VulkanScene3dFrameReason.SessionStart, "PipelineLayout 创建失败。");
            if (!CreateVertexBuffers(gridVertices, unitVertices))
                return FailFrame(VulkanScene3dFrameReason.SessionStart, "VertexBuffer 创建失败。");
            if (!CreateGroundCursorBuffer())
                return FailFrame(VulkanScene3dFrameReason.SessionStart, "GroundCursor VertexBuffer 创建失败。");

            var scFuncs = VulkanScene3dSwapchainFunctions.TryLoad(
                _fnCreateSwapchain, _fnDestroySwapchain, _fnGetSwapchainImages,
                _fnAcquireNextImage, _fnQueuePresent,
                _fnGetCaps, _fnGetFormats, _fnGetModes);
            if (scFuncs is null)
                return FailFrame(VulkanScene3dFrameReason.SessionStart, "Swapchain 函数指针加载不完整。");
            _swapchainFunctions = scFuncs;

            var createResult = VulkanScene3dSwapchainResources.TryCreate(
                _vk, _device, _physicalDevice, _surface,
                reqW, reqH, _queueIndex, _swapchainFunctions, default);
            if (!createResult.IsSucceeded)
                return FailFrame(VulkanScene3dFrameReason.SessionStart, createResult.Message);
            _swapchainRes = createResult.Resources!;
            _swapchainGeneration++;

            if (!VulkanScene3dPipelines.Create(_vk, _device,
                    _swapchainRes.RenderPass, _pipelineLayout,
                    _vertModule, _fragModule,
                    _swapchainRes.Extent.Width, _swapchainRes.Extent.Height,
                    out _gridPipeline, out _unitPipeline, out var pipeErr))
                return FailFrame(VulkanScene3dFrameReason.SessionStart, pipeErr);
            _gridPipeOk = true; _unitPipeOk = true;
            _pipelineCreateCount++;

            _overlayResources = Overlay.VulkanOverlayResources.TryCreate(
                _vk, _device, _physicalDevice, _swapchainRes.RenderPass,
                _swapchainRes.Extent.Width, _swapchainRes.Extent.Height, out var overlayErr);
            if (_overlayResources is null)
                System.Diagnostics.Debug.WriteLine($"[Overlay] 创建失败：{overlayErr}，将继续运行无 Overlay 的 3D 场景。");

            _status = VulkanScene3dSessionStatus.Active;
            _recreateRequested = false;
            _consecutiveAcquireTimeouts = 0;

            if (!VulkanScene3dSwapchainInvariant.IsActiveValid())
            {
                var diag = VulkanScene3dSwapchainInvariant.GetDiagnosticReport();
                DisposeResources();
                _status = VulkanScene3dSessionStatus.Failed;
                return VulkanScene3dFrameResult.Failed(0, VulkanScene3dFrameReason.SessionStart,
                    $"[严重]Swapchain 生命周期不变量启动校验失败。\n{diag}");
            }

            sw.Stop();
            return RenderFrameInternal(
                VulkanScene3dFrameReason.SessionStart, cameraPose, unitDraws, sw);
        }
        catch (Exception ex)
        {
            _status = VulkanScene3dSessionStatus.Failed;
            return VulkanScene3dFrameResult.Failed(0, VulkanScene3dFrameReason.SessionStart,
                $"Session 启动异常：{ex.Message}");
        }
    }
}
