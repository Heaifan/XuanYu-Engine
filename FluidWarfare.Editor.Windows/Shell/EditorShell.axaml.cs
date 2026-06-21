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
    private DispatcherTimer? _viewportResizeRenderTimer;
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
    private bool _groundPointerUpdatePending; // 调度合并
    private long _lastGroundPointerUpdateTicks;

    // ─── Transform 编辑状态 ─────────────────────────────────────────
    private readonly EditorGroundPlacementState _groundPlacementState = new();
    private readonly EditorWorldDirtyState _worldDirtyState = new();

    public EditorShell()
    {
        // _probeRoute.State.Gate 由字段初始化器在构造函数体之前执行
        _probeRoute.State.Scene3d = new VulkanScene3dInfo(
            VulkanScene3dStatus.NotChecked,
            _probeRoute.State.Gate.Message,
            0, 0, 0, 0, 0, 0, 0, "无", 0, false,
            0, 0, 0,
            _probeRoute.State.Gate.CanRun ? "可用" : "不可用（已隔离）", 0);
        _startupRoute = new(_projectBootstrap, _worldBootstrap, _renderSceneStore, _selectionRoute);
        AvaloniaXamlLoader.Load(this);
        FindShellControls();
        _panelApplyRoute.SetPanels(new(_inspectorPanel, _statusBarPanel, _viewportPlaceholderPanel, _dockPanel));
        SubscribePanelEvents();
        InitializeFeedback();
        LoadSampleProject();
        _lifecycle = new Scene3dSessionLifecycle(_renderSceneStore);
        RunStartupVulkanProbe();
        ProbeVulkanValidation();
    }

    private void FindShellControls()
    {
        _inspectorPanel = this.FindControl<InspectorPanel>("InspectorPanel");
        _debugDockPanel = this.FindControl<DebugDockPanel>("DebugDockPanel");
        _statusBarPanel = this.FindControl<StatusBarPanel>("EditorStatusBarPanel");
        _viewportPlaceholderPanel = this.FindControl<ViewportPlaceholderPanel>("ViewportPlaceholderPanel");
        _vulkanViewportHostPanel = this.FindControl<VulkanViewportHostPanel>("VulkanViewportHostPanel");
        _viewportToolPalette = this.FindControl<ViewportToolPalette>("ViewportToolPalette");
        if (_viewportToolPalette is not null)
            _viewportToolPalette.ToolChanged += HandleViewportToolChanged;
        _dockPanel = this.FindControl<ProjectWorldDockPanel>("ProjectWorldDockPanel");
        var runMenuBtn = this.FindControl<Button>("RunMenuButton");
        if (runMenuBtn is not null) _runMenu.Attach(runMenuBtn);
        _runMenu.RestartScene3dRequested += HandleRestartScene3d;

        // 设置/帮助菜单
        var prefsItem = this.FindControl<MenuItem>("PreferencesMenuItem");
        if (prefsItem is not null) prefsItem.Click += HandlePreferencesClicked;
        var bindingsItem = this.FindControl<MenuItem>("ShowInputBindingsMenuItem");
        if (bindingsItem is not null) bindingsItem.Click += HandleShowInputBindingsClicked;
        var aboutItem = this.FindControl<MenuItem>("AboutFluidWarfareMenuItem");
        if (aboutItem is not null) aboutItem.Click += HandleAboutFluidWarfareClicked;
    }

    private void SubscribePanelEvents()
    {
        if (_viewportPlaceholderPanel is not null)
        {
            _viewportPlaceholderPanel.ViewportFocused += HandleViewportFocused;
        }

        if (_dockPanel is not null)
        {
            _dockPanel.EntitySelectionRequested += OnHierarchyEntitySelected;
            _dockPanel.ContentSelectionRequested += OnProjectContentSelected;
        }

        if (_vulkanViewportHostPanel is not null)
        {
            _vulkanViewportHostPanel.NativeHostInfoChanged += HandleVulkanViewportNativeHostInfoChanged;
            // 原始输入事件转发由 WindowsViewportInputTranslator + ExecuteInputAction 处理
            // 在 AttachedToVisualTree 后初始化
            _vulkanViewportHostPanel.RawPointerButtonDown += HandleRawPointerButtonDown;
            _vulkanViewportHostPanel.RawPointerButtonUp += HandleRawPointerButtonUp;
            _vulkanViewportHostPanel.RawPointerMoved += HandleRawPointerMoved;
            _vulkanViewportHostPanel.RawKeyDown += HandleRawKeyDown;
            _vulkanViewportHostPanel.RawKeyUp += HandleRawKeyUp;
            _vulkanViewportHostPanel.RawMouseWheel += HandleRawMouseWheel;
            _vulkanViewportHostPanel.RawInputFocusLost += HandleRawInputFocusLost;
            _vulkanViewportHostPanel.PickRequested += HandleViewportPick;
            _vulkanViewportHostPanel.NavigationPointerPressed += HandleOverlayPointerPressed;
            _vulkanViewportHostPanel.NavigationPointerMoved += HandleOverlayPointerMoved;
            _vulkanViewportHostPanel.NavigationPointerReleased += HandleOverlayPointerReleased;
            _vulkanViewportHostPanel.NavigationCaptureLost += HandleOverlayCaptureLost;
            _vulkanViewportHostPanel.SceneToolPointerPressed += HandleSceneToolPointerPressed;
            _vulkanViewportHostPanel.SceneToolPointerReleased += HandleSceneToolPointerReleased;
            _vulkanViewportHostPanel.PointerMoved += HandleViewportPointerMoved;
            _vulkanViewportHostPanel.PointerLeft += HandleViewportPointerLeft;
        }

        if (_inspectorPanel is not null)
        {
            _inspectorPanel.TransformDraftChanged += HandleTransformDraftChanged;
            _inspectorPanel.TransformApplyRequested += HandleTransformApply;
            _inspectorPanel.TransformResetRequested += HandleTransformReset;
            _inspectorPanel.GroundPlacementRequested += HandleGroundPlacementToggle;
            _inspectorPanel.ScrubValueChanged += HandleScrubValueChanged;
            _inspectorPanel.ScrubCompleted += HandleScrubCompleted;
            _inspectorPanel.ScrubCancelled += HandleScrubCancelled;
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
        NativeHostReportAction: () => { var r = _startupVulkanRoute.TryRunAttachProbes(BuildStartupVulkanRequest()); ApplyStartupVulkanResult(r); },
        InputPipelineInitAction: InitializeInputPipeline);

    private static void ApplyAttachResult(EditorShellAttachResult result)
    {
        // 路由已通过 Dispatcher.UIThread.Post 调度 NativeHostReport + InputPipelineInit。
        // 后续附加不再重复调度（AttachRoute 内含 _dispatched 守卫）。
    }

    private EditorShellDetachRequest BuildDetachRequest() => new(
        Lifecycle: _lifecycle,
        ResizeRenderTimer: _viewportResizeRenderTimer,
        ResizeRenderTimerTickHandler: HandleViewportResizeRenderTimerTick);

    private void ApplyDetachResult(EditorShellDetachResult result)
    {
        _sessionActive = false;
        _startupVulkanRoute.Reset();
        if (result.TimerCleanedUp) _viewportResizeRenderTimer = null;
    }

    private void RunStartupVulkanProbe()
    {
        ApplyStartupVulkanResult(_startupVulkanRoute.RunConstructProbes(BuildStartupVulkanRequest()));
    }

    private EditorStartupVulkanRequest BuildStartupVulkanRequest()
    {
        return new EditorStartupVulkanRequest(
            ProbeRoute: _probeRoute,
            Lifecycle: _lifecycle,
            RenderSceneStore: _renderSceneStore,
            GetNativeHostInfo: () => _vulkanViewportHostPanel?.GetNativeHostInfo() ?? VulkanViewportNativeHostInfo.NotAvailable,
            InfoLog: AppendInfoLog,
            WarnLog: AppendWarningLog,
            RefreshDiagnostics: RefreshDiagnostics,
            RequestScene3dStart: () => ApplyScene3dCommandResult(_scene3dCommandRoute.Execute(BuildScene3dCommandRequest(EditorScene3dCommandKind.Restart))));
    }

    private void ApplyStartupVulkanResult(EditorStartupVulkanResult result)
    {
        if (result.DiagnosticsRefreshRequested)
            RefreshDiagnostics();
    }

    private void HandleVulkanViewportNativeHostInfoChanged(object? sender, VulkanViewportNativeHostInfo nativeHostInfo)
    {
        if (!nativeHostInfo.HasNativeHandle || nativeHostInfo.Width < 1 || nativeHostInfo.Height < 1)
        {
            return;
        }

        if (!_startupVulkanRoute.State.NativeHostReported)
        {
            Dispatcher.UIThread.Post(RunStartupVulkanProbe);
            return;
        }

        ScheduleVulkanViewportRedraw();
    }

    private void ScheduleVulkanViewportRedraw()
    {
        _viewportResizeRenderTimer ??= new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(180)
        };

        _viewportResizeRenderTimer.Tick -= HandleViewportResizeRenderTimerTick;
        _viewportResizeRenderTimer.Tick += HandleViewportResizeRenderTimerTick;
        _viewportResizeRenderTimer.Stop();
        _viewportResizeRenderTimer.Start();
    }

    private void HandleViewportResizeRenderTimerTick(object? sender, EventArgs e)
    {
        _viewportResizeRenderTimer?.Stop();
        RedrawVulkanViewportOnce();
    }

    private void RedrawVulkanViewportOnce()
    {
        var request = new Scene3dResizeRenderRequest(
            _probeRoute.State.Backend.IsAvailable, _probeRoute.State.Device.IsCreated,
            _vulkanViewportHostPanel?.GetNativeHostInfo() ?? VulkanViewportNativeHostInfo.NotAvailable,
            _sessionActive, _lifecycle.State.Session,
            _cameraRoute, _renderSceneStore.Current, _renderSeq);

        var result = _resizeRenderRoute.RenderOnce(
            request, _lifecycle, _probeRoute, AppendInfoLog, AppendWarningLog);

        ApplyResizeRenderResult(result);
    }

    private void ApplyResizeRenderResult(Scene3dResizeRenderResult result)
    {
        if (result.LogMessage is not null)
        { if (result.LogIsWarning) AppendWarningLog(result.LogMessage); else AppendInfoLog(result.LogMessage); }
        if (result.NewRenderSeq > 0) _renderSeq = result.NewRenderSeq;
        if (result.Action == Scene3dResizeAction.ClearFallbackAfterFailure) _sessionActive = false;
        if (result.Action is Scene3dResizeAction.ClearFallback or Scene3dResizeAction.ClearFallbackAfterFailure)
        {
            if (_probeRoute.State.Clear.IsSucceeded) _renderLastMode = "Clear";
            RefreshDiagnostics();
        }
    }

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

    private void HandlePreferencesClicked(object? sender, RoutedEventArgs e) =>
        ApplyWindowResult(_windowRoute.Open(EditorShellWindowCommand.Preferences));

    private void HandleShowInputBindingsClicked(object? sender, RoutedEventArgs e) =>
        ApplyWindowResult(_windowRoute.Open(EditorShellWindowCommand.InputBindings));

    private void HandleAboutFluidWarfareClicked(object? sender, RoutedEventArgs e) =>
        ApplyWindowResult(_windowRoute.Open(EditorShellWindowCommand.About));

    private void ApplyWindowResult(EditorShellWindowResult r)
    { if (r.LogMessage is not null) AppendInfoLog(r.LogMessage); }


    private void OnHierarchyEntitySelected(string? entityId)
    {
        ApplyEntitySelection(entityId, EditorEntitySelectionOrigin.WorldHierarchy);
    }

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

    private void LoadSampleProject()
    {
        var result = _startupRoute.LoadSampleProject();
        ApplyStartupBootstrapResult(result);
    }

    private void ApplyStartupBootstrapResult(EditorStartupBootstrapResult result)
    {
        if (!result.Success) { _panelApplyRoute.ShowProjectLoadFailure(result.FailureMessage ?? "未知错误", AppendErrorLog); return; }

        _projectInfo = result.Project;
        _contentFiles = result.Project?.ContentFiles;
        RebuildAndShowHierarchy();

        _worldState = result.WorldResult?.World;
        var summary = result.WorldResult is not null
            ? _viewportSelectionPresenter.CreateRenderSceneSummary(result.WorldResult.RenderScene)
            : ViewportRenderSceneSummary.Empty;
        _panelApplyRoute.ApplyStartupWorld(new(result.WorldResult?.HasEntities ?? false, summary));

        foreach (var m in result.LogMessages) AppendInfoLog(m);
        foreach (var w in result.LogWarnings) AppendWarningLog(w);
    }

    private void ProbeVulkanValidation()
    {
        _probeRoute.ProbeValidation(AppendInfoLog, AppendWarningLog);
        RefreshDiagnostics();
    }

    private void HandleScene3dRunRequested(object? sender, EventArgs e)
    {
        _viewportResizeRenderTimer?.Stop();
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

    private void ExecuteOpenPreferences()
    {
        var r = _windowRoute.Open(EditorShellWindowCommand.Preferences);
        if (r.LogMessage is not null) AppendInfoLog(r.LogMessage);
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
        HandleTransformReset();
    }

    // ─── Overlay 导航输入 ──────────────────────────────
    private ViewportNavigationLayout? GetPresentedNavigationLayout()
    {
        if (_lifecycle.State.Session is null || _vulkanViewportHostPanel is null)
            return null;

        var snapshot = _lifecycle.State.Session.LastPresentedOverlaySnapshot;
        if (!snapshot.IsAvailable || snapshot.Layout is null)
            return null;

        var host = _vulkanViewportHostPanel.GetNativeHostInfo();
        if (host.Width != snapshot.ViewportWidth || host.Height != snapshot.ViewportHeight)
            return null;

        return snapshot.Layout;
    }

    private bool ApplyOverlayVisualState(ViewportNavigationElement hovered, ViewportNavigationElement active)
    {
        if (_lifecycle.State.Session?.SetNavigationOverlayState(hovered, active) == true)
        { ScheduleScene3dFrame(VulkanScene3dFrameReason.OverlayNavigationChanged); return true; }
        return false;
    }

    private ViewportNavigationPressResult HandleOverlayPointerPressed(int pixelX, int pixelY)
    {
        var layout = GetPresentedNavigationLayout();
        if (layout is null) return ViewportNavigationPressResult.NotHandled;

        var response = _navigationRoute.Press(pixelX, pixelY, layout);
        if (response.Result == ViewportNavigationPressResult.NotHandled)
            return response.Result;

        ApplyOverlayVisualState(response.Element, response.Element);

        if (response.CameraCommand is not null)
        {
            var camResult = _cameraRoute.Apply(response.CameraCommand);
            if (camResult.StateChanged && camResult.NeedsFrame)
                ScheduleScene3dFrame(camResult.Reason);
            ApplyOverlayVisualState(response.Element, ViewportNavigationElement.None);
            _navigationRoute.Release(false);
        }

        return response.Result;
    }

    private bool HandleOverlayPointerMoved(int pixelX, int pixelY)
    {
        var layout = GetPresentedNavigationLayout();
        var vhFallback = _vulkanViewportHostPanel?.GetNativeHostInfo().Height ?? 1;
        var response = _navigationRoute.Move(pixelX, pixelY, layout, _cameraRoute, vhFallback);
        if (response.VisualStateChanged)
            ApplyOverlayVisualState(response.Hovered, response.Active);
        if (response.NeedsFrame)
            ScheduleScene3dFrame(VulkanScene3dFrameReason.OverlayNavigationChanged);
        return response.Handled;
    }

    private void HandleOverlayPointerReleased()
    {
        var r = _navigationRoute.Release(true);
        if (r.NeedsCleanupFrame)
            ApplyOverlayVisualState(_navigationRoute.HoverElement, ViewportNavigationElement.None);
    }

    private void HandleOverlayCaptureLost()
    {
        var r = _navigationRoute.Release(true);
        if (r.NeedsCleanupFrame)
            ApplyOverlayVisualState(_navigationRoute.HoverElement, ViewportNavigationElement.None);
    }

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
        if (_lifecycle.State.FrameSubmitRoute is null) return;

        var entityPos = _pointerRoute.Session.IsActive
            ? _pointerRoute.Session.PreviewTransform.Position
            : _selectionRoute.State.SelectedWorldEntity is not null
                ? _worldState?.FindPosition(_selectionRoute.State.SelectedWorldEntity.EntityId)?.Value ?? Vector3d.Zero
                : Vector3d.Zero;

        _lifecycle.State.FrameSubmitRoute.Request(new Scene3dFrameSubmitInput(
            reason, _cameraRoute.LastCameraState, _cameraRoute.CameraRevision, _renderSeq,
            _pointerRoute.IsMoveToolActive,
            _selectionRoute.State.SelectedWorldEntity?.EntityId ?? default,
            entityPos,
            _pointerRoute.HoveredElement,
            _worldDirtyState.Revision), () =>
        {
            _renderSeq = _lifecycle.State.FrameSubmitRoute.RenderSeq;
            RefreshDiagnostics();
        });
    }

    // ─── 选择路由 ────────────────────────────────────────────

    private void ApplyEntitySelection(string? entityIdStr, EditorEntitySelectionOrigin origin)
    {
        var reason = MapReason(origin);
        var result = entityIdStr is null
            ? _selectionRoute.ClearSelection(reason)
            : _selectionRoute.SelectEntity(new(entityIdStr, reason, _worldState));
        if (!result.IsChanged) return;
        if (result.Entity is not null)
        { ShowWorldEntitySelection(result.Entity); SyncSceneSelection(result.Entity.EntityId.Value.ToString()); }
        else
            _panelApplyRoute.ClearSelection();
    }

    private void SyncSceneSelection(string entityId)
    {
        if (_lifecycle.State.Session is null || !_sessionActive) return;
        if (_lifecycle.State.Session.SetSelectedEntity(entityId))
            ScheduleScene3dFrame(VulkanScene3dFrameReason.SelectionChanged);
    }

    private void ClearSelection()
    {
        _selectionRoute.ClearSelection(EditorSelectionReason.SelectionRestore);
        _panelApplyRoute.ClearSelection();
    }

    static EditorSelectionReason MapReason(EditorEntitySelectionOrigin o) => o switch
    {
        EditorEntitySelectionOrigin.ViewportPicking => EditorSelectionReason.ViewportPicking,
        EditorEntitySelectionOrigin.WorldHierarchy => EditorSelectionReason.WorldHierarchy,
        _ => EditorSelectionReason.ViewportFocused,
    };

    // ─── 地面指针移动 ─────────────────────────────────────────────

    /// <summary>
    /// 鼠标在视口内移动 → 地面射线求交 → 状态栏反馈。
    /// 采用"最新值覆盖 + 单次调度"合并模式，最多约每 16ms 更新一次。
    /// </summary>
    private void HandleViewportPointerMoved(int pixelX, int pixelY)
    {
        if (!_sessionActive || _lifecycle.State.Session?.Status != VulkanScene3dSessionStatus.Active) return;
        var nh = _vulkanViewportHostPanel?.GetNativeHostInfo() ?? VulkanViewportNativeHostInfo.NotAvailable;
        if (!nh.HasNativeHandle || nh.Width < 1 || nh.Height < 1) return;

        if (_groundPointerUpdatePending) { _lastGroundPointerUpdateTicks = (pixelX << 16) | (pixelY & 0xFFFF); return; }
        _groundPointerUpdatePending = true;
        var cx = pixelX; var cy = pixelY;
        Dispatcher.UIThread.Post(() =>
        {
            _groundPointerUpdatePending = false;
            if (_lastGroundPointerUpdateTicks != 0) { cx = (int)(_lastGroundPointerUpdateTicks >> 16); cy = (int)(_lastGroundPointerUpdateTicks & 0xFFFF); _lastGroundPointerUpdateTicks = 0; }
            var host = _vulkanViewportHostPanel?.GetNativeHostInfo() ?? VulkanViewportNativeHostInfo.NotAvailable;
            _groundHoverRoute.HandlePointerMoved(new(cx, cy, _lifecycle, _groundPointerState, _navigationRoute,
                msg => { if (_statusBarPanel is not null) _statusBarPanel.SetGroundPosition(msg); },
                msg => { if (_statusBarPanel is not null) _statusBarPanel.SetCurrentSelection(msg); }), host);
        }, DispatcherPriority.Background);
    }

    /// <summary>
    /// 鼠标离开视口 → 清除地面坐标显示。
    /// </summary>
    private void HandleViewportPointerLeft()
    {
        var nav = _groundHoverRoute.HandlePointerLeft(new(0, 0, _lifecycle, _groundPointerState, _navigationRoute,
            msg => { if (_statusBarPanel is not null) _statusBarPanel.SetGroundPosition(msg); },
            msg => { if (_statusBarPanel is not null) _statusBarPanel.SetCurrentSelection(msg); }),
            _selectionRoute);
        if (nav.VisualStateChanged) ApplyOverlayVisualState(nav.Hovered, nav.Active);
    }

    /// <summary>
    /// 视口点击 Picking 处理。
    /// 像素坐标 → 世界射线 → RenderScene Picker → 统一选择入口。
    /// </summary>
    /// <summary>
    /// 视口点击 Picking 处理。
    /// </summary>
    private void HandleViewportPick(int pixelX, int pixelY)
    {
        var r = _pickInputRoute.Pick(pixelX, pixelY, _lifecycle, _viewportPickRoute, _renderSceneStore,
            _selectionRoute, _groundPlacementState, _groundPointerState,
            ApplyEntitySelection, AppendInfoLog,
            msg => { if (_statusBarPanel is not null) _statusBarPanel.SetCurrentSelection(msg); },
            RefreshDiagnostics, ShowGroundCursor, HideGroundCursor, CompleteGroundPlacement,
            ScheduleScene3dFrame);
        if (!r.SelectionChanged)
        {
            var snap = _lifecycle.State.Session?.LastPresentedSnapshot;
            if (snap?.IsValid == true && _vulkanViewportHostPanel is not null)
            { var nh = _vulkanViewportHostPanel.GetNativeHostInfo(); if (nh.HasNativeHandle) { var rb = RayBuilder.Build(new(pixelX, pixelY, snap, _lifecycle.State.FrameRoute?.Snapshots.PresentedPick ?? PresentedScenePickSnapshot.None, _renderSceneStore.Current, SceneGroundPlane.Default)); if (rb is not null) ViewportPickTrace.Write(pixelX, pixelY, snap, rb, _renderSceneStore.Current); } }
        }
    }

    /// <summary>Debug Picking 诊断：记录射线命中的实体和距离细节。</summary>


    // ─── 地面标记控制 ─────────────────────────────────────────────

    private void ShowGroundCursor(Vector3d worldPosition)
    {
        _groundPointerState.Commit(worldPosition);
        if (_lifecycle.State.Session is not null && _lifecycle.State.Session.SetGroundCursor(worldPosition))
        {
            ScheduleScene3dFrame(VulkanScene3dFrameReason.GroundCursorChanged);
        }
    }

    private void HideGroundCursor()
    {
        _groundPointerState.ClearCommit();
        if (_lifecycle.State.Session is not null)
        {
            _lifecycle.State.Session.SetGroundCursor(null);
            ScheduleScene3dFrame(VulkanScene3dFrameReason.GroundCursorChanged);
        }
    }

    // ─── Transform 编辑 ─────────────────────────────────────────────

    private void HandleTransformApply(string xText, string yText, string zText)
    {
        _transformApplyRoute.HandleInspectorApply(xText, yText, zText, _selectionRoute, _worldState,
            _commitApplier, ScheduleScene3dFrame, AppendInfoLog,
            err => { if (_inspectorPanel is not null) _inspectorPanel.ShowTransformError(err); });
    }

    private void HandleTransformReset()
    {
        if (_selectionRoute.State.SelectedWorldEntity is null) return;
        var pos = _worldState?.FindPosition(_selectionRoute.State.SelectedWorldEntity.EntityId);
        if (pos is null) return;
        var v = pos.Value.Value;
        _inspectorPanel?.SetTransformTexts(
            v.X.ToString("F3", System.Globalization.CultureInfo.InvariantCulture),
            v.Y.ToString("F3", System.Globalization.CultureInfo.InvariantCulture),
            v.Z.ToString("F3", System.Globalization.CultureInfo.InvariantCulture));
        _inspectorPanel?.SetTransformDraftState(false, false, null);
    }

    private void HandleTransformDraftChanged(string xText, string yText, string zText)
    {
        if (_selectionRoute.State.SelectedWorldEntity is null)
        {
            _inspectorPanel?.SetTransformDraftState(false, false, null);
            return;
        }

        if (!EditorEntityTransformValidation.TryParse(xText, yText, zText,
                out var newPos, out var error))
        {
            _inspectorPanel?.SetTransformDraftState(false, true, error);
            return;
        }

        var currentPos = _worldState?.FindPosition(_selectionRoute.State.SelectedWorldEntity.EntityId);
        if (currentPos is null)
        {
            _inspectorPanel?.SetTransformDraftState(false, false, null);
            return;
        }

        var changed = newPos != currentPos.Value.Value;
        _inspectorPanel?.SetTransformDraftState(
            canApply: changed && !_groundPlacementState.IsActive,
            canReset: changed,
            error: null);
    }

    // ─── 数值拖拽处理 ──────────────────────────────────────

    private void HandleScrubValueChanged(string entityId, TransformPositionAxis axis, double value)
    {
        if (_selectionRoute.State.SelectedWorldEntity is null) return;
        if (_selectionRoute.State.SelectedWorldEntity.EntityId.Value.ToString() != entityId) { AppendWarningLog("数值拖拽目标实体已变化，忽略本次更新。"); return; }
        var pos = _worldState?.FindPosition(_selectionRoute.State.SelectedWorldEntity.EntityId);
        if (pos is null) return;
        var cur = pos.Value.Value;
        var newPos = axis switch { TransformPositionAxis.X => new Vector3d(value, cur.Y, cur.Z), TransformPositionAxis.Y => new Vector3d(cur.X, value, cur.Z), _ => new Vector3d(cur.X, cur.Y, value) };
        _transformApplyRoute.Apply(_transformApplyRoute.CurrentEntityTransform(_selectionRoute, _worldState) with { Position = newPos }, EditorEntityTransformOrigin.DragScrub, _selectionRoute, _worldState, _commitApplier, ScheduleScene3dFrame, AppendInfoLog);
    }

    private void HandleScrubCompleted(string entityId, TransformPositionAxis axis, double value) => AppendInfoLog($"数值拖拽完成：{axis} = {value:F3}");

    private void HandleScrubCancelled(string entityId, TransformPositionAxis axis, double initialValue)
    {
        if (_selectionRoute.State.SelectedWorldEntity is null || _selectionRoute.State.SelectedWorldEntity.EntityId.Value.ToString() != entityId) return;
        var pos = _worldState?.FindPosition(_selectionRoute.State.SelectedWorldEntity.EntityId);
        if (pos is null) return;
        var cur = pos.Value.Value;
        var restored = axis switch { TransformPositionAxis.X => new Vector3d(initialValue, cur.Y, cur.Z), TransformPositionAxis.Y => new Vector3d(cur.X, initialValue, cur.Z), _ => new Vector3d(cur.X, cur.Y, initialValue) };
        _transformApplyRoute.Apply(_transformApplyRoute.CurrentEntityTransform(_selectionRoute, _worldState) with { Position = restored }, EditorEntityTransformOrigin.DragScrub, _selectionRoute, _worldState, _commitApplier, ScheduleScene3dFrame, AppendInfoLog);
        AppendInfoLog("数值拖拽已取消");
    }

    private void HandleGroundPlacementToggle()
    {
        _groundPlacementRoute.Toggle(_selectionRoute, _groundPlacementState, _sessionActive, _lifecycle, _inspectorPanel, _statusBarPanel, AppendWarningLog);
    }

    private void ApplyEntityTransform(SceneTransform transform, EditorEntityTransformOrigin origin)
    {
        _transformApplyRoute.Apply(transform, origin, _selectionRoute, _worldState, _commitApplier, ScheduleScene3dFrame, AppendInfoLog);
    }

    private SceneTransform CurrentEntityTransform()
    {
        return _transformApplyRoute.CurrentEntityTransform(_selectionRoute, _worldState);
    }

    private void CompleteGroundPlacement(Vector3d groundPosition)
    {
        var r = _groundPlacementRoute.Complete(groundPosition, _selectionRoute, _groundPlacementState,
            _worldState, _commitApplier, ScheduleScene3dFrame, _inspectorPanel, AppendInfoLog);
        if (r.Completed) HideGroundCursor();
    }

    /// <summary>
    /// 从当前 WorldState + RenderScene 构建层级树并显示。
    /// </summary>
    private void RebuildAndShowHierarchy()
    {
        if (_projectInfo is not null) try { _dockPanel?.ShowProjectContent(ProjectContentTreeBuilder.Build(_projectInfo)); } catch (Exception ex) { AppendErrorLog($"项目内容树构建失败：{ex.Message}"); }
        if (_worldState is not null) try { _dockPanel?.ShowWorldHierarchy(WorldHierarchyTreeBuilder.Build(_worldState, BuildGroupLookup())); } catch (Exception ex) { AppendErrorLog($"世界层级树构建失败：{ex.Message}"); }
    }

    private Dictionary<EntityId, string>? BuildGroupLookup()
    {
        if (_renderSceneStore.Current.Objects.Count == 0) return null;
        var map = new Dictionary<EntityId, string>();
        foreach (var obj in _renderSceneStore.Current.Objects)
            map[obj.EntityId] = obj.VisualKind == RenderObjectVisualKind.UnitMarker ? "单位" : "其他";
        return map;
    }



    private void RefreshDiagnostics()
    {
        _feedback.RefreshViewportStatusLine(_sessionActive, _lifecycle.State, _probeRoute.State, _renderLastMode);
        var ps = _probeRoute.State;
        var nh = _vulkanViewportHostPanel?.GetNativeHostInfo() ?? VulkanViewportNativeHostInfo.NotAvailable;
        var s3d = ps.Scene3d;
        _feedback.RefreshAllDiagnostics(ps, nh, _renderSceneStore.Current.Objects, s3d.IsSucceeded, s3d.Message, s3d.CameraSummary,
            s3d.GridVertexCount, s3d.GridLineCount, s3d.UnitVertexCount, s3d.UnitTriangleCount,
            s3d.RenderedUnitCount, s3d.RenderObjectCount, s3d.IgnoredObjectCount, s3d.DrawCallCount, s3d.DepthFormat,
            s3d.DepthAttachmentCount, s3d.DepthTestEnabled, ps.Instance.ElapsedMilliseconds, ps.Device.ElapsedMilliseconds,
            ps.Swapchain.ElapsedMilliseconds, ps.Clear.ElapsedMilliseconds, s3d.ElapsedMilliseconds);
        _runMenu.SetScene3dEnabled(VulkanScene3dRunGate.Evaluate().CanRun);
        _statusBarPanel?.SetVulkanStatus(ps.Backend.IsAvailable ? "已接入" : "不可用");
    }


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

    private void UpdateVulkanViewportHost()
    {
        if (_probeRoute.State.Backend.IsAvailable)
        {
            _vulkanViewportHostPanel?.ShowClearStatus("Vulkan 后端就绪，等待 Surface/Swapchain。");
        }
        else
        {
            _vulkanViewportHostPanel?.ShowClearStatus("Vulkan 后端不可用。");
        }
    }
}
