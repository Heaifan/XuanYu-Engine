using XuanYu.Engine.Editor.Windows.Panels.Viewport;
using XuanYu.Engine.Editor.Windows.Viewport.Camera;
using XuanYu.Engine.Editor.Windows.Viewport.Selection.Route;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Diagnostics;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Lifecycle;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Application;

namespace XuanYu.Engine.Editor.Windows.Shell.Scene3D.Commands;

/// <summary>Shell → Scene3dCommandRoute 的请求。CameraRoute 仅用于 Session Start，Route 不持有。</summary>
public sealed record EditorScene3dCommandRequest(
    EditorScene3dCommandKind Kind,
    VulkanViewportProbeRoute ProbeRoute,
    Scene3dSessionLifecycle Lifecycle,
    ViewportRenderSceneStore RenderSceneStore,
    VulkanViewportNativeHostInfo NativeHostInfo,
    ViewportCameraRoute CameraRoute,
    int CurrentRenderSeq,
    Action<string> InfoLog,
    Action<string> WarnLog);
