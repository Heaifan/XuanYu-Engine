namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

/// <summary>
/// DisposeResources 释放顺序（12 步）。
/// 由 FailFrame / Resize 失败路径调用，保留 Instance/Device/Surface。
/// </summary>
unsafe partial class VulkanScene3dSession
{
    /// <summary>
    /// 释放顺序：
    ///   1.  Swapchain 资源
    ///   2.  Guard：Vk/Device 有效？
    ///   3.  Unit Pipeline → 清除标记
    ///   4.  Grid Pipeline → 清除标记
    ///   5.  Pipeline Layout → 清除标记
    ///   6.  Fragment Shader → 清除标记
    ///   7.  Vertex Shader → 清除标记
    ///   8.  Unit Buffer + Memory → 清除标记
    ///   9.  Grid Buffer + Memory → 清除标记
    ///  10.  Cursor Buffer + Memory → 清除标记
    ///  11.  Overlay 资源
    ///  12.  Overlay 状态重置
    /// </summary>
    private void DisposeResources()
    {
        // Step 1
        _swapchainRes?.Dispose();
        _swapchainRes = null;

        // Step 2: Guard
        if (_vk is null || _device.Handle == 0) return;

        // Step 3
        if (_unitPipeOk) { _vk.DestroyPipeline(_device, _unitPipeline, null); _unitPipeOk = false; }
        // Step 4
        if (_gridPipeOk) { _vk.DestroyPipeline(_device, _gridPipeline, null); _gridPipeOk = false; }
        // Step 5
        if (_layoutOk) { _vk.DestroyPipelineLayout(_device, _pipelineLayout, null); _layoutOk = false; }
        // Step 6
        if (_fragModOk) { _vk.DestroyShaderModule(_device, _fragModule, null); _fragModOk = false; }
        // Step 7
        if (_vertModOk) { _vk.DestroyShaderModule(_device, _vertModule, null); _vertModOk = false; }
        // Step 8
        if (_unitBufOk)
        {
            if (_unitBuffer.Handle != 0) _vk.DestroyBuffer(_device, _unitBuffer, null);
            if (_unitMemory.Handle != 0) _vk.FreeMemory(_device, _unitMemory, null);
            _unitBufOk = false;
        }
        // Step 9
        if (_gridBufOk)
        {
            if (_gridBuffer.Handle != 0) _vk.DestroyBuffer(_device, _gridBuffer, null);
            if (_gridMemory.Handle != 0) _vk.FreeMemory(_device, _gridMemory, null);
            _gridBufOk = false;
        }
        // Step 10
        if (_cursorBufOk)
        {
            if (_cursorBuffer.Handle != 0) _vk.DestroyBuffer(_device, _cursorBuffer, null);
            if (_cursorMemory.Handle != 0) _vk.FreeMemory(_device, _cursorMemory, null);
            _cursorBufOk = false;
        }

        // Step 11
        if (_overlayResources is not null)
        {
            _overlayResources.Dispose();
            _overlayResources = null;
        }

        // Step 12
        _pendingOverlayLayout = null;
        _lastPresentedOverlaySnapshot = Overlay.PresentedNavigationOverlaySnapshot.Empty;
        _lastOverlayVertexCount = 0;
    }
}
