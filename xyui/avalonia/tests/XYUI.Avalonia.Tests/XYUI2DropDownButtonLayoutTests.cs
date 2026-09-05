using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

// XYUI-2-05 布局合同：内容占据 * 列，Chevron 占据固定 28 DIP 槽，不发生视觉重叠。
public sealed partial class XYUI2DropDownButtonRuntimeTests
{
    [Fact]
    public void Content_stays_before_fixed_chevron_track() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var dropdown = new XYDropDownButton { Width = 156, Content = "导出格式：ASTC" };
        var window = XyuiBatchTestHost.Show(dropdown);
        var grid = (Grid)dropdown.GetVisualDescendants().Single(c => c.Name == "PART_Grid");
        var content = grid.Children.OfType<ContentPresenter>()
            .Single(c => c.Name == "PART_ContentPresenter");
        var track = Track(dropdown);

        Assert.Equal(0, Grid.GetColumn(content));
        Assert.Equal(1, Grid.GetColumn(track));
        Assert.Equal(XYDropDownButton.ChevronTrackWidth, grid.ColumnDefinitions[1].Width.Value);
        Assert.True(content.Bounds.Right <= track.Bounds.Left + 0.1,
            "ContentPresenter 不得进入 Chevron Track 固定槽");
        Assert.False(content.IsHitTestVisible, "内容层不得形成第二命中区");
        window.Close();
    });
}
