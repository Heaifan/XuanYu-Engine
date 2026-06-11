using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FluidWarfare.Bridge.ProjectEngine.World;
using FluidWarfare.Core.Identity;
using FluidWarfare.Core.Logging;
using FluidWarfare.Core.Math;
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
using FluidWarfare.Render.World;

namespace FluidWarfare.Editor.Windows.Shell;

public sealed partial class EditorShell : UserControl
{
    private InspectorPanel? _inspectorPanel;
    private LogPanel? _logPanel;
    private ProjectPanel? _projectPanel;
    private StatusBarPanel? _statusBarPanel;
    private ViewportPlaceholderPanel? _viewportPlaceholderPanel;
    private VulkanViewportHostPanel? _vulkanViewportHostPanel;
    private WorldEntityListPanel? _worldEntityListPanel;
    private readonly Dictionary<string, GameContentFolderInfo> _contentFoldersByDisplayName = [];
    private IReadOnlyList<GameContentFileInfo>? _contentFiles;
    private WorldState? _worldState;
    private EntityId _firstEntityId;
    private WorldEntityInfo? _selectedWorldEntity;
    private RenderScene _renderScene = RenderScene.Empty;
    private VulkanBackendInfo _vulkanBackendInfo = VulkanBackendInfo.NotChecked;

    public EditorShell()
    {
        AvaloniaXamlLoader.Load(this);
        FindShellControls();
        SubscribePanelEvents();
        InitializeLogs();
        LoadSampleProject();
        ProbeVulkanBackend();
    }

    private void FindShellControls()
    {
        _inspectorPanel = this.FindControl<InspectorPanel>("InspectorPanel");
        _logPanel = this.FindControl<LogPanel>("EditorLogPanel");
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
    }

    private void InitializeLogs()
    {
        var logs = CreateStartupLogs()
            .Select(entry => entry.ToDisplayString())
            .ToArray();

        _logPanel?.SetLogMessages(logs);
    }

    private void HandleProjectItemSelected(object? sender, string itemName)
    {
        var selection = CreateProjectSelection(itemName);

        _inspectorPanel?.ShowSelection(selection);
        _statusBarPanel?.SetCurrentSelection(selection.DisplayName);
        AppendInfoLog($"选择项目内容目录：{selection.DisplayName}。");

        if (_contentFoldersByDisplayName.TryGetValue(itemName, out var contentFolder) &&
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

        _logPanel?.AppendLogMessage(entry.ToDisplayString());
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

        _viewportPlaceholderPanel?.ShowVulkanBackendStatus(
            _vulkanBackendInfo.Message);

        UpdateVulkanViewportHost();
    }

    private void UpdateVulkanViewportHost()
    {
        VulkanViewportHostInfo hostInfo;

        if (_vulkanBackendInfo.IsAvailable)
        {
            hostInfo = new VulkanViewportHostInfo(
                VulkanViewportHostState.WaitingForSurface,
                "已创建占位宿主，等待 Surface / Swapchain 接入。");
        }
        else
        {
            hostInfo = new VulkanViewportHostInfo(
                VulkanViewportHostState.Disabled,
                "不可启用，Vulkan 后端不可用。");
        }

        _vulkanViewportHostPanel?.ShowHostInfo(hostInfo);
    }

    private void ShowLoadedProject(GameProjectInfo project)
    {
        _contentFoldersByDisplayName.Clear();
        _contentFiles = project.ContentFiles;

        foreach (var contentFolder in project.ContentFolders)
        {
            _contentFoldersByDisplayName[contentFolder.DisplayName] = contentFolder;
        }

        var categoryNames = project.ContentFolders
            .Select(folder => folder.DisplayName)
            .Append("配置")
            .ToArray();

        _projectPanel?.ShowProject(project.DisplayName, categoryNames);
    }

    private EditorSelection CreateProjectSelection(string itemName)
    {
        if (_contentFoldersByDisplayName.TryGetValue(itemName, out var contentFolder))
        {
            return new EditorSelection(
                "项目内容目录",
                contentFolder.DisplayName,
                contentFolder.Description);
        }

        if (itemName == "配置")
        {
            return new EditorSelection(
                "项目分类",
                "配置",
                "这里将显示项目配置、模拟参数与编辑器设置。");
        }

        return new EditorSelection(
            "未知项目内容目录",
            itemName,
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
