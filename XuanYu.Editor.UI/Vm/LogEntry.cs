namespace XuanYu.Editor.UI;

public sealed record LogEntry(
    string Time,
    EditorLogLevel Level,
    EditorLogSource Source,
    EditorLogCategory Category,
    string Message,
    string Detail = "",
    string ContextId = "",
    string CorrelationId = "",
    int RepeatCount = 1)
{
    public string LevelText => Level switch
    {
        EditorLogLevel.Trace => "追踪",
        EditorLogLevel.Debug => "调试",
        EditorLogLevel.Info => "信息",
        EditorLogLevel.Warning => "警告",
        EditorLogLevel.Error => "错误",
        _ => "信息"
    };

    public string SourceText => Source switch
    {
        EditorLogSource.Editor => "编辑器",
        EditorLogSource.Project => "项目",
        EditorLogSource.Render => "渲染",
        EditorLogSource.Build => "构建",
        EditorLogSource.Task => "任务",
        EditorLogSource.Input => "输入",
        _ => "编辑器"
    };

    public string CategoryText => Category switch
    {
        EditorLogCategory.Layout => "布局",
        EditorLogCategory.Load => "加载",
        EditorLogCategory.Backend => "后端",
        EditorLogCategory.Queue => "队列",
        EditorLogCategory.Import => "导入",
        EditorLogCategory.Capture => "捕获",
        EditorLogCategory.Selection => "选择",
        EditorLogCategory.Tool => "工具",
        EditorLogCategory.Command => "命令",
        EditorLogCategory.Save => "保存",
        _ => "布局"
    };
    public bool IsRepeated => RepeatCount > 1;
    public string RepeatText => IsRepeated ? $"重复 {RepeatCount} 次" : "";

    public string Accent => Level switch
    {
        EditorLogLevel.Error => "#c75b5b",
        EditorLogLevel.Warning => "#d89b32",
        EditorLogLevel.Info => "#4f7fb8",
        EditorLogLevel.Debug => "#6b7a90",
        EditorLogLevel.Trace => "#8b96a8",
        _ => "#4f7fb8"
    };
}
