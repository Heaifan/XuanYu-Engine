using XuanYu.Engine.Render.Vulkan.Scene3D.Session;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Frame;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Submit;

namespace XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Lifecycle;

/// <summary>Scene3D 会话启动结果。不含文案，Message 仅用于日志。</summary>
public sealed record Scene3dSessionStartResult(
    bool Success,
    VulkanScene3dSession? Session,
    Scene3dFrameRoute? FrameRoute,
    Scene3dFrameSubmitRoute? FrameSubmitRoute,
    string Message)
{
    public static Scene3dSessionStartResult Failed(string msg) =>
        new(false, null, null, null, msg);
}
