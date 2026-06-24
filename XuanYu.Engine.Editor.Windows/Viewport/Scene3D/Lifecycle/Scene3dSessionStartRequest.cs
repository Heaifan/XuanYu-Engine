using XuanYu.Engine.Render.Camera;

namespace FluidWarfare.Editor.Windows.Viewport.Scene3D.Lifecycle;

/// <summary>Scene3D 会话启动请求。不含 Shell/面板引用。</summary>
public readonly record struct Scene3dSessionStartRequest(
    IntPtr InstanceHandle,
    IntPtr WindowHandle,
    uint Width,
    uint Height,
    SceneCameraPose CameraPose);
