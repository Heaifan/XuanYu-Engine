namespace XuanYu.Editor.UI;

// 底部"最近"通知（F3 方案 B）：完整日志仍按真实时间保留在面板；
// 通知选择策略——最新 Error/Warning 最高优先，其次最新用户动作（Editor/Project），
// Render/Vulkan 信息只作兜底。解决 Vulkan 日志经 Dispatcher.Post 异步到达后覆盖用户通知的问题。
public sealed record EditorLogSummary(int ErrorCount, int WarningCount, string LatestMessage)
{
    public string Text => $"日志  错误 {ErrorCount}  警告 {WarningCount}  最近：{LatestMessage}";

    public static EditorLogSummary From(IReadOnlyList<LogEntry> entries)
    {
        var errors = entries.Count(x => x.Level == EditorLogLevel.Error);
        var warnings = entries.Count(x => x.Level == EditorLogLevel.Warning);
        var latest = ChooseLatest(entries);
        return new EditorLogSummary(errors, warnings, latest);
    }

    static string ChooseLatest(IReadOnlyList<LogEntry> entries)
    {
        if (entries.Count == 0) return "暂无日志";
        for (var i = entries.Count - 1; i >= 0; i--)
        {
            var level = entries[i].Level;
            if (level is EditorLogLevel.Error or EditorLogLevel.Warning) return entries[i].Message;
        }
        for (var i = entries.Count - 1; i >= 0; i--)
        {
            var source = entries[i].Source;
            if (source is EditorLogSource.Editor or EditorLogSource.Project) return entries[i].Message;
        }
        return entries[^1].Message;
    }
}
