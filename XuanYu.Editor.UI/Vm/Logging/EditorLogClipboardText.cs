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
}
