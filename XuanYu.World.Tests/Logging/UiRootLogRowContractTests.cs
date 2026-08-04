using System.IO;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Logging;

// MAP-A-R2-D3-F4：日志区垂直尺寸自适应源码合同。
// 根因（A4 裁定）：UiRoot Row3 日志区 Auto+MaxHeight=420 与 Row1 主工作区 MinHeight=320
// 的最小和超过矮窗口可用高度 → 日志区被窗口底部裁切（外部布局边界，非滚动问题）。
// 本测试锁定修复后的布局合同；几何级验证由真机复验（F4-A1~A8）承担。
public sealed class UiRootLogRowContractTests
{
    static readonly string UiRootAxaml = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Root", "UiRoot.axaml"));
    static readonly string UiRootCs = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Root", "UiRoot.axaml.cs"));
    static readonly string FootAxaml = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Foot", "Foot.axaml"));

    [Fact]
    public void Log_row_has_upper_cap()
    {
        Assert.Contains("MaxHeight=\"420\"", UiRootAxaml);
    }

    [Fact]
    public void Main_work_area_has_floor()
    {
        Assert.Contains("MinHeight=\"320\"", UiRootAxaml);
    }

    [Fact]
    public void Code_adapts_log_row_to_window_height()
    {
        // 展开时按可用高度 clamp 为像素行；折叠时回 Auto——缺一不可。
        Assert.Contains("GridLength.Auto", UiRootCs);
        Assert.Contains("Math.Clamp", UiRootCs);
        Assert.Contains("IsLogOpen", UiRootCs);
    }

    [Fact]
    public void Log_panel_border_no_longer_blocks_shrinking()
    {
        // F4：日志展开 Border 不得保留 180 DIP 最低高度——矮窗口下会阻止列表 Viewport 缩小。
        Assert.DoesNotContain("MinHeight=\"180\"", FootAxaml);
    }

    [Fact]
    public void Log_panel_can_shrink_gracefully()
    {
        Assert.Contains("MinHeight=\"0\"", FootAxaml);
    }
}
