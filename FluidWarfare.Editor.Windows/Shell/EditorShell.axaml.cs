using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FluidWarfare.Core.Logging;
using FluidWarfare.Editor.Windows.Panels.Inspector;
using FluidWarfare.Editor.Windows.Panels.Logging;
using FluidWarfare.Editor.Windows.Panels.Project;
using FluidWarfare.Editor.Windows.Panels.Status;
using FluidWarfare.Editor.Windows.Panels.Viewport;
using FluidWarfare.Project.Content;
using FluidWarfare.Project.Loading;
using FluidWarfare.Project.Metadata;
using FluidWarfare.Project.Paths;

namespace FluidWarfare.Editor.Windows.Shell;

public sealed partial class EditorShell : UserControl
{
    private InspectorPanel? _inspectorPanel;
    private LogPanel? _logPanel;
    private ProjectPanel? _projectPanel;
    private StatusBarPanel? _statusBarPanel;
    private ViewportPlaceholderPanel? _viewportPlaceholderPanel;
    private readonly Dictionary<string, GameContentFolderInfo> _contentFoldersByDisplayName = [];
    private IReadOnlyList<GameContentFileInfo>? _contentFiles;

    public EditorShell()
    {
        AvaloniaXamlLoader.Load(this);
        FindShellControls();
        SubscribePanelEvents();
        InitializeLogs();
        LoadSampleProject();
    }

    private void FindShellControls()
    {
        _inspectorPanel = this.FindControl<InspectorPanel>("InspectorPanel");
        _logPanel = this.FindControl<LogPanel>("EditorLogPanel");
        _projectPanel = this.FindControl<ProjectPanel>("ProjectPanel");
        _statusBarPanel = this.FindControl<StatusBarPanel>("EditorStatusBarPanel");
        _viewportPlaceholderPanel = this.FindControl<ViewportPlaceholderPanel>("ViewportPlaceholderPanel");
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
        var selection = CreateViewportSelection();

        _inspectorPanel?.ShowSelection(selection);
        _statusBarPanel?.SetCurrentSelection(selection.DisplayName);
        AppendInfoLog("视口获得焦点。");
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

    private void HandleMenuClicked(string menuName)
    {
        AppendInfoLog($"点击菜单：{menuName}。");
    }

    private void AppendInfoLog(string message)
    {
        AppendLog(EngineLogLevel.Info, message);
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
            ShowProjectLoadFailure(pathResult.Error?.Message ?? "未知错误。");
            return;
        }

        var loadResult = GameProjectLoader.LoadFromDirectory(projectDirectory);

        if (loadResult.Result.IsSuccess && loadResult.Project is not null)
        {
            ShowLoadedProject(loadResult.Project);
            AppendInfoLog($"已加载示例项目：{loadResult.Project.DisplayName}。");
            return;
        }

        ShowProjectLoadFailure(loadResult.Result.Error?.Message ?? "未知错误。");
    }

    private void ShowProjectLoadFailure(string message)
    {
        _projectPanel?.ShowNoProject();

        var selection = new EditorSelection(
            "项目加载",
            "加载失败",
            $"项目加载失败：{message}");

        _inspectorPanel?.ShowSelection(selection);
        _statusBarPanel?.SetCurrentSelection("项目加载失败");
        AppendErrorLog($"项目加载失败：{message}");
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

    private static EditorSelection CreateViewportSelection()
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
