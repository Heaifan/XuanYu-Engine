namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D5（纠偏）：日志绑定刷新通知（拆分多行，不压缩单行）。
public sealed partial class UiVm
{
    void RefreshLogBindings()
    {
        RefreshLogCollectionBindings();
        RefreshLogCountBindings();
    }

    void RefreshLogCollectionBindings()
    {
        LogCollectionRefreshCount++;
        var visible = _logBuffer.All.Where(_logFilterState.Allows).ToArray();
        var problems = _logBuffer.All.Where(entry => entry.Level is EditorLogLevel.Warning or EditorLogLevel.Error).ToArray();
        var builds = _logBuffer.All.Where(entry => EditorLogFilter.Build.Allows(entry)).ToArray();
        var tasks = _logBuffer.All.Where(entry => EditorLogFilter.Task.Allows(entry)).ToArray();
        if (_logItems.Sync(visible)) OnPropertyChanged(nameof(LogItems));
        if (_problemItems.Sync(problems)) OnPropertyChanged(nameof(ProblemItems));
        if (_buildItems.Sync(builds)) OnPropertyChanged(nameof(BuildItems));
        if (_taskItems.Sync(tasks)) OnPropertyChanged(nameof(TaskItems));
        OnPropertyChanged(nameof(LogSummary));
        OnPropertyChanged(nameof(HasNoLogItems));
        OnPropertyChanged(nameof(ShowInitialLogEmpty));
        OnPropertyChanged(nameof(ShowNoFilterResults));
    }

    void RefreshLogCountBindings()
    {
        OnPropertyChanged(nameof(AllCount));
        OnPropertyChanged(nameof(InfoCount));
        OnPropertyChanged(nameof(WarningCount));
        OnPropertyChanged(nameof(ErrorCount));
    }

    void RefreshLogFilterBindings(bool searchChanged = false, bool sourceChanged = false,
        bool severityChanged = false)
    {
        RefreshLogCollectionBindings();
        if (sourceChanged)
        {
            OnPropertyChanged(nameof(LogSourceFilter));
            OnPropertyChanged(nameof(LogSourceFilterText));
        }
        if (searchChanged) OnPropertyChanged(nameof(LogSearchText));
        if (severityChanged)
        {
            OnPropertyChanged(nameof(IsLogFilterAll));
            OnPropertyChanged(nameof(IsLogFilterInfo));
            OnPropertyChanged(nameof(IsLogFilterWarning));
            OnPropertyChanged(nameof(IsLogFilterError));
        }
    }

    void RefreshLogSelectionBindings()
    {
        OnPropertyChanged(nameof(SelectedLogEntry));
        OnPropertyChanged(nameof(HasSelectedLogEntry));
        OnPropertyChanged(nameof(SelectedLogClipboardText));
    }
}
