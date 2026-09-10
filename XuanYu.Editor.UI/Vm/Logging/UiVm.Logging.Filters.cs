namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public bool IsLogFilterAll => _logFilterState.IsAllSelected;
    public bool IsLogFilterInfo => _logFilterState.Info;
    public bool IsLogFilterWarning => _logFilterState.Warning;
    public bool IsLogFilterError => _logFilterState.Error;
    public EditorLogSource? LogSourceFilter => _logFilterState.Source;
    public string LogSourceFilterText => LogSourceFilter is null ? "来源：全部" : $"来源：{SourceText(LogSourceFilter.Value)}";
    public string LogSearchText { get => _logFilterState.SearchText; set { var text = value ?? ""; if (_logFilterState.SearchText == text) return; _logFilterState = _logFilterState with { SearchText = text }; RefreshLogBindings(); } }
    public string InfoCount => Count(EditorLogLevel.Info).ToString();
    public string WarningCount => Count(EditorLogLevel.Warning).ToString();
    public string ErrorCount => Count(EditorLogLevel.Error).ToString();
    public string AllCount => _logBuffer.All.Count.ToString();

    void SetLogFilter(string name)
    {
        if (name == "全部") _logFilterState = _logFilterState.SetAll(true);
        else if (EditorLogFilterText.FromText(name) is var filter &&
                 (filter is EditorLogFilter.Info or EditorLogFilter.Warning or EditorLogFilter.Error))
            _logFilterState = _logFilterState.ToggleSeverity(filter switch { EditorLogFilter.Info => EditorLogLevel.Info, EditorLogFilter.Warning => EditorLogLevel.Warning, _ => EditorLogLevel.Error });
        else if (name == "清空筛选") _logFilterState = EditorLogFilterState.CreateDefault();
        RefreshLogBindings();
    }

    public void SetLogSource(EditorLogSource? source) { _logFilterState = _logFilterState with { Source = source }; RefreshLogBindings(); }
    void ClearLogs() { _logBuffer.Clear(); RefreshLogBindings(); }
    int Count(EditorLogLevel level) => _logBuffer.All.Count(entry => entry.Level == level);
    static string SourceText(EditorLogSource source) => source switch { EditorLogSource.Render => "渲染", EditorLogSource.Build => "构建", EditorLogSource.Task => "任务", EditorLogSource.Input => "输入", EditorLogSource.Project => "项目", _ => "编辑器" };
}
