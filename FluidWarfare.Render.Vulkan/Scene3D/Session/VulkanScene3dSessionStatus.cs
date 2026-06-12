namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

/// <summary>
/// Scene3D 会话状态机。
///
/// 转换规则：
///   Inactive → Starting → Active
///   Active → RecreatingSwapchain → Active
///   Starting / Active / RecreatingSwapchain → Failed
///   Inactive / Active / Failed → Disposed
/// 禁止：Disposed → Active, Failed → Active
/// </summary>
public enum VulkanScene3dSessionStatus
{
    /// <summary>会话未启动。</summary>
    Inactive,

    /// <summary>正在创建初始资源。</summary>
    Starting,

    /// <summary>会话活跃，可接收帧请求。</summary>
    Active,

    /// <summary>正在重建 Swapchain（resize）。</summary>
    RecreatingSwapchain,

    /// <summary>会话失败，不可继续使用。</summary>
    Failed,

    /// <summary>已释放。</summary>
    Disposed
}
