namespace XuanYu.Engine.Render.Vulkan.Scene3D.Session;

/// <summary>
/// DisposeSessionResources 释放顺序编排器（13 步）。
/// 由 Dispose() 调用，销毁所有 Vulkan 资源（含 Device/Surface/Instance）。
/// Device Handle 无效时跳过中间步骤（goto ReleaseDeviceAndAbove）。
/// </summary>
unsafe partial class VulkanScene3dSession
{
    private void DisposeSessionResources()
    {
        DisposeSwapchainStep();

        if (_device.Handle == 0) goto ReleaseDeviceAndAbove;

        DisposeUnitPipelineStep();
        DisposeGridPipelineStep();
        DisposePipelineLayoutStep();
        DisposeFragmentShaderStep();
        DisposeVertexShaderStep();
        DisposeUnitBufferStep();
        DisposeGridBufferStep();
        DisposeCursorBufferStep();

    ReleaseDeviceAndAbove:
        DisposeDeviceStep();
        DisposeSurfaceStep();
        DisposeDebugMessengerStep();
        DisposeInstanceStep();
    }
}
