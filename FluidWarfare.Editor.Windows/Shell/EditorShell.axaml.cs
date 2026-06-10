using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FluidWarfare.Core.Logging;
using FluidWarfare.Editor.Windows.Panels.Inspector;
using FluidWarfare.Editor.Windows.Panels.Logging;
using FluidWarfare.Editor.Windows.Panels.Project;
using FluidWarfare.Editor.Windows.Panels.Status;

namespace FluidWarfare.Editor.Windows.Shell;

public sealed partial class EditorShell : UserControl
{
    private InspectorPanel? _inspectorPanel;
    private LogPanel? _logPanel;
    private ProjectPanel? _projectPanel;
    private StatusBarPanel? _statusBarPanel;

    public EditorShell()
    {
        AvaloniaXamlLoader.Load(this);
        FindShellControls();
        SubscribePanelEvents();
        InitializeLogs();
    }

    private void FindShellControls()
    {
        _inspectorPanel = this.FindControl<InspectorPanel>("InspectorPanel");
        _logPanel = this.FindControl<LogPanel>("EditorLogPanel");
        _projectPanel = this.FindControl<ProjectPanel>("ProjectPanel");
        _statusBarPanel = this.FindControl<StatusBarPanel>("EditorStatusBarPanel");
    }

    private void SubscribePanelEvents()
    {
        if (_projectPanel is not null)
        {
            _projectPanel.ProjectItemSelected += HandleProjectItemSelected;
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
        AppendInfoLog($"选择项目项：{selection.DisplayName}。");
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
        var entry = EngineLogEntry.Create(
            0.0,
            EngineLogLevel.Info,
            "Editor",
            message);

        _logPanel?.AppendLogMessage(entry.ToDisplayString());
    }

    private static EditorSelection CreateProjectSelection(string itemName)
    {
        return itemName switch
        {
            "场景" => new EditorSelection(
                "项目占位项",
                "场景",
                "这里将显示场景列表与场景配置。"),

            "单位" => new EditorSelection(
                "项目占位项",
                "单位",
                "这里将显示单位模板与编队配置。"),

            "资源" => new EditorSelection(
                "项目占位项",
                "资源",
                "这里将显示模型、贴图、材质与音频资源。"),

            "配置" => new EditorSelection(
                "项目占位项",
                "配置",
                "这里将显示项目配置、模拟参数与编辑器设置。"),

            _ => new EditorSelection(
                "未知占位项",
                itemName,
                "当前项目项没有说明。")
        };
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
