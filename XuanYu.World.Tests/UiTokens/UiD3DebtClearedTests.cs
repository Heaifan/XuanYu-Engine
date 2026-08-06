using System.IO;
using System.Linq;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D3：D3 债务清零与行数门禁（基线只减不增；5+100 防回归）。
public sealed class UiD3DebtClearedTests
{
    static readonly string RepoRoot = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..");

    [Fact]
    public void D3_remediated_debt_entries_are_removed_from_baseline()
    {
        // W17（UiWin 背景）+ W21（UiRoot 分隔条/视口边框）：迁移后基线条目必须删除，总数只减不增。
        var remaining = UiDebtBaseline.Entries
            .Where(e => e.Path == "XuanYu.Editor.UI/Win/UiWin.axaml"
                || e.Path == "XuanYu.Editor.UI/Root/UiRoot.axaml").ToList();
        Assert.Empty(remaining);
    }

    [Fact]
    public void Baseline_total_shrinks_by_exactly_four()
    {
        // D3 清除 4 条（W17×1 + W21×3）；基线 230 → 226。
        Assert.Equal(226, UiDebtBaseline.Entries.Count);
    }

    [Fact]
    public void New_d3_files_stay_within_100_line_rule()
    {
        foreach (var rel in new[]
        {
            "XuanYu.Editor.UI/Right/TopTabStripModel.cs",
            "XuanYu.Editor.UI/Right/TopTabStripController.cs",
            "XuanYu.Editor.UI/Right/TopTabStripController.AllTabs.cs",
            "XuanYu.Editor.UI/Right/TopTabStripController.Hint.cs",
            "XuanYu.Editor.UI/Right/TopTabStripController.Visible.cs",
            "XuanYu.Editor.UI/Right/TopTabStripTemplate.axaml",
        })
            Assert.True(File.ReadAllLines(Path.Combine(RepoRoot, rel)).Length <= 100, $"{rel} 超 100 行");
    }
}
