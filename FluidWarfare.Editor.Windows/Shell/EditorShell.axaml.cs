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
using FluidWarfare.Editor.WorldHierarchy;
using FluidWarfare.Editor.Windows.Panels.DebugDock;
using FluidWarfare.Editor.Windows.Panels.LeftDock;
using FluidWarfare.Editor.Windows.Panels.Inspector;
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
using FluidWarfare.Render.Selection;
using FluidWarfare.Render.Vulkan.Backend;
using FluidWarfare.Render.Vulkan.Device;
using FluidWarfare.Render.Vulkan.Instance;
using FluidWarfare.Render.Vulkan.Clear;
using FluidWarfare.Render.Vulkan.Camera;
using FluidWarfare.Render.Camera;
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
    private SceneCameraState _lastCameraState = SceneCameraDefaults.CreateDefault();
    private bool _framePending;
    private bool _sessionActive;
    private bool _scene3dAutoStartAttempted;
    private bool _isSynchronizingSelection;

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
        _dockPanel = this.FindControl<ProjectWorldDockPanel>("ProjectWorldDockPanel");
        _runMenuButton = this.FindControl<Button>("RunMenuButton");
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
            _vulkanViewportHostPanel.CameraPanRequested += HandleCameraPan;
            _vulkanViewportHostPanel.CameraZoomRequested += HandleCameraZoom;
            _vulkanViewportHostPanel.CameraResetRequested += HandleCameraReset;
            _vulkanViewportHostPanel.PickRequested += HandleViewportPick;
        }
    }

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        Dispatcher.UIThread.Post(ReportVulkanViewportNativeHost);
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
                    var result = _scene3dSession.Resize(
                        (uint)nativeHostInfo.Width,
                        (uint)nativeHostInfo.Height,
                        _lastCameraState,
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

    private void HandleFileMenuClicked(object? sender, RoutedEventArgs e)
    {
        HandleMenuClicked("文件");
    }

    private void HandleEditMenuClicked(object? sender, RoutedEventArgs e)
    {
        HandleMenuClicked("编辑");
    }

    private void HandleViewMenuClicked(object? sender, RoutedEventArgs e)
    {
        HandleMenuClicked("视图");
    }

    private void HandleRunMenuClicked(object? sender, RoutedEventArgs e)
    {
        HandleMenuClicked("运行");
    }

    private void HandleExportMenuClicked(object? sender, RoutedEventArgs e)
    {
        HandleMenuClicked("导出");
    }

    private void HandleHelpMenuClicked(object? sender, RoutedEventArgs e)
    {
        HandleMenuClicked("帮助");
    }

    private void OnHierarchyEntitySelected(string? entityId)
    {
        ApplyEntitySelection(entityId, EditorSelectionOrigin.WorldList);
    }

    private void OnProjectContentSelected(string? relativePath)
    {
        // 项目文件选择：只保存路径，不修改 EntityId，不影响 3D 场景
        AppendInfoLog($"项目文件已选择：{relativePath}");
    }

    private void ShowWorldEntitySelection(WorldEntityInfo entityInfo)
    {
        var selection = CreateEntitySelection(entityInfo);

        _inspectorPanel?.ShowSelection(selection);
        _statusBarPanel?.SetCurrentSelection(entityInfo.DisplayName);
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

    private void HandleMenuClicked(string menuName)
    {
        AppendInfoLog($"点击菜单：{menuName}。");
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
        flyout.Opened += (_, _) => HandleMenuClicked("运行");

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

        // 保存 RenderScene 快照
        var gridVertices = VulkanScene3dVertices.BuildGrid(20, 2);
        var unitVertices = VulkanScene3dVertices.BuildCube(0, 0, 0, 1.0f);

        var unitDraws = new List<VulkanScene3dUnitDrawInfo>();
        foreach (var obj in _renderScene.Objects)
        {
            if (obj.VisualKind != RenderObjectVisualKind.UnitMarker) continue;
            unitDraws.Add(new VulkanScene3dUnitDrawInfo(
                obj.EntityId.Value.ToString(),
                (float)obj.Position.X,
                (float)obj.Position.Y + 0.5f,
                (float)obj.Position.Z,
                1.25f));
        }

        _lastCameraState = SceneCameraDefaults.CreateDefault();

        var session = new VulkanScene3dSession();
        var result = session.Start(
            nativeHostInfo.InstanceHandle,
            nativeHostInfo.WindowHandle,
            (uint)nativeHostInfo.Width,
            (uint)nativeHostInfo.Height,
            _lastCameraState,
            gridVertices.AsSpan(),
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

    // ─── 相机输入处理 ─────────────────────────────────────────

    private void HandleCameraPan(int deltaX, int deltaY, int viewportW, int viewportH)
    {
        if (!_sessionActive || _scene3dSession is null) return;
        if (_scene3dSession.Status != VulkanScene3dSessionStatus.Active) return;

        _lastCameraState = SceneCameraMotion.Pan(_lastCameraState, deltaX, deltaY, viewportH);
        ScheduleScene3dFrame(VulkanScene3dFrameReason.CameraPan);
    }

    private void HandleCameraZoom(float wheelNotches)
    {
        if (!_sessionActive || _scene3dSession is null) return;
        if (_scene3dSession.Status != VulkanScene3dSessionStatus.Active) return;

        _lastCameraState = SceneCameraMotion.Zoom(_lastCameraState, wheelNotches);
        ScheduleScene3dFrame(VulkanScene3dFrameReason.CameraZoom);
    }

    private void HandleCameraReset()
    {
        if (!_sessionActive || _scene3dSession is null) return;

        _lastCameraState = SceneCameraMotion.Reset();
        ScheduleScene3dFrame(VulkanScene3dFrameReason.CameraReset);
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
            var result = _scene3dSession.RenderFrame(reason, _lastCameraState, [.. unitDraws]);

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

    // ─── 选择系统 ────────────────────────────────────────────

    private enum EditorSelectionOrigin { WorldList, ViewportPicking, Programmatic }

    /// <summary>
    /// 统一选择入口。接收 EntityId 字符串，同步更新 World 列表、Inspector、状态栏和 Scene3D。
    /// 通过 _isSynchronizingSelection 防止双向递归。
    /// </summary>
    private void ApplyEntitySelection(string? entityIdStr, EditorSelectionOrigin origin)
    {
        if (_isSynchronizingSelection) return;
        _isSynchronizingSelection = true;

        try
        {
            WorldEntityInfo? entityInfo = null;

            // 通过 EntityId 查找
            if (entityIdStr is not null && int.TryParse(entityIdStr, out var entityIdVal) && entityIdVal > 0)
            {
                var targetId = EntityId.FromInt(entityIdVal);
                var entities = _worldState?.ListEntities() ?? [];
                entityInfo = entities.FirstOrDefault(e => e.EntityId == targetId);
            }

            if (entityInfo is not null)
            {
                // 选中
                _selectedWorldEntity = entityInfo;

                // Scene3D 高亮
                if (_scene3dSession is not null && _sessionActive)
                {
                    if (_scene3dSession.SetSelectedEntity(entityInfo.EntityId.Value.ToString()))
                    {
                        ScheduleScene3dFrame(VulkanScene3dFrameReason.SelectionChanged);
                    }
                }

                // 更新左侧列表（从 Viewport 选择时同步到列表）
                _dockPanel?.RevealEntity(entityInfo.EntityId.Value.ToString());

                // 更新检查器和状态栏
                ShowWorldEntitySelection(entityInfo);
                UpdateViewportForEntity(entityInfo);
            }
            else
            {
                // 清除选择
                ClearSelection();
            }
        }
        finally
        {
            _isSynchronizingSelection = false;
        }
    }

    /// <summary>
    /// 清除当前选择。
    /// </summary>
    private void ClearSelection()
    {
        _selectedWorldEntity = null;

        if (_scene3dSession is not null && _sessionActive)
        {
            if (_scene3dSession.SetSelectedEntity(null))
            {
                ScheduleScene3dFrame(VulkanScene3dFrameReason.SelectionChanged);
            }
        }

        _dockPanel?.ClearEntitySelection();
        _inspectorPanel?.ShowNoSelection();
        _statusBarPanel?.SetCurrentSelection("无");
        _viewportPlaceholderPanel?.ShowNoWorldEntity();
    }

    /// <summary>
    /// 视口点击 Picking 处理。
    /// 像素坐标 → 世界射线 → RenderScene Picker → 统一选择入口。
    /// </summary>
    private void HandleViewportPick(int pixelX, int pixelY)
    {
        if (!_sessionActive || _scene3dSession is null) return;
        if (_scene3dSession.Status != VulkanScene3dSessionStatus.Active) return;

        var nativeHostInfo = _vulkanViewportHostPanel?.GetNativeHostInfo()
            ?? VulkanViewportNativeHostInfo.NotAvailable;
        if (!nativeHostInfo.HasNativeHandle || nativeHostInfo.Width < 1 || nativeHostInfo.Height < 1)
            return;

        // 计算当前 VP 矩阵
        var (dirX, dirY, dirZ) = SceneCameraState.DefaultViewDirection();
        var (camX, camY, camZ) = _lastCameraState.ComputePosition();
        var targetX = camX + dirX * _lastCameraState.Distance;
        var targetY = camY + dirY * _lastCameraState.Distance;
        var targetZ = camZ + dirZ * _lastCameraState.Distance;

        var camInfo = new VulkanCameraInfo(
            (float)camX, (float)camY, (float)camZ,
            (float)targetX, (float)targetY, (float)targetZ,
            0, 1, 0,
            (float)_lastCameraState.FieldOfViewDegrees,
            (float)_lastCameraState.NearPlane,
            (float)_lastCameraState.FarPlane);

        var aspect = nativeHostInfo.Width / (float)nativeHostInfo.Height;
        var vp = VulkanCameraMatrices.ComputeVulkanMVP(camInfo, aspect);

        // 构建射线
        if (!VulkanSceneRayBuilder.TryBuild(
                pixelX, pixelY,
                (uint)nativeHostInfo.Width, (uint)nativeHostInfo.Height,
                vp,
                new Vector3d(camX, camY, camZ),
                out var ray, out var rayErr))
        {
            AppendWarningLog($"Picking 射线构建失败：{rayErr}");
            return;
        }

        // 执行 Picking
        var pickResult = RenderScenePicker.Pick(ray, _renderScene);

        if (pickResult.IsHit && pickResult.EntityId is not null)
        {
            ApplyEntitySelection(pickResult.EntityId.Value.Value.ToString(), EditorSelectionOrigin.ViewportPicking);
        }
        else
        {
            ApplyEntitySelection(null, EditorSelectionOrigin.ViewportPicking);
        }

        // 更新诊断信息
        UpdateAllDiagnostics();
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
            list.Add(new VulkanScene3dUnitDrawInfo(
                obj.EntityId.Value.ToString(),
                (float)obj.Position.X,
                (float)obj.Position.Y + 0.5f,
                (float)obj.Position.Z,
                1.25f));
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

            var cy = (float)obj.Position.Y + 0.5f;
            unitDraws.Add(new VulkanScene3dUnitDrawInfo(
                obj.EntityId.Value.ToString(),
                (float)obj.Position.X,
                cy,
                (float)obj.Position.Z,
                1.25f));
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
