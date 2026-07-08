using System.Collections.Generic;

namespace XuanYu.Editor.UI;

public static class EditorLogClipboardText
{
    public static string From(LogEntry entry) => string.Join(Environment.NewLine,
        $"时间：{entry.Time}",
        $"级别：{entry.LevelText}",
        $"来源：{entry.SourceText}",
        $"分类：{entry.CategoryText}",
        $"消息：{entry.Message}",
        $"重复次数：{entry.RepeatCount}",
        $"详情：{entry.DetailText}",
        $"上下文：{entry.ContextText}",
        $"操作链路：{entry.CorrelationText}");

    public static string FromMany(IEnumerable<LogEntry> entries)
    {
        var lines = new List<string> { "时间\t级别\t来源\t分类\t消息\t详情" };
        foreach (var e in entries)
            lines.Add(string.Join("\t", e.Time, e.LevelText, e.SourceText, e.CategoryText, e.Message, e.DetailText));
        return string.Join(Environment.NewLine, lines);
    }
}
