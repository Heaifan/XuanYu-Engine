using System.Runtime.InteropServices;
using FluidWarfare.Render.Camera;
using FluidWarfare.Render.Camera.Navigation;
using FluidWarfare.Render.Vulkan.Camera;
using FluidWarfare.Render.Vulkan.Scene3D.Overlay;
using FluidWarfare.Render.ViewportNavigation;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

partial class VulkanScene3dSession
{
    float[] ComputeViewProjection(SceneCameraPose pose, float aspect)
    {
        var ci = new VulkanCameraInfo(pose.PositionX, pose.PositionY, pose.PositionZ,
            pose.TargetX, pose.TargetY, pose.TargetZ, pose.UpX, pose.UpY, pose.UpZ,
            pose.FieldOfViewDegrees, pose.NearPlane, pose.FarPlane);
        return pose.ProjectionMode == SceneProjectionMode.Orthographic
            ? VulkanCameraMatrices.ComputeVulkanOrthoMVP(ci, aspect, pose.OrthographicHeight)
            : VulkanCameraMatrices.ComputeVulkanMVP(ci, aspect);
    }

    (VulkanScene3dCommandRecorder.UnitDrawData[] Data, int Count) BuildUnitDrawData(
        float[] vp, ReadOnlySpan<VulkanScene3dUnitDrawInfo> unitDraws)
    {
        if (_cachedUnitDraws.Length != unitDraws.Length) _cachedUnitDraws = unitDraws.ToArray();
        var data = new VulkanScene3dCommandRecorder.UnitDrawData[_cachedUnitDraws.Length];
        var count = 0;
        for (var i = 0; i < _cachedUnitDraws.Length; i++)
        {
            var draw = _cachedUnitDraws[i];
            var mvp = VulkanCameraMatrices.Mul(VulkanCameraMatrices.Mul(vp,
                VulkanCameraMatrices.CreateTranslation(draw.X, draw.Y, draw.Z)),
                VulkanCameraMatrices.CreateScale(draw.Scale));
            data[i] = new(mvp, draw.EntityId == _selectedEntityId
                ? VulkanScene3dPushConstants.SelectedTint : VulkanScene3dPushConstants.NormalTint);
            count++;
        }
        return (data, count);
    }

    VulkanScene3dCommandRecorder.GroundCursorDrawData? BuildGroundCursorData(float[] vp)
    {
        if (!_cursorState.IsVisible || _cursorState.WorldPosition is null || !_cursorBufOk || _cursorBuffer.Handle == 0)
            return null;
        var ct = VulkanCameraMatrices.CreateTranslation(
            (float)_cursorState.WorldPosition.Value.X, (float)_cursorState.WorldPosition.Value.Y,
            (float)_cursorState.WorldPosition.Value.Z + 0.02f);
        return new(_cursorBuffer, _cursorVertexCount, VulkanCameraMatrices.Mul(vp, ct));
    }

    (int, Silk.NET.Vulkan.Buffer?, Pipeline?, PipelineLayout?) BuildOverlay(SceneCameraPose pose)
    {
        _pendingOverlayLayout = null; _lastOverlayVertexCount = 0;
        if (_overlayResources is null || !_overlayResources.IsValid) return (0, null, null, null);
        try
        {
            var extW = (int)_swapchainRes!.Extent.Width; var extH = (int)_swapchainRes.Extent.Height;
            var projText = pose.ProjectionMode == SceneProjectionMode.Perspective ? "Persp" : "Ortho";
            _lastOverlayLayout = ViewportNavigationLayoutCompute.Compute(extW, extH, pose);
            var verts = VulkanNavigationOverlayGeometry.Build(_lastOverlayLayout, _overlayHovered, _overlayActive, projText);
            var merged = _pendingGizmoVerts is { Length: > 0 } ? [.. verts, .. _pendingGizmoVerts] : verts;
            _pendingGizmoVerts = null;
            if (_overlayResources.UploadVertices(merged, out var err))
            { _lastOverlayVertexCount = merged.Length; _pendingOverlayLayout = _lastOverlayLayout; return (merged.Length, _overlayResources.VertexBuffer, _overlayResources.Pipeline, _overlayResources.Layout); }
            System.Diagnostics.Debug.WriteLine($"[Overlay] 顶点上传失败：{err}。");
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[Overlay] 异常：{ex.Message}"); }
        return (0, null, null, null);
    }

    unsafe Result SubmitFrame()
    {
        _vk!.GetDeviceQueue(_device, _queueIndex, 0, out _queue);
        var ws = stackalloc Silk.NET.Vulkan.Semaphore[] { _swapchainRes!.SemAvail };
        var wst = stackalloc Silk.NET.Vulkan.PipelineStageFlags[] { PipelineStageFlags.ColorAttachmentOutputBit };
        var cb = stackalloc Silk.NET.Vulkan.CommandBuffer[] { _swapchainRes.CommandBuffer };
        var ss = stackalloc Silk.NET.Vulkan.Semaphore[] { _swapchainRes.SemFin };
        var si = new SubmitInfo
        {
            SType = StructureType.SubmitInfo, WaitSemaphoreCount = 1, PWaitSemaphores = ws,
            PWaitDstStageMask = wst, CommandBufferCount = 1, PCommandBuffers = cb,
            SignalSemaphoreCount = 1, PSignalSemaphores = ss
        };
        return _vk.QueueSubmit(_queue, 1, &si, _swapchainRes.Fence);
    }

    unsafe Result PresentFrame(uint imgIndex)
    {
        var fn = Marshal.GetDelegateForFunctionPointer<QueuePresentFn>(_fnQueuePresent);
        var pws = stackalloc Silk.NET.Vulkan.Semaphore[] { _swapchainRes!.SemFin };
        var psc = stackalloc SwapchainKHR[] { _swapchainRes.Swapchain };
        uint idx = imgIndex;
        var pi = new PresentInfoKHR { SType = StructureType.PresentInfoKhr, WaitSemaphoreCount = 1,
            PWaitSemaphores = pws, SwapchainCount = 1, PSwapchains = psc, PImageIndices = &idx };
        return fn(_queue, &pi);
    }
}
