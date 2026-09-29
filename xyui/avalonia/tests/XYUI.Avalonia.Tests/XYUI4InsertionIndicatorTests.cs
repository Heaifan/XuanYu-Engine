using Avalonia;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

public sealed class XYUI4InsertionIndicatorTests
{
    [Fact]
    public void Insertion_indicator_is_a_non_interactive_position_overlay()
    {
        var indicator = new XYInsertionIndicator
        {
            TargetRect = new Rect(20, 40, 180, 32),
            Mode = XyuiInsertionMode.Before,
            IndentLevel = 2,
            IsActive = true
        };

        Assert.Equal("XYUI-4-4.13", indicator.CanonicalId);
        Assert.False(indicator.IsHitTestVisible);
        Assert.Equal(new Rect(20, 40, 180, 32), indicator.TargetRect);
        Assert.Equal(XyuiInsertionMode.Before, indicator.Mode);
        Assert.Equal(32, indicator.AnchorOffset);
    }

    [Fact]
    public void Insertion_indicator_exposes_gap_preview_without_forcing_layout()
    {
        var indicator = new XYInsertionIndicator
        {
            IsActive = true,
            Mode = XyuiInsertionMode.Into,
            ShowGapPreview = true,
            GapRect = new Rect(40, 100, 160, 36),
            GapText = "插入此处"
        };

        Assert.True(indicator.ShowGapPreview);
        Assert.Equal(new Rect(40, 100, 160, 36), indicator.GapRect);
        Assert.Equal("插入此处", indicator.GapText);
        Assert.Equal(0, indicator.DesiredSize.Width);
        Assert.Equal(0, indicator.DesiredSize.Height);
    }
}
