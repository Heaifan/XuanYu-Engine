using System.Linq;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D3：顶层页签条纯布局状态机测试（合同 §10.1：单行/箭头/渐隐/滚轮/当前可见/一次性提示/全部页签）。
public sealed class UiTopTabStripModelTests
{
    static TopTabStripModel New(double extent, double viewport, double offset)
    {
        var m = new TopTabStripModel();
        m.Update(extent, viewport, offset);
        return m;
    }

    [Fact]
    public void Overflow_requires_extent_beyond_viewport()
    {
        Assert.True(New(1000, 500, 0).Overflowing);
        Assert.False(New(500, 500, 0).Overflowing);
        Assert.False(New(400, 500, 0).Overflowing);
    }

    [Fact]
    public void Arrow_state_follows_offset_boundaries()
    {
        var m = New(1000, 500, 0);
        Assert.True(m.CanScrollLeft == false && m.CanScrollRight);
        m.Update(1000, 500, 500);
        Assert.True(m.CanScrollLeft && m.CanScrollRight == false);
        m.Update(1000, 500, 250);
        Assert.True(m.CanScrollLeft && m.CanScrollRight);
        m.Update(500, 500, 0);
        Assert.True(m.CanScrollLeft == false && m.CanScrollRight == false);
    }

    [Fact]
    public void Fade_follows_hidden_direction()
    {
        var m = New(1000, 500, 250);
        Assert.Equal(m.CanScrollLeft, m.FadeLeft);
        Assert.Equal(m.CanScrollRight, m.FadeRight);
    }

    [Fact]
    public void Wheel_delta_maps_to_horizontal_and_offset_clamps()
    {
        Assert.Equal(120, New(1000, 500, 0).WheelDelta(120));
        Assert.Equal(-120, New(1000, 500, 0).WheelDelta(-120));
        var m = New(1000, 500, 999);
        Assert.Equal(500, m.OffsetX);
        m.Update(1000, 500, -10);
        Assert.Equal(0, m.OffsetX);
    }

    [Fact]
    public void Arrow_step_uses_registered_width_token()
    {
        // 箭头单击步进必须与正式 Token Size.Width.96 一致（不得另造步进值）。
        var size96 = UiSourceContractAnalyzer.ParseManifest()
            .First(t => t.Key == "Size.Width.96").Value;
        Assert.Equal(TopTabStripModel.ScrollStep, double.Parse(size96));
    }

    [Fact]
    public void Scroll_arrow_moves_by_step_and_clamps()
    {
        var m = New(1000, 500, 250);
        Assert.Equal(250 + TopTabStripModel.ScrollStep, m.ScrollRight());
        Assert.Equal(250 - TopTabStripModel.ScrollStep, m.ScrollLeft());
        m.Update(1000, 500, 500);
        Assert.Equal(500, m.ScrollRight());
        m.Update(1000, 500, 0);
        Assert.Equal(0, m.ScrollLeft());
    }

    [Fact]
    public void Fully_visible_requires_whole_item_inside_viewport()
    {
        var m = New(1000, 500, 0);
        Assert.True(m.IsFullyVisible(100, 50));
        Assert.False(m.IsFullyVisible(480, 50));
        m.Update(1000, 500, 300);
        Assert.True(m.IsFullyVisible(310, 50));
        Assert.False(m.IsFullyVisible(100, 50));
    }

    [Fact]
    public void Offset_for_visible_scrolls_minimal_amount()
    {
        var m = New(1000, 500, 300);
        Assert.Equal(300, m.OffsetForVisible(400, 50));   // 已完全可见：不动
        Assert.Equal(300, m.OffsetForVisible(350, 50));   // 完全可见但左侧贴近：不动（回归）
        Assert.Equal(100, m.OffsetForVisible(100, 50));   // 左侧越界：左移
        m.Update(1000, 500, 0);
        Assert.Equal(30, m.OffsetForVisible(480, 50));    // 右侧越界：右移
    }
}
