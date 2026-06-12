namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

/// <summary>
/// Scene3D 帧触发原因，用于诊断日志。
/// </summary>
public enum VulkanScene3dFrameReason
{
    /// <summary>会话首次启动。</summary>
    SessionStart,

    /// <summary>相机平移触发。</summary>
    CameraPan,

    /// <summary>相机缩放触发。</summary>
    CameraZoom,

    /// <summary>相机重置触发。</summary>
    CameraReset,

    /// <summary>窗口 resize 触发。</summary>
    Resize
}
