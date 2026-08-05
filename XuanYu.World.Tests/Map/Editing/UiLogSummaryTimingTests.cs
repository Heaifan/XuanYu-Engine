using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Map.Editing;

// MAP-A-R2-D4-F3 预验收补丁：通知时序（F/G/H）——最新重要事件优先，旧警告不永久霸占。
public sealed class UiLogSummaryTimingTests
{
    static LogEntry Entry(EditorLogSource source, string message,
        EditorLogLevel level = EditorLogLevel.Info) =>
        new("00:00:00", level, source, EditorLogCategory.Command, message);

    [Fact]
    public void F_old_warning_then_new_editor_action_shows_action()
    {
        var entries = new List<LogEntry>
        {
            Entry(EditorLogSource.Render, "警告：交换链重建", EditorLogLevel.Warning),
            Entry(EditorLogSource.Editor, "重命名图层：区域 1 → 主战区"),
            Entry(EditorLogSource.Render, "地图资源更新决策：处理=无需重建；序号=9")
        };
        var summary = EditorLogSummary.From(entries);
        Assert.Contains("重命名图层：区域 1 → 主战区", summary.Text);
    }

    [Fact]
    public void G_old_editor_action_then_new_warning_shows_warning()
    {
        var entries = new List<LogEntry>
        {
            Entry(EditorLogSource.Editor, "锁定图层：区域 1（区域）"),
            Entry(EditorLogSource.Render, "警告：交换链重建", EditorLogLevel.Warning)
        };
        var summary = EditorLogSummary.From(entries);
        Assert.Contains("交换链重建", summary.Text);
    }

    [Fact]
    public void H_full_log_keeps_all_entries_with_order()
    {
        var entries = new List<LogEntry>
        {
            Entry(EditorLogSource.Editor, "重命名图层：区域 1 → 主战区"),
            Entry(EditorLogSource.Render, "地图资源更新决策：处理=无需重建；序号=9")
        };
        EditorLogSummary.From(entries);
        Assert.Equal(2, entries.Count);
        Assert.Equal(EditorLogSource.Render, entries[^1].Source); // Vulkan 记录按真实时间保留
    }
}
