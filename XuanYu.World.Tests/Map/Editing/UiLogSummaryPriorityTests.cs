using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map.Editing;

// MAP-A-R2-D4-F3：底部通知优先级（L01/L02/L04/L05）——Error/Warning > Editor 动作 > Render 兜底。
public sealed class UiLogSummaryPriorityTests
{
    static LogEntry Entry(EditorLogSource source, string message,
        EditorLogLevel level = EditorLogLevel.Info) =>
        new("00:00:00", level, source, EditorLogCategory.Command, message);

    static UiVm NewVm() => new(null, () => true);

    [Fact]
    public void L01_lock_notice_beats_later_vulkan_info()
    {
        var entries = new List<LogEntry>
        {
            Entry(EditorLogSource.Editor, "锁定图层：区域 1（区域）"),
            Entry(EditorLogSource.Render, "地图资源更新决策：处理=无需重建；序号=7")
        };
        var summary = EditorLogSummary.From(entries);
        Assert.Contains("锁定图层：区域 1（区域）", summary.Text);
    }

    [Fact]
    public void L02_rename_notice_stays_visible()
    {
        var entries = new List<LogEntry>
        {
            Entry(EditorLogSource.Editor, "重命名图层：区域 1 → 主战区"),
            Entry(EditorLogSource.Render, "地图资源更新决策：处理=无需重建；序号=8")
        };
        var summary = EditorLogSummary.From(entries);
        Assert.Contains("重命名图层：区域 1 → 主战区", summary.Text);
    }

    [Fact]
    public void L04_full_log_keeps_vulkan_entries()
    {
        var entries = new List<LogEntry>
        {
            Entry(EditorLogSource.Editor, "锁定图层：区域 1（区域）"),
            Entry(EditorLogSource.Render, "地图资源更新决策：处理=无需重建；序号=7")
        };
        EditorLogSummary.From(entries);
        Assert.Equal(2, entries.Count);
        Assert.Contains(entries, e => e.Source == EditorLogSource.Render);
    }

    [Fact]
    public void L05_error_warning_beat_editor_actions()
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
    public void Lock_then_summary_shows_lock_action()
    {
        var vm = NewVm();
        vm.SelectedLayer = vm.LayerItems[0];
        vm.LayerInspectorLocked = true;
        Assert.Contains("锁定图层：区域 1（区域）", vm.LogSummary);
    }

    [Fact]
    public void Empty_log_summary_shows_placeholder()
    {
        Assert.Contains("暂无日志", EditorLogSummary.From([]).Text);
    }
}
