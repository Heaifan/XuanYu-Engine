using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using FluidWarfare.Bridge.ProjectEngine.World;
using FluidWarfare.Core.Identity;
using FluidWarfare.Core.Logging;
using FluidWarfare.Core.Math;
using FluidWarfare.Editor.ProjectContentTreeModel;
using FluidWarfare.Editor.EntityTransform;
using FluidWarfare.Editor.Selection;
using FluidWarfare.Editor.WorldHierarchy;
using FluidWarfare.Editor.Input.Actions;
using FluidWarfare.Editor.Input.Runtime;
using FluidWarfare.Editor.Transform.Edit;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Frame;
using FluidWarfare.Editor.Windows.Viewport.Picking;
using FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;
using FluidWarfare.Editor.Windows.Viewport.Transform.Drag;
using FluidWarfare.Editor.Windows.Viewport.Transform.Interaction;
using FluidWarfare.Editor.Windows.Viewport.Transform.Application;
using FluidWarfare.Editor.Windows.Viewport.Transform.Presentation;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Submit;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Lifecycle;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Diagnostics;
using FluidWarfare.Editor.Windows.Viewport.Selection.Presentation;
using FluidWarfare.Editor.Windows.Viewport.Selection.Route;
using FluidWarfare.Editor.Windows.Viewport.Project;
using FluidWarfare.Editor.Windows.Viewport.World.Bootstrap;
using FluidWarfare.Editor.Windows.Viewport.Camera;
using FluidWarfare.Editor.Windows.Viewport.Navigation;
using FluidWarfare.Editor.Windows.Viewport.Selection.Focus;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Resize;
using FluidWarfare.Editor.Windows.Shell.Feedback;
using FluidWarfare.Editor.Windows.Shell.Windows;
using FluidWarfare.Editor.Windows.Shell.Startup;
using FluidWarfare.Editor.Windows.Shell.Lifecycle;
using FluidWarfare.Editor.Windows.Shell.Input;
using FluidWarfare.Editor.Windows.Shell.Input.Picking;
using FluidWarfare.Editor.Windows.Shell.Panels;
using FluidWarfare.Editor.Windows.Shell.Scene3D.Commands;
using FluidWarfare.Editor.Windows.Shell.Composition;
using FluidWarfare.Editor.Windows.Shell.Diagnostics;
using FluidWarfare.Editor.Windows.Shell.Transform;
using FluidWarfare.Editor.Windows.Shell.Startup.Vulkan;
using FluidWarfare.Editor.Windows.Shell.Menu;
using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost;
using FluidWarfare.Editor.Windows.Panels.DebugDock;
using FluidWarfare.Editor.Windows.Panels.LeftDock;
using FluidWarfare.Editor.Windows.Panels.Viewport.Input;
using FluidWarfare.Editor.Windows.Panels.Viewport.Tools;
using FluidWarfare.Editor.Windows.Panels.Inspector;
using FluidWarfare.Editor.Windows.Panels.Inspector.Transform;
using FluidWarfare.Editor.Windows.Panels.Logging;
using FluidWarfare.Editor.Windows.Panels.Status;
using FluidWarfare.Editor.Windows.Panels.Viewport;
using FluidWarfare.Engine.World;
using FluidWarfare.Project.Content;
using FluidWarfare.Project.Loading;
using FluidWarfare.Project.Metadata;
using FluidWarfare.Project.Paths;
using FluidWarfare.Project.Validation;
using FluidWarfare.Project.World.Transform;
using FluidWarfare.Render.Scene;
using FluidWarfare.Render.Scene.Position;
using FluidWarfare.Render.Selection;
using FluidWarfare.Render.Selection.Ground;
using FluidWarfare.Render.Selection.Pointer;
using FluidWarfare.Render.Selection.Presented;
using FluidWarfare.Render.Selection.Screen;
using FluidWarfare.Render.Vulkan.Backend;
using FluidWarfare.Render.Vulkan.Device;
using FluidWarfare.Render.Vulkan.Instance;
using FluidWarfare.Render.Vulkan.Clear;
using FluidWarfare.Render.Vulkan.Camera;
using FluidWarfare.Render.Camera.Navigation;
using FluidWarfare.Render.ViewportNavigation;
using FluidWarfare.Render.Vulkan.Scene3D;
using FluidWarfare.Render.Vulkan.Scene3D.Session;
using FluidWarfare.Render.Vulkan.Surface;
using FluidWarfare.Render.Vulkan.Validation;
using FluidWarfare.Render.Vulkan.Swapchain;
using FluidWarfare.Render.World;
using FluidWarfare.Editor.Windows.Shell.Navigation;
using FluidWarfare.Editor.Windows.Shell.Picking;
using FluidWarfare.Editor.Windows.Shell.Transform.Edit;
using FluidWarfare.Editor.Windows.Shell.Viewport;
using FluidWarfare.Editor.Windows.Shell.Commands;
using FluidWarfare.Editor.Windows.Shell.Hierarchy;
using FluidWarfare.Editor.Windows.Shell.Project;
using FluidWarfare.Editor.Windows.Shell.Selection;

namespace FluidWarfare.Editor.Windows.Shell;

public sealed partial class EditorShell : UserControl
{
    private InspectorPanel? _inspectorPanel;
    private DebugDockPanel? _debugDockPanel;
    private StatusBarPanel? _statusBarPanel;
    private ViewportPlaceholderPanel? _viewportPlaceholderPanel;
    private VulkanViewportHostPanel? _vulkanViewportHostPanel;
    private ProjectWorldDockPanel? _dockPanel;
    private IReadOnlyList<GameContentFileInfo>? _contentFiles;
    private GameProjectInfo? _projectInfo;
    private WorldState? _worldState;
    private readonly EditorSelectionRoute _selectionRoute = new();
    private readonly ProjectBootstrapRoute _projectBootstrap = new();
    private readonly WorldBootstrapRoute _worldBootstrap = new();
    private readonly VulkanViewportProbeRoute _probeRoute = new();
    private readonly EditorFeedbackRoute _feedback = new();
    private readonly EditorRunMenuRoute _runMenu = new();
    private readonly EditorStartupVulkanRoute _startupVulkanRoute = new();
    private int _renderSeq;
    private string _renderLastMode = "无";
    private Scene3dSessionLifecycle _lifecycle = null!;
    private readonly ViewportPointerPickRoute _viewportPickRoute = new();
    private readonly ViewportCameraRoute _cameraRoute = new();
    private bool _sessionActive;
    private readonly ViewportNavigationRoute _navigationRoute = new();
    private readonly ViewportFocusSelectionRoute _viewportFocusRoute = new();
    private readonly Scene3dResizeRenderRoute _resizeRenderRoute = new();
    private readonly EditorShellWindowRoute _windowRoute = new();
    private readonly EditorStartupBootstrapRoute _startupRoute;
    private readonly EditorShellAttachRoute _attachRoute = new();
    private readonly EditorShellDetachRoute _detachRoute = new();
    private readonly EditorViewportInputRoute _viewportInputRoute = new();
    private readonly EditorGroundHoverInputRoute _groundHoverRoute = new();
    private readonly EditorPickInputRoute _pickInputRoute = new();
    private readonly EditorScene3dCommandRoute _scene3dCommandRoute = new();
    private readonly EditorPanelApplyRoute _panelApplyRoute = new();
    private readonly EditorTransformApplyRoute _transformApplyRoute = new();
    private readonly EditorGroundPlacementRoute _groundPlacementRoute = new();
    private readonly EditorDiagnosticsRefreshRoute _diagnosticsRoute = new();
    private EditorShellRouteSet _r = null!;
    private EditorShellControlRefs _c = null!;

    // ─── 视口编辑工具 ────────────────────────────────────
    private ViewportToolPalette? _viewportToolPalette;

    // ─── Transform 路由 ────────────────────────────────────
    private readonly TransformPointerRoute _pointerRoute = new();
    // _interactState via _pointerRoute.State

    // ─── Transform Application 层 ───────────────────────────
    private readonly ViewportRenderSceneStore _renderSceneStore = new();
    private EntityTransformPreview? _previewApplier;
    private EntityTransformCommit? _commitApplier;
    private EntityTransformCancel? _cancelApplier;

    // ─── Selection Presentation ──────────────────────────────
    private readonly WorldEntitySelectionPresenter _worldSelectionPresenter = new();
    private readonly ProjectContentSelectionPresenter _contentSelectionPresenter = new();
    private readonly ViewportSelectionPresenter _viewportSelectionPresenter = new();

    // ─── 动作去重守卫 ──────────────────────────────────────
    private bool _frameSelectedPending;

    // ─── 输入上下文栈（由 InputRoute 管理后暂未外移） ──────


    // ─── 地面拾取状态 ─────────────────────────────────────────────
    private readonly FluidWarfare.Editor.ViewportGround.EditorGroundPointerState _groundPointerState = new();
    // (调度合并 moved to EditorShellGroundPointerRoute)

    // ─── Transform 编辑状态 ─────────────────────────────────────────
    private readonly EditorGroundPlacementState _groundPlacementState = new();
    private readonly EditorWorldDirtyState _worldDirtyState = new();

    // ─── H-2A 提取路由 ──────────────────────────────────────────
    private readonly EditorShellOverlayNavigationRoute _overlayNavRoute;
    private readonly EditorShellGroundPointerRoute _groundPointerRoute;
    private readonly EditorShellPickingRoute _pickingRoute;

    // ─── H-2B 提取路由 ──────────────────────────────────────────
    private readonly EditorShellTransformRoute _transformRoute;
    private readonly EditorShellScrubRoute _scrubRoute;

    // ─── H-2C 提取路由 ──────────────────────────────────────────
    private readonly EditorShellViewportRedrawRoute _viewportRedrawRoute;

    // ─── H-2D 提取路由 ──────────────────────────────────────────
    private readonly EditorShellWindowCommandsRoute _windowCommandsRoute;

    // ─── H-2E 提取路由 ──────────────────────────────────────────
    private readonly EditorShellHierarchyRoute _hierarchyRoute;
    private readonly EditorShellSelectionSyncRoute _selectionSyncRoute;

    // ─── H-2F 提取路由 ──────────────────────────────────────────
    private readonly EditorShellStartupVulkanProbeRoute _startupProbeRoute;

    // ─── H-2G 提取路由 ──────────────────────────────────────────
    private readonly EditorShellProjectBootstrapRoute _projectBootstrapRoute;

    public EditorShell()
    {
        AvaloniaXamlLoader.Load(this);
        _c = EditorShellControlRefs.Find(this);
        _inspectorPanel = _c.Inspector; _debugDockPanel = _c.DebugDock; _statusBarPanel = _c.StatusBar;
        _viewportPlaceholderPanel = _c.ViewportPlaceholder; _vulkanViewportHostPanel = _c.VulkanViewportHost;
        _dockPanel = _c.DockPanel; _viewportToolPalette = _c.ToolPalette;
        _r = EditorShellRouteBuild.Build(_c, out _lifecycle);
        // Apply route fields from composition
        _selectionRoute = _r.Selection; _projectBootstrap = _r.ProjectBootstrap; _worldBootstrap = _r.WorldBootstrap;
        _probeRoute = _r.Probe; _feedback = _r.Feedback; _runMenu = _r.RunMenu; _startupVulkanRoute = _r.StartupVulkan;
        _viewportPickRoute = _r.Pick; _cameraRoute = _r.Camera; _navigationRoute = _r.Navigation;
        _viewportFocusRoute = _r.Focus; _resizeRenderRoute = _r.ResizeRender; _windowRoute = _r.Window;
        _startupRoute = _r.Startup; _attachRoute = _r.Attach; _detachRoute = _r.Detach;
        _viewportInputRoute = _r.Input; _groundHoverRoute = _r.GroundHover; _pickInputRoute = _r.PickInput;
        _scene3dCommandRoute = _r.Scene3dCommand; _panelApplyRoute = _r.PanelApply;
        _transformApplyRoute = _r.TransformApply; _groundPlacementRoute = _r.GroundPlacement;
        _diagnosticsRoute = _r.Diagnostics; _renderSceneStore = _r.RenderSceneStore; _pointerRoute = _r.Pointer;

        _diagnosticsRoute.SetContext(new(_probeRoute, _feedback, _lifecycle, _renderSceneStore, _cameraRoute, _runMenu,
            () => _vulkanViewportHostPanel?.GetNativeHostInfo() ?? VulkanViewportNativeHostInfo.NotAvailable,
            _vulkanViewportHostPanel, _statusBarPanel, _selectionRoute, _pointerRoute, _worldDirtyState, _worldState));

        _overlayNavRoute = new EditorShellOverlayNavigationRoute(
            _lifecycle, _vulkanViewportHostPanel, _navigationRoute, _cameraRoute,
            ScheduleScene3dFrame);
        _groundPointerRoute = new EditorShellGroundPointerRoute(
            _lifecycle, _vulkanViewportHostPanel, _groundHoverRoute, _groundPointerState,
            _navigationRoute, _statusBarPanel, _selectionRoute,
            _overlayNavRoute.ApplyOverlayVisualState,
            ScheduleScene3dFrame);
        _pickingRoute = new EditorShellPickingRoute(
            _pickInputRoute, _lifecycle, _viewportPickRoute, _renderSceneStore,
            _selectionRoute, _groundPlacementState, _groundPointerState,
            _vulkanViewportHostPanel, AppendInfoLog,
            msg => _statusBarPanel?.SetCurrentSelection(msg),
            RefreshDiagnostics, CompleteGroundPlacement, ScheduleScene3dFrame);

        _transformRoute = new EditorShellTransformRoute(
            _transformApplyRoute, _selectionRoute, _groundPlacementState, _groundPlacementRoute,
            _inspectorPanel, _statusBarPanel,
            () => _worldState, () => _sessionActive, _lifecycle,
            () => _commitApplier,
            ScheduleScene3dFrame, AppendInfoLog, AppendWarningLog);
        _scrubRoute = new EditorShellScrubRoute(
            _transformApplyRoute, _selectionRoute,
            () => _worldState, () => _commitApplier,
            ScheduleScene3dFrame, AppendInfoLog);

        _viewportRedrawRoute = new EditorShellViewportRedrawRoute(
            _startupVulkanRoute, _probeRoute, _vulkanViewportHostPanel,
            () => _sessionActive, _lifecycle, _cameraRoute, _renderSceneStore,
            () => _renderSeq, _resizeRenderRoute, AppendInfoLog, AppendWarningLog,
            _diagnosticsRoute, RefreshDiagnostics,
            () => _startupProbeRoute.RunStartupVulkanProbe(),
            v => _sessionActive = v,
            v => _renderSeq = v,
            v => _renderLastMode = v);

        _startupProbeRoute = new EditorShellStartupVulkanProbeRoute(
            _probeRoute, _startupVulkanRoute, _lifecycle, _renderSceneStore,
            _vulkanViewportHostPanel, AppendInfoLog, AppendWarningLog,
            RefreshDiagnostics,
            () => ApplyScene3dCommandResult(_scene3dCommandRoute.Execute(
                BuildScene3dCommandRequest(EditorScene3dCommandKind.Restart))),
            _diagnosticsRoute);

        _projectBootstrapRoute = new EditorShellProjectBootstrapRoute(
            _startupRoute, _panelApplyRoute, _hierarchyRoute,
            _viewportSelectionPresenter, AppendInfoLog, AppendWarningLog, AppendErrorLog,
            v => _projectInfo = v,
            v => _contentFiles = v,
            v => _worldState = v);

        _windowCommandsRoute = new EditorShellWindowCommandsRoute(
            _windowRoute, AppendInfoLog);

        _hierarchyRoute = new EditorShellHierarchyRoute(
            _dockPanel, () => _projectInfo, () => _worldState,
            _renderSceneStore, AppendErrorLog);
        _selectionSyncRoute = new EditorShellSelectionSyncRoute(
            _selectionRoute, _panelApplyRoute,
            () => _worldState, () => _sessionActive, _lifecycle,
            ScheduleScene3dFrame);

        SubscribePanelEvents();
        InitializeFeedback();
        _projectBootstrapRoute.LoadSampleProject();
        _startupProbeRoute.RunStartupVulkanProbe();
        _startupProbeRoute.ProbeVulkanValidation();
    }

    private void SubscribePanelEvents()
    {
        if (_c.ViewportPlaceholder is not null)
            _c.ViewportPlaceholder.ViewportFocused += HandleViewportFocused;
        if (_c.DockPanel is not null)
        {
            _c.DockPanel.EntitySelectionRequested += OnHierarchyEntitySelected;
            _c.DockPanel.ContentSelectionRequested += OnProjectContentSelected;
        }
        if (_c.VulkanViewportHost is not null)
        {
            _c.VulkanViewportHost.NativeHostInfoChanged += _viewportRedrawRoute.HandleNativeHostInfoChanged;
            _c.VulkanViewportHost.RawPointerButtonDown += HandleRawPointerButtonDown;
            _c.VulkanViewportHost.RawPointerButtonUp += HandleRawPointerButtonUp;
            _c.VulkanViewportHost.RawPointerMoved += HandleRawPointerMoved;
            _c.VulkanViewportHost.RawKeyDown += HandleRawKeyDown;
            _c.VulkanViewportHost.RawKeyUp += HandleRawKeyUp;
            _c.VulkanViewportHost.RawMouseWheel += HandleRawMouseWheel;
            _c.VulkanViewportHost.RawInputFocusLost += HandleRawInputFocusLost;
            _c.VulkanViewportHost.PickRequested += HandleViewportPick;
            _c.VulkanViewportHost.NavigationPointerPressed += _overlayNavRoute.HandleOverlayPointerPressed;
            _c.VulkanViewportHost.NavigationPointerMoved += _overlayNavRoute.HandleOverlayPointerMoved;
            _c.VulkanViewportHost.NavigationPointerReleased += _overlayNavRoute.HandleOverlayPointerReleased;
            _c.VulkanViewportHost.NavigationCaptureLost += _overlayNavRoute.HandleOverlayCaptureLost;
            _c.VulkanViewportHost.SceneToolPointerPressed += HandleSceneToolPointerPressed;
            _c.VulkanViewportHost.SceneToolPointerReleased += HandleSceneToolPointerReleased;
            _c.VulkanViewportHost.PointerMoved += _groundPointerRoute.HandleViewportPointerMoved;
            _c.VulkanViewportHost.PointerLeft += _groundPointerRoute.HandleViewportPointerLeft;
        }
        if (_c.Inspector is not null)
        {
            _c.Inspector.TransformDraftChanged += _transformRoute.HandleTransformDraftChanged;
            _c.Inspector.TransformApplyRequested += _transformRoute.HandleTransformApply;
            _c.Inspector.TransformResetRequested += _transformRoute.HandleTransformReset;
            _c.Inspector.GroundPlacementRequested += _transformRoute.HandleGroundPlacementToggle;
            _c.Inspector.ScrubValueChanged += _scrubRoute.HandleScrubValueChanged;
            _c.Inspector.ScrubCompleted += _scrubRoute.HandleScrubCompleted;
            _c.Inspector.ScrubCancelled += _scrubRoute.HandleScrubCancelled;
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        ApplyAttachResult(_attachRoute.Attach(BuildAttachRequest()));
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        ApplyDetachResult(_detachRoute.Detach(BuildDetachRequest()));
        base.OnDetachedFromVisualTree(e);
    }

    private EditorShellAttachRequest BuildAttachRequest() => new(
        NativeHostReportAction: _startupProbeRoute.RunAttachedProbes,
        InputPipelineInitAction: InitializeInputPipeline);

    private static void ApplyAttachResult(EditorShellAttachResult result)
    {
        // 路由已通过 Dispatcher.UIThread.Post 调度 NativeHostReport + InputPipelineInit。
        // 后续附加不再重复调度（AttachRoute 内含 _dispatched 守卫）。
    }

    private EditorShellDetachRequest BuildDetachRequest() => new(
        Lifecycle: _lifecycle,
        ResizeRenderTimer: _viewportRedrawRoute.Timer,
        ResizeRenderTimerTickHandler: _viewportRedrawRoute.TimerTickHandler);

    private void ApplyDetachResult(EditorShellDetachResult result)
    {
        _sessionActive = false;
        _startupVulkanRoute.Reset();
        if (result.TimerCleanedUp) _viewportRedrawRoute.ClearTimer();
    }

    // ─── Viewport 重绘（委托至 _viewportRedrawRoute）───

    private void InitializeFeedback()
    {
        _feedback.Attach(_debugDockPanel, _statusBarPanel, _vulkanViewportHostPanel);
        _feedback.SetStartupLogs();
    }

    private void HandleViewportFocused(object? sender, EventArgs e)
    {
        var r = _viewportFocusRoute.Focus(_worldState, _selectionRoute);
        _panelApplyRoute.ShowViewportFocused(r.InspectorSelection, r.StatusBarText, r.ShowEmptyWorld);
        foreach (var m in r.LogMessages) AppendInfoLog(m);
        foreach (var w in r.LogWarnings) AppendWarningLog(w);
        if (r.EntityToShow is not null) ShowWorldEntitySelection(r.EntityToShow);
    }

    // ─── 窗口菜单命令（委托至 _windowCommandsRoute）───

    private void OnHierarchyEntitySelected(string? entityId) =>
        _selectionSyncRoute.ApplyEntitySelection(entityId, EditorEntitySelectionOrigin.WorldHierarchy, ShowWorldEntitySelection);

    private void OnProjectContentSelected(string? relativePath)
    {
        var r = _contentSelectionPresenter.Present(relativePath, _contentFiles);
        _panelApplyRoute.ApplyProjectContentSelection(r.InspectorSelection, r.StatusBarSelection, r.LogMessage, AppendInfoLog);
    }

    private void ShowWorldEntitySelection(WorldEntityInfo entityInfo)
    {
        var is3d = _sessionActive && _lifecycle.State.Session?.Status == VulkanScene3dSessionStatus.Active;
        var r = _worldSelectionPresenter.Present(entityInfo, _worldState, _renderSceneStore.Current, is3d);
        _panelApplyRoute.ApplyEntitySelection(r.InspectorSelection, r.InspectorEntityId, r.EntityPosition,
            r.EntitySourcePath, r.GroundPlaceEnabled, r.StatusBarSelection,
            r.ViewportSummary, r.LogMessage, AppendInfoLog);
    }

    private void AppendInfoLog(string message) => _feedback.Info(message);
    private void AppendWarningLog(string message) => _feedback.Warn(message);
    private void AppendErrorLog(string message) => _feedback.Error(message);

    // ─── 项目加载 + World Bootstrap（委托至 _projectBootstrapRoute）───

    private void HandleScene3dRunRequested(object? sender, EventArgs e)
    {
        _viewportRedrawRoute.StopTimer();
        ApplyScene3dCommandResult(_scene3dCommandRoute.Execute(BuildScene3dCommandRequest(EditorScene3dCommandKind.Run)));
    }

    private void HandleRestartScene3d()
    {
        ApplyScene3dCommandResult(_scene3dCommandRoute.Execute(BuildScene3dCommandRequest(EditorScene3dCommandKind.Restart)));
    }

    private void InitializeInputPipeline()
    {
        EditorInputService.Instance.Initialize();
        _viewportInputRoute.State.Translator = new WindowsViewportInputTranslator(EditorInputService.Instance.CurrentSnapshot);
        EditorInputService.Instance.SnapshotReplaced += snapshot =>
        {
            _viewportInputRoute.State.Translator?.OnSnapshotReplaced(snapshot);
        };
    }

    private void HandleRawKeyDown(int virtualKeyCode)
    {
        _viewportInputRoute.HandleKeyDown(BuildInputRequest(EditorViewportInputKind.KeyDown, keyCode: virtualKeyCode));
    }

    private void HandleRawKeyUp(int virtualKeyCode)
    {
        _viewportInputRoute.HandleKeyUp(BuildInputRequest(EditorViewportInputKind.KeyUp, keyCode: virtualKeyCode));
    }

    private void HandleRawPointerButtonDown(int buttonCode, int x, int y)
    {
        _viewportInputRoute.HandlePointerDown(BuildInputRequest(EditorViewportInputKind.PointerDown, buttonCode: buttonCode, x: x, y: y));
    }

    private void HandleRawPointerMoved(int x, int y)
    {
        _viewportInputRoute.HandlePointerMoved(BuildInputRequest(EditorViewportInputKind.PointerMoved, x: x, y: y));
    }

    private void HandleRawPointerButtonUp(int buttonCode, int x, int y)
    {
        _viewportInputRoute.HandlePointerUp(BuildInputRequest(EditorViewportInputKind.PointerUp, buttonCode: buttonCode, x: x, y: y));
    }

    private void HandleRawInputFocusLost()
    {
        _viewportInputRoute.HandleFocusLost(BuildInputRequest(EditorViewportInputKind.FocusLost));
    }

    // ─── 场景工具仲裁 ──────────────────────────────────

    private ViewportSceneToolPressResult HandleSceneToolPointerPressed(int x, int y)
    {
        return _viewportInputRoute.HandleSceneToolPressed(BuildInputRequest(EditorViewportInputKind.PointerDown, x: x, y: y));
    }

    private void HandleSceneToolPointerReleased(int x, int y)
    {
        _viewportInputRoute.HandleSceneToolReleased(BuildInputRequest(EditorViewportInputKind.PointerUp, x: x, y: y));
    }

    /// <summary>初始化 Transform Application 层（Session 启动后调用）。</summary>
    private void InitTransformApplication()
    {
        if (_lifecycle.State.Session is null) return;
        var vulkan = new Scene3dEntityPositionWriter(_lifecycle.State.Session);
        var inspect = new InspectorTransformDisplay(_inspectorPanel);
        _previewApplier = new EntityTransformPreview(_renderSceneStore, vulkan, inspect);
        _cancelApplier = new EntityTransformCancel(_renderSceneStore, vulkan, inspect);
        if (_worldState is not null)
        {
            var world = new WorldTransformWriter(_worldState, _worldDirtyState, _statusBarPanel);
            _commitApplier = new EntityTransformCommit(world, _renderSceneStore, vulkan, inspect);
        }
    }

    /// <summary>从当前 Shell 状态构建 TransformStartSnapshot。返回 null 当缺实体或相机快照。</summary>
    private TransformStartSnapshot? BuildTransformStartSnapshot()
    {
        if (_selectionRoute.State.SelectedWorldEntity is null) return null;
        var pos = _worldState?.FindPosition(_selectionRoute.State.SelectedWorldEntity.EntityId);
        if (pos is null) return null;
        var cam = _lifecycle.State.Session?.LastPresentedSnapshot;
        if (cam is not { IsValid: true }) return null;
        return new TransformStartSnapshot(
            _selectionRoute.State.SelectedWorldEntity.EntityId,
            new SceneTransform(pos.Value.Value, default, default),
            _worldDirtyState.IsDirty,
            cam,
            _lifecycle.State.FrameRoute?.Snapshots.PresentedGizmo ?? PresentedMoveGizmoSnapshot.None);
    }

    private void ApplyPreviewPosition()
    {
        _transformApplyRoute.Preview(_selectionRoute, _previewApplier, _pointerRoute, ScheduleScene3dFrame);
    }

    private void CancelActiveTransform(TransformInteractionResult r)
    {
        _transformApplyRoute.Cancel(r, _selectionRoute, _cancelApplier, ScheduleScene3dFrame);
    }

    // ─── 视口工具 ──────────────────────────────────────

    private void HandleViewportToolChanged(ViewportEditorTool tool)
    {
        _pointerRoute.ActivateMoveTool(tool == ViewportEditorTool.Move);
        if (tool == ViewportEditorTool.Move && _selectionRoute.State.SelectedWorldEntity is null)
            _statusBarPanel?.SetCurrentSelection("请先选择实体。");
    }

    private void HandleRawMouseWheel(int delta, int packedModifiers)
    {
        _viewportInputRoute.HandleMouseWheel(BuildInputRequest(EditorViewportInputKind.MouseWheel, wheelDelta: delta));
    }

    private void ExecuteViewportFrameSelected()
    {
        if (_frameSelectedPending) return;
        _frameSelectedPending = true;
        try
        {
            if (!_sessionActive || _lifecycle.State.Session is null) return;
            if (_selectionRoute.State.SelectedWorldEntity is null)
            {
                _statusBarPanel?.SetCurrentSelection("没有可聚焦的世界实体。");
                return;
            }
            var target = ViewportCameraFocusTarget.Compute(
                _selectionRoute.State.SelectedWorldEntity.EntityId, _worldState!);
            if (target is null) return;
            var (cx, cy, cz, r) = target.Value;
            var result = _cameraRoute.Apply(new ViewportCameraCommand.FrameSelected(cx, cy, cz, r));
            _statusBarPanel?.SetCurrentSelection($"已聚焦实体 {_selectionRoute.State.SelectedWorldEntity.DisplayName}。");
            if (result.NeedsFrame) ScheduleScene3dFrame(result.Reason);
        }
        finally { Dispatcher.UIThread.Post(() => _frameSelectedPending = false); }
    }

    private EditorViewportInputRequest BuildInputRequest(
        EditorViewportInputKind kind, int keyCode = 0, int buttonCode = 0, int x = 0, int y = 0, int wheelDelta = 0)
    {
        return new EditorViewportInputRequest(kind, keyCode, buttonCode, x, y, wheelDelta,
            _viewportInputRoute.State, _pointerRoute, _selectionRoute, _viewportToolPalette,
            _cameraRoute, _lifecycle, _viewportPickRoute, _renderSceneStore,
            _groundPlacementState, _worldDirtyState,
            AppendInfoLog, AppendWarningLog, ScheduleScene3dFrame, BuildTransformStartSnapshot,
            ApplyEntityTransform, CancelActiveTransform, ApplyPreviewPosition, ExecuteViewportFrameSelected);
    }

    private EditorScene3dCommandRequest BuildScene3dCommandRequest(EditorScene3dCommandKind kind) => new(kind,
        _probeRoute, _lifecycle, _renderSceneStore,
        _vulkanViewportHostPanel?.GetNativeHostInfo() ?? VulkanViewportNativeHostInfo.NotAvailable,
        _cameraRoute, _renderSeq, AppendInfoLog, AppendWarningLog);

    private void ApplyScene3dCommandResult(EditorScene3dCommandResult r)
    {
        if (r.SessionStarted) { _sessionActive = true; _renderLastMode = "Scene3D"; _renderSeq = r.NewRenderSeq; InitTransformApplication(); }
        if (!r.SessionStarted && r.NeedsTransformInit) _sessionActive = false;
        if (r.NeedsDiagnosticsRefresh) RefreshDiagnostics();
        if (r.NewRenderSeq > _renderSeq) _renderSeq = r.NewRenderSeq;
    }

    private void ExecuteCancelCurrentTool()
    {
        HandleViewportEscape();
    }

    private void ExecuteTransformApply()
    {
        // 应用 Transform 需要由 Inspector 面板提供当前草稿值，此处不自动执行
    }

    private void ExecuteTransformResetDraft()
    {
        // 重置 Transform 草稿（通过面板的 Reset 事件）
        _transformRoute.HandleTransformReset();
    }

    // ─── Overlay 导航输入（委托至 _overlayNavRoute）───

    private void HandleViewportEscape()
    {
        if (_groundPlacementState.IsActive)
        {
            _groundPlacementState.Cancel();
            _inspectorPanel?.SetPlacementMode(false);
            AppendInfoLog("放置模式已取消。");
        }
    }

    private void ScheduleScene3dFrame(VulkanScene3dFrameReason reason)
    {
        _diagnosticsRoute.ScheduleFrame(reason, _renderSeq, _selectionRoute, _worldState,
            () => { _renderSeq = _lifecycle.State.FrameSubmitRoute?.RenderSeq ?? _renderSeq; RefreshDiagnostics(); });
    }

    // ─── 选择同步（委托至 _selectionSyncRoute）───

    // ─── Picking（委托至 _pickingRoute）───

    /// <summary>视口点击 Picking 处理。委托至 _pickingRoute。</summary>
    private void HandleViewportPick(int pixelX, int pixelY) =>
        _pickingRoute.HandleViewportPick(pixelX, pixelY,
            (id, or) => _selectionSyncRoute.ApplyEntitySelection(id, or, ShowWorldEntitySelection));

    // ─── Transform 编辑 + Scrub（委托至 _transformRoute / _scrubRoute）───

    /// <summary>Transform 编辑转发。委托至 _transformRoute。</summary>
    private void ApplyEntityTransform(SceneTransform transform, EditorEntityTransformOrigin origin) =>
        _transformRoute.ApplyEntityTransform(transform, origin);

    private void CompleteGroundPlacement(Vector3d groundPosition)
    {
        var r = _groundPlacementRoute.Complete(groundPosition, _selectionRoute, _groundPlacementState,
            _worldState, _commitApplier, ScheduleScene3dFrame, _inspectorPanel, AppendInfoLog);
        if (r.Completed) _pickingRoute.HideGroundCursor();
    }

    // ─── 层级树构建（委托至 _hierarchyRoute）───



    private void RefreshDiagnostics() => _diagnosticsRoute.Refresh(_sessionActive, _renderLastMode);


    private static bool TryGetValidViewportSize(
        VulkanViewportNativeHostInfo nativeHostInfo,
        out uint width,
        out uint height,
        out string message)
    {
        width = 0;
        height = 0;

        if (nativeHostInfo.Width < 1 || nativeHostInfo.Height < 1)
        {
            message = "Vulkan 视口尺寸尚未就绪，跳过本次绘制。";
            return false;
        }

        width = checked((uint)nativeHostInfo.Width);
        height = checked((uint)nativeHostInfo.Height);
        message = string.Empty;
        return true;
    }

    private void UpdateVulkanViewportHost() => _diagnosticsRoute.UpdateViewportHost();
}
