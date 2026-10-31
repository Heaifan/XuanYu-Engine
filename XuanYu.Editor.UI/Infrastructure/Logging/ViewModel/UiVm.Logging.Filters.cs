namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public bool IsLogFilterAll => _logPresenter.IsFilterAll;
    public bool IsLogFilterInfo => _logPresenter.IsFilterInfo;
    public bool IsLogFilterWarning => _logPresenter.IsFilterWarning;
    public bool IsLogFilterError => _logPresenter.IsFilterError;
    public EditorLogSource? LogSourceFilter => _logPresenter.Source;
    public string LogSourceFilterText => LogSourceFilter is null ? "来源：全部" : $"来源：{SourceText(LogSourceFilter.Value)}";
    public string LogSearchText { get => _logPresenter.SearchText; set { var text = value ?? ""; if (_logPresenter.SearchText != text) _logPresenter.SetSearchText(text); } }
    public string InfoCount => _logPresenter.InfoCount;
    public string WarningCount => _logPresenter.WarningCount;
    public string ErrorCount => _logPresenter.ErrorCount;
    public string AllCount => _logPresenter.AllCount;

    void SetLogFilter(string name)
    {
        _logPresenter.SetFilter(name);
    }

    public void SetLogSource(EditorLogSource? source) => _logPresenter.SetSource(source);
    void ClearLogs() => _logPresenter.Clear();
    static string SourceText(EditorLogSource source) => source switch { EditorLogSource.Render => "渲染", EditorLogSource.Build => "构建", EditorLogSource.Task => "任务", EditorLogSource.Input => "输入", EditorLogSource.Project => "项目", _ => "编辑器" };
}
