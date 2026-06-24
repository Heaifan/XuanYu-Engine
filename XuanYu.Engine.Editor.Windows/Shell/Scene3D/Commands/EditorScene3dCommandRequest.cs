using FluidWarfare.Editor.Windows.Panels.Viewport;
using FluidWarfare.Editor.Windows.Viewport.Camera;
using FluidWarfare.Editor.Windows.Viewport.Selection.Route;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Diagnostics;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Lifecycle;
using FluidWarfare.Editor.Windows.Viewport.Transform.Application;

namespace FluidWarfare.Editor.Windows.Shell.Scene3D.Commands;

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
