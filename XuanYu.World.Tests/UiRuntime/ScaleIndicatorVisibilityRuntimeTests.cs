using XuanYu.Editor.UI;

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
    }
}
