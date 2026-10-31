using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace XuanYu.Editor.UI;

public sealed class LogPresenter : INotifyPropertyChanged
{
    readonly EditorLogBuffer _buffer = new();
    readonly LogProjectionCache _logs = new(), _problems = new(), _builds = new(), _tasks = new();
    EditorLogFilterState _filter = EditorLogFilterState.CreateDefault();
    LogEntry? _selectedLogEntry;
    LogEntry[] _selectedEntries = [];
    bool _isOpen, _isDetailsOpen;

    public event PropertyChangedEventHandler? PropertyChanged;
    public EditorLogBus LogBus { get; }
    public IReadOnlyList<LogEntry> LogItems => _logs.Items;
    public IReadOnlyList<LogEntry> ProblemItems => _problems.Items;
    public IReadOnlyList<LogEntry> BuildItems => _builds.Items;
    public IReadOnlyList<LogEntry> TaskItems => _tasks.Items;
    public string LogSummary => EditorLogSummary.From(_buffer.All).Text;
    public bool IsFilterAll => _filter.IsAllSelected;
    public bool IsFilterInfo => _filter.Info;
    public bool IsFilterWarning => _filter.Warning;
    public bool IsFilterError => _filter.Error;
    public EditorLogSource? Source => _filter.Source;
    public string SearchText { get => _filter.SearchText; set => SetSearchText(value ?? ""); }
    public string SourceFilterText => Source is null ? "来源：全部" : $"来源：{SourceText(Source.Value)}";
    public string AllCount => _buffer.All.Count.ToString();
    public string InfoCount => Count(EditorLogLevel.Info).ToString();
    public string WarningCount => Count(EditorLogLevel.Warning).ToString();
    public string ErrorCount => Count(EditorLogLevel.Error).ToString();
    public bool ShowInitialLogEmpty => IsFilterAll && LogItems.Count == 0;
    public bool ShowNoFilterResults => !IsFilterAll && LogItems.Count == 0;
    public int RefreshCount => _logs.RefreshCount;
    public int RebuildCount => _logs.RebuildCount;
    public int CollectionRefreshCount { get; private set; }
    public IReadOnlyList<LogEntry> Entries => _buffer.All;
    public bool IsOpen { get => _isOpen; set { if (_isOpen == value) return; _isOpen = value; OnPropertyChanged(); } }
    public bool IsDetailsOpen { get => _isDetailsOpen; set { if (_isDetailsOpen == value) return; _isDetailsOpen = value; OnPropertyChanged(); } }
    public LogEntry? SelectedLogEntry { get => _selectedLogEntry; set { if (ReferenceEquals(_selectedLogEntry, value)) return; _selectedLogEntry = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasSelectedLogEntry)); OnPropertyChanged(nameof(SelectedLogClipboardText)); } }
    public bool HasSelectedLogEntry => SelectedLogEntry is not null;
    public string SelectedLogClipboardText => SelectedLogEntry is null ? "" : EditorLogClipboardText.From(SelectedLogEntry);
    public bool HasSelectedEntries => _selectedEntries.Length > 0;
    public string SelectedEntriesClipboardText => EditorLogClipboardText.FromMany(_selectedEntries);
    public ICommand ToggleLogCommand { get; }
    public ICommand SelectLogFilterCommand { get; }
    public ICommand ClearLogsCommand { get; }

    public LogPresenter()
    {
        LogBus = new EditorLogBus(_buffer);
        ToggleLogCommand = new RelayCommand(_ => IsOpen = !IsOpen);
        SelectLogFilterCommand = new RelayCommand(name => SetFilter(name?.ToString() ?? "全部"));
        ClearLogsCommand = new RelayCommand(_ => Clear());
        _buffer.Changed += () => Refresh();
    }

    public void SetSelectedEntries(IEnumerable<LogEntry> items)
    {
        _selectedEntries = items as LogEntry[] ?? items.ToArray();
        OnPropertyChanged(nameof(HasSelectedEntries)); OnPropertyChanged(nameof(SelectedEntriesClipboardText));
    }

    public void NotifyLogCopied() => LogBus.Info(EditorLogSource.Editor, EditorLogCategory.Command,
        $"已复制 {_selectedEntries.Length} 条日志到剪贴板", "Ctrl+C 复制选中日志，可粘贴给 AI 审计。");

    public void Refresh(bool counts = true)
    {
        CollectionRefreshCount++;
        var all = _buffer.All;
        if (_logs.Sync(all.Where(_filter.Allows).ToArray())) OnPropertyChanged(nameof(LogItems));
        if (_problems.Sync(all.Where(x => x.Level is EditorLogLevel.Warning or EditorLogLevel.Error).ToArray())) OnPropertyChanged(nameof(ProblemItems));
        if (_builds.Sync(all.Where(entry => EditorLogFilter.Build.Allows(entry)).ToArray())) OnPropertyChanged(nameof(BuildItems));
        if (_tasks.Sync(all.Where(entry => EditorLogFilter.Task.Allows(entry)).ToArray())) OnPropertyChanged(nameof(TaskItems));
        OnPropertyChanged(nameof(LogSummary));
        OnPropertyChanged(nameof(ShowInitialLogEmpty));
        OnPropertyChanged(nameof(ShowNoFilterResults));
        if (counts) foreach (var name in new[] { nameof(AllCount), nameof(InfoCount), nameof(WarningCount), nameof(ErrorCount) }) OnPropertyChanged(name);
    }

    public void SetSearchText(string value) { if (_filter.SearchText == value) return; _filter = _filter with { SearchText = value }; Refresh(false); OnPropertyChanged(nameof(SearchText)); }
    public void SetSource(EditorLogSource? value) { if (_filter.Source == value) return; _filter = _filter with { Source = value }; Refresh(false); OnPropertyChanged(nameof(Source)); OnPropertyChanged(nameof(SourceFilterText)); }
    public void SetFilter(string name)
    {
        if (name == "全部") _filter = _filter.SetAll(true);
        else if (EditorLogFilterText.FromText(name) is var f && f is EditorLogFilter.Info or EditorLogFilter.Warning or EditorLogFilter.Error)
            _filter = _filter.ToggleSeverity(f switch { EditorLogFilter.Info => EditorLogLevel.Info, EditorLogFilter.Warning => EditorLogLevel.Warning, _ => EditorLogLevel.Error });
        else if (name == "清空筛选") _filter = EditorLogFilterState.CreateDefault();
        Refresh(false);
        foreach (var property in new[] { nameof(IsFilterAll), nameof(IsFilterInfo), nameof(IsFilterWarning), nameof(IsFilterError) }) OnPropertyChanged(property);
    }
    public void Clear() => _buffer.Clear();
    public void NotifyCounts() { foreach (var name in new[] { nameof(AllCount), nameof(InfoCount), nameof(WarningCount), nameof(ErrorCount) }) OnPropertyChanged(name); }
    int Count(EditorLogLevel level) => _buffer.All.Count(entry => entry.Level == level);
    static string SourceText(EditorLogSource source) => source switch { EditorLogSource.Render => "渲染", EditorLogSource.Build => "构建", EditorLogSource.Task => "任务", EditorLogSource.Input => "输入", EditorLogSource.Project => "项目", _ => "编辑器" };
    void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
