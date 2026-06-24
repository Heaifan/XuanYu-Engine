namespace XuanYu.Engine.Render.Vulkan.Scene3D.Session;

/// <summary>
/// DisposeResources 释放顺序编排器（12 步）。
/// 由 FailFrame / Resize 失败路径调用，保留 Instance/Device/Surface。
/// 每个步骤委托到具名释放方法（定义在 PipelineDispose / ShaderDispose / BufferDispose / OverlayDispose 中）。
/// </summary>
unsafe partial class VulkanScene3dSession
{
    private void DisposeResources()
    {
        DisposeSwapchainStep();
        if (!IsDeviceValid) return;

        DisposeUnitPipelineStep();
        DisposeGridPipelineStep();
        DisposePipelineLayoutStep();
        DisposeFragmentShaderStep();
        DisposeVertexShaderStep();
        DisposeUnitBufferStep();
        DisposeGridBufferStep();
        DisposeCursorBufferStep();
        DisposeOverlayResourcesStep();
        ClearFrameOverlayState();
    }

    // ─── Swapchain（两个路径共享） ────────────────────────────────

    private void DisposeSwapchainStep()
    {
        _swapchainRes?.Dispose();
        _swapchainRes = null;
    }
}
