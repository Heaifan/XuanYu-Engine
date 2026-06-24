using XuanYu.Engine.Editor.Windows.Panels.Viewport;
using XuanYu.Engine.Editor.Windows.Shell.Diagnostics;
using XuanYu.Engine.Editor.Windows.Shell.Feedback;
using XuanYu.Engine.Editor.Windows.Shell.Input;
using XuanYu.Engine.Editor.Windows.Shell.Input.Picking;
using XuanYu.Engine.Editor.Windows.Shell.Input.Transform;
using XuanYu.Engine.Editor.Windows.Shell.Lifecycle;
using XuanYu.Engine.Editor.Windows.Shell.Menu;
using XuanYu.Engine.Editor.Windows.Shell.Panels;
using XuanYu.Engine.Editor.Windows.Shell.Scene3D.Commands;
using XuanYu.Engine.Editor.Windows.Shell.Startup;
using XuanYu.Engine.Editor.Windows.Shell.Startup.Vulkan;
using XuanYu.Engine.Editor.Windows.Shell.Transform;
using XuanYu.Engine.Editor.Windows.Shell.Windows;
using XuanYu.Engine.Editor.Windows.Viewport.Camera;
using XuanYu.Engine.Editor.Windows.Viewport.Navigation;
using XuanYu.Engine.Editor.Windows.Viewport.Picking;
using XuanYu.Engine.Editor.Windows.Viewport.Project;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Diagnostics;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Frame;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Lifecycle;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Resize;
using XuanYu.Engine.Editor.Windows.Viewport.Selection.Focus;
using XuanYu.Engine.Editor.Windows.Viewport.Selection.Route;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Application;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Interaction;
using XuanYu.Engine.Editor.Windows.Viewport.World.Bootstrap;

namespace XuanYu.Engine.Editor.Windows.Shell.Composition;

/// <summary>创建并初始化所有 Route。Shell 在构造期调用 Build(controls) 获得 RouteSet。</summary>
public static class EditorShellRouteBuild
{
    public static EditorShellRouteSet Build(EditorShellControlRefs c, out Scene3dSessionLifecycle lifecycle)
    {
        var probe = new VulkanViewportProbeRoute();
        probe.State.Scene3d = new(0, probe.State.Gate.Message, 0, 0, 0, 0, 0, 0, 0, "无", 0, false, 0, 0, 0, probe.State.Gate.CanRun ? "可用" : "不可用（已隔离）", 0);

        var renderStore = new ViewportRenderSceneStore();
        var selection = new EditorSelectionRoute();
        var projectBootstrap = new ProjectBootstrapRoute();
        var worldBootstrap = new WorldBootstrapRoute();
        var feedback = new EditorFeedbackRoute();
        var runMenu = new EditorRunMenuRoute();
        var startupVulkan = new EditorStartupVulkanRoute();
        var pick = new ViewportPointerPickRoute();
        var camera = new ViewportCameraRoute();
        var navigation = new ViewportNavigationRoute();
        var focus = new ViewportFocusSelectionRoute();
        var resizeRender = new Scene3dResizeRenderRoute();
        var window = new EditorShellWindowRoute();
        var startup = new EditorStartupBootstrapRoute(projectBootstrap, worldBootstrap, renderStore, selection);
        var attach = new EditorShellAttachRoute();
        var detach = new EditorShellDetachRoute();
        var input = new EditorViewportInputRoute();
        var groundHover = new EditorGroundHoverInputRoute();
        var pickInput = new EditorPickInputRoute();
        var scene3dCmd = new EditorScene3dCommandRoute();
        var panelApply = new EditorPanelApplyRoute();
        var transformApply = new EditorTransformApplyRoute();
        var groundPlacement = new EditorGroundPlacementRoute();
        var diagnostics = new EditorDiagnosticsRefreshRoute();
        var pointer = new TransformPointerRoute();

        lifecycle = new Scene3dSessionLifecycle(renderStore);

        panelApply.SetPanels(new(c.Inspector, c.StatusBar, c.ViewportPlaceholder, c.DockPanel));
        if (c.RunMenuButton is not null) runMenu.Attach(c.RunMenuButton);

        return new(selection, projectBootstrap, worldBootstrap, probe, feedback, runMenu, startupVulkan,
            pick, camera, navigation, focus, resizeRender, window, startup, attach, detach, input,
            groundHover, pickInput, scene3dCmd, panelApply, transformApply, groundPlacement, diagnostics,
            renderStore, pointer);
    }
}
