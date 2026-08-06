using System.Collections.Generic;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly EditorLogBuffer _logBuffer = new();
    EditorLogBus _logBus = null!;
    EditorLogFilter _logFilter = EditorLogFilter.All;
    LogEntry? _selectedLogEntry;
    LogEntry[] _selectedEntries = [];

    public IReadOnlyList<LogEntry> LogItems => _logBuffer.Filter(_logFilter);
    public IReadOnlyList<LogEntry> ProblemItems => _logBuffer.Filter(EditorLogFilter.Warning)
        .Concat(_logBuffer.Filter(EditorLogFilter.Error)).ToArray();
    public IReadOnlyList<LogEntry> BuildItems => _logBuffer.Filter(EditorLogFilter.Build);
    public IReadOnlyList<LogEntry> TaskItems => _logBuffer.Filter(EditorLogFilter.Task);
    public string LogSummary => EditorLogSummary.From(_logBuffer.All).Text;
    public bool HasNoLogItems => LogItems.Count == 0; public bool ShowNoFilterResults => !IsLogFilterAll && HasNoLogItems; // D5：筛选空态区分
    public bool IsLogFilterAll => _logFilter == EditorLogFilter.All;
    public bool IsLogFilterInfo => _logFilter == EditorLogFilter.Info;
    public bool IsLogFilterWarning => _logFilter == EditorLogFilter.Warning;
    public bool IsLogFilterError => _logFilter == EditorLogFilter.Error;
    public bool IsLogFilterBuild => _logFilter == EditorLogFilter.Build;
    public bool IsLogFilterTask => _logFilter == EditorLogFilter.Task;
    public bool IsLogFilterInput => _logFilter == EditorLogFilter.Input;
    public bool IsLogFilterRender => _logFilter == EditorLogFilter.Render;
    public LogEntry? SelectedLogEntry
    {
        get => _selectedLogEntry;
        set
        {
            if (!Set(ref _selectedLogEntry, value)) return;
            OnPropertyChanged(nameof(HasSelectedLogEntry));
            OnPropertyChanged(nameof(SelectedLogClipboardText));
        }
    }
    public bool HasSelectedLogEntry => SelectedLogEntry is not null;
    public string SelectedLogClipboardText => SelectedLogEntry is null
        ? "" : EditorLogClipboardText.From(SelectedLogEntry);
    public bool HasSelectedEntries => _selectedEntries.Length > 0;
    public string SelectedEntriesClipboardText => EditorLogClipboardText.FromMany(_selectedEntries);

    public void SetSelectedEntries(IEnumerable<LogEntry> items)
    {
        _selectedEntries = items as LogEntry[] ?? items.ToArray();
        OnPropertyChanged(nameof(HasSelectedEntries));
        OnPropertyChanged(nameof(SelectedEntriesClipboardText));
    }

    public void NotifyLogCopied()
    {
        _logBus.Info(EditorLogSource.Editor, EditorLogCategory.Command,
            $"已复制 {_selectedEntries.Length} 条日志到剪贴板", "Ctrl+C 复制选中日志，可粘贴给 AI 审计。");
        RefreshLogBindings();
    }

    void InitLogs()
    {
        _logBus = new EditorLogBus(_logBuffer);
        // 生产运行时不再注入示例/种子日志；空状态由 UI「暂无日志」占位呈现。
        RefreshLogBindings();
    }

    void SetLogFilter(string name)
    {
        _logFilter = EditorLogFilterText.FromText(name);
        RefreshLogBindings();
    }

    void LogCommand(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return;
        var category = name is "保存" ? EditorLogCategory.Save : EditorLogCategory.Command;
        var source = name is "构建" ? EditorLogSource.Build : EditorLogSource.Editor;
        _logBus.Info(source, category, $"命令已触发：{name}", "顶部工具栏低频命令。");
        RefreshLogBindings();
    }

    void LogTool(string tool)
    {
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Tool, $"当前工具切换为：{tool}", "工具切换是低频输入事件。");
        RefreshLogBindings();
    }

    void LogInteraction(string message, string detail)
    {
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture, message, detail);
        RefreshLogBindings();
    }

    public void LogVulkanLifecycle(string message, string detail)
    {
        if (EditorLogNoiseFilter.SuppressRenderBackendInfo(message)) return;
        _logBus.Info(EditorLogSource.Render, EditorLogCategory.Backend, message, detail);
        RefreshLogBindings();
    }

    void RefreshLogBindings() { OnPropertyChanged(nameof(LogItems)); OnPropertyChanged(nameof(ProblemItems)); OnPropertyChanged(nameof(BuildItems)); OnPropertyChanged(nameof(TaskItems)); OnPropertyChanged(nameof(LogSummary)); OnPropertyChanged(nameof(HasNoLogItems)); OnPropertyChanged(nameof(ShowNoFilterResults)); OnPropertyChanged(nameof(SelectedLogEntry)); OnPropertyChanged(nameof(HasSelectedLogEntry)); OnPropertyChanged(nameof(SelectedLogClipboardText)); OnPropertyChanged(nameof(IsLogFilterAll)); OnPropertyChanged(nameof(IsLogFilterInfo)); OnPropertyChanged(nameof(IsLogFilterWarning)); OnPropertyChanged(nameof(IsLogFilterError)); OnPropertyChanged(nameof(IsLogFilterBuild)); OnPropertyChanged(nameof(IsLogFilterTask)); OnPropertyChanged(nameof(IsLogFilterInput)); OnPropertyChanged(nameof(IsLogFilterRender)); }
}
