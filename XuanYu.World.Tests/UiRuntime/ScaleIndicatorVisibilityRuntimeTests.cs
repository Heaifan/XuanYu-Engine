using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class ScaleIndicatorVisibilityRuntimeTests
{
    [Fact]
    public void Scale_indicator_hides_below_100m_when_inspector_tab_is_selected()
    {
        var vm = new UiVm(null, seedInitialScene: false) { RightTabIndex = 0 };
        vm.UpdateViewportFrame(800, 600);
        Assert.False(vm.IsScaleIndicatorVisible);
        Assert.True(string.IsNullOrWhiteSpace(vm.ScaleIndicatorText));
        var projection = vm.RenderProjection.Projection.ScaleIndicator;
        Assert.False(projection.Visible);
        Assert.Equal(vm.ScaleIndicatorText, projection.Label);
    }

    [Fact]
    public void Inspector_tab_dolly_is_not_limited_by_scale_indicator_floor()
    {
        var vm = new UiVm(null, seedInitialScene: false) { RightTabIndex = 0 };
        vm.UpdateViewportFrame(800, 600);
        var before = vm.NavigationCamera;
        for (var i = 0; i < 80; i++) vm.DollyCamera(1.0);
        var after = vm.NavigationCamera;
        Assert.NotEqual(before.Position, after.Position);
        Assert.True(after.Revision > before.Revision);
        Assert.InRange(vm.ScaleIndicatorWidthDip, 0.0, ScaleIndicatorMetric.FixedBarWidthDip);
    }
}
