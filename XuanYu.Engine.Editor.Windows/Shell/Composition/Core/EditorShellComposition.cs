namespace FluidWarfare.Editor.Windows.Shell.Composition;

/// <summary>EditorShell 组合根构建器。负责创建上下文、Route、事件接线和初始化。</summary>
static class EditorShellComposition
{
    public static EditorShellContext Build(EditorShell shell)
    {
        var ctx = new EditorShellContext();
        var c = EditorShellControlRefs.Find(shell);
        ctx.InspectorPanel = c.Inspector; ctx.DebugDockPanel = c.DebugDock; ctx.StatusBarPanel = c.StatusBar;
        ctx.ViewportPlaceholderPanel = c.ViewportPlaceholder; ctx.VulkanViewportHostPanel = c.VulkanViewportHost;
        ctx.DockPanel = c.DockPanel; ctx.ToolPalette = c.ToolPalette;

        var r = EditorShellRouteBuild.Build(c, out var lifecycle);
        ctx.Lifecycle = lifecycle;
        ctx.ApplyRouteSet(r);

        ctx.DiagnosticsRoute.SetContext(new(ctx.ProbeRoute, ctx.Feedback, ctx.Lifecycle, ctx.RenderSceneStore, ctx.CameraRoute, ctx.RunMenu,
            () => ctx.VulkanViewportHostPanel?.GetNativeHostInfo() ?? VulkanViewportNativeHostInfo.NotAvailable,
            ctx.VulkanViewportHostPanel, ctx.StatusBarPanel, ctx.SelectionRoute, ctx.PointerRoute, ctx.WorldDirtyState, ctx.WorldState));

        var rt = new EditorShellCompositionRuntime(ctx);
        ctx.LogRoute = new(ctx.Feedback, ctx.DiagnosticsRoute, () => ctx.SessionActive, () => ctx.RenderLastMode);
        ctx.OverlayNavRoute = new(ctx.Lifecycle, ctx.VulkanViewportHostPanel, ctx.NavigationRoute, ctx.CameraRoute, rt.ScheduleFrame);
        ctx.GroundPointerRoute = new(ctx.Lifecycle, ctx.VulkanViewportHostPanel, ctx.GroundHoverRoute, ctx.GroundPointerState,
            ctx.NavigationRoute, ctx.StatusBarPanel, ctx.SelectionRoute, ctx.OverlayNavRoute.ApplyOverlayVisualState, rt.ScheduleFrame);
        ctx.PickingRoute = new(ctx.PickInputRoute, ctx.Lifecycle, ctx.ViewportPickRoute, ctx.RenderSceneStore,
            ctx.SelectionRoute, ctx.GroundPlacementState, ctx.GroundPointerState, ctx.VulkanViewportHostPanel,
            ctx.LogRoute.Info, m => ctx.StatusBarPanel?.SetCurrentSelection(m), ctx.LogRoute.RefreshDiagnostics, rt.CompleteGroundPlacement, rt.ScheduleFrame);
        ctx.TransformRoute = new(ctx.TransformApplyRoute, ctx.SelectionRoute, ctx.GroundPlacementState, ctx.GroundPlacementRoute,
            ctx.InspectorPanel, ctx.StatusBarPanel, () => ctx.WorldState, () => ctx.SessionActive, ctx.Lifecycle,
            () => ctx.CommitApplier, rt.ScheduleFrame, ctx.LogRoute.Info, ctx.LogRoute.Warn);
        ctx.ScrubRoute = new(ctx.TransformApplyRoute, ctx.SelectionRoute, () => ctx.WorldState, () => ctx.CommitApplier, rt.ScheduleFrame, ctx.LogRoute.Info);
        ctx.ViewportRedrawRoute = new(ctx.StartupVulkanRoute, ctx.ProbeRoute, ctx.VulkanViewportHostPanel,
            () => ctx.SessionActive, ctx.Lifecycle, ctx.CameraRoute, ctx.RenderSceneStore, () => ctx.RenderSeq,
            ctx.ResizeRenderRoute, ctx.LogRoute.Info, ctx.LogRoute.Warn, ctx.DiagnosticsRoute, ctx.LogRoute.RefreshDiagnostics,
            () => ctx.StartupProbeRoute.RunStartupVulkanProbe(), v => ctx.SessionActive = v, v => ctx.RenderSeq = v, v => ctx.RenderLastMode = v);
        ctx.StartupProbeRoute = new(ctx.ProbeRoute, ctx.StartupVulkanRoute, ctx.Lifecycle, ctx.RenderSceneStore,
            ctx.VulkanViewportHostPanel, ctx.LogRoute.Info, ctx.LogRoute.Warn, ctx.LogRoute.RefreshDiagnostics,
            () => rt.ApplyScene3dCommandResult_Direct(), ctx.DiagnosticsRoute);
        ctx.ProjectBootstrapRoute = new(ctx.StartupRoute, ctx.PanelApplyRoute, ctx.HierarchyRoute,
            ctx.ViewportSelectionPresenter, ctx.LogRoute.Info, ctx.LogRoute.Warn, ctx.LogRoute.Error,
            v => ctx.ProjectInfo = v, v => ctx.ContentFiles = v, v => ctx.WorldState = v);
        ctx.WindowCommandsRoute = new(ctx.WindowRoute, ctx.LogRoute.Info);
        ctx.HierarchyRoute = new(ctx.DockPanel, () => ctx.ProjectInfo, () => ctx.WorldState, ctx.RenderSceneStore, ctx.LogRoute.Error);
        ctx.SelectionSyncRoute = new(ctx.SelectionRoute, ctx.PanelApplyRoute, () => ctx.WorldState, () => ctx.SessionActive, ctx.Lifecycle, rt.ScheduleFrame);
        ctx.RawInputRoute = new(ctx.ViewportInputRoute, rt.BuildInputRequest);
        ctx.ViewportFrameRoute = new(() => ctx.SessionActive, ctx.Lifecycle, ctx.SelectionRoute, () => ctx.WorldState, ctx.StatusBarPanel, ctx.CameraRoute, rt.ScheduleFrame);
        ctx.Scene3dCmdRoute = new(ctx.Scene3dCommandRoute, () => ctx.ViewportRedrawRoute.StopTimer(),
            () => rt.BuildScene3dCmdRequest(EditorScene3dCommandKind.Run), r2 => rt.ApplyScene3dCommandResult(r2));
        ctx.ViewFocusRoute = new(ctx.ViewportFocusRoute, ctx.PanelApplyRoute, ctx.WorldSelectionPresenter, ctx.RenderSceneStore,
            () => ctx.SessionActive, ctx.Lifecycle, () => ctx.WorldState, ctx.SelectionRoute, ctx.LogRoute.Info, ctx.LogRoute.Warn);

        new EditorShellEventWiring(ctx, ctx.OverlayNavRoute, ctx.GroundPointerRoute, ctx.PickingRoute,
            ctx.TransformRoute, ctx.ScrubRoute, ctx.RawInputRoute, ctx.ViewportRedrawRoute, ctx.ViewFocusRoute,
            ctx.SelectionSyncRoute, ctx.LogRoute).Wire();

        ctx.Feedback.Attach(ctx.DebugDockPanel, ctx.StatusBarPanel, ctx.VulkanViewportHostPanel);
        ctx.Feedback.SetStartupLogs();
        ctx.ProjectBootstrapRoute.LoadSampleProject();
        ctx.StartupProbeRoute.RunStartupVulkanProbe();
        ctx.StartupProbeRoute.ProbeVulkanValidation();
        return ctx;
    }
}
