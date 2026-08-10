using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class ScaleIndicatorVisibilityRuntimeTests
{
    [Fact]
    public void Scale_indicator_stays_visible_when_inspector_tab_is_selected()
    {
        var vm = new UiVm(null, seedInitialScene: false) { RightTabIndex = 0 };
        vm.UpdateViewportFrame(800, 600);
        Assert.True(vm.IsScaleIndicatorVisible);
        Assert.False(string.IsNullOrWhiteSpace(vm.ScaleIndicatorText));
        var projection = vm.RenderProjection.Projection.ScaleIndicator;
        Assert.True(projection.Visible);
        Assert.Equal(vm.ScaleIndicatorText, projection.Label);
    }

    [Fact]
    public void Inspector_tab_dolly_is_not_limited_by_scale_indicator_floor()
    {
        var vm = new UiVm(null, seedInitialScene: false) { RightTabIndex = 0 };
        vm.UpdateViewportFrame(800, 600);
        for (var i = 0; i < 80; i++) vm.DollyCamera(1.0);
        Assert.NotEqual("100 m", vm.ScaleIndicatorText);
        Assert.Equal(ScaleIndicatorMetric.FixedBarWidthDip, vm.ScaleIndicatorWidthDip);
    }
}
