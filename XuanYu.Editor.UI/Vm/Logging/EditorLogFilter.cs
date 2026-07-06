namespace XuanYu.Editor.UI;

public enum EditorLogFilter
{
    All,
    Info,
    Warning,
    Error,
    Build,
    Task,
    Input,
    Render
}

public static class EditorLogFilterText
{
    public static EditorLogFilter FromText(string text) => text switch
    {
        "信息" => EditorLogFilter.Info,
        "警告" => EditorLogFilter.Warning,
        "错误" => EditorLogFilter.Error,
        "构建" => EditorLogFilter.Build,
        "任务" => EditorLogFilter.Task,
        "输入" => EditorLogFilter.Input,
        "渲染" => EditorLogFilter.Render,
        _ => EditorLogFilter.All
    };
}
