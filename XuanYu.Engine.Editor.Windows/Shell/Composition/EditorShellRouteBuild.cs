using FluidWarfare.Editor.Windows.Panels.Viewport;
using FluidWarfare.Editor.Windows.Shell.Diagnostics;
using FluidWarfare.Editor.Windows.Shell.Feedback;
using FluidWarfare.Editor.Windows.Shell.Input;
using FluidWarfare.Editor.Windows.Shell.Input.Picking;
using FluidWarfare.Editor.Windows.Shell.Input.Transform;
using FluidWarfare.Editor.Windows.Shell.Lifecycle;
using FluidWarfare.Editor.Windows.Shell.Menu;
using FluidWarfare.Editor.Windows.Shell.Panels;
using FluidWarfare.Editor.Windows.Shell.Scene3D.Commands;
using FluidWarfare.Editor.Windows.Shell.Startup;
using FluidWarfare.Editor.Windows.Shell.Startup.Vulkan;
using FluidWarfare.Editor.Windows.Shell.Transform;
using FluidWarfare.Editor.Windows.Shell.Windows;
using FluidWarfare.Editor.Windows.Viewport.Camera;
using FluidWarfare.Editor.Windows.Viewport.Navigation;
using FluidWarfare.Editor.Windows.Viewport.Picking;
using FluidWarfare.Editor.Windows.Viewport.Project;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Diagnostics;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Frame;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Lifecycle;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Resize;
using FluidWarfare.Editor.Windows.Viewport.Selection.Focus;
using FluidWarfare.Editor.Windows.Viewport.Selection.Route;
using FluidWarfare.Editor.Windows.Viewport.Transform.Application;
using FluidWarfare.Editor.Windows.Viewport.Transform.Interaction;
using FluidWarfare.Editor.Windows.Viewport.World.Bootstrap;

namespace FluidWarfare.Editor.Windows.Shell.Composition;

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
