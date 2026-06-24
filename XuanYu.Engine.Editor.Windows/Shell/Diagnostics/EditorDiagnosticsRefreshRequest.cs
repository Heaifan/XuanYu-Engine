using FluidWarfare.Editor.EntityTransform;
using FluidWarfare.Editor.Windows.Panels.Status;
using FluidWarfare.Editor.Windows.Panels.Viewport;
using FluidWarfare.Editor.Windows.Shell.Feedback;
using FluidWarfare.Editor.Windows.Shell.Menu;
using FluidWarfare.Editor.Windows.Viewport.Camera;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Diagnostics;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Frame;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Lifecycle;
using FluidWarfare.Editor.Windows.Viewport.Selection.Route;
using FluidWarfare.Editor.Windows.Viewport.Transform.Application;
using FluidWarfare.Editor.Windows.Viewport.Transform.Interaction;
using FluidWarfare.Engine.World;

namespace FluidWarfare.Editor.Windows.Shell.Diagnostics;

/// <summary>诊断路由的上下文依赖。Shell 在构造后初始化一次。</summary>
public sealed record EditorDiagnosticsContext(
    VulkanViewportProbeRoute ProbeRoute,
    EditorFeedbackRoute Feedback,
    Scene3dSessionLifecycle Lifecycle,
    ViewportRenderSceneStore RenderSceneStore,
    ViewportCameraRoute CameraRoute,
    EditorRunMenuRoute RunMenu,
    Func<VulkanViewportNativeHostInfo> GetNativeHostInfo,
    VulkanViewportHostPanel? VulkanHost,
    StatusBarPanel? StatusBar,
    EditorSelectionRoute SelectionRoute,
    TransformPointerRoute PointerRoute,
    EditorWorldDirtyState WorldDirtyState,
    WorldState? WorldState);
