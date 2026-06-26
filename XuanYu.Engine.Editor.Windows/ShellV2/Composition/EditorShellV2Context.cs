using Avalonia.Threading;
using XuanYu.Engine.Editor.Windows.Panels.Viewport;
using XuanYu.Engine.Editor.Windows.Viewport.Camera;
using XuanYu.Engine.Editor.Windows.Viewport.Navigation;
using XuanYu.Engine.Editor.Windows.Viewport.Picking;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Diagnostics;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Lifecycle;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Resize;
using XuanYu.Engine.Editor.Windows.Viewport.Selection.Focus;
using XuanYu.Engine.Editor.Windows.Viewport.Selection.Route;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Interaction;
using XuanYu.Engine.Editor.Windows.Shell.Input;
using XuanYu.Engine.Editor.Windows.Shell.Input.Transform;
using XuanYu.Engine.Editor.Windows.Shell.Input.Picking;
using XuanYu.Engine.Editor.Windows.Shell.Startup.Vulkan;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Application;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;

namespace XuanYu.Engine.Editor.Windows.ShellV2.Composition;

/// <summary>EditorShellV2 上下文。只持有 V2 需要的路由和控件引用。</summary>
sealed class EditorShellV2Context
{
    // ─── 控件引用 ────────────────────────────────────────
    public VulkanViewportHostPanel? ViewportPanel { get; set; }

    // ─── 核心路由 ├───────────────────────────────────────
    public VulkanViewportProbeRoute ProbeRoute = null!;
    public EditorStartupVulkanRoute StartupVulkan = null!;
    public Scene3dSessionLifecycle Lifecycle = null!;
    public ViewportRenderSceneStore RenderStore = null!;
    public EditorDiagnosticsRefreshRoute Diagnostics = null!;
    public Scene3dResizeRenderRoute ResizeRender = null!;

    // ─── 输入路由 ────────────────────────────────────────
    public EditorSelectionRoute Selection = null!;
    public TransformPointerRoute Pointer = null!;
    public EditorSceneToolInputRoute SceneToolInput = null!;
    public EditorTransformApplyRoute TransformApply = null!;
    public ViewportCameraRoute Camera = null!;
    public ViewportNavigationRoute Navigation = null!;
    public ViewportFocusSelectionRoute Focus = null!;
    public ViewportPointerPickRoute Pick = null!;
    public EditorPickInputRoute PickInput = null!;
    public EditorGroundHoverInputRoute GroundHover = null!;
    public EditorGroundPlacementRoute GroundPlacement = null!;
    public EditorViewportInputRoute ViewportInput = null!;

    // ─── 框架辅助 ────────────────────────────────────────
    public DispatcherTimer? RenderTimer;
    public int RenderSeq;
    public string RenderLastMode = "无";
    public bool SessionActive;
    public Action<VulkanScene3dFrameReason>? FrameScheduler;

    public void Shutdown()
    {
        RenderTimer?.Stop();
        RenderTimer = null;
    }
}
