namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

/// <summary>
/// FailFrame 文案构造。
/// </summary>
partial class VulkanScene3dSession
{
    /// <summary>
    /// 构造致命失败帧结果，释放资源，标记 Failed 状态。
    /// </summary>
    private VulkanScene3dFrameResult FailFrame(VulkanScene3dFrameReason reason, string message)
    {
        DisposeResources();
        _status = VulkanScene3dSessionStatus.Failed;
        return VulkanScene3dFrameResult.Failed(_frameIndex, reason, message);
    }
}
