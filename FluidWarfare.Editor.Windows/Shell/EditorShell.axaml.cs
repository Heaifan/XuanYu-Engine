using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using FluidWarfare.Bridge.ProjectEngine.World;
using FluidWarfare.Core.Identity;
using FluidWarfare.Core.Logging;
using FluidWarfare.Core.Math;
using FluidWarfare.Editor.Windows.Panels.DebugDock;
using FluidWarfare.Editor.Windows.Panels.Inspector;
using FluidWarfare.Editor.Windows.Panels.Logging;
using FluidWarfare.Editor.Windows.Panels.Project;
using FluidWarfare.Editor.Windows.Panels.Status;
using FluidWarfare.Editor.Windows.Panels.Viewport;
using FluidWarfare.Editor.Windows.Panels.WorldEntities;
using FluidWarfare.Engine.World;
using FluidWarfare.Project.Content;
using FluidWarfare.Project.Loading;
using FluidWarfare.Project.Metadata;
using FluidWarfare.Project.Paths;
using FluidWarfare.Project.Validation;
using FluidWarfare.Render.Scene;
using FluidWarfare.Render.Vulkan.Backend;
using FluidWarfare.Render.Vulkan.Device;
using FluidWarfare.Render.Vulkan.Instance;
using FluidWarfare.Render.Vulkan.Clear;
using FluidWarfare.Render.Vulkan.Markers;
using FluidWarfare.Render.Vulkan.Camera;
using FluidWarfare.Render.Vulkan.Scene3D;
using FluidWarfare.Render.Vulkan.Surface;
using FluidWarfare.Render.Vulkan.Swapchain;
using FluidWarfare.Render.World;

namespace FluidWarfare.Editor.Windows.Shell;

public sealed partial class EditorShell : UserControl
{
    private InspectorPanel? _inspectorPanel;
    private DebugDockPanel? _debugDockPanel;
    private ProjectPanel? _projectPanel;
    private StatusBarPanel? _statusBarPanel;
    private ViewportPlaceholderPanel? _viewportPlaceholderPanel;
    private VulkanViewportHostPanel? _vulkanViewportHostPanel;
    private WorldEntityListPanel? _worldEntityListPanel;
    private readonly Dictionary<string, GameContentFolderInfo> _contentFoldersByFolderName = [];
    private IReadOnlyList<GameContentFileInfo>? _contentFiles;
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
    private VulkanMarkerDrawResult _vulkanMarkerDrawResult = VulkanMarkerDrawResult.NotChecked;
    private VulkanScene3dInfo _vulkanScene3dInfo = VulkanScene3dInfo.NotChecked;
    private DispatcherTimer? _viewportResizeRenderTimer;
    private bool _vulkanViewportNativeHostReported;
    private bool _vulkanViewportRendering;

    public EditorShell()
    {
        AvaloniaXamlLoader.Load(this);
        FindShellControls();
        SubscribePanelEvents();
        InitializeLogs();
        LoadSampleProject();
        ProbeVulkanBackend();
        AttachedToVisualTree += OnAttachedToVisualTree;
        DetachedFromVisualTree += OnDetachedFromVisualTree;
    }

    private void FindShellControls()
    {
        _inspectorPanel = this.FindControl<InspectorPanel>("InspectorPanel");
        _debugDockPanel = this.FindControl<DebugDockPanel>("DebugDockPanel");
        _projectPanel = this.FindControl<ProjectPanel>("ProjectPanel");
        _statusBarPanel = this.FindControl<StatusBarPanel>("EditorStatusBarPanel");
        _viewportPlaceholderPanel = this.FindControl<ViewportPlaceholderPanel>("ViewportPlaceholderPanel");
        _vulkanViewportHostPanel = this.FindControl<VulkanViewportHostPanel>("VulkanViewportHostPanel");
        _worldEntityListPanel = this.FindControl<WorldEntityListPanel>("WorldEntityListPanel");
    }

    private void SubscribePanelEvents()
    {
        if (_projectPanel is not null)
        {
            _projectPanel.ProjectItemSelected += HandleProjectItemSelected;
        }

        if (_viewportPlaceholderPanel is not null)
        {
            _viewportPlaceholderPanel.ViewportFocused += HandleViewportFocused;
        }

        if (_worldEntityListPanel is not null)
        {
            _worldEntityListPanel.EntitySelected += OnWorldEntitySelected;
        }

        if (_vulkanViewportHostPanel is not null)
        {
            _vulkanViewportHostPanel.NativeHostInfoChanged += HandleVulkanViewportNativeHostInfoChanged;
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
            // resize/maximize 时只执行 3D 场景渲染 probe（含自带的清屏 + 绘制 + Present）。
            // 不重新执行 Swapchain/Clear/MarkerDraw 等 probe，避免在同一 HWND 上反复
            // create/destroy Vulkan Instance+Surface+Device+Swapchain 导致驱动崩溃。
            ProbeVulkanScene3D();
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

    private void HandleProjectItemSelected(object? sender, ProjectContentFolderSelection selectedFolder)
    {
        var selection = CreateProjectSelection(selectedFolder);

        _inspectorPanel?.ShowSelection(selection);
        _statusBarPanel?.SetCurrentSelection(selection.DisplayName);
        AppendInfoLog($"选择项目内容目录：{selection.DisplayName}。");

        if (_contentFoldersByFolderName.TryGetValue(selectedFolder.FolderName, out var contentFolder) &&
            _contentFiles is not null)
        {
            var folderFiles = _contentFiles
                .Where(f => f.FolderName == contentFolder.FolderName)
                .ToList();

            if (folderFiles.Count > 0)
            {
                AppendInfoLog($"项目内容目录“{contentFolder.DisplayName}”包含 {folderFiles.Count} 个合法内容文件入口。");
            }
        }
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

    private void OnWorldEntitySelected(WorldEntityInfo entityInfo)
    {
        _selectedWorldEntity = entityInfo;
        ShowWorldEntitySelection(entityInfo);
        UpdateViewportForEntity(entityInfo);
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
        _projectPanel?.ShowNoProject();
        _worldEntityListPanel?.ShowEntities([]);
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
            _worldEntityListPanel?.ShowEntities([]);
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
            _worldEntityListPanel?.ShowEntities([]);
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

        // 更新实体列表，视口保持默认状态等待用户选择
        _worldEntityListPanel?.ShowEntities(entities);
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
            ProbeVulkanClear();
            ProbeVulkanMarkerDraw();
            ProbeVulkanScene3D();
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

    private void ProbeVulkanClear()
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

    private void ProbeVulkanMarkerDraw()
    {
        var nativeHostInfo = _vulkanViewportHostPanel?.GetNativeHostInfo()
            ?? VulkanViewportNativeHostInfo.NotAvailable;

        if (!TryGetValidViewportSize(nativeHostInfo, out var viewportWidth, out var viewportHeight, out var viewportSizeMessage))
        {
            _vulkanMarkerDrawResult = new VulkanMarkerDrawResult(
                VulkanMarkerDrawStatus.Failed, viewportSizeMessage, 0, 0);
            ShowVulkanMarkerDrawInfo();
            return;
        }

        if (!nativeHostInfo.HasNativeHandle || nativeHostInfo.InstanceHandle == 0 || nativeHostInfo.WindowHandle == 0)
        {
            _vulkanMarkerDrawResult = new VulkanMarkerDrawResult(
                VulkanMarkerDrawStatus.Failed, "缺少原生句柄，跳过点位绘制。", 0, 0);
            ShowVulkanMarkerDrawInfo();
            return;
        }

        if (_renderScene.Objects.Count == 0)
        {
            _vulkanMarkerDrawResult = new VulkanMarkerDrawResult(
                VulkanMarkerDrawStatus.Failed, "RenderScene 没有可绘制对象，跳过 Vulkan 点位绘制。", 0, 0);
            ShowVulkanMarkerDrawInfo();
            return;
        }

        // 取 RenderScene 第一个对象
        var firstObject = _renderScene.Objects[0];

        // 初版坐标映射：(0,0,0) → 视口中心
        var markerInfo = VulkanMarkerDrawInfo.FromWorldPosition(
            firstObject.DisplayName,
            (float)firstObject.Position.X,
            (float)firstObject.Position.Z,
            (int)viewportWidth,
            (int)viewportHeight);

        _vulkanMarkerDrawResult = VulkanMarkerClearRectRenderer.RenderWindows(
            nativeHostInfo.InstanceHandle,
            nativeHostInfo.WindowHandle,
            viewportWidth,
            viewportHeight,
            markerInfo);

        ShowVulkanMarkerDrawInfo();
    }

    private void ShowVulkanMarkerDrawInfo()
    {
        if (_vulkanMarkerDrawResult.IsSucceeded)
        {
            AppendInfoLog(_vulkanMarkerDrawResult.Message);
        }
        else if (_vulkanMarkerDrawResult.Status != VulkanMarkerDrawStatus.NotChecked)
        {
            AppendWarningLog($"Vulkan 点位绘制失败：{_vulkanMarkerDrawResult.Message}");
        }

        UpdateVulkanViewportStatusLine();
        UpdateAllDiagnostics();
    }

    private void ProbeVulkanScene3D()
    {
        var nativeHostInfo = _vulkanViewportHostPanel?.GetNativeHostInfo()
            ?? VulkanViewportNativeHostInfo.NotAvailable;

        if (!nativeHostInfo.HasNativeHandle || nativeHostInfo.InstanceHandle == 0 || nativeHostInfo.WindowHandle == 0)
        {
            _vulkanScene3dInfo = new VulkanScene3dInfo(
                VulkanScene3dStatus.Failed, "缺少原生句柄，跳过 3D 场景绘制。",
                0, 0, 0, 0, 0, 0, 0, "无", 0);
            ShowVulkanScene3DInfo();
            return;
        }

        // 使用视口实际尺寸（避免 maximize 时硬编码出错）
        var vpW = (uint)Math.Max(nativeHostInfo.Width, 1);
        var vpH = (uint)Math.Max(nativeHostInfo.Height, 1);

        // 生成地面网格（范围 -20 到 +20，间隔 2）
        var gridVertices = VulkanScene3dVertices.BuildGrid(20, 2);

        // 从 RenderScene 取第一个对象的位置
        VulkanScene3dVertex[] unitVertices;
        if (_renderScene.Objects.Count > 0)
        {
            var obj = _renderScene.Objects[0];
            var cx = obj.Position.X;
            var cy = obj.Position.Y + 0.5;
            var cz = obj.Position.Z;
            unitVertices = VulkanScene3dVertices.BuildCube(
                (float)cx, (float)cy, (float)cz, 1.0f);
        }
        else
        {
            unitVertices = VulkanScene3dVertices.BuildCube(0, 0.5f, 0, 1.0f);
        }

        var camera = VulkanCameraInfo.DefaultBattlefield;

        _vulkanScene3dInfo = VulkanScene3dRenderer.RenderWindows(
            nativeHostInfo.InstanceHandle,
            nativeHostInfo.WindowHandle,
            vpW, vpH, camera,
            gridVertices.AsSpan(),
            unitVertices.AsSpan());

        ShowVulkanScene3DInfo();
    }

    private void ShowVulkanScene3DInfo()
    {
        if (_vulkanScene3dInfo.IsSucceeded)
        {
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
        var markerSuffix = _vulkanMarkerDrawResult.IsSucceeded
            ? $" | 点位：{_vulkanMarkerDrawResult.DrawnMarkerCount}"
            : string.Empty;

        var scene3dSuffix = _vulkanScene3dInfo.IsSucceeded
            ? $" | Grid: {_vulkanScene3dInfo.GridVertexCount} | Unit: {_vulkanScene3dInfo.UnitVertexCount} | DrawCall: {_vulkanScene3dInfo.DrawCallCount}"
            : string.Empty;

        _vulkanViewportHostPanel?.ShowClearStatus(
            _vulkanClearInfo.IsSucceeded
                ? $"3D 场景成功{scene3dSuffix} | {_vulkanClearInfo.ClearColorText}"
                : $"清屏：{_vulkanClearInfo.Message}");
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
            _vulkanMarkerDrawResult.IsSucceeded
                ? $"绘制成功，数量 {_vulkanMarkerDrawResult.DrawnMarkerCount}，尺寸：{nativeHostInfo.Width}x{nativeHostInfo.Height}，用时 {_vulkanMarkerDrawResult.ElapsedMilliseconds:F2} ms"
                : _vulkanMarkerDrawResult.Message);

        _debugDockPanel?.SetScene3d(
            _vulkanScene3dInfo.IsSucceeded
                ? "成功"
                : _vulkanScene3dInfo.Message,
            _vulkanScene3dInfo.CameraSummary,
            _vulkanScene3dInfo.IsSucceeded
                ? $"{_vulkanScene3dInfo.GridVertexCount} 顶点/{_vulkanScene3dInfo.GridLineCount} 线段"
                : "-",
            _vulkanScene3dInfo.IsSucceeded
                ? $"{_vulkanScene3dInfo.UnitVertexCount} 顶点/{_vulkanScene3dInfo.UnitTriangleCount} 三角形"
                : "-",
            _vulkanScene3dInfo.IsSucceeded
                ? $"{_vulkanScene3dInfo.DrawCallCount}"
                : "-");

        // 性能计时
        _debugDockPanel?.SetPerformance(
            _vulkanInstanceInfo.ElapsedMilliseconds.ToString("F2"),
            _vulkanDeviceInfo.ElapsedMilliseconds.ToString("F2"),
            _vulkanSwapchainInfo.ElapsedMilliseconds.ToString("F2"),
            _vulkanClearInfo.ElapsedMilliseconds.ToString("F2"),
            _vulkanMarkerDrawResult.IsSucceeded
                ? _vulkanMarkerDrawResult.ElapsedMilliseconds.ToString("F2")
                : "-",
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
        _contentFoldersByFolderName.Clear();
        _contentFiles = project.ContentFiles;

        foreach (var contentFolder in project.ContentFolders)
        {
            _contentFoldersByFolderName[contentFolder.FolderName] = contentFolder;
        }

        var contentFolderSelections = project.ContentFolders
            .Select(folder => new ProjectContentFolderSelection(
                folder.FolderName,
                folder.DisplayName,
                folder.ContentKind))
            .ToArray();

        _projectPanel?.ShowProject(project.DisplayName, contentFolderSelections);
    }

    private EditorSelection CreateProjectSelection(ProjectContentFolderSelection selectedFolder)
    {
        if (_contentFoldersByFolderName.TryGetValue(selectedFolder.FolderName, out var contentFolder))
        {
            return new EditorSelection(
                "项目内容目录",
                contentFolder.DisplayName,
                contentFolder.Description);
        }

        return new EditorSelection(
            "未知项目内容目录",
            selectedFolder.DisplayName,
            "当前项目内容目录没有说明。");
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
