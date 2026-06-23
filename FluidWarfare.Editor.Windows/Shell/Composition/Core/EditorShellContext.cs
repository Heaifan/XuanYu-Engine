namespace FluidWarfare.Editor.Windows.Shell.Composition;

/// <summary>EditorShell 组合根上下文。持有所有控件引用、Route 引用和可变状态。</summary>
sealed class EditorShellContext
{
    // ─── UI 控件引用 ──────────────────────────────────
    public InspectorPanel? InspectorPanel { get; set; }
    public DebugDockPanel? DebugDockPanel { get; set; }
    public StatusBarPanel? StatusBarPanel { get; set; }
    public ViewportPlaceholderPanel? ViewportPlaceholderPanel { get; set; }
    public VulkanViewportHostPanel? VulkanViewportHostPanel { get; set; }
    public ProjectWorldDockPanel? DockPanel { get; set; }
    public ViewportToolPalette? ToolPalette { get; set; }

    // ─── 业务对象 ─────────────────────────────────────
    public IReadOnlyList<GameContentFileInfo>? ContentFiles { get; set; }
    public GameProjectInfo? ProjectInfo { get; set; }
    public WorldState? WorldState { get; set; }

    // ─── Route 引用（来自 Build）────────────────────
    public EditorSelectionRoute SelectionRoute = null!;
    public ProjectBootstrapRoute ProjectBootstrap = null!;
    public WorldBootstrapRoute WorldBootstrap = null!;
    public VulkanViewportProbeRoute ProbeRoute = null!;
    public EditorFeedbackRoute Feedback = null!;
    public EditorRunMenuRoute RunMenu = null!;
    public EditorStartupVulkanRoute StartupVulkanRoute = null!;
    public ViewportPointerPickRoute ViewportPickRoute = null!;
    public ViewportCameraRoute CameraRoute = null!;
    public ViewportNavigationRoute NavigationRoute = null!;
    public ViewportFocusSelectionRoute ViewportFocusRoute = null!;
    public Scene3dResizeRenderRoute ResizeRenderRoute = null!;
    public EditorShellWindowRoute WindowRoute = null!;
    public EditorStartupBootstrapRoute StartupRoute = null!;
    public EditorShellAttachRoute AttachRoute = null!;
    public EditorShellDetachRoute DetachRoute = null!;
    public EditorViewportInputRoute ViewportInputRoute = null!;
    public EditorGroundHoverInputRoute GroundHoverRoute = null!;
    public EditorPickInputRoute PickInputRoute = null!;
    public EditorScene3dCommandRoute Scene3dCommandRoute = null!;
    public EditorPanelApplyRoute PanelApplyRoute = null!;
    public EditorTransformApplyRoute TransformApplyRoute = null!;
    public EditorGroundPlacementRoute GroundPlacementRoute = null!;
    public EditorDiagnosticsRefreshRoute DiagnosticsRoute = null!;
    public TransformPointerRoute PointerRoute = null!;
    public ViewportRenderSceneStore RenderSceneStore = null!;
    public Scene3dSessionLifecycle Lifecycle = null!;

    // ─── Transform Application 层 ──────────────────────
    public EntityTransformPreview? PreviewApplier { get; set; }
    public EntityTransformCommit? CommitApplier { get; set; }
    public EntityTransformCancel? CancelApplier { get; set; }

    // ─── Selection Presentation ────────────────────────
    public WorldEntitySelectionPresenter WorldSelectionPresenter { get; } = new();
    public ProjectContentSelectionPresenter ContentSelectionPresenter { get; } = new();
    public ViewportSelectionPresenter ViewportSelectionPresenter { get; } = new();

    // ─── 可变状态 ──────────────────────────────────────
    public int RenderSeq;
    public string RenderLastMode = "无";
    public bool SessionActive;
    public readonly EditorGroundPointerState GroundPointerState = new();
    public readonly EditorGroundPlacementState GroundPlacementState = new();
    public readonly EditorWorldDirtyState WorldDirtyState = new();
    public EditorShellLogRoute LogRoute = null!;
    public EditorShellOverlayNavigationRoute OverlayNavRoute = null!;
    public EditorShellGroundPointerRoute GroundPointerRoute = null!;
    public EditorShellPickingRoute PickingRoute = null!;
    public EditorShellTransformRoute TransformRoute = null!;
    public EditorShellScrubRoute ScrubRoute = null!;
    public EditorShellViewportRedrawRoute ViewportRedrawRoute = null!;
    public EditorShellStartupVulkanProbeRoute StartupProbeRoute = null!;
    public EditorShellProjectBootstrapRoute ProjectBootstrapRoute = null!;
    public EditorShellWindowCommandsRoute WindowCommandsRoute = null!;
    public EditorShellHierarchyRoute HierarchyRoute = null!;
    public EditorShellSelectionSyncRoute SelectionSyncRoute = null!;
    public EditorShellRawInputRoute RawInputRoute = null!;
    public EditorShellViewportFrameRoute ViewportFrameRoute = null!;
    public EditorShellScene3dCommandRoute Scene3dCmdRoute = null!;
    public EditorShellViewportFocusRoute ViewFocusRoute = null!;

    public void ApplyRouteSet(EditorShellRouteSet r)
    {
        SelectionRoute = r.Selection; ProjectBootstrap = r.ProjectBootstrap; WorldBootstrap = r.WorldBootstrap;
        ProbeRoute = r.Probe; Feedback = r.Feedback; RunMenu = r.RunMenu; StartupVulkanRoute = r.StartupVulkan;
        ViewportPickRoute = r.Pick; CameraRoute = r.Camera; NavigationRoute = r.Navigation;
        ViewportFocusRoute = r.Focus; ResizeRenderRoute = r.ResizeRender; WindowRoute = r.Window;
        StartupRoute = r.Startup; AttachRoute = r.Attach; DetachRoute = r.Detach;
        ViewportInputRoute = r.Input; GroundHoverRoute = r.GroundHover; PickInputRoute = r.PickInput;
        Scene3dCommandRoute = r.Scene3dCommand; PanelApplyRoute = r.PanelApply;
        TransformApplyRoute = r.TransformApply; GroundPlacementRoute = r.GroundPlacement;
        DiagnosticsRoute = r.Diagnostics; RenderSceneStore = r.RenderSceneStore; PointerRoute = r.Pointer;
    }
}
