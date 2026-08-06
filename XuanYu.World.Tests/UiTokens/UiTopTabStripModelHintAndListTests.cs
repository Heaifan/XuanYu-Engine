using System.Linq;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D3：一次性提示门与「全部页签」列表（合同 §10.1-7/8/11）。
public sealed class UiTopTabStripModelHintAndListTests
{
    [Fact]
    public void First_overflow_hint_fires_only_once_per_environment()
    {
        Assert.False(TopTabStripModel.ShouldShowHint(overflowing: false, shownThisSession: false, persisted: false));
        Assert.True(TopTabStripModel.ShouldShowHint(overflowing: true, shownThisSession: false, persisted: false));
        Assert.False(TopTabStripModel.ShouldShowHint(overflowing: true, shownThisSession: true, persisted: false));
        Assert.False(TopTabStripModel.ShouldShowHint(overflowing: true, shownThisSession: false, persisted: true));
    }

    [Fact]
    public void All_tabs_list_contains_real_tabs_and_marks_current()
    {
        var items = TopTabStripModel.BuildTabList(["检查器", "地图编辑器", "调试"], 1);
        Assert.Equal(3, items.Count);
        Assert.Equal("地图编辑器", items[1].Header);
        Assert.True(items[1].IsSelected);
        Assert.False(items[0].IsSelected);
        Assert.Empty(TopTabStripModel.BuildTabList([], -1));
    }
}
