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
using FluidWarfare.Editor.Windows.Viewport.Transform.Input;
using FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;
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
using FluidWarfare.Render.Scene;
using FluidWarfare.Render.Scene.Position;
using FluidWarfare.Render.Selection;
using FluidWarfare.Render.Selection.Ground;
using FluidWarfare.Render.Selection.Pointer;
using FluidWarfare.Render.Vulkan.Backend;
using FluidWarfare.Render.Vulkan.Device;
using FluidWarfare.Render.Vulkan.Instance;
using FluidWarfare.Render.Vulkan.Clear;
using FluidWarfare.Render.Vulkan.Camera;
using FluidWarfare.Render.Camera;
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
    private EntityId _firstEntityId;
    private WorldEntityInfo? _selectedWorldEntity;
    private RenderScene _renderScene = RenderScene.Empty;
    private VulkanBackendInfo _vulkanBackendInfo = VulkanBackendInfo.NotChecked;
    private VulkanInstanceInfo _vulkanInstanceInfo = VulkanInstanceInfo.NotChecked;
    private VulkanDeviceInfo _vulkanDeviceInfo = VulkanDeviceInfo.NotChecked;
    private VulkanSurfaceInfo _vulkanSurfaceInfo = VulkanSurfaceInfo.NotChecked;
    private VulkanSwapchainInfo _vulkanSwapchainInfo = VulkanSwapchainInfo.NotChecked;
    private VulkanClearInfo _vulkanClearInfo = VulkanClearInfo.NotChecked;
    private VulkanScene3dRunGate _scene3dGate = VulkanScene3dRunGate.Evaluate();
    private VulkanScene3dInfo _vulkanScene3dInfo = VulkanScene3dInfo.NotChecked;
    private Button? _runMenuButton;
    private MenuItem? _runScene3dMenuItem;
    private VulkanValidationInfo _vulkanValidationInfo = VulkanValidationInfo.Disabled;
    private DispatcherTimer? _viewportResizeRenderTimer;
    private bool _vulkanViewportNativeHostReported;
    private bool _vulkanViewportRendering;
    private int _renderSeq;
    private string _renderLastMode = "无";
    private VulkanScene3dSession? _scene3dSession;
    private SceneOrbitCameraState _lastCameraState = SceneOrbitCameraMotion.CreateDefault();
    private int _cameraRevision;
    private bool _framePending;
    private bool _sessionActive;
    private bool _scene3dAutoStartAttempted;
    private ViewportNavigationDragMode _navigationDragMode = ViewportNavigationDragMode.None;
    private ViewportNavigationElement _navigationActiveElement = ViewportNavigationElement.None;
    private ViewportNavigationElement _navigationHoverElement = ViewportNavigationElement.None;
    private int _navigationLastPixelX;
    private int _navigationLastPixelY;

    // ─── 输入动作映射系统 ───────────────────────────────────
    private EditorInputService _inputService = EditorInputService.Instance;
    private WindowsViewportInputTranslator? _inputTranslator;

    // ─── 视口编辑工具 ────────────────────────────────────
    private ViewportToolPalette? _viewportToolPalette;

    // ─── Transform 路由 ────────────────────────────────────
    private readonly TransformInputRoute _transformRoute = new();
    private bool _moveToolActive;

    // ─── 动作去重守卫 ──────────────────────────────────────
    private bool _frameSelectedPending;

    // ─── 输入上下文栈 ──────────────────────────────────────
    // 使用 List 作为栈，栈顶 = 当前活动上下文。
    // 优先级由 EditorInputContextChain.ContextChain 中的顺序决定。
    private readonly List<EditorInputActionContext> _inputContextStack = new() { EditorInputActionContext.Global };
    private EditorInputActionContext _activeContext => _inputContextStack[^1];
    private int _lastPointerX;
    private int _lastPointerY;

    private static readonly bool s_traceEnabled = Environment.GetEnvironmentVariable("FW_INPUT_TRACE") == "1";

    // ─── 地面拾取状态 ─────────────────────────────────────────────
    private readonly FluidWarfare.Editor.ViewportGround.EditorGroundPointerState _groundPointerState = new();
    private bool _groundPointerUpdatePending; // 调度合并
    private long _lastGroundPointerUpdateTicks;

    // ─── Transform 编辑状态 ─────────────────────────────────────────
    private readonly EditorGroundPlacementState _groundPlacementState = new();
    private readonly EditorWorldDirtyState _worldDirtyState = new();

    public EditorShell()
    {
        // _scene3dGate 由字段初始化器在构造函数体之前执行
        _vulkanScene3dInfo = new VulkanScene3dInfo(
            VulkanScene3dStatus.NotChecked,
            _scene3dGate.Message,
            0, 0, 0, 0, 0, 0, 0, "无", 0, false,
            0, 0, 0,
            _scene3dGate.CanRun ? "可用" : "不可用（已隔离）", 0);
        AvaloniaXamlLoader.Load(this);
        FindShellControls();
        SubscribePanelEvents();
        SetupRunMenuFlyout();
        InitializeLogs();
        LoadSampleProject();
        ProbeVulkanBackend();
        ProbeVulkanValidation();
        AttachedToVisualTree += OnAttachedToVisualTree;
        DetachedFromVisualTree += OnDetachedFromVisualTree;
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
        _runMenuButton = this.FindControl<Button>("RunMenuButton");

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

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            ReportVulkanViewportNativeHost();
            InitializeInputPipeline();
        });
    }

    private void OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        _vulkanViewportNativeHostReported = false;
        _vulkanViewportRendering = false;
        _sessionActive = false;
        _scene3dSession?.Dispose();
        _scene3dSession = null;
        if (_viewportResizeRenderTimer is not null)
        {
            _viewportResizeRenderTimer.Stop();
            _viewportResizeRenderTimer.Tick -= HandleViewportResizeRenderTimerTick;
            _viewportResizeRenderTimer = null;
        }
    }

    private void HandleVulkanViewportNativeHostInfoChanged(object? sender, VulkanViewportNativeHostInfo nativeHostInfo)
    {
        if (!nativeHostInfo.HasNativeHandle || nativeHostInfo.Width < 1 || nativeHostInfo.Height < 1)
        {
            return;
        }

        if (!_vulkanViewportNativeHostReported)
        {
            Dispatcher.UIThread.Post(ReportVulkanViewportNativeHost);
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
        if (!_vulkanBackendInfo.IsAvailable || !_vulkanDeviceInfo.IsCreated)
        {
            return;
        }

        if (_vulkanViewportRendering)
        {
            return;
        }

        _vulkanViewportRendering = true;
        try
        {
            if (_sessionActive && _scene3dSession is not null)
            {
                // 会话活跃时 resize 只重建 swapchain
                var nativeHostInfo = _vulkanViewportHostPanel?.GetNativeHostInfo()
                    ?? VulkanViewportNativeHostInfo.NotAvailable;

                if (nativeHostInfo.Width > 0 && nativeHostInfo.Height > 0)
                {
                    _cameraRevision++;
                    var resizePose = SceneCameraPose.FromOrbitState(_lastCameraState, _cameraRevision);
                    var result = _scene3dSession.Resize(
                        (uint)nativeHostInfo.Width,
                        (uint)nativeHostInfo.Height,
                        resizePose,
                        [.. BuildUnitDrawList()]);

                    if (result.Success)
                    {
                        AppendInfoLog($"Scene3D resize：{result.ViewportWidth}x{result.ViewportHeight}");
                    }
                    else
                    {
                        AppendWarningLog($"Scene3D resize 失败：{result.Message}，回退 Clear。");
                        // 回退：销毁会话，Clear
                        _scene3dSession.Dispose();
                        _scene3dSession = null;
                        _sessionActive = false;
                        ProbeVulkanClear("resize");
                    }
                }
            }
            else
            {
                // resize/maximize 时只执行最小清屏 probe
                ProbeVulkanClear("resize");
            }
        }
        finally
        {
            _vulkanViewportRendering = false;
        }
    }

    private void InitializeLogs()
    {
        var logs = CreateStartupLogs()
            .Select(entry => entry.ToDisplayString())
            .ToArray();

        _debugDockPanel?.LogPanel?.SetLogMessages(logs);
    }

    private void HandleViewportFocused(object? sender, EventArgs e)
    {
        if (_selectedWorldEntity is not null && _worldState is not null)
        {
            // 已有选中实体，保持选择
            _inspectorPanel?.ShowSelection(CreateEntitySelection(_selectedWorldEntity));
            _statusBarPanel?.SetCurrentSelection(_selectedWorldEntity.DisplayName);
            AppendInfoLog("视口获得焦点。");
            AppendInfoLog($"当前 World 占位实体：{_selectedWorldEntity.DisplayName}。");
        }
        else if (_worldState is not null)
        {
            var entities = _worldState.ListEntities();
            if (entities.Count > 0)
            {
                // 自动选中第一个实体
                _selectedWorldEntity = entities[0];
                _firstEntityId = _selectedWorldEntity.EntityId;
                ShowWorldEntitySelection(_selectedWorldEntity);
                AppendInfoLog("视口获得焦点。");
            }
            else
            {
                // World 为空
                _viewportPlaceholderPanel?.ShowEmptyWorld();
                _inspectorPanel?.ShowSelection(CreateDefaultViewportSelection());
                _statusBarPanel?.SetCurrentSelection("3D 视口");
                AppendInfoLog("视口获得焦点。");
                AppendWarningLog("当前 World 没有可显示实体。");
            }
        }
        else
        {
            // World 未创建
            var selection = CreateDefaultViewportSelection();
            _inspectorPanel?.ShowSelection(selection);
            _statusBarPanel?.SetCurrentSelection(selection.DisplayName);
            AppendInfoLog("视口获得焦点。");
        }
    }

    private void HandlePreferencesClicked(object? sender, RoutedEventArgs e)
    {
        AppendInfoLog("已打开偏好设置。");
        OpenPreferencesWindow();
    }

    private void HandleShowInputBindingsClicked(object? sender, RoutedEventArgs e)
    {
        AppendInfoLog("已打开键位设置。");
        OpenPreferencesWindow();
    }

    private void HandleAboutFluidWarfareClicked(object? sender, RoutedEventArgs e)
    {
        AppendInfoLog("已打开关于 FluidWarfare。");
        OpenAboutWindow();
    }

    private Window? _preferencesWindow;
    private Window? _aboutWindow;

    private void OpenPreferencesWindow()
    {
        if (_preferencesWindow is { IsVisible: true }) { _preferencesWindow.Activate(); return; }
        _preferencesWindow = new Preferences.EditorPreferencesWindow();
        _preferencesWindow.Closed += (_, _) => _preferencesWindow = null;
        _preferencesWindow.Show();
    }

    private void OpenAboutWindow()
    {
        if (_aboutWindow is { IsVisible: true }) { _aboutWindow.Activate(); return; }
        _aboutWindow = new About.AboutFluidWarfareWindow();
        _aboutWindow.Closed += (_, _) => _aboutWindow = null;
        _aboutWindow.Show();
    }

    private void OnHierarchyEntitySelected(string? entityId)
    {
        ApplyEntitySelection(entityId, EditorEntitySelectionOrigin.WorldHierarchy);
    }

    private void OnProjectContentSelected(string? relativePath)
    {
        // 项目文件选择：只保存路径，不修改 EntityId，不影响 3D 场景
        if (relativePath is null) return;

        // 在检查器中显示文件信息
        var fileInfo = _contentFiles?.FirstOrDefault(f => f.RelativePath.Replace('\\', '/') == relativePath);
        if (fileInfo is not null)
        {
            var selection = new EditorSelection(
                "项目文件",
                fileInfo.FileName,
                $"路径：{fileInfo.RelativePath}\n类型：{fileInfo.ContentKind}\n目录：{fileInfo.FolderName}");
            _inspectorPanel?.ShowProjectFileSelection(selection);
            _statusBarPanel?.SetCurrentSelection(fileInfo.FileName);
        }
        else
        {
            _inspectorPanel?.ShowNoSelection();
            _statusBarPanel?.SetCurrentSelection(relativePath);
        }
        AppendInfoLog($"项目文件已选择：{relativePath}");
    }

    private void ShowWorldEntitySelection(WorldEntityInfo entityInfo)
    {
        var selection = CreateEntitySelection(entityInfo);
        var position = _worldState?.FindPosition(entityInfo.EntityId);

        _inspectorPanel!.ShowWorldEntitySelection(
            selection,
            entityInfo.EntityId.Value.ToString(),
            position?.Value,
            entityInfo.Source?.RelativePath);
        _inspectorPanel.ScrubEntityId = entityInfo.EntityId.Value.ToString();
        _statusBarPanel?.SetCurrentSelection(entityInfo.DisplayName);

        // 启用地面放置按钮（Session 激活时）
        _inspectorPanel?.SetGroundPlaceEnabled(
            _sessionActive && _scene3dSession?.Status == VulkanScene3dSessionStatus.Active);

        AppendInfoLog($"已选择 {selection.Kind}：{entityInfo.DisplayName}。");
    }

    private void UpdateViewportForEntity(WorldEntityInfo entityInfo)
    {
        var position = _worldState?.FindPosition(entityInfo.EntityId);
        var visualKind = FindVisualKindText(entityInfo.EntityId);

        var summary = new ViewportEntitySummary(
            entityInfo.DisplayName,
            entityInfo.EntityId.ToString(),
            position is not null
                ? $"({position.Value.Value.X}, {position.Value.Value.Y}, {position.Value.Value.Z})"
                : "未知",
            entityInfo.Source?.RelativePath,
            visualKind);

        _viewportPlaceholderPanel?.ShowEntitySummary(summary);
    }

    private string FindVisualKindText(EntityId entityId)
    {
        var renderObj = _renderScene.Objects.FirstOrDefault(o => o.EntityId == entityId);
        return renderObj is not null
            ? ToVisualKindText(renderObj.VisualKind)
            : "未生成";
    }

    private static string ToVisualKindText(RenderObjectVisualKind kind)
    {
        return kind switch
        {
            RenderObjectVisualKind.UnitMarker => "unit_marker",
            _ => kind.ToString()
        };
    }

    private ViewportRenderSceneSummary CreateViewportRenderSceneSummary()
    {
        if (_renderScene.Objects.Count == 0)
        {
            return ViewportRenderSceneSummary.Empty;
        }

        var objects = _renderScene.Objects
            .Select(o => new ViewportRenderObjectSummary(
                o.DisplayName,
                ToVisualKindText(o.VisualKind),
                $"({o.Position.X}, {o.Position.Y}, {o.Position.Z})",
                o.SourcePath))
            .ToArray();

        return new ViewportRenderSceneSummary(objects);
    }

    private void AppendInfoLog(string message)
    {
        AppendLog(EngineLogLevel.Info, message);
    }

    private void AppendWarningLog(string message)
    {
        AppendLog(EngineLogLevel.Warning, message);
    }

    private void AppendErrorLog(string message)
    {
        AppendLog(EngineLogLevel.Error, message);
    }

    private void AppendLog(EngineLogLevel level, string message)
    {
        var entry = EngineLogEntry.Create(
            0.0,
            level,
            "Editor",
            message);

        _debugDockPanel?.LogPanel?.AppendLogMessage(entry.ToDisplayString());
    }

    private void LoadSampleProject()
    {
        var pathResult = SampleProjectPath.TryFindFrom(
            Environment.CurrentDirectory,
            out var projectDirectory);

        if (!pathResult.IsSuccess)
        {
            ShowProjectLoadFailure(pathResult.Error?.Message ?? "未知错误。", ProjectValidationReport.Empty);
            return;
        }

        var loadResult = GameProjectLoader.LoadFromDirectory(projectDirectory);

        if (loadResult.Result.IsSuccess && loadResult.Project is not null)
        {
            ShowLoadedProject(loadResult.Project);
            CreateWorldFromProject(loadResult.Project);
            AppendInfoLog($"已加载示例项目：{loadResult.Project.DisplayName}。");
            return;
        }

        ShowProjectLoadFailure(
            loadResult.Result.Error?.Message ?? "未知错误。",
            loadResult.ValidationReport);
    }

    private void ShowProjectLoadFailure(string message, ProjectValidationReport report)
    {
        // 旧项目面板已移除，使用左侧双树代替
        _viewportPlaceholderPanel?.ShowNoWorldEntity();

        var selection = new EditorSelection(
            "项目加载",
            "加载失败",
            $"项目加载失败：{message}");

        _inspectorPanel?.ShowSelection(selection);
        _statusBarPanel?.SetCurrentSelection("项目加载失败");
        AppendErrorLog($"项目加载失败：{message}");

        if (report.IssueCount > 1)
        {
            AppendWarningLog($"项目校验发现 {report.IssueCount} 个问题，请先修复项目结构。");
        }
    }

    private void CreateWorldFromProject(GameProjectInfo project)
    {
        _worldState = new WorldState();
        _selectedWorldEntity = null;

        if (_contentFiles is null || _contentFiles.Count == 0)
        {
            RebuildAndShowHierarchy();
            _viewportPlaceholderPanel?.ShowNoWorldEntity();
            _viewportPlaceholderPanel?.ShowRenderSceneSummary(ViewportRenderSceneSummary.Empty);
            AppendWarningLog("项目中没有可生成 World 占位实体的单位模板文件。");
            return;
        }

        var seedResult = ProjectContentWorldSeeder.SeedUnitTemplatePlaceholders(
            _worldState,
            _contentFiles);

        if (seedResult.CreatedEntityCount == 0)
        {
            RebuildAndShowHierarchy();
            _viewportPlaceholderPanel?.ShowNoWorldEntity();
            _viewportPlaceholderPanel?.ShowRenderSceneSummary(ViewportRenderSceneSummary.Empty);
            AppendWarningLog("项目中没有可生成 World 占位实体的单位模板文件。");
            return;
        }

        // 记录第一个实体 ID，用于视口点击显示
        var entities = _worldState.ListEntities();
        _firstEntityId = entities.Count > 0 ? entities[0].EntityId : default;

        AppendInfoLog("最小 World 已创建。");

        foreach (var sourcePath in seedResult.SourcePaths)
        {
            AppendInfoLog($"已从项目内容生成 World 占位实体：{sourcePath}。");
        }

        // 生成 RenderScene
        _renderScene = WorldToRenderSceneBuilder.Build(_worldState);
        AppendInfoLog($"RenderScene 已生成，渲染对象数量：{_renderScene.Objects.Count}。");

        // 构建层级树并显示
        RebuildAndShowHierarchy();
        _viewportPlaceholderPanel?.ShowNoWorldEntity();
        _viewportPlaceholderPanel?.ShowRenderSceneSummary(CreateViewportRenderSceneSummary());
    }

    private void ProbeVulkanBackend()
    {
        _vulkanBackendInfo = VulkanBackendProbe.Probe();

        if (_vulkanBackendInfo.IsAvailable)
        {
            AppendInfoLog($"Vulkan 后端状态：{_vulkanBackendInfo.Message}");
        }
        else
        {
            AppendWarningLog($"Vulkan 后端不可用：{_vulkanBackendInfo.Message}");
        }

        _statusBarPanel?.SetVulkanStatus(
            _vulkanBackendInfo.IsAvailable ? "已接入" : "不可用");

        UpdateVulkanViewportHost();
        ProbeVulkanInstance();
    }

    private void ProbeVulkanValidation()
    {
        _vulkanValidationInfo = VulkanValidationAvailabilityProbe.Probe();

        if (_vulkanValidationInfo.IsEnabled)
        {
            AppendInfoLog(_vulkanValidationInfo.Message);
        }
        else if (_vulkanValidationInfo.Status != VulkanValidationStatus.Disabled)
        {
            AppendWarningLog(_vulkanValidationInfo.Message);
        }

        UpdateAllDiagnostics();
    }

    private void ProbeVulkanInstance()
    {
        if (!_vulkanBackendInfo.IsAvailable)
        {
            _vulkanInstanceInfo = new VulkanInstanceInfo(
                VulkanInstanceStatus.Failed,
                "Vulkan 后端不可用，跳过 Instance 创建。",
                "未知",
                0,
                0);
            return;
        }

        _vulkanInstanceInfo = VulkanInstanceProbe.Probe();

        if (_vulkanInstanceInfo.IsCreated)
        {
            AppendInfoLog(
                $"Vulkan Instance 创建成功，API 版本：{_vulkanInstanceInfo.ApiVersionText}，扩展数量：{_vulkanInstanceInfo.ExtensionCount}，用时：{_vulkanInstanceInfo.ElapsedMilliseconds:F2} ms。");
        }
        else
        {
            AppendWarningLog(_vulkanInstanceInfo.Message);
        }

        ProbeVulkanDevice();
    }

    private void ProbeVulkanDevice()
    {
        if (!_vulkanInstanceInfo.IsCreated)
        {
            _vulkanDeviceInfo = new VulkanDeviceInfo(
                VulkanDeviceStatus.Failed,
                "Vulkan Instance 未创建，跳过 Device 创建。",
                "未知",
                "未知",
                -1,
                0);
            ProbeVulkanSurface();
            return;
        }

        _vulkanDeviceInfo = VulkanDeviceProbe.Probe();

        if (_vulkanDeviceInfo.IsCreated)
        {
            AppendInfoLog(
                $"Vulkan Device 创建成功，显卡：{_vulkanDeviceInfo.PhysicalDeviceName}，类型：{_vulkanDeviceInfo.PhysicalDeviceTypeText}，图形队列族：{_vulkanDeviceInfo.GraphicsQueueFamilyIndex}，用时：{_vulkanDeviceInfo.ElapsedMilliseconds:F2} ms。");
        }
        else
        {
            AppendWarningLog(_vulkanDeviceInfo.Message);
        }

        ProbeVulkanSurface();
    }

    private void ProbeVulkanSurface()
    {
        if (!_vulkanDeviceInfo.IsCreated)
        {
            _vulkanSurfaceInfo = new VulkanSurfaceInfo(
                VulkanSurfaceStatus.Failed,
                "Vulkan Device 未创建，跳过 Surface 创建。",
                "未知",
                false,
                0);

            ShowVulkanSurfaceInfo();
            return;
        }

        var nativeHostInfo = _vulkanViewportHostPanel?.GetNativeHostInfo()
            ?? VulkanViewportNativeHostInfo.NotAvailable;

        if (!nativeHostInfo.HasNativeHandle)
        {
            _vulkanSurfaceInfo = new VulkanSurfaceInfo(
                VulkanSurfaceStatus.Failed,
                nativeHostInfo.Message,
                nativeHostInfo.PlatformText,
                false,
                0);

            ShowVulkanSurfaceInfo();
            return;
        }

        _vulkanSurfaceInfo = VulkanSurfaceProbe.ProbeWindows(
            nativeHostInfo.InstanceHandle,
            nativeHostInfo.WindowHandle);

        ShowVulkanSurfaceInfo();
    }

    private void ShowVulkanSurfaceInfo()
    {
        if (_vulkanSurfaceInfo.IsCreated)
        {
            AppendInfoLog(
                $"Vulkan Surface 创建成功，平台：{_vulkanSurfaceInfo.PlatformText}，用时：{_vulkanSurfaceInfo.ElapsedMilliseconds:F2} ms。");
        }
        else if (_vulkanSurfaceInfo.Status == VulkanSurfaceStatus.NotChecked)
        {
            AppendInfoLog(_vulkanSurfaceInfo.Message);
        }
        else
        {
            AppendWarningLog(_vulkanSurfaceInfo.Message);
        }

        UpdateAllDiagnostics();
    }

    private void ReportVulkanViewportNativeHost()
    {
        if (_vulkanViewportNativeHostReported)
        {
            return;
        }

        _vulkanViewportNativeHostReported = true;

        var nativeHostInfo = _vulkanViewportHostPanel?.GetNativeHostInfo()
            ?? VulkanViewportNativeHostInfo.NotAvailable;

        if (nativeHostInfo.HasNativeHandle)
        {
            AppendInfoLog(
                $"Windows Vulkan 视口子窗口已创建，HWND：0x{nativeHostInfo.WindowHandle.ToInt64():X16}。");
            ProbeVulkanSwapchain();
            ProbeVulkanClear("初始启动");
            ReportScene3dIsolation();
            TryAutoStartScene3dSession();
        }
        else
        {
            AppendWarningLog(nativeHostInfo.Message);
            ProbeVulkanSurface();
        }
    }

    private void ProbeVulkanSwapchain()
    {
        var nativeHostInfo = _vulkanViewportHostPanel?.GetNativeHostInfo()
            ?? VulkanViewportNativeHostInfo.NotAvailable;

        if (!TryGetValidViewportSize(nativeHostInfo, out var viewportWidth, out var viewportHeight, out var viewportSizeMessage))
        {
            _vulkanSwapchainInfo = new VulkanSwapchainInfo(
                VulkanSwapchainStatus.Failed,
                viewportSizeMessage,
                0, "未知", "未知", 0, 0, 0);
            ShowVulkanSwapchainInfo();
            return;
        }

        if (!nativeHostInfo.HasNativeHandle || nativeHostInfo.InstanceHandle == 0 || nativeHostInfo.WindowHandle == 0)
        {
            _vulkanSwapchainInfo = new VulkanSwapchainInfo(
                VulkanSwapchainStatus.Failed,
                "缺少 Windows 原生视口句柄，跳过 Swapchain 创建。",
                0, "未知", "未知", 0, 0, 0);
            ShowVulkanSwapchainInfo();
            return;
        }

        _vulkanSwapchainInfo = VulkanSwapchainProbe.ProbeWindows(
            nativeHostInfo.InstanceHandle,
            nativeHostInfo.WindowHandle,
            viewportWidth,
            viewportHeight);

        ShowVulkanSwapchainInfo();
    }

    private void ShowVulkanSwapchainInfo()
    {
        if (_vulkanSwapchainInfo.IsCreated)
        {
            AppendInfoLog(
                $"Vulkan Swapchain 创建成功，图像数量：{_vulkanSwapchainInfo.ImageCount}，" +
                $"格式：{_vulkanSwapchainInfo.SurfaceFormatText}，" +
                $"Present：{_vulkanSwapchainInfo.PresentModeText}，" +
                $"尺寸：{_vulkanSwapchainInfo.Width}x{_vulkanSwapchainInfo.Height}，" +
                $"用时：{_vulkanSwapchainInfo.ElapsedMilliseconds:F2} ms。");
        }
        else if (_vulkanSwapchainInfo.Status != VulkanSwapchainStatus.NotChecked)
        {
            AppendWarningLog($"Vulkan Swapchain 创建失败：{_vulkanSwapchainInfo.Message}");
        }

        UpdateAllDiagnostics();
    }

    private void ProbeVulkanClear(string reason = "resize")
    {
        var nativeHostInfo = _vulkanViewportHostPanel?.GetNativeHostInfo()
            ?? VulkanViewportNativeHostInfo.NotAvailable;

        if (!TryGetValidViewportSize(nativeHostInfo, out var viewportWidth, out var viewportHeight, out var viewportSizeMessage))
        {
            _vulkanClearInfo = new VulkanClearInfo(
                VulkanClearStatus.Failed, viewportSizeMessage, "未知", 0, 0, 0);
            ShowVulkanClearInfo();
            return;
        }

        if (!nativeHostInfo.HasNativeHandle || nativeHostInfo.InstanceHandle == 0 || nativeHostInfo.WindowHandle == 0)
        {
            _vulkanClearInfo = new VulkanClearInfo(
                VulkanClearStatus.Failed, "缺少原生句柄，跳过清屏。", "未知", 0, 0, 0);
            ShowVulkanClearInfo();
            return;
        }

        _renderSeq++;
        AppendInfoLog($"RenderSeq-{_renderSeq:D3} | Clear | {viewportWidth}x{viewportHeight} | {reason}");

        _vulkanClearInfo = VulkanClearProbe.ProbeWindows(
            nativeHostInfo.InstanceHandle,
            nativeHostInfo.WindowHandle,
            viewportWidth,
            viewportHeight);

        ShowVulkanClearInfo();
    }

    private void ShowVulkanClearInfo()
    {
        if (_vulkanClearInfo.IsSucceeded)
        {
            _renderLastMode = "Clear";
            AppendInfoLog(
                $"Vulkan 最小清屏成功，颜色：{_vulkanClearInfo.ClearColorText}，" +
                $"尺寸：{_vulkanClearInfo.Width}x{_vulkanClearInfo.Height}，" +
                $"用时：{_vulkanClearInfo.ElapsedMilliseconds:F2} ms。");
        }
        else if (_vulkanClearInfo.Status != VulkanClearStatus.NotChecked)
        {
            AppendWarningLog($"Vulkan 最小清屏失败：{_vulkanClearInfo.Message}");
        }

        UpdateVulkanViewportStatusLine();

        UpdateAllDiagnostics();
    }

    private void ReportScene3dIsolation()
    {
        AppendInfoLog(_scene3dGate.Message);
        ShowVulkanScene3DInfo();
    }

    private void TryAutoStartScene3dSession()
    {
        if (_scene3dAutoStartAttempted) return;
        _scene3dAutoStartAttempted = true;

        if (!_scene3dGate.CanRun)
        {
            AppendWarningLog($"Scene3D 自动启动跳过：{_scene3dGate.Message}");
            return;
        }

        if (_scene3dSession is not null || _sessionActive)
        {
            AppendInfoLog("Scene3D 会话已存在，跳过自动启动。");
            return;
        }

        if (_renderScene.Objects.Count == 0)
        {
            AppendWarningLog("Scene3D 自动启动跳过：RenderScene 为空。");
            return;
        }

        AppendInfoLog("Scene3D 自动启动...");
        StartScene3dSession();
    }

    private void HandleScene3dRunRequested(object? sender, EventArgs e)
    {
        var currentGate = VulkanScene3dRunGate.Evaluate();
        _scene3dGate = currentGate with { }; // update gate with fresh Evaluate
        // Refactor: can't reassign readonly field, use the gate's current state
        // Actually _scene3dGate is readonly, but we need to re-evaluate.
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
            _vulkanScene3dInfo = new VulkanScene3dInfo(
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
            _vulkanScene3dInfo = new VulkanScene3dInfo(
                VulkanScene3dStatus.Failed, "场景3D：视口未就绪，跳过运行。",
                0, 0, 0, 0, 0, 0, 0, "无", 0, false,
                0, 0, 0, "不可用", 0);
            ShowVulkanScene3DInfo();
            return;
        }

        ProbeVulkanScene3D();
    }

    private void SetupRunMenuFlyout()
    {
        if (_runMenuButton is null) return;

        var flyout = new MenuFlyout();
        flyout.Opened += (_, _) => AppendInfoLog("运行菜单已打开。");

        _runScene3dMenuItem = new MenuItem { Header = "重新启动 Scene3D 会话" };
        _runScene3dMenuItem.Click += HandleRunScene3dMenuClicked;
        flyout.Items.Add(_runScene3dMenuItem);

        _runMenuButton.Flyout = flyout;
    }

    private void HandleRunScene3dMenuClicked(object? sender, RoutedEventArgs e)
    {
        if (_scene3dSession is not null)
        {
            // 重新启动：先清理旧 Session
            var oldSession = _scene3dSession;
            _scene3dSession = null;
            oldSession.Dispose();

            if (VulkanScene3dSwapchainResources.LiveCount != 0)
            {
                AppendErrorLog(
                    $"拒绝重启 Scene3D：仍有 {VulkanScene3dSwapchainResources.LiveCount} 个 Swapchain 存活。");
                _sessionActive = false;
                return;
            }
        }

        if (!_scene3dGate.CanRun)
        {
            AppendWarningLog(_scene3dGate.Message);
            return;
        }

        StartScene3dSession();
    }

    private void StartScene3dSession()
    {
        var nativeHostInfo = _vulkanViewportHostPanel?.GetNativeHostInfo()
            ?? VulkanViewportNativeHostInfo.NotAvailable;

        if (!nativeHostInfo.HasNativeHandle || nativeHostInfo.Width < 1 || nativeHostInfo.Height < 1)
        {
            AppendWarningLog("Scene3D 会话：视口未就绪。");
            return;
        }

        AppendInfoLog("正在启动 Scene3D 会话...");
        _sessionActive = true;

        // 保存 RenderScene 快照（网格 + 世界主轴共享同一 LineList Pipeline）
        var gridOnly = VulkanScene3dVertices.BuildGrid(20, 2);
        var axisVerts = VulkanScene3dVertices.BuildAxes(20, 8);
        var combinedGridVertices = new VulkanScene3dVertex[gridOnly.Length + axisVerts.Length];
        Array.Copy(gridOnly, 0, combinedGridVertices, 0, gridOnly.Length);
        Array.Copy(axisVerts, 0, combinedGridVertices, gridOnly.Length, axisVerts.Length);
        var unitVertices = VulkanScene3dVertices.BuildCube(0, 0, 0, 1.0f);

        var unitDraws = new List<VulkanScene3dUnitDrawInfo>();
        foreach (var obj in _renderScene.Objects)
        {
            if (obj.VisualKind != RenderObjectVisualKind.UnitMarker) continue;
            var p = obj.Placement;
            var center = p?.VisualCenter ?? new Vector3d(obj.Position.X, obj.Position.Y, obj.Position.Z + 0.5);
            unitDraws.Add(new VulkanScene3dUnitDrawInfo(
                obj.EntityId.Value.ToString(),
                (float)center.X,
                (float)center.Y,
                (float)center.Z,
                (float)RenderUnitPlacement.Scale));
        }

        _lastCameraState = SceneOrbitCameraMotion.CreateDefault();
        _cameraRevision++;
        var sessionPose = SceneCameraPose.FromOrbitState(_lastCameraState, _cameraRevision);

        var session = new VulkanScene3dSession();
        var result = session.Start(
            nativeHostInfo.InstanceHandle,
            nativeHostInfo.WindowHandle,
            (uint)nativeHostInfo.Width,
            (uint)nativeHostInfo.Height,
            sessionPose,
            combinedGridVertices.AsSpan(),
            unitVertices.AsSpan(),
            [.. unitDraws]);

        if (result.Success)
        {
            _scene3dSession = session;
            _renderLastMode = "Scene3D";
            _renderSeq++;
            AppendInfoLog($"RenderSeq-{_renderSeq:D3} | Scene3D Session 启动 | " +
                $"{result.ViewportWidth}x{result.ViewportHeight}");
            AppendInfoLog(result.Message);
        }
        else
        {
            _scene3dSession = null;
            session.Dispose();
            _sessionActive = false;
            AppendErrorLog($"Scene3D 会话启动失败：{result.Message}");
            AppendInfoLog($"Swapchain LiveCount：{VulkanScene3dSwapchainResources.LiveCount}");
        }

        UpdateVulkanViewportStatusLine();
        UpdateAllDiagnostics();
    }

    // ─── 输入动作映射系统 ──────────────────────────────────
    // 数据流：Win32 WM_ → RawPointerButtonDown/KeyDown 等
    //       → WindowsViewportInputTranslator.OnRaw*()
    //       → EditorInputMatch → ExecuteInputAction() → 统一执行方法

    private void InitializeInputPipeline()
    {
        _inputService.Initialize();
        _inputTranslator = new WindowsViewportInputTranslator(_inputService.CurrentSnapshot);
        _inputService.SnapshotReplaced += snapshot =>
        {
            _inputTranslator?.OnSnapshotReplaced(snapshot);
        };
    }

    private void HandleRawKeyDown(int virtualKeyCode)
    {
        if (s_traceEnabled)
            System.Diagnostics.Debug.WriteLine(
                $"[InputTrace-Shell] RawKeyDown vk=0x{virtualKeyCode:X2}");

        // Esc: 取消活动变换
        if (virtualKeyCode == 0x1B && _transformRoute.Session.IsActive)
        {
            _transformRoute.CancelDrag();
            AppendInfoLog("变换已取消");
            return;
        }

        // G: 进入移动模式（Blender 风格）
        if (virtualKeyCode == 0x47 && _selectedWorldEntity is not null)
        {
            _moveToolActive = true;
            _viewportToolPalette?.SetActiveTool(ViewportEditorTool.Move);
            AppendInfoLog("移动模式（使用 Gizmo 拖动）");
            return;
        }

        if (_inputTranslator is null)
        {
            if (s_traceEnabled)
                System.Diagnostics.Debug.WriteLine("[InputTrace-Shell] _inputTranslator is NULL!");
            return;
        }
        var match = _inputTranslator.OnRawKeyDown(virtualKeyCode, _lastPointerX, _lastPointerY);
        ExecuteInputAction(match);
    }

    private void HandleRawKeyUp(int virtualKeyCode)
    {
        _inputTranslator?.OnRawKeyUp(virtualKeyCode);
    }

    private void HandleRawPointerButtonDown(int buttonCode, int x, int y)
    {
        if (s_traceEnabled)
            System.Diagnostics.Debug.WriteLine(
                $"[InputTrace-Shell] RawPointerButtonDown btn={buttonCode} x={x} y={y}");
        _lastPointerX = x;
        _lastPointerY = y;

        if (_inputTranslator is null)
        {
            if (s_traceEnabled)
                System.Diagnostics.Debug.WriteLine("[InputTrace-Shell] _inputTranslator is NULL!");
            return;
        }
        var match = _inputTranslator.OnRawPointerButtonDown(buttonCode, x, y);
        ExecuteInputAction(match);
    }

    /// <summary>
    /// 原始鼠标移动入口。转发到输入转换器。
    /// </summary>
    private void HandleRawPointerMoved(int x, int y)
    {
        _lastPointerX = x;
        _lastPointerY = y;

        if (_inputTranslator is null) return;
        var match = _inputTranslator.OnRawPointerMoved(x, y);
        ExecuteInputAction(match);
    }

    private void HandleRawPointerButtonUp(int buttonCode, int x, int y)
    {
        _inputTranslator?.OnRawPointerButtonUp(buttonCode);
    }

    private void HandleRawInputFocusLost()
    {
        _inputTranslator?.OnRawInputFocusLost();
        _transformRoute.CancelDrag();
    }

    // ─── 场景工具仲裁 ──────────────────────────────────

    private ViewportSceneToolPressResult HandleSceneToolPointerPressed(int x, int y)
    {
        if (!_moveToolActive || _selectedWorldEntity is null)
            return ViewportSceneToolPressResult.NotHandled;

        var pos = _worldState?.FindPosition(_selectedWorldEntity.EntityId);
        if (pos is null) return ViewportSceneToolPressResult.NotHandled;

        // 启动编辑事务
        if (!TransformEditSessionStart.TryBegin(_worldState, _selectedWorldEntity.EntityId,
                TransformEditKind.Translation, _worldDirtyState.IsDirty, _transformRoute.Session))
            return ViewportSceneToolPressResult.NotHandled;

        var pivot = pos.Value.Value;
        var result = _transformRoute.OnPointerPressed(1, x, y, pivot);
        if (!result.Started)
        {
            _transformRoute.Session.Cancel();
            return ViewportSceneToolPressResult.NotHandled;
        }

        return ViewportSceneToolPressResult.BeginDrag;
    }

    private void HandleSceneToolPointerReleased(int x, int y)
    {
        var result = _transformRoute.OnPointerReleased();
        if (!result.Handled) return;

        var finalPos = _transformRoute.Session.PreviewTransform.Position;
        ApplyEntityTransform(finalPos, EditorEntityTransformOrigin.MoveTool);
        AppendInfoLog($"移动完成 ({finalPos.X:F3}, {finalPos.Y:F3}, {finalPos.Z:F3})");
    }

    // ─── 视口工具 ──────────────────────────────────────

    private void HandleViewportToolChanged(ViewportEditorTool tool)
    {
        _moveToolActive = tool == ViewportEditorTool.Move;
        if (_moveToolActive && _selectedWorldEntity is null)
            _statusBarPanel?.SetCurrentSelection("请先选择实体。");
    }

    private void HandleRawMouseWheel(int delta, int packedModifiers)
    {
        if (s_traceEnabled)
            System.Diagnostics.Debug.WriteLine(
                $"[InputTrace-Shell] RawMouseWheel delta={delta} mk=0x{packedModifiers:X4}");
        if (_inputTranslator is null)
        {
            if (s_traceEnabled)
                System.Diagnostics.Debug.WriteLine("[InputTrace-Shell] _inputTranslator is NULL!");
            return;
        }
        var match = _inputTranslator.OnRawMouseWheel(delta, packedModifiers,
            _lastPointerX, _lastPointerY);
        ExecuteInputAction(match);
    }

    /// <summary>
    /// 统一动作调度入口。所有动作 —— 键盘快捷键、鼠标手势、覆盖层按钮 ——
    /// 最终都调用此方法分发到具体执行函数。
    /// </summary>
    private void ExecuteInputAction(EditorInputMatch match)
    {
        if (!match.IsMatch || match.Definition is null) return;

        // 上下文过滤：当前活动上下文必须允许该动作的上下文
        if (!CanExecuteInCurrentContext(match.Definition.Context))
        {
            if (s_traceEnabled)
                System.Diagnostics.Debug.WriteLine(
                    $"[InputTrace-Shell] BLOCKED action=\"{match.ActionId}\" " +
                    $"ctx={match.Definition.Context} activeCtx={_activeContext}");
            return;
        }

        if (s_traceEnabled)
            System.Diagnostics.Debug.WriteLine(
                $"[InputTrace-Shell] Executing action=\"{match.ActionId}\" " +
                $"kind={match.ValueKind} dx={match.DeltaX} dy={match.DeltaY} wheel={match.WheelDelta}");

        switch (match.ActionId)
        {
            case "viewport.orbit":
                // DeltaX → Yaw（左右旋转），DeltaY → Pitch（上下俯仰）
                // 负号与已正常工作的 Gizmo Orbit 方向保持一致
                ExecuteViewportOrbit(-match.DeltaX, -match.DeltaY);
                break;
            case "viewport.pan":
                ExecuteViewportPan(match.DeltaX, match.DeltaY);
                break;
            case "viewport.dolly":
                ExecuteViewportDolly(match.DeltaY);
                break;
            case "viewport.zoom":
                ExecuteViewportZoom(match.WheelDelta);
                break;
            case "viewport.frame_all":
                ExecuteViewportFrameAll();
                break;
            case "viewport.frame_selected":
                ExecuteViewportFrameSelected();
                break;
            case "viewport.toggle_projection":
                ExecuteViewportToggleProjection();
                break;
            case "viewport.view_front":
            case "viewport.view_back":
            case "viewport.view_right":
            case "viewport.view_left":
            case "viewport.view_top":
            case "viewport.view_bottom":
                ExecuteViewportSnapToView(match.ActionId);
                break;
            case "tool.select":
                _viewportToolPalette?.SetActiveTool(ViewportEditorTool.Select);
                break;
            case "tool.move":
                _viewportToolPalette?.SetActiveTool(ViewportEditorTool.Move);
                break;
            case "editor.open_preferences":
                ExecuteOpenPreferences();
                break;
            case "tool.cancel_current":
                ExecuteCancelCurrentTool();
                break;
            case "transform.apply":
                ExecuteTransformApply();
                break;
            case "transform.reset_draft":
                ExecuteTransformResetDraft();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 上下文过滤。使用 EditorInputContextChain 的显式候选链判断
    /// 动作上下文在当前活动上下文中是否允许执行。
    /// </summary>
    private bool CanExecuteInCurrentContext(EditorInputActionContext actionContext)
        => EditorInputContextChain.IsContextAllowed(actionContext, _activeContext);

    /// <summary>
    /// 推送输入上下文（更高优先级上下文会拦截低优先级动作）。
    /// 只在严格提升优先级时压栈。
    /// </summary>
    public void PushInputContext(EditorInputActionContext context)
    {
        if (EditorInputContextChain.IndexOf(context) < EditorInputContextChain.IndexOf(_activeContext))
            _inputContextStack.Add(context);
    }

    /// <summary>
    /// 弹出输入上下文（恢复到栈中上一层）。
    /// </summary>
    public void PopInputContext(EditorInputActionContext context)
    {
        if (_inputContextStack.Count > 1 && _inputContextStack[^1] == context)
            _inputContextStack.RemoveAt(_inputContextStack.Count - 1);
    }

    // ─── 统一动作执行方法 ──────────────────────────────────

    private void ExecuteViewportOrbit(float deltaYaw, float deltaPitch)
    {
        if (!_sessionActive || _scene3dSession?.Status != VulkanScene3dSessionStatus.Active)
            return;
        if (deltaYaw == 0 && deltaPitch == 0) return;
        _lastCameraState = SceneOrbitCameraMotion.Orbit(_lastCameraState, deltaYaw, deltaPitch);
        ScheduleScene3dFrame(VulkanScene3dFrameReason.CameraPan);
    }

    private void ExecuteViewportPan(int deltaX, int deltaY)
    {
        if (!_sessionActive || _scene3dSession?.Status != VulkanScene3dSessionStatus.Active)
            return;
        if (deltaX == 0 && deltaY == 0) return;
        var h = _vulkanViewportHostPanel?.GetNativeHostInfo().Height ?? 1;
        _lastCameraState = SceneOrbitCameraMotion.Pan(_lastCameraState, deltaX, deltaY, Math.Max(1, h));
        ScheduleScene3dFrame(VulkanScene3dFrameReason.CameraPan);
    }

    private void ExecuteViewportDolly(float deltaPixels)
    {
        if (!_sessionActive || _scene3dSession?.Status != VulkanScene3dSessionStatus.Active)
            return;
        if (deltaPixels == 0) return;
        _lastCameraState = SceneOrbitCameraMotion.Dolly(_lastCameraState, deltaPixels);
        ScheduleScene3dFrame(VulkanScene3dFrameReason.CameraZoom);
    }

    private void ExecuteViewportZoom(float wheelNotches)
    {
        if (!_sessionActive || _scene3dSession?.Status != VulkanScene3dSessionStatus.Active)
            return;
        if (wheelNotches == 0) return;
        _lastCameraState = SceneOrbitCameraMotion.Zoom(_lastCameraState, wheelNotches);
        ScheduleScene3dFrame(VulkanScene3dFrameReason.CameraZoom);
    }

    private void ExecuteViewportFrameAll()
    {
        if (!_sessionActive || _scene3dSession is null) return;
        _lastCameraState = SceneOrbitCameraMotion.FrameAll();
        ScheduleScene3dFrame(VulkanScene3dFrameReason.CameraReset);
    }

    private void ExecuteViewportFrameSelected()
    {
        if (_frameSelectedPending) return;
        _frameSelectedPending = true;
        try
        {
            if (!_sessionActive || _scene3dSession is null) return;
            if (_selectedWorldEntity is null)
            {
                _statusBarPanel?.SetCurrentSelection("没有可聚焦的世界实体。");
                return;
            }

            var pos = _worldState?.FindPosition(_selectedWorldEntity.EntityId);
            if (pos is null) return;

            var p = pos.Value.Value;
            var placement = new RenderUnitPlacement(p);
            _lastCameraState = SceneOrbitCameraMotion.FrameSelected(
                _lastCameraState,
                (float)placement.VisualCenter.X,
                (float)placement.VisualCenter.Y,
                (float)placement.VisualCenter.Z,
                (float)RenderUnitPlacement.HalfExtent);
            _statusBarPanel?.SetCurrentSelection($"已聚焦实体 {_selectedWorldEntity.DisplayName}。");
            ScheduleScene3dFrame(VulkanScene3dFrameReason.CameraReset);
        }
        finally
        {
            Dispatcher.UIThread.Post(() => _frameSelectedPending = false);
        }
    }

    private void ExecuteViewportToggleProjection()
    {
        if (!_sessionActive || _scene3dSession is null) return;
        if (_scene3dSession.Status != VulkanScene3dSessionStatus.Active) return;

        _lastCameraState = SceneNavigationCameraMotion.ToggleProjection(_lastCameraState);
        var mode = _lastCameraState.ProjectionMode;
        AppendInfoLog($"投影模式切换为：{mode}");
        ScheduleScene3dFrame(VulkanScene3dFrameReason.CameraReset);
    }

    private void ExecuteViewportSnapToView(string actionId)
    {
        if (!_sessionActive || _scene3dSession is null) return;
        if (_scene3dSession.Status != VulkanScene3dSessionStatus.Active) return;

        var view = actionId switch
        {
            "viewport.view_front" => SceneNavigationView.PositiveY,
            "viewport.view_back" => SceneNavigationView.NegativeY,
            "viewport.view_right" => SceneNavigationView.PositiveX,
            "viewport.view_left" => SceneNavigationView.NegativeX,
            "viewport.view_top" => SceneNavigationView.PositiveZ,
            "viewport.view_bottom" => SceneNavigationView.NegativeZ,
            _ => SceneNavigationView.Free
        };
        if (view == SceneNavigationView.Free) return;

        _lastCameraState = SceneNavigationCameraMotion.SnapToView(_lastCameraState, view);
        AppendInfoLog($"切换到：{actionId}");
        ScheduleScene3dFrame(VulkanScene3dFrameReason.CameraReset);
    }

    private void ExecuteOpenPreferences()
    {
        OpenPreferencesWindow();
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

    // ─── 相机输入处理 ─────────────────────────────────────────

    private void HandleCameraOrbit(float deltaYaw, float deltaPitch)
    {
        if (!_sessionActive || _scene3dSession is null) return;
        if (_scene3dSession.Status != VulkanScene3dSessionStatus.Active) return;

        _lastCameraState = SceneOrbitCameraMotion.Orbit(_lastCameraState, deltaYaw, deltaPitch);
        ScheduleScene3dFrame(VulkanScene3dFrameReason.CameraPan);
    }

    private void HandleCameraPan(int deltaX, int deltaY, int viewportW, int viewportH)
    {
        if (!_sessionActive || _scene3dSession is null) return;
        if (_scene3dSession.Status != VulkanScene3dSessionStatus.Active) return;

        _lastCameraState = SceneOrbitCameraMotion.Pan(_lastCameraState, deltaX, deltaY, viewportH);
        ScheduleScene3dFrame(VulkanScene3dFrameReason.CameraPan);
    }

    private void HandleCameraDolly(float deltaPixels)
    {
        if (!_sessionActive || _scene3dSession is null) return;
        if (_scene3dSession.Status != VulkanScene3dSessionStatus.Active) return;

        _lastCameraState = SceneOrbitCameraMotion.Dolly(_lastCameraState, deltaPixels);
        ScheduleScene3dFrame(VulkanScene3dFrameReason.CameraZoom);
    }

    private void HandleCameraZoom(float wheelNotches)
    {
        if (!_sessionActive || _scene3dSession is null) return;
        if (_scene3dSession.Status != VulkanScene3dSessionStatus.Active) return;

        _lastCameraState = SceneOrbitCameraMotion.Zoom(_lastCameraState, wheelNotches);
        ScheduleScene3dFrame(VulkanScene3dFrameReason.CameraZoom);
    }

    private void HandleCameraReset()
    {
        if (!_sessionActive || _scene3dSession is null) return;

        _lastCameraState = SceneOrbitCameraMotion.FrameAll();
        ScheduleScene3dFrame(VulkanScene3dFrameReason.CameraReset);
    }

    private void HandleCameraProjectionToggle()
    {
        if (!_sessionActive || _scene3dSession is null) return;
        if (_scene3dSession.Status != VulkanScene3dSessionStatus.Active) return;

        _lastCameraState = FluidWarfare.Render.Camera.Navigation.SceneNavigationCameraMotion.ToggleProjection(_lastCameraState);
        var mode = _lastCameraState.ProjectionMode;
        AppendInfoLog($"投影模式切换为：{mode}");
        ScheduleScene3dFrame(VulkanScene3dFrameReason.CameraReset);
    }

    private void HandleNumpadPeriod()
    {
        if (!_sessionActive || _scene3dSession is null) return;
        if (_selectedWorldEntity is null)
        {
            _statusBarPanel?.SetCurrentSelection("没有可聚焦的世界实体。");
            return;
        }

        var pos = _worldState?.FindPosition(_selectedWorldEntity.EntityId);
        if (pos is null) return;

        var p = pos.Value.Value;
        _lastCameraState = SceneOrbitCameraMotion.FrameSelected(
            _lastCameraState,
            (float)p.X, (float)p.Y, (float)p.Z, 2.5f);

        ScheduleScene3dFrame(VulkanScene3dFrameReason.CameraReset);
    }

    // ─── Overlay 导航输入 ──────────────────────────────
    // HitTest uses the last presented overlay snapshot so input matches visible pixels.
    private ViewportNavigationPressResult HandleOverlayPointerPressed(int pixelX, int pixelY)
    {
        var layout = GetPresentedNavigationLayout();
        if (layout is null) return ViewportNavigationPressResult.NotHandled;

        var element = layout.HitTest(pixelX, pixelY);
        if (element == ViewportNavigationElement.None)
            return ViewportNavigationPressResult.NotHandled;

        _navigationActiveElement = element;
        _navigationLastPixelX = pixelX;
        _navigationLastPixelY = pixelY;
        SetOverlayVisualState(element, element);

        switch (element)
        {
            case ViewportNavigationElement.PositiveX:
            case ViewportNavigationElement.NegativeX:
            case ViewportNavigationElement.PositiveY:
            case ViewportNavigationElement.NegativeY:
            case ViewportNavigationElement.PositiveZ:
            case ViewportNavigationElement.NegativeZ:
                SnapCameraToNavigationElement(element);
                SetOverlayVisualState(element, ViewportNavigationElement.None);
                EndOverlayDrag(false);
                return ViewportNavigationPressResult.HandledClick;

            case ViewportNavigationElement.GizmoCenter:
                _navigationDragMode = ViewportNavigationDragMode.GizmoOrbit;
                return ViewportNavigationPressResult.BeginDrag;

            case ViewportNavigationElement.PanButton:
                _navigationDragMode = ViewportNavigationDragMode.Pan;
                return ViewportNavigationPressResult.BeginDrag;

            case ViewportNavigationElement.FrameButton:
                ExecuteViewportFrameAll();
                return ViewportNavigationPressResult.HandledClick;

            case ViewportNavigationElement.ProjectionButton:
                ExecuteViewportToggleProjection();
                return ViewportNavigationPressResult.HandledClick;

            default:
                return ViewportNavigationPressResult.HandledClick;
        }
    }

    private bool HandleOverlayPointerMoved(int pixelX, int pixelY)
    {
        // 拖动已由 NativeHost 捕获后，即使 resize 期间 Presented Layout 暂时不可用，
        // 也必须继续吞掉鼠标消息，避免穿透到地面 Hover/Picking。
        if (_navigationDragMode != ViewportNavigationDragMode.None)
        {
            var deltaX = pixelX - _navigationLastPixelX;
            var deltaY = pixelY - _navigationLastPixelY;
            _navigationLastPixelX = pixelX;
            _navigationLastPixelY = pixelY;

            if (deltaX == 0 && deltaY == 0)
                return true;

            var viewportHeight = GetPresentedNavigationLayout()?.ViewportHeight
                ?? _vulkanViewportHostPanel?.GetNativeHostInfo().Height
                ?? 1;
            ApplyNavigationDrag(deltaX, deltaY, Math.Max(1, viewportHeight));
            return true;
        }

        var layout = GetPresentedNavigationLayout();
        if (layout is null) return false;

        var element = layout.HitTest(pixelX, pixelY);
        if (element != _navigationHoverElement)
            SetOverlayVisualState(element, ViewportNavigationElement.None);

        return element != ViewportNavigationElement.None;
    }

    private void HandleOverlayPointerReleased()
    {
        EndOverlayDrag(true);
    }

    private void HandleOverlayCaptureLost()
    {
        EndOverlayDrag(true);
    }

    private ViewportNavigationLayout? GetPresentedNavigationLayout()
    {
        if (_scene3dSession is null || _vulkanViewportHostPanel is null)
            return null;

        var snapshot = _scene3dSession.LastPresentedOverlaySnapshot;
        if (!snapshot.IsAvailable || snapshot.Layout is null)
            return null;

        var host = _vulkanViewportHostPanel.GetNativeHostInfo();
        if (host.Width != snapshot.ViewportWidth || host.Height != snapshot.ViewportHeight)
            return null;

        return snapshot.Layout;
    }

    private void SetOverlayVisualState(
        ViewportNavigationElement hovered,
        ViewportNavigationElement active)
    {
        _navigationHoverElement = hovered;
        _navigationActiveElement = active;

        if (_scene3dSession?.SetNavigationOverlayState(hovered, active) == true)
            ScheduleScene3dFrame(VulkanScene3dFrameReason.OverlayNavigationChanged);
    }

    private void EndOverlayDrag(bool requestCleanupFrame)
    {
        var hadVisualState = _navigationActiveElement != ViewportNavigationElement.None ||
            _navigationDragMode != ViewportNavigationDragMode.None;

        _navigationDragMode = ViewportNavigationDragMode.None;
        _navigationActiveElement = ViewportNavigationElement.None;
        _navigationLastPixelX = 0;
        _navigationLastPixelY = 0;

        if (requestCleanupFrame && hadVisualState)
            SetOverlayVisualState(_navigationHoverElement, ViewportNavigationElement.None);
    }

    private void ApplyNavigationDrag(int deltaX, int deltaY, int viewportHeight)
    {
        if (!_sessionActive || _scene3dSession is null) return;
        if (_scene3dSession.Status != VulkanScene3dSessionStatus.Active) return;

        var previous = _lastCameraState;
        _lastCameraState = _navigationDragMode switch
        {
            ViewportNavigationDragMode.GizmoOrbit =>
                SceneOrbitCameraMotion.Orbit(_lastCameraState, -deltaX, -deltaY),
            ViewportNavigationDragMode.Pan =>
                SceneOrbitCameraMotion.Pan(_lastCameraState, deltaX, deltaY, viewportHeight),
            ViewportNavigationDragMode.Zoom =>
                ApplyNavigationZoom(_lastCameraState, -deltaY),
            _ => _lastCameraState
        };

        if (_lastCameraState != previous)
            ScheduleScene3dFrame(VulkanScene3dFrameReason.OverlayNavigationChanged);
    }

    private static SceneOrbitCameraState ApplyNavigationZoom(
        SceneOrbitCameraState state,
        float deltaPixels) =>
        SceneOrbitCameraMotion.Dolly(state, deltaPixels);

    private void SnapCameraToNavigationElement(ViewportNavigationElement element)
    {
        if (!_sessionActive || _scene3dSession is null) return;
        if (_scene3dSession.Status != VulkanScene3dSessionStatus.Active) return;

        var view = element switch
        {
            ViewportNavigationElement.PositiveX => SceneNavigationView.PositiveX,
            ViewportNavigationElement.NegativeX => SceneNavigationView.NegativeX,
            ViewportNavigationElement.PositiveY => SceneNavigationView.PositiveY,
            ViewportNavigationElement.NegativeY => SceneNavigationView.NegativeY,
            ViewportNavigationElement.PositiveZ => SceneNavigationView.PositiveZ,
            ViewportNavigationElement.NegativeZ => SceneNavigationView.NegativeZ,
            _ => SceneNavigationView.Free
        };

        if (view == SceneNavigationView.Free)
            return;

        _lastCameraState = SceneNavigationCameraMotion.SnapToView(_lastCameraState, view);
        AppendInfoLog($"视图已切换到 {view}。");
        ScheduleScene3dFrame(VulkanScene3dFrameReason.OverlayNavigationChanged);
    }

    private void FrameNavigationTarget()
    {
        if (_selectedWorldEntity is null)
        {
            _lastCameraState = SceneOrbitCameraMotion.FrameAll(_lastCameraState);
            AppendInfoLog("已显示全部场景对象。");
            ScheduleScene3dFrame(VulkanScene3dFrameReason.OverlayNavigationChanged);
            return;
        }

        var pos = _worldState?.FindPosition(_selectedWorldEntity.EntityId);
        if (pos is null) return;

        var placement = new RenderUnitPlacement(pos.Value.Value);
        _lastCameraState = SceneOrbitCameraMotion.FrameSelected(
            _lastCameraState,
            (float)placement.VisualCenter.X,
            (float)placement.VisualCenter.Y,
            (float)placement.VisualCenter.Z,
            (float)RenderUnitPlacement.HalfExtent);
        AppendInfoLog($"已聚焦实体 {_selectedWorldEntity.DisplayName}。");
        ScheduleScene3dFrame(VulkanScene3dFrameReason.OverlayNavigationChanged);
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
        if (_framePending) return;
        _framePending = true;

        Dispatcher.UIThread.Post(() =>
        {
            _framePending = false;
            if (_scene3dSession is null) return;

            var unitDraws = BuildUnitDrawList();
            _cameraRevision++;
            var sessionPose = SceneCameraPose.FromOrbitState(_lastCameraState, _cameraRevision);

            // 提交 Move Gizmo 顶点
            SubmitMoveGizmoVertices();

            var result = _scene3dSession.RenderFrame(reason, sessionPose, [.. unitDraws]);

            if (result.Success)
            {
                _renderSeq++;
                _renderLastMode = "Scene3D";
                UpdateVulkanViewportStatusLine();
            }
            else
            {
                AppendWarningLog($"Scene3D 帧失败：{result.Message}");
            }
        });
    }

    // ─── Move Gizmo 顶点提交 ──────────────────────────────

    private void SubmitMoveGizmoVertices()
    {
        if (!_moveToolActive || _selectedWorldEntity is null)
        {
            ClearGizmo();
            return;
        }

        var session = _scene3dSession;
        if (session is null) { ClearGizmo(); return; }

        var pos = _worldState?.FindPosition(_selectedWorldEntity.EntityId);
        if (pos is null) { ClearGizmo(); return; }

        var snapshot = session.LastPresentedSnapshot;
        if (!snapshot.IsValid) { ClearGizmo(); return; }

        var pivot = pos.Value.Value;
        var vp = snapshot.ViewProjection;
        var w = snapshot.ViewportWidth;
        var h = snapshot.ViewportHeight;

        // 投影 Pivot 到屏幕
        if (!TryProjectToScreen(pivot, vp, w, h, out var pp))
        { session.SetMoveGizmoVertices(null); return; }

        // 计算世界单位每像素
        var cameraDist = _lastCameraState.Distance;
        var fov = _lastCameraState.FieldOfViewDegrees;
        var isOrtho = _lastCameraState.ProjectionMode == SceneProjectionMode.Orthographic;
        var orthoH = _lastCameraState.OrthographicHeight;
        var wpp = isOrtho ? orthoH / Math.Max(1, h)
            : 2.0 * cameraDist * Math.Tan(fov * Math.PI / 360.0) / Math.Max(1, h);

        const double gizmoScreenLen = 80.0;
        var worldLen = gizmoScreenLen * wpp;

        // 计算三轴端点像素坐标
        var axes = new[] { Vector3d.UnitX, Vector3d.UnitY, Vector3d.UnitZ };
        var endPixels = new (double X, double Y)[3];
        var degenerate = new bool[3];

        for (var i = 0; i < 3; i++)
        {
            var endWorld = pivot + axes[i] * worldLen;
            if (TryProjectToScreen(endWorld, vp, w, h, out var ep))
            {
                endPixels[i] = ep;
                degenerate[i] = false;
            }
            else
            {
                endPixels[i] = pp;
                degenerate[i] = true;
            }
        }

        // 构建布局并生成顶点
        var layout = MoveGizmoLayout.Build(
            (pp.X, pp.Y),
            (endPixels[0].X, endPixels[0].Y),
            (endPixels[1].X, endPixels[1].Y),
            (endPixels[2].X, endPixels[2].Y),
            degenerate[0], degenerate[1], degenerate[2]);

        if (layout is null) { session.SetMoveGizmoVertices(null); return; }

        var drawVerts = MoveGizmoDrawList.Build(layout,
            MoveGizmoVisualState.Normal, MoveGizmoElement.None);

        var overlayVerts = new FluidWarfare.Render.Vulkan.Scene3D.Overlay.VulkanOverlayVertex[drawVerts.Length];
        for (var i = 0; i < drawVerts.Length; i++)
        {
            overlayVerts[i] = new FluidWarfare.Render.Vulkan.Scene3D.Overlay.VulkanOverlayVertex(
                drawVerts[i].X, drawVerts[i].Y,
                drawVerts[i].R, drawVerts[i].G,
                drawVerts[i].B, drawVerts[i].A);
        }

        session.SetMoveGizmoVertices(overlayVerts);
    }

    private static bool TryProjectToScreen(
        Vector3d world, float[] vp, int w, int h,
        out (double X, double Y) pixel)
    {
        pixel = default;
        if (vp is not { Length: 16 } || w <= 0 || h <= 0) return false;
        var cw = vp[3] * world.X + vp[7] * world.Y + vp[11] * world.Z + vp[15];
        if (!double.IsFinite(cw) || Math.Abs(cw) < 1e-6) return false;
        var nx = (vp[0] * world.X + vp[4] * world.Y + vp[8] * world.Z + vp[12]) / cw;
        var ny = (vp[1] * world.X + vp[5] * world.Y + vp[9] * world.Z + vp[13]) / cw;
        if (!double.IsFinite(nx) || !double.IsFinite(ny)) return false;
        pixel = ((nx * 0.5 + 0.5) * w, (ny * 0.5 + 0.5) * h);
        return true;
    }

    private void ClearGizmo()
    {
        _scene3dSession?.SetMoveGizmoVertices(null);
    }

    // ─── 单向选择状态流 ──────────────────────────────────────

    private readonly EditorEntitySelectionState _selectionState = new();
    private readonly EditorSelectionDiagnostics _selectionDiag = new();
    private int _selectionDispatchDepth;

    /// <summary>
    /// 唯一选择入口。单向流：TryApply 幂等 → 顺序刷新各界面。
    /// _selectionDispatchDepth 熔断反馈递归。
    /// </summary>
    private void ApplyEntitySelection(string? entityIdStr, EditorEntitySelectionOrigin origin)
    {
        _selectionDiag.SelectionRequestCount++;

        if (_selectionDispatchDepth > 0)
        {
            _selectionDiag.FeedbackLoopBlockedCount++;
            System.Diagnostics.Debug.WriteLine(
                $"[严重] 检测到选择反馈环，已熔断。EntityId={entityIdStr} Origin={origin}");
            return;
        }

        _selectionDispatchDepth++;
        try
        {
            var change = _selectionState.TryApply(entityIdStr, origin);

            if (!change.IsChanged)
            {
                _selectionDiag.SelectionNoOpCount++;
                if (origin == EditorEntitySelectionOrigin.ViewportPicking && entityIdStr is not null)
                    _dockPanel?.RevealEntity(entityIdStr);
                return;
            }

            _selectionDiag.SelectionChangeCount++;
            _selectionDiag.LastRevision = change.Revision;

            WorldEntityInfo? entityInfo = null;
            if (entityIdStr is not null && int.TryParse(entityIdStr, out var entityIdVal) && entityIdVal > 0)
            {
                var targetId = EntityId.FromInt(entityIdVal);
                var entities = _worldState?.ListEntities() ?? [];
                entityInfo = entities.FirstOrDefault(e => e.EntityId == targetId);
            }

            ApplySelectionToScene3d(change, entityInfo);
            ApplySelectionToInspector(change, entityInfo);
            ApplySelectionToStatusBar(change, entityInfo);
            ApplySelectionToHierarchy(change, entityInfo);
            ApplySelectionLog(change, entityInfo);
        }
        finally
        {
            _selectionDispatchDepth--;
        }
    }

    private void ApplySelectionToScene3d(EditorEntitySelectionChange change, WorldEntityInfo? entityInfo)
    {
        if (_scene3dSession is null || !_sessionActive) return;
        if (change.CurrentEntityId is not null &&
            _scene3dSession.SetSelectedEntity(change.CurrentEntityId))
        {
            _selectionDiag.SceneSelectionFrameCount++;
            ScheduleScene3dFrame(VulkanScene3dFrameReason.SelectionChanged);
        }
    }

    private void ApplySelectionToInspector(EditorEntitySelectionChange change, WorldEntityInfo? entityInfo)
    {
        if (entityInfo is not null)
        {
            _selectedWorldEntity = entityInfo;
            ShowWorldEntitySelection(entityInfo);
            UpdateViewportForEntity(entityInfo);
        }
        else
        {
            _selectedWorldEntity = null;
            _inspectorPanel?.ShowNoSelection();
        }
    }

    private void ApplySelectionToStatusBar(EditorEntitySelectionChange change, WorldEntityInfo? entityInfo)
    {
        if (entityInfo is not null)
            _statusBarPanel?.SetCurrentSelection(entityInfo.DisplayName);
        else
            _statusBarPanel?.SetCurrentSelection("无");
    }

    private void ApplySelectionToHierarchy(EditorEntitySelectionChange change, WorldEntityInfo? entityInfo)
    {
        if (change.CurrentEntityId is not null &&
            change.Origin != EditorEntitySelectionOrigin.WorldHierarchy)
        {
            _selectionDiag.HierarchyRevealCount++;
            _dockPanel?.RevealEntity(change.CurrentEntityId);
        }
        else if (change.CurrentEntityId is null)
        {
            _dockPanel?.ClearEntitySelection();
        }
    }

    private void ApplySelectionLog(EditorEntitySelectionChange change, WorldEntityInfo? entityInfo)
    {
        if (entityInfo is not null)
            AppendInfoLog($"已选择 World 实体：{entityInfo.DisplayName} (Rev#{change.Revision}, {change.Origin})");
        else
            AppendInfoLog($"清除选择 (Rev#{change.Revision}, {change.Origin})");
    }

    // ─── 地面指针移动 ─────────────────────────────────────────────

    /// <summary>
    /// 鼠标在视口内移动 → 地面射线求交 → 状态栏反馈。
    /// 采用"最新值覆盖 + 单次调度"合并模式，最多约每 16ms 更新一次。
    /// </summary>
    private void HandleViewportPointerMoved(int pixelX, int pixelY)
    {
        if (!_sessionActive || _scene3dSession is null)
            return;
        if (_scene3dSession.Status != VulkanScene3dSessionStatus.Active)
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
        if (_navigationDragMode == ViewportNavigationDragMode.None &&
            _navigationHoverElement != ViewportNavigationElement.None)
        {
            SetOverlayVisualState(ViewportNavigationElement.None, ViewportNavigationElement.None);
        }

        if (_statusBarPanel is null) return;

        _groundPointerState.SetHover(null, null);
        _statusBarPanel.SetCurrentSelection(
            _selectedWorldEntity is not null
                ? _selectedWorldEntity.DisplayName
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
        if (_scene3dSession is null || _vulkanViewportHostPanel is null) return;

        var nativeHostInfo = _vulkanViewportHostPanel.GetNativeHostInfo();
        if (!nativeHostInfo.HasNativeHandle || nativeHostInfo.Width < 1 || nativeHostInfo.Height < 1)
            return;

        // 使用已呈现快照构建射线（只能从 Snapshot 读取参数）
        var snapshot = _scene3dSession.LastPresentedSnapshot;
        var status = VulkanSceneRayBuilder.TryBuild(
            pixelX, pixelY,
            snapshot,
            (uint)nativeHostInfo.Width, (uint)nativeHostInfo.Height,
            out var ray);

        if (status != SceneRayBuildStatus.Success)
        {
            // 技术失败（Snapshot 不可用、尺寸不匹配等）不清除 Hover
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
    /// 将轨道相机转换为 VulkanCameraInfo（Z-Up，Up = (0,0,1)）。
    /// </summary>
    private VulkanCameraInfo OrbitToCameraInfo(SceneOrbitCameraState orbit, float aspect)
    {
        var (camX, camY, camZ) = orbit.ComputePosition();
        return new VulkanCameraInfo(
            camX, camY, camZ,
            orbit.PivotX, orbit.PivotY, orbit.PivotZ,
            0, 0, 1,  // Z-Up
            orbit.FieldOfViewDegrees,
            orbit.NearPlane,
            orbit.FarPlane);
    }

    /// <summary>
    /// 清除选择。
    /// </summary>
    private void ClearSelection()
    {
        ApplyEntitySelection(null, EditorEntitySelectionOrigin.SelectionRestore);
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
        if (!_sessionActive || _scene3dSession is null) return;
        if (_scene3dSession.Status != VulkanScene3dSessionStatus.Active) return;

        var nativeHostInfo = _vulkanViewportHostPanel?.GetNativeHostInfo()
            ?? VulkanViewportNativeHostInfo.NotAvailable;
        if (!nativeHostInfo.HasNativeHandle || nativeHostInfo.Width < 1 || nativeHostInfo.Height < 1)
            return;

        // 使用已呈现快照构建射线（统一透视/正交，尺寸闸门）
        var snapshot = _scene3dSession.LastPresentedSnapshot;
        var buildStatus = VulkanSceneRayBuilder.TryBuild(
            pixelX, pixelY,
            snapshot,
            (uint)nativeHostInfo.Width, (uint)nativeHostInfo.Height,
            out var ray);

        // Phase D：技术失败（Snapshot 不可用、尺寸不匹配、矩阵无效）不清除选择
        if (buildStatus != SceneRayBuildStatus.Success)
        {
            if (buildStatus != SceneRayBuildStatus.Success)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[Pick] 射线构建非成功：{buildStatus}，保持当前选择。");
            }
            return;
        }

        // 统一 Picking：Entity 优先 → Ground → None
        var pointerResult = ScenePointerPicker.Pick(ray, _renderScene, SceneGroundPlane.Default);

        if (_groundPlacementState.IsActive)
        {
            // ─── 地面放置模式：只接受空白 Ground ──────────────────
            switch (pointerResult.Kind)
            {
                case ScenePointerPickKind.Ground when pointerResult.GroundPosition is not null:
                    CompleteGroundPlacement(pointerResult.GroundPosition.Value);
                    break;

                case ScenePointerPickKind.Entity:
                    _statusBarPanel?.SetCurrentSelection("请点击空白地面完成放置");
                    break;

                default:
                    _statusBarPanel?.SetCurrentSelection("当前位置未命中地面，请调整相机或点击其他区域");
                    break;
            }
        }
        else
        {
            // ─── 普通模式：Entity → Ground → None ────────────────
            switch (pointerResult.Kind)
            {
                case ScenePointerPickKind.Entity when pointerResult.EntityId is not null:
                    ApplyEntitySelection(
                        pointerResult.EntityId.Value.Value.ToString(),
                        EditorEntitySelectionOrigin.ViewportPicking);
                    HideGroundCursor();
                    System.Diagnostics.Debug.WriteLine(
                        $"[Pick] Entity hit: {pointerResult.EntityId.Value.Value}");
                    break;

                case ScenePointerPickKind.Ground when pointerResult.GroundPosition is not null:
                    ApplyEntitySelection(null, EditorEntitySelectionOrigin.ViewportPicking);
                    ShowGroundCursor(pointerResult.GroundPosition.Value);
                    AppendInfoLog(
                        $"地面落点：X {pointerResult.GroundPosition.Value.X:F2}，" +
                        $"Y {pointerResult.GroundPosition.Value.Y:F2}，" +
                        $"Z {pointerResult.GroundPosition.Value.Z:F2}。");
                    break;

                default:
                    ApplyEntitySelection(null, EditorEntitySelectionOrigin.ViewportPicking);
                    HideGroundCursor();
                    break;
            }
        }

        // 更新诊断信息
        UpdateAllDiagnostics();
    }

    // ─── 地面标记控制 ─────────────────────────────────────────────

    private void ShowGroundCursor(Vector3d worldPosition)
    {
        _groundPointerState.Commit(worldPosition);
        if (_scene3dSession is not null && _scene3dSession.SetGroundCursor(worldPosition))
        {
            ScheduleScene3dFrame(VulkanScene3dFrameReason.GroundCursorChanged);
        }
    }

    private void HideGroundCursor()
    {
        _groundPointerState.ClearCommit();
        if (_scene3dSession is not null)
        {
            _scene3dSession.SetGroundCursor(null);
            ScheduleScene3dFrame(VulkanScene3dFrameReason.GroundCursorChanged);
        }
    }

    // ─── Transform 编辑 ─────────────────────────────────────────────

    private void HandleTransformApply(string xText, string yText, string zText)
    {
        if (_selectedWorldEntity is null) return;

        if (!EditorEntityTransformValidation.TryParse(xText, yText, zText,
                out var newPos, out var error))
        {
            _inspectorPanel?.ShowTransformError(error);
            return;
        }

        ApplyEntityTransform(newPos, EditorEntityTransformOrigin.InspectorInput);
    }

    private void HandleTransformReset()
    {
        if (_selectedWorldEntity is null) return;
        var pos = _worldState?.FindPosition(_selectedWorldEntity.EntityId);
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
        if (_selectedWorldEntity is null)
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

        var currentPos = _worldState?.FindPosition(_selectedWorldEntity.EntityId);
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
        if (_selectedWorldEntity is null) return;
        // 防串写：事件携带的 entityId 必须与当前选中实体一致
        if (_selectedWorldEntity.EntityId.Value.ToString() != entityId)
        {
            AppendWarningLog("数值拖拽目标实体已变化，忽略本次更新。");
            return;
        }
        var pos = _worldState?.FindPosition(_selectedWorldEntity.EntityId);
        if (pos is null) return;

        var current = pos.Value.Value;
        var newPos = axis switch
        {
            TransformPositionAxis.X => new Vector3d(value, current.Y, current.Z),
            TransformPositionAxis.Y => new Vector3d(current.X, value, current.Z),
            _ => new Vector3d(current.X, current.Y, value),
        };

        ApplyEntityTransform(newPos, EditorEntityTransformOrigin.DragScrub);
    }

    private void HandleScrubCompleted(string entityId, TransformPositionAxis axis, double value)
    {
        AppendInfoLog($"数值拖拽完成：{axis} = {value:F3}");
    }

    private void HandleScrubCancelled(string entityId, TransformPositionAxis axis, double initialValue)
    {
        if (_selectedWorldEntity is null) return;
        if (_selectedWorldEntity.EntityId.Value.ToString() != entityId)
            return;

        var pos = _worldState?.FindPosition(_selectedWorldEntity.EntityId);
        if (pos is null) return;

        var current = pos.Value.Value;
        var restoredPos = axis switch
        {
            TransformPositionAxis.X => new Vector3d(initialValue, current.Y, current.Z),
            TransformPositionAxis.Y => new Vector3d(current.X, initialValue, current.Z),
            _ => new Vector3d(current.X, current.Y, initialValue),
        };

        ApplyEntityTransform(restoredPos, EditorEntityTransformOrigin.DragScrub);
        AppendInfoLog("数值拖拽已取消");
    }

    private void HandleGroundPlacementToggle()
    {
        if (_selectedWorldEntity is null) return;
        if (!_sessionActive || _scene3dSession?.Status != VulkanScene3dSessionStatus.Active)
        {
            AppendWarningLog("Scene3D 未激活，无法进入放置模式。");
            return;
        }

        if (_groundPlacementState.IsActive)
        {
            _groundPlacementState.Cancel();
            _inspectorPanel?.SetPlacementMode(false);
            _statusBarPanel?.SetCurrentSelection(
                _selectedWorldEntity?.DisplayName ?? "无");
        }
        else
        {
            _groundPlacementState.Begin(_selectedWorldEntity.EntityId.Value.ToString());
            _inspectorPanel?.SetPlacementMode(true);
            _statusBarPanel?.SetCurrentSelection(
                $"放置模式：点击空白地面放置 {_selectedWorldEntity.DisplayName}，Esc 取消");
        }
    }

    /// <summary>
    /// 原子式 Transform 提交。
    /// </summary>
    private void ApplyEntityTransform(Vector3d newPosition, EditorEntityTransformOrigin origin)
    {
        if (_worldState is null || _selectedWorldEntity is null) return;

        var entityId = _selectedWorldEntity.EntityId;
        var entityIdStr = entityId.Value.ToString();

        // 1. 写入 WorldState
        if (!_worldState.SetPosition(entityId, newPosition))
        {
            // NoOp: 相同位置
            _inspectorPanel?.SetTransformDraftState(false, false, null);
            return;
        }

        // 2. 同步 RenderScene
        var renderResult = RenderSceneObjectPositionWriter.Update(
            _renderScene, entityId, newPosition);
        if (!renderResult.IsSuccess)
        {
            // 回滚 WorldState
            _worldState.SetPosition(entityId, _renderScene.Objects
                .FirstOrDefault(o => o.EntityId == entityId)?.Position ?? newPosition);
            AppendErrorLog($"RenderScene 同步失败：{renderResult.Message}");
            return;
        }
        if (renderResult.NewScene is not null)
            _renderScene = renderResult.NewScene;

        // 3. 同步 Scene3D Session
        var unitPos = EntityToScene3dPosition(newPosition);
        if (_scene3dSession is not null)
            _scene3dSession.UpdateEntityPosition(
                entityIdStr, unitPos.X, unitPos.Y, unitPos.Z);

        // 4. 标记场景 Dirty
        _worldDirtyState.MarkDirty(entityIdStr);
        _statusBarPanel?.SetDirtyState(true);

        // 5. 更新 Inspector 显示的坐标
        _inspectorPanel?.SetTransformTexts(
            newPosition.X.ToString("F3", System.Globalization.CultureInfo.InvariantCulture),
            newPosition.Y.ToString("F3", System.Globalization.CultureInfo.InvariantCulture),
            newPosition.Z.ToString("F3", System.Globalization.CultureInfo.InvariantCulture));
        _inspectorPanel?.SetTransformDraftState(false, false, null);

        // 6. 请求一帧
        ScheduleScene3dFrame(VulkanScene3dFrameReason.EntityTransformChanged);

        // 7. 日志（数值拖拽不逐帧写日志，移动工具只在完成时写日志）
        if (origin != EditorEntityTransformOrigin.DragScrub && origin != EditorEntityTransformOrigin.MoveTool)
        {
            var prevPos = renderResult.Change?.OldPosition;
            if (prevPos is not null)
            {
                AppendInfoLog(
                    $"实体 {_selectedWorldEntity.DisplayName} 坐标已修改：" +
                    $"({prevPos.Value.X:F2}, {prevPos.Value.Y:F2}, {prevPos.Value.Z:F2}) → " +
                    $"({newPosition.X:F2}, {newPosition.Y:F2}, {newPosition.Z:F2})。");
            }
        }
    }

    private static (float X, float Y, float Z) EntityToScene3dPosition(Vector3d position)
    {
        // 使用 RenderUnitPlacement 统一计算视觉中心
        var placement = new RenderUnitPlacement(position);
        return ((float)placement.VisualCenter.X,
                (float)placement.VisualCenter.Y,
                (float)placement.VisualCenter.Z);
    }

    // ─── 地面放置 ──────────────────────────────────────────────────

    private void CompleteGroundPlacement(Vector3d groundPosition)
    {
        if (!_groundPlacementState.IsActive || _selectedWorldEntity is null) return;

        // 地面放置：实体在地面锚点，Z = 平面高程（0）
        var entityPos = new Vector3d(groundPosition.X, groundPosition.Y, 0);

        ApplyEntityTransform(entityPos, EditorEntityTransformOrigin.GroundPlacement);

        if (_groundPlacementState.IsActive)
        {
            _groundPlacementState.Complete();
            _inspectorPanel?.SetPlacementMode(false);
            HideGroundCursor();
            AppendInfoLog(
                $"实体 {_selectedWorldEntity.DisplayName} 已放置到 " +
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
        if (_renderScene.Objects.Count > 0)
        {
            groupLookup = new Dictionary<EntityId, string>();
            foreach (var obj in _renderScene.Objects)
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

    private List<VulkanScene3dUnitDrawInfo> BuildUnitDrawList()
    {
        var list = new List<VulkanScene3dUnitDrawInfo>();
        foreach (var obj in _renderScene.Objects)
        {
            if (obj.VisualKind != RenderObjectVisualKind.UnitMarker) continue;
            // 从 RenderUnitPlacement 读取视觉中心和缩放（单一真源）
            var p = obj.Placement;
            if (p is not null)
            {
                list.Add(new VulkanScene3dUnitDrawInfo(
                    obj.EntityId.Value.ToString(),
                    (float)p.VisualCenter.X,
                    (float)p.VisualCenter.Y,
                    (float)p.VisualCenter.Z,
                    (float)RenderUnitPlacement.Scale));
            }
            else
            {
                // 防御性回退
                list.Add(new VulkanScene3dUnitDrawInfo(
                    obj.EntityId.Value.ToString(),
                    (float)obj.Position.X,
                    (float)obj.Position.Y,
                    (float)obj.Position.Z + (float)RenderUnitPlacement.HalfExtent,
                    (float)RenderUnitPlacement.Scale));
            }
        }
        return list;
    }

    private void ProbeVulkanScene3D()
    {
        if (!_scene3dGate.CanRun)
        {
            ReportScene3dIsolation();
            return;
        }

        var nativeHostInfo = _vulkanViewportHostPanel?.GetNativeHostInfo()
            ?? VulkanViewportNativeHostInfo.NotAvailable;

        if (!nativeHostInfo.HasNativeHandle || nativeHostInfo.InstanceHandle == 0 || nativeHostInfo.WindowHandle == 0)
        {
            _vulkanScene3dInfo = new VulkanScene3dInfo(
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
        foreach (var obj in _renderScene.Objects)
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

        _vulkanScene3dInfo = VulkanScene3dRenderer.RenderWindows(
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
        if (_vulkanScene3dInfo.IsSucceeded)
        {
            _renderLastMode = "Scene3D";
            AppendInfoLog(_vulkanScene3dInfo.Message);
        }
        else if (_vulkanScene3dInfo.Status != VulkanScene3dStatus.NotChecked)
        {
            AppendWarningLog($"Vulkan 3D 场景绘制失败：{_vulkanScene3dInfo.Message}");
        }

        UpdateVulkanViewportStatusLine();
        UpdateAllDiagnostics();
    }

    private void UpdateVulkanViewportStatusLine()
    {
        var isActive = _sessionActive && _scene3dSession is not null &&
            _scene3dSession.Status == VulkanScene3dSessionStatus.Active;

        var scene3dSuffix = isActive
            ? $" | Scene3D Active | Frame #{_scene3dSession!.FrameIndex}"
            : _scene3dGate.CanRun
                ? " | Scene3D Ready"
                : " | Scene3D 已隔离";

        var lastRenderSuffix = $" | 最近渲染：{_renderLastMode}";

        _vulkanViewportHostPanel?.ShowClearStatus(
            _vulkanClearInfo.IsSucceeded
                ? $"Vulkan Clear 稳定{scene3dSuffix}{lastRenderSuffix} | {_vulkanClearInfo.ClearColorText}"
                : $"清屏：{_vulkanClearInfo.Message}{lastRenderSuffix}");
    }

    private void UpdateAllDiagnostics()
    {
        // 渲染诊断
        var nativeHostInfo = _vulkanViewportHostPanel?.GetNativeHostInfo()
            ?? VulkanViewportNativeHostInfo.NotAvailable;
        var nativeHostMsg = nativeHostInfo.HasNativeHandle
            ? $"已获取独立子窗口 HWND，尺寸：{nativeHostInfo.Width}x{nativeHostInfo.Height}"
            : "未获取";
        _debugDockPanel?.SetDiagnostics(
            _vulkanBackendInfo.Message,
            _vulkanInstanceInfo.IsCreated
                ? $"创建成功，API 版本：{_vulkanInstanceInfo.ApiVersionText}，扩展数量：{_vulkanInstanceInfo.ExtensionCount}，用时：{_vulkanInstanceInfo.ElapsedMilliseconds:F2} ms"
                : _vulkanInstanceInfo.Message,
            _vulkanDeviceInfo.IsCreated
                ? $"创建成功，显卡：{_vulkanDeviceInfo.PhysicalDeviceName}，类型：{_vulkanDeviceInfo.PhysicalDeviceTypeText}，队列族：{_vulkanDeviceInfo.GraphicsQueueFamilyIndex}，用时：{_vulkanDeviceInfo.ElapsedMilliseconds:F2} ms"
                : _vulkanDeviceInfo.Message,
            nativeHostMsg,
            _vulkanSurfaceInfo.IsCreated
                ? $"创建成功，平台：{_vulkanSurfaceInfo.PlatformText}，用时：{_vulkanSurfaceInfo.ElapsedMilliseconds:F2} ms"
                : _vulkanSurfaceInfo.Message,
            _vulkanSwapchainInfo.IsCreated
                ? $"创建成功，图像：{_vulkanSwapchainInfo.ImageCount}，格式：{_vulkanSwapchainInfo.SurfaceFormatText}，Present：{_vulkanSwapchainInfo.PresentModeText}，尺寸：{_vulkanSwapchainInfo.Width}x{_vulkanSwapchainInfo.Height}，用时：{_vulkanSwapchainInfo.ElapsedMilliseconds:F2} ms"
                : _vulkanSwapchainInfo.Message,
            _vulkanClearInfo.IsSucceeded
                ? $"成功，{_vulkanClearInfo.ClearColorText}，尺寸：{_vulkanClearInfo.Width}x{_vulkanClearInfo.Height}，用时：{_vulkanClearInfo.ElapsedMilliseconds:F2} ms"
                : _vulkanClearInfo.Message,
            "已退役（MarkerDraw 路径在 8.3.1 移除）",
            _vulkanValidationInfo.IsEnabled
                ? $"已启用，消息 {_vulkanValidationInfo.MessageCount} 条"
                : _vulkanValidationInfo.Message);

        _debugDockPanel?.SetScene3d(
            _vulkanScene3dInfo.IsSucceeded
                ? $"成功"
                : _vulkanScene3dInfo.Message,
            _vulkanScene3dInfo.CameraSummary,
            _vulkanScene3dInfo.IsSucceeded
                ? $"{_vulkanScene3dInfo.GridVertexCount} 顶点/{_vulkanScene3dInfo.GridLineCount} 线段"
                : "-",
            _vulkanScene3dInfo.IsSucceeded
                ? $"{_vulkanScene3dInfo.UnitVertexCount} 顶点/{_vulkanScene3dInfo.UnitTriangleCount} 三角形 | 渲染 {_vulkanScene3dInfo.RenderedUnitCount}/{_vulkanScene3dInfo.RenderObjectCount} | 忽略 {_vulkanScene3dInfo.IgnoredObjectCount}"
                : "-",
            _vulkanScene3dInfo.IsSucceeded
                ? $"{_vulkanScene3dInfo.DrawCallCount} | Depth {_vulkanScene3dInfo.DepthFormat} x{_vulkanScene3dInfo.DepthAttachmentCount} {( _vulkanScene3dInfo.DepthTestEnabled ? "已启用" : "未启用")}"
                : "-");

        // Scene3D 菜单项状态同步
        if (_runScene3dMenuItem is not null)
            _runScene3dMenuItem.IsEnabled = VulkanScene3dRunGate.Evaluate().CanRun;

        // 性能计时
        _debugDockPanel?.SetPerformance(
            _vulkanInstanceInfo.ElapsedMilliseconds.ToString("F2"),
            _vulkanDeviceInfo.ElapsedMilliseconds.ToString("F2"),
            _vulkanSwapchainInfo.ElapsedMilliseconds.ToString("F2"),
            _vulkanClearInfo.ElapsedMilliseconds.ToString("F2"),
            "-",
            _vulkanScene3dInfo.IsSucceeded
                ? _vulkanScene3dInfo.ElapsedMilliseconds.ToString("F2")
                : "-");

        // RenderScene 调试列表
        if (_renderScene.Objects.Count > 0)
        {
            var entries = _renderScene.Objects.Select(o =>
                $"{o.DisplayName} | unit_marker | ({o.Position.X}, {o.Position.Y}, {o.Position.Z}) | {o.SourcePath ?? "无"}").ToList();
            _debugDockPanel?.SetRenderSceneSummary(
                $"RenderScene 调试对象（共 {entries.Count} 个）", entries);
        }
        else
        {
            _debugDockPanel?.SetRenderSceneSummary("RenderScene 调试对象", []);
        }

        // 主视口和状态栏
        _statusBarPanel?.SetVulkanStatus(
            _vulkanBackendInfo.IsAvailable ? "已接入" : "不可用");
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
        if (_vulkanBackendInfo.IsAvailable)
        {
            _vulkanViewportHostPanel?.ShowClearStatus("Vulkan 后端就绪，等待 Surface/Swapchain。");
        }
        else
        {
            _vulkanViewportHostPanel?.ShowClearStatus("Vulkan 后端不可用。");
        }
    }

    private void ShowLoadedProject(GameProjectInfo project)
    {
        _projectInfo = project;
        _contentFiles = project.ContentFiles;

        // 项目加载后重建左侧双树
        RebuildAndShowHierarchy();
    }

    private EditorSelection CreateEntitySelection(WorldEntityInfo entityInfo)
    {
        var position = _worldState?.FindPosition(entityInfo.EntityId);

        var typeLabel = entityInfo.Source is not null ? "World 占位实体" : "World 实体";
        var description = position is not null
            ? $"EntityId({entityInfo.EntityId.Value})，来源：{entityInfo.Source?.RelativePath ?? "无"}，位置：({position.Value.Value.X}, {position.Value.Value.Y}, {position.Value.Value.Z})"
            : $"EntityId({entityInfo.EntityId.Value})，来源：{entityInfo.Source?.RelativePath ?? "无"}";

        return new EditorSelection(typeLabel, entityInfo.DisplayName, description);
    }

    private static EditorSelection CreateDefaultViewportSelection()
    {
        return new EditorSelection(
            "编辑器占位区",
            "3D 视口",
            "这里将显示 Vulkan 渲染的 3D 战场。");
    }

    private static IReadOnlyList<EngineLogEntry> CreateStartupLogs()
    {
        return
        [
            EngineLogEntry.Create(
                0.0,
                EngineLogLevel.Info,
                "Editor",
                "FluidWarfare Editor 启动完成。"),

            EngineLogEntry.Create(
                0.0,
                EngineLogLevel.Info,
                "Core",
                "Core 基础模块已加载。")
        ];
    }
}
