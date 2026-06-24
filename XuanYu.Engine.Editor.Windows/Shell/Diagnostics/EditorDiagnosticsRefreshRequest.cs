using XuanYu.Engine.Editor.EntityTransform;
using XuanYu.Engine.Editor.Windows.Panels.Status;
using XuanYu.Engine.Editor.Windows.Panels.Viewport;
using XuanYu.Engine.Editor.Windows.Shell.Feedback;
using XuanYu.Engine.Editor.Windows.Shell.Menu;
using XuanYu.Engine.Editor.Windows.Viewport.Camera;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Diagnostics;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Frame;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Lifecycle;
using XuanYu.Engine.Editor.Windows.Viewport.Selection.Route;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Application;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Interaction;
using XuanYu.Engine.World;

namespace XuanYu.Engine.Editor.Windows.Shell.Diagnostics;

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
