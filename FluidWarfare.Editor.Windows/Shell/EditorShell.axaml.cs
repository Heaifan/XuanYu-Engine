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
            RequestScene3dStart: StartScene3dSession);
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
        var result = _viewportFocusRoute.Focus(_worldState, _selectionRoute);
        _inspectorPanel?.ShowSelection(result.InspectorSelection);
        _statusBarPanel?.SetCurrentSelection(result.StatusBarText);
        if (result.ShowEmptyWorld) _viewportPlaceholderPanel?.ShowEmptyWorld();
        foreach (var m in result.LogMessages) AppendInfoLog(m);
        foreach (var w in result.LogWarnings) AppendWarningLog(w);
        if (result.EntityToShow is not null) ShowWorldEntitySelection(result.EntityToShow);
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
        var result = _contentSelectionPresenter.Present(relativePath, _contentFiles);
        _inspectorPanel?.ShowProjectFileSelection(result.InspectorSelection);
        _statusBarPanel?.SetCurrentSelection(result.StatusBarSelection ?? "无");
        if (!string.IsNullOrEmpty(result.LogMessage))
            AppendInfoLog(result.LogMessage);
    }

    private void ShowWorldEntitySelection(WorldEntityInfo entityInfo)
    {
        var isScene3dActive = _sessionActive && _lifecycle.State.Session?.Status == VulkanScene3dSessionStatus.Active;
        var result = _worldSelectionPresenter.Present(entityInfo, _worldState,
            _renderSceneStore.Current, isScene3dActive);

        _inspectorPanel!.ShowWorldEntitySelection(
            result.InspectorSelection, result.InspectorEntityId ?? "",
            result.EntityPosition, result.EntitySourcePath);
        _inspectorPanel.ScrubEntityId = result.InspectorEntityId ?? "";
        _statusBarPanel?.SetCurrentSelection(result.StatusBarSelection ?? "无");
        _inspectorPanel?.SetGroundPlaceEnabled(result.GroundPlaceEnabled);
        if (result.ViewportSummary is not null)
            _viewportPlaceholderPanel?.ShowEntitySummary(result.ViewportSummary);
        AppendInfoLog(result.LogMessage);
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
        if (!result.Success)
        {
            _viewportPlaceholderPanel?.ShowNoWorldEntity();
            var sel = new EditorSelection("项目加载", "加载失败", $"项目加载失败：{result.FailureMessage}");
            _inspectorPanel?.ShowSelection(sel);
            _statusBarPanel?.SetCurrentSelection("项目加载失败");
            AppendErrorLog($"项目加载失败：{result.FailureMessage}");
            return;
        }

        _projectInfo = result.Project;
        _contentFiles = result.Project?.ContentFiles;
        RebuildAndShowHierarchy();

        if (result.WorldResult is { HasEntities: true })
        {
            _worldState = result.WorldResult.World;
            var summary = _viewportSelectionPresenter.CreateRenderSceneSummary(result.WorldResult.RenderScene);
            _viewportPlaceholderPanel?.ShowNoWorldEntity();
            _viewportPlaceholderPanel?.ShowRenderSceneSummary(summary);
        }
        else
        {
            _viewportPlaceholderPanel?.ShowNoWorldEntity();
            _viewportPlaceholderPanel?.ShowRenderSceneSummary(ViewportRenderSceneSummary.Empty);
        }

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
        var currentGate = VulkanScene3dRunGate.Evaluate();
        _probeRoute.State.Gate = currentGate with { }; // update gate with fresh Evaluate
        // Refactor: can't reassign readonly field, use the gate's current state
        // Actually _probeRoute.State.Gate is readonly, but we need to re-evaluate.
        // Fix: make the field non-readonly or store the message separately.
        TryRunScene3dProbeManually(currentGate);
    }

    private void TryRunScene3dProbeManually(VulkanScene3dRunGate gate)
    {
        // 取消 pending resize 防抖，防止 Clear 覆盖 Scene3D 画面
        _viewportResizeRenderTimer?.Stop();

        if (!gate.CanRun)
        {
            AppendWarningLog(gate.Message);
            _probeRoute.State.Scene3d = new VulkanScene3dInfo(
                VulkanScene3dStatus.NotChecked, gate.Message,
                0, 0, 0, 0, 0, 0, 0, "无", 0, false,
                0, 0, 0,
                gate.CanRun ? "可用" : "不可用（已隔离）", 0);
            ShowVulkanScene3DInfo();
            return;
        }

        // Gate says Ready — try running
        var nativeHostInfo = _vulkanViewportHostPanel?.GetNativeHostInfo()
            ?? VulkanViewportNativeHostInfo.NotAvailable;

        if (!nativeHostInfo.HasNativeHandle || nativeHostInfo.Width < 1 || nativeHostInfo.Height < 1)
        {
            _probeRoute.State.Scene3d = new VulkanScene3dInfo(
                VulkanScene3dStatus.Failed, "场景3D：视口未就绪，跳过运行。",
                0, 0, 0, 0, 0, 0, 0, "无", 0, false,
                0, 0, 0, "不可用", 0);
            ShowVulkanScene3DInfo();
            return;
        }

        ProbeVulkanScene3D();
    }

    private void HandleRestartScene3d()
    {
        if (_lifecycle.State.Session is not null)
        {
            _lifecycle.Stop();

            if (VulkanScene3dSwapchainResources.LiveCount != 0)
            {
                AppendErrorLog(
                    $"拒绝重启 Scene3D：仍有 {VulkanScene3dSwapchainResources.LiveCount} 个 Swapchain 存活。");
                _sessionActive = false;
                return;
            }
        }

        if (!_probeRoute.State.Gate.CanRun)
        {
            AppendWarningLog(_probeRoute.State.Gate.Message);
            return;
        }

        StartScene3dSession();
    }

    private void StartScene3dSession()
    {
        var nativeHostInfo = _vulkanViewportHostPanel?.GetNativeHostInfo()
            ?? VulkanViewportNativeHostInfo.NotAvailable;
        if (!nativeHostInfo.HasNativeHandle || nativeHostInfo.Width < 1 || nativeHostInfo.Height < 1)
        { AppendWarningLog("Scene3D 会话：视口未就绪。"); return; }

        AppendInfoLog("正在启动 Scene3D 会话...");
        _sessionActive = true;

        _cameraRoute.Reset();
        var pose = _cameraRoute.CreatePose();

        var request = new Scene3dSessionStartRequest(
            nativeHostInfo.InstanceHandle, nativeHostInfo.WindowHandle,
            (uint)nativeHostInfo.Width, (uint)nativeHostInfo.Height, pose);
        var result = _lifecycle.Start(request);

        if (result.Success)
        {
            InitTransformApplication();
            _renderLastMode = "Scene3D";
            _renderSeq++;
            AppendInfoLog($"RenderSeq-{_renderSeq:D3} | Scene3D Session 启动 | " +
                $"{nativeHostInfo.Width}x{nativeHostInfo.Height}");
            AppendInfoLog(result.Message);
        }
        else
        {
            _sessionActive = false;
            AppendErrorLog($"Scene3D 会话启动失败：{result.Message}");
        }

        RefreshDiagnostics();
        RefreshDiagnostics();
    }

    // ─── 输入动作映射系统 ──────────────────────────────────
    // 数据流：Win32 WM_ → RawPointerButtonDown/KeyDown 等
    //       → WindowsViewportInputTranslator.OnRaw*()
    //       → EditorInputMatch → ExecuteInputAction() → 统一执行方法

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

    /// <summary>
    /// 实时 Preview：将拖动的预览位置同步到 _renderSceneStore.Current + Vulkan，但不修改 WorldState。
    /// 鼠标释放时由 HandleSceneToolPointerReleased 统一提交。
    /// Preview 必须更新 _renderSceneStore.Current，否则 ScenePointerPicker.Pick 读到的仍是旧位置。
    /// </summary>
    private void ApplyPreviewPosition()
    {
        if (_selectionRoute.State.SelectedWorldEntity is null || _previewApplier is null) return;
        _previewApplier.Apply(_pointerRoute.Session.PreviewTransform, _selectionRoute.State.SelectedWorldEntity.EntityId);
        ScheduleScene3dFrame(VulkanScene3dFrameReason.TransformPreview);
    }

    // ─── 统一取消 ──────────────────────────────────────────────

    /// <summary>
    /// 统一取消活动变换。恢复视觉位置、_renderSceneStore.Current、Inspector，重置模态状态。
    /// </summary>
    private void CancelActiveTransform(TransformInteractionResult r)
    {
        if (r.Action != TransformInteractionAction.Cancelled || _selectionRoute.State.SelectedWorldEntity is null) return;
        if (_cancelApplier is not null)
            _cancelApplier.Apply(r.Transform, _selectionRoute.State.SelectedWorldEntity.EntityId);
        ScheduleScene3dFrame(VulkanScene3dFrameReason.TransformPreview);
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
        var req = new EditorSelectionRequest(entityIdStr, reason, _worldState);
        var result = entityIdStr is null
            ? _selectionRoute.ClearSelection(reason)
            : _selectionRoute.SelectEntity(req);

        if (!result.IsChanged) return;

        if (result.Entity is not null)
        {
            ShowWorldEntitySelection(result.Entity);
            SyncSceneSelection(result.Entity.EntityId.Value.ToString());
        }
        else
        {
            _inspectorPanel?.ShowNoSelection();
            _statusBarPanel?.SetCurrentSelection("无");
            _dockPanel?.ClearEntitySelection();
        }
    }

    private void SyncSceneSelection(string entityId)
    {
        if (_lifecycle.State.Session is null || !_sessionActive) return;
        if (_lifecycle.State.Session.SetSelectedEntity(entityId))
            ScheduleScene3dFrame(VulkanScene3dFrameReason.SelectionChanged);
    }

    private void ClearSelection()
    {
        var r = _selectionRoute.ClearSelection(EditorSelectionReason.SelectionRestore);
        _inspectorPanel?.ShowNoSelection();
        _statusBarPanel?.SetCurrentSelection("无");
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
        if (!_sessionActive || _lifecycle.State.Session is null)
            return;
        if (_lifecycle.State.Session.Status != VulkanScene3dSessionStatus.Active)
            return;

        var nativeHostInfo = _vulkanViewportHostPanel?.GetNativeHostInfo()
            ?? VulkanViewportNativeHostInfo.NotAvailable;
        if (!nativeHostInfo.HasNativeHandle || nativeHostInfo.Width < 1 || nativeHostInfo.Height < 1)
            return;

        // 调度合并：如果已有待执行更新，只保存最新坐标，不重复调度
        if (_groundPointerUpdatePending)
        {
            _lastGroundPointerUpdateTicks = (pixelX << 16) | (pixelY & 0xFFFF); // 暂存最新坐标
            return;
        }

        _groundPointerUpdatePending = true;
        var capturedX = pixelX;
        var capturedY = pixelY;

        Dispatcher.UIThread.Post(() =>
        {
            _groundPointerUpdatePending = false;

            // 读取最新坐标（如果有暂存值）
            var curX = capturedX;
            var curY = capturedY;
            if (_lastGroundPointerUpdateTicks != 0)
            {
                curX = (int)(_lastGroundPointerUpdateTicks >> 16);
                curY = (int)(_lastGroundPointerUpdateTicks & 0xFFFF);
                _lastGroundPointerUpdateTicks = 0;
            }

            UpdateGroundHover(curX, curY);
        }, DispatcherPriority.Background);
    }

    /// <summary>
    /// 鼠标离开视口 → 清除地面坐标显示。
    /// </summary>
    private void HandleViewportPointerLeft()
    {
        if (_navigationRoute.DragMode == ViewportNavigationDragMode.None)
        {
            var r = _navigationRoute.ClearHover();
            if (r.VisualStateChanged) ApplyOverlayVisualState(r.Hovered, r.Active);
        }

        if (_statusBarPanel is null) return;

        _groundPointerState.SetHover(null, null);
        _statusBarPanel.SetCurrentSelection(
            _selectionRoute.State.SelectedWorldEntity is not null
                ? _selectionRoute.State.SelectedWorldEntity.DisplayName
                : "无");

        // 更新状态栏额外行显示地面坐标不可用
        _statusBarPanel.SetGroundPosition("地面坐标：无");
    }

    /// <summary>
    /// 执行地面 Hover 射线求交并更新状态栏。
    /// 只执行 CPU 数学，不提交 GPU 帧。
    /// </summary>
    private void UpdateGroundHover(int pixelX, int pixelY)
    {
        if (_lifecycle.State.Session is null || _vulkanViewportHostPanel is null) return;

        var nativeHostInfo = _vulkanViewportHostPanel.GetNativeHostInfo();
        if (!nativeHostInfo.HasNativeHandle || nativeHostInfo.Width < 1 || nativeHostInfo.Height < 1)
            return;

        // 使用已呈现快照构建射线（只能从 Snapshot 读取参数）
        var snapshot = _lifecycle.State.Session.LastPresentedSnapshot;
        var status = VulkanSceneRayBuilder.TryBuild(
            pixelX, pixelY,
            snapshot,
            (uint)nativeHostInfo.Width, (uint)nativeHostInfo.Height,
            out var ray);

        if (status != SceneRayBuildStatus.Success || ray is null)
        {
            if (status == SceneRayBuildStatus.SnapshotUnavailable ||
                status == SceneRayBuildStatus.SnapshotExtentMismatch)
                return;

            _groundPointerState.SetHover(null, null);
            _statusBarPanel?.SetGroundPosition("地面坐标：无");
            return;
        }

        var groundHit = SceneRayGroundIntersection.Intersect(ray, SceneGroundPlane.Default);

        if (groundHit.IsHit && groundHit.WorldPosition is not null)
        {
            var pos = groundHit.WorldPosition.Value;
            _groundPointerState.SetHover(pos, "鼠标");
            _statusBarPanel?.SetGroundPosition(
                $"地面坐标：X {pos.X:F2} | Y {pos.Y:F2} | Z {pos.Z:F2}");
        }
        else
        {
            _groundPointerState.SetHover(null, null);
            _statusBarPanel?.SetGroundPosition("地面坐标：无");
        }
    }

    /// <summary>
    /// 清除选择。
    /// </summary>
    /// <summary>
    /// 视口点击 Picking 处理。
    /// 像素坐标 → 世界射线 → RenderScene Picker → 统一选择入口。
    /// </summary>
    /// <summary>
    /// 视口点击 Picking 处理。
    /// </summary>
    private void HandleViewportPick(int pixelX, int pixelY)
    {
        if (!_sessionActive || _lifecycle.State.Session is null) return;
        if (_lifecycle.State.Session.Status != VulkanScene3dSessionStatus.Active) return;

        var nativeHostInfo = _vulkanViewportHostPanel?.GetNativeHostInfo()
            ?? VulkanViewportNativeHostInfo.NotAvailable;
        if (!nativeHostInfo.HasNativeHandle || nativeHostInfo.Width < 1 || nativeHostInfo.Height < 1)
            return;

        var snapshot = _lifecycle.State.Session.LastPresentedSnapshot;
        if (!snapshot.IsValid) return;

        var pickSnapshot = _lifecycle.State.FrameRoute?.Snapshots.PresentedPick ?? PresentedScenePickSnapshot.None;
        var req = new ViewportPickRequest(pixelX, pixelY, snapshot, pickSnapshot, _renderSceneStore.Current, SceneGroundPlane.Default);
        var result = _viewportPickRoute.Pick(req);

        if (_groundPlacementState.IsActive)
        {
            switch (result.Kind)
            {
                case ViewportPickKind.Ground when result.GroundPosition is not null:
                    CompleteGroundPlacement(result.GroundPosition.Value); break;
                case ViewportPickKind.Entity:
                    _statusBarPanel?.SetCurrentSelection("请点击空白地面完成放置"); break;
                default:
                    _statusBarPanel?.SetCurrentSelection("当前位置未命中地面，请调整相机或点击其他区域"); break;
            }
        }
        else
        {
            switch (result.Kind)
            {
                case ViewportPickKind.Entity when result.EntityId is not null:
                    ApplyEntitySelection(result.EntityId.Value.Value.ToString(), EditorEntitySelectionOrigin.ViewportPicking);
                    HideGroundCursor();
                    System.Diagnostics.Debug.WriteLine($"[Pick] Entity hit: {result.EntityId.Value.Value}");
                    break;
                case ViewportPickKind.Ground when result.GroundPosition is not null:
                    ApplyEntitySelection(null, EditorEntitySelectionOrigin.ViewportPicking);
                    ShowGroundCursor(result.GroundPosition.Value);
                    AppendInfoLog($"地面落点：X {result.GroundPosition.Value.X:F2}，Y {result.GroundPosition.Value.Y:F2}，Z {result.GroundPosition.Value.Z:F2}。");
                    break;
                default:
                    ApplyEntitySelection(null, EditorEntitySelectionOrigin.ViewportPicking);
                    HideGroundCursor(); break;
            }
        }
        RefreshDiagnostics();
        if (result.Kind != ViewportPickKind.Entity)
        {
            var rb = RayBuilder.Build(req);
            if (rb is not null) ViewportPickTrace.Write(pixelX, pixelY, snapshot, rb, _renderSceneStore.Current);
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
        if (_selectionRoute.State.SelectedWorldEntity is null) return;

        if (!EditorEntityTransformValidation.TryParse(xText, yText, zText,
                out var newPos, out var error))
        {
            _inspectorPanel?.ShowTransformError(error);
            return;
        }

        ApplyEntityTransform(CurrentEntityTransform() with { Position = newPos }, EditorEntityTransformOrigin.InspectorInput);
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
        // 防串写：事件携带的 entityId 必须与当前选中实体一致
        if (_selectionRoute.State.SelectedWorldEntity.EntityId.Value.ToString() != entityId)
        {
            AppendWarningLog("数值拖拽目标实体已变化，忽略本次更新。");
            return;
        }
        var pos = _worldState?.FindPosition(_selectionRoute.State.SelectedWorldEntity.EntityId);
        if (pos is null) return;

        var current = pos.Value.Value;
        var newPos = axis switch
        {
            TransformPositionAxis.X => new Vector3d(value, current.Y, current.Z),
            TransformPositionAxis.Y => new Vector3d(current.X, value, current.Z),
            _ => new Vector3d(current.X, current.Y, value),
        };

        ApplyEntityTransform(CurrentEntityTransform() with { Position = newPos }, EditorEntityTransformOrigin.DragScrub);
    }

    private void HandleScrubCompleted(string entityId, TransformPositionAxis axis, double value)
    {
        AppendInfoLog($"数值拖拽完成：{axis} = {value:F3}");
    }

    private void HandleScrubCancelled(string entityId, TransformPositionAxis axis, double initialValue)
    {
        if (_selectionRoute.State.SelectedWorldEntity is null) return;
        if (_selectionRoute.State.SelectedWorldEntity.EntityId.Value.ToString() != entityId)
            return;

        var pos = _worldState?.FindPosition(_selectionRoute.State.SelectedWorldEntity.EntityId);
        if (pos is null) return;

        var current = pos.Value.Value;
        var restoredPos = axis switch
        {
            TransformPositionAxis.X => new Vector3d(initialValue, current.Y, current.Z),
            TransformPositionAxis.Y => new Vector3d(current.X, initialValue, current.Z),
            _ => new Vector3d(current.X, current.Y, initialValue),
        };

        ApplyEntityTransform(CurrentEntityTransform() with { Position = restoredPos }, EditorEntityTransformOrigin.DragScrub);
        AppendInfoLog("数值拖拽已取消");
    }

    private void HandleGroundPlacementToggle()
    {
        if (_selectionRoute.State.SelectedWorldEntity is null) return;
        if (!_sessionActive || _lifecycle.State.Session?.Status != VulkanScene3dSessionStatus.Active)
        {
            AppendWarningLog("Scene3D 未激活，无法进入放置模式。");
            return;
        }

        if (_groundPlacementState.IsActive)
        {
            _groundPlacementState.Cancel();
            _inspectorPanel?.SetPlacementMode(false);
            _statusBarPanel?.SetCurrentSelection(
                _selectionRoute.State.SelectedWorldEntity?.DisplayName ?? "无");
        }
        else
        {
            _groundPlacementState.Begin(_selectionRoute.State.SelectedWorldEntity.EntityId.Value.ToString());
            _inspectorPanel?.SetPlacementMode(true);
            _statusBarPanel?.SetCurrentSelection(
                $"放置模式：点击空白地面放置 {_selectionRoute.State.SelectedWorldEntity.DisplayName}，Esc 取消");
        }
    }

    /// <summary>
    /// 原子式 Transform 提交。
    /// </summary>
    private void ApplyEntityTransform(SceneTransform transform, EditorEntityTransformOrigin origin)
    {
        if (_selectionRoute.State.SelectedWorldEntity is null || _commitApplier is null) return;
        _commitApplier.Apply(transform, _selectionRoute.State.SelectedWorldEntity.EntityId);
        ScheduleScene3dFrame(VulkanScene3dFrameReason.EntityTransformChanged);

        // 日志（数值拖拽和移动工具已完成时不写日志，由调用层写）
        if (origin != EditorEntityTransformOrigin.DragScrub && origin != EditorEntityTransformOrigin.MoveTool)
        {
            AppendInfoLog(
                $"实体 {_selectionRoute.State.SelectedWorldEntity.DisplayName} 坐标修改为 " +
                $"({transform.Position.X:F2}, {transform.Position.Y:F2}, {transform.Position.Z:F2})。");
        }
    }

    /// <summary>从当前选中实体的 WorldState 位置构造 SceneTransform。</summary>
    private SceneTransform CurrentEntityTransform()
    {
        if (_selectionRoute.State.SelectedWorldEntity is null) return default;
        var pos = _worldState?.FindPosition(_selectionRoute.State.SelectedWorldEntity.EntityId);
        return pos is not null ? new SceneTransform(pos.Value.Value, default, default) : default;
    }

    // ─── 地面放置 ──────────────────────────────────────────────────

    private void CompleteGroundPlacement(Vector3d groundPosition)
    {
        if (!_groundPlacementState.IsActive || _selectionRoute.State.SelectedWorldEntity is null) return;

        // 地面放置：实体在地面锚点，Z = 平面高程（0）
        var entityPos = new Vector3d(groundPosition.X, groundPosition.Y, 0);

        ApplyEntityTransform(CurrentEntityTransform() with { Position = entityPos }, EditorEntityTransformOrigin.GroundPlacement);

        if (_groundPlacementState.IsActive)
        {
            _groundPlacementState.Complete();
            _inspectorPanel?.SetPlacementMode(false);
            HideGroundCursor();
            AppendInfoLog(
                $"实体 {_selectionRoute.State.SelectedWorldEntity.DisplayName} 已放置到 " +
                $"X {entityPos.X:F2}，Y {entityPos.Y:F2}，Z {entityPos.Z:F2}。");
        }
    }

    /// <summary>
    /// 从当前 WorldState + RenderScene 构建层级树并显示。
    /// </summary>
    private void RebuildAndShowHierarchy()
    {
        // 项目内容树
        if (_projectInfo is not null)
        {
            try
            {
                var projectTree = ProjectContentTreeBuilder.Build(_projectInfo);
                _dockPanel?.ShowProjectContent(projectTree);
            }
            catch (Exception ex)
            {
                AppendErrorLog($"项目内容树构建失败：{ex.Message}");
            }
        }

        // 世界层级树
        if (_worldState is null)
        {
            _dockPanel?.ShowWorldHierarchy(WorldHierarchyTree.Empty);
            return;
        }

        // 从 RenderScene 构建 EntityId → 分组名映射
        Dictionary<EntityId, string>? groupLookup = null;
        if (_renderSceneStore.Current.Objects.Count > 0)
        {
            groupLookup = new Dictionary<EntityId, string>();
            foreach (var obj in _renderSceneStore.Current.Objects)
            {
                var groupName = obj.VisualKind switch
                {
                    RenderObjectVisualKind.UnitMarker => "单位",
                    _ => "其他"
                };
                groupLookup[obj.EntityId] = groupName;
            }
        }

        var tree = WorldHierarchyTreeBuilder.Build(_worldState, groupLookup);
        _dockPanel?.ShowWorldHierarchy(tree);
    }



    private void ProbeVulkanScene3D()
    {
        if (!_probeRoute.State.Gate.CanRun)
        {
            AppendInfoLog(_probeRoute.State.Gate.Message);
            ShowVulkanScene3DInfo();
            return;
        }

        var nativeHostInfo = _vulkanViewportHostPanel?.GetNativeHostInfo()
            ?? VulkanViewportNativeHostInfo.NotAvailable;

        if (!nativeHostInfo.HasNativeHandle || nativeHostInfo.InstanceHandle == 0 || nativeHostInfo.WindowHandle == 0)
        {
            _probeRoute.State.Scene3d = new VulkanScene3dInfo(
                VulkanScene3dStatus.Failed, "缺少原生句柄，跳过 3D 场景绘制。",
                0, 0, 0, 0, 0, 0, 0, "无", 0, false,
                0, 0, 0, "无", 0);
            ShowVulkanScene3DInfo();
            return;
        }

        // 使用视口实际尺寸（避免 maximize 时硬编码出错）
        var vpW = (uint)Math.Max(nativeHostInfo.Width, 1);
        var vpH = (uint)Math.Max(nativeHostInfo.Height, 1);

        // 生成地面网格（范围 -20 到 +20，间隔 2）
        var gridVertices = VulkanScene3dVertices.BuildGrid(20, 2);

        // 共享单位网格（单位立方体，位置由 per-object MVP 控制）
        var unitVertices = VulkanScene3dVertices.BuildCube(0, 0, 0, 1.0f);

        // 单位绘制信息：从 RenderScene 收集所有 UnitMarker 对象
        var unitDraws = new List<VulkanScene3dUnitDrawInfo>();
        foreach (var obj in _renderSceneStore.Current.Objects)
        {
            if (obj.VisualKind != RenderObjectVisualKind.UnitMarker)
                continue;

            var p = obj.Placement;
            unitDraws.Add(new VulkanScene3dUnitDrawInfo(
                obj.EntityId.Value.ToString(),
                (float)(p?.VisualCenter.X ?? obj.Position.X),
                (float)(p?.VisualCenter.Y ?? obj.Position.Y),
                (float)(p?.VisualCenter.Z ?? obj.Position.Z + RenderUnitPlacement.HalfExtent),
                (float)RenderUnitPlacement.Scale));
        }

        var camera = VulkanCameraInfo.DefaultBattlefield;

        _renderSeq++;
        AppendInfoLog($"RenderSeq-{_renderSeq:D3} | Scene3D | {vpW}x{vpH} | 手动触发");

        _probeRoute.State.Scene3d = VulkanScene3dRenderer.RenderWindows(
            nativeHostInfo.InstanceHandle,
            nativeHostInfo.WindowHandle,
            vpW, vpH, camera,
            gridVertices.AsSpan(),
            unitVertices.AsSpan(),
            [.. unitDraws]);

        ShowVulkanScene3DInfo();
    }

    private void ShowVulkanScene3DInfo()
    {
        if (_probeRoute.State.Scene3d.IsSucceeded) { _renderLastMode = "Scene3D"; AppendInfoLog(_probeRoute.State.Scene3d.Message); }
        else if (_probeRoute.State.Scene3d.Status != VulkanScene3dStatus.NotChecked) AppendWarningLog($"Vulkan 3D 场景绘制失败：{_probeRoute.State.Scene3d.Message}");
        RefreshDiagnostics();
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
