using Avalonia;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

public sealed class XYUI4DropIndicatorTests
{
    [Fact]
    public void Drop_indicator_is_a_non_interactive_overlay_with_explicit_target()
    {
        var indicator = new XYDropIndicator
        {
            TargetRect = new Rect(12, 18, 120, 42),
            State = XyuiDropState.Valid,
            ActionHint = "添加到组"
        };

        Assert.Equal("XYUI-4-4.12", indicator.CanonicalId);
        Assert.False(indicator.IsHitTestVisible);
        Assert.Equal(new Rect(12, 18, 120, 42), indicator.TargetRect);
        Assert.Equal(XyuiDropState.Valid, indicator.State);
        Assert.Equal("添加到组", indicator.ActionHint);
    }

    [Fact]
    public void Drop_indicator_supports_non_color_state_semantics()
    {
        var indicator = new XYDropIndicator { State = XyuiDropState.Conditional, IsActive = true };

        Assert.True(indicator.IsActive);
        Assert.Equal(XyuiDropState.Conditional, indicator.State);
        Assert.Equal("GAP-004", indicator.ConditionalTokenGap);
        Assert.Equal(0, indicator.DesiredSize.Width);
        Assert.Equal(0, indicator.DesiredSize.Height);
    }
}
