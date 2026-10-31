using System.Collections.Generic;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly LogPresenter _logPresenter = new();
    EditorLogBus _logBus => _logPresenter.LogBus;
    LogEntry? _selectedLogEntry;
    LogEntry[] _selectedEntries = [];

    public IReadOnlyList<LogEntry> LogItems => _logPresenter.LogItems;
    public IReadOnlyList<LogEntry> ProblemItems => _logPresenter.ProblemItems;
    public IReadOnlyList<LogEntry> BuildItems => _logPresenter.BuildItems;
    public IReadOnlyList<LogEntry> TaskItems => _logPresenter.TaskItems;
    public string LogSummary => _logPresenter.LogSummary;
    public LogEntry? SelectedLogEntry
    {
        get => _selectedLogEntry;
        set
        {
            if (!Set(ref _selectedLogEntry, value)) return;
            RefreshLogSelectionBindings();
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

    }

    void InitLogs()
    {
        _logPresenter.PropertyChanged += OnLogPresenterPropertyChanged;
        // 生产运行时不再注入示例/种子日志；空状态由 UI「暂无日志」占位呈现。

    }

    void OnLogPresenterPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs args)
    {
        var property = args.PropertyName switch
        {
            nameof(LogPresenter.IsFilterAll) => nameof(IsLogFilterAll),
            nameof(LogPresenter.IsFilterInfo) => nameof(IsLogFilterInfo),
            nameof(LogPresenter.IsFilterWarning) => nameof(IsLogFilterWarning),
            nameof(LogPresenter.IsFilterError) => nameof(IsLogFilterError),
            nameof(LogPresenter.Source) => nameof(LogSourceFilter),
            nameof(LogPresenter.SearchText) => nameof(LogSearchText),
            _ => args.PropertyName
        };
        if (property is not null) OnPropertyChanged(property);
        if (args.PropertyName == nameof(LogPresenter.Source)) OnPropertyChanged(nameof(LogSourceFilterText));
    }


    void LogCommand(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return;
        var category = name is "保存" ? EditorLogCategory.Save : EditorLogCategory.Command;
        var source = name is "构建" ? EditorLogSource.Build : EditorLogSource.Editor;
        _logBus.Info(source, category, $"命令已触发：{name}", "顶部工具栏低频命令。");

    }

    void LogTool(string tool)
    {
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Tool, $"当前工具切换为：{tool}", "工具切换是低频输入事件。");

    }

    void LogInteraction(string message, string detail)
    {
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture, message, detail);

    }

    public void LogVulkanLifecycle(string message, string detail)
    {
        if (EditorLogNoiseFilter.SuppressRenderBackendInfo(message)) return;
        _logBus.Info(EditorLogSource.Render, EditorLogCategory.Backend, message, detail);

    }
}
