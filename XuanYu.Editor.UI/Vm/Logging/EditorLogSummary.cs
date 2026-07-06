namespace XuanYu.Editor.UI;

public sealed record EditorLogSummary(int ErrorCount, int WarningCount, string LatestMessage)
{
    public string Text => $"日志  错误 {ErrorCount}  警告 {WarningCount}  最近：{LatestMessage}";

    public static EditorLogSummary From(IReadOnlyList<LogEntry> entries)
    {
        var errors = entries.Count(x => x.Level == EditorLogLevel.Error);
        var warnings = entries.Count(x => x.Level == EditorLogLevel.Warning);
        var latest = entries.Count == 0 ? "暂无日志" : entries[^1].Message;
        return new EditorLogSummary(errors, warnings, latest);
    }
}
