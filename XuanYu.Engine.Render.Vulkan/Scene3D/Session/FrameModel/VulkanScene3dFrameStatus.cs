namespace XuanYu.Engine.Render.Vulkan.Scene3D.Session;

/// <summary>
/// 单帧渲染结果状态。
/// 不再将"跳过""重建请求"与"致命失败"混为同一状态。
/// </summary>
public enum VulkanScene3dFrameStatus
{
    /// <summary>帧成功提交并呈现。</summary>
    Presented,

    /// <summary>帧跳过（Acquire 超时 / NotReady / 窗口最小化），Session 保持 Active。</summary>
    Skipped,

    /// <summary>帧完成但请求 Swapchain 重建（Suboptimal / OutOfDate）。</summary>
    RecreateRequested,

    /// <summary>帧遇到致命错误，Session 应标记为 Failed。</summary>
    Failed
}
