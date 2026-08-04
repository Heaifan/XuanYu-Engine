using System.IO;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Logging;

// MAP-A-R2-D3-F3：控制器源码合同——两阶段尾项定位结构与副作用禁令。
// 说明：仓库无 Avalonia Headless 基础设施（不引入新依赖），
// 几何级验证由真机验收（A4）承担；本测试锁定源码结构合同。
public sealed class LogListAutoScrollControllerContractTests
{
    static readonly string[] Sources =
    [
        File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
            "..", "..", "..", "..", "XuanYu.Editor.UI", "Foot", "LogListAutoScrollController.cs")),
        File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
            "..", "..", "..", "..", "XuanYu.Editor.UI", "Foot", "LogListAutoScrollController.Follow.cs")),
    ];

    static string All => string.Join('\n', Sources);

    [Fact]
    public void Latest_item_is_scroll_target()
    {
        Assert.Contains("ScrollIntoView", All);
        Assert.Contains("GetLatestItem", All);
    }

    [Fact]
    public void Two_stage_positioning_render_then_background()
    {
        Assert.Contains("DispatcherPriority.Render", All);
        Assert.Contains("DispatcherPriority.Background", All);
    }

    [Fact]
    public void Final_correction_reads_final_scroll_range()
    {
        Assert.Contains("Extent.Height", All);
        Assert.Contains("Viewport.Height", All);
    }

    [Fact]
    public void Final_correction_scheduled_at_most_once()
    {
        Assert.Contains("_tailCorrectionScheduled", All);
    }

    [Fact]
    public void Correction_preserves_horizontal_offset()
    {
        Assert.Contains("Offset.X", All);
    }

    [Fact]
    public void Request_merge_invalidates_stale_requests()
    {
        Assert.Contains("_requestVersion", All);
    }

    [Fact]
    public void Programmatic_correction_guard_exists()
    {
        Assert.Contains("_programmaticCorrection", All);
    }

    [Fact]
    public void No_recursive_polling_or_timers()
    {
        Assert.DoesNotContain("while (", All);
        Assert.DoesNotContain("DispatcherTimer", All);
        Assert.DoesNotContain("Task.Delay", All);
    }

    [Fact]
    public void No_editor_log_bus_reference()
    {
        Assert.DoesNotContain("EditorLogBus", All);
    }
}
