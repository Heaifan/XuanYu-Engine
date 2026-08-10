using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.World;

public sealed partial class CameraNavigationUiTests
{
    [Fact]
    public void Focus_without_selected_entity_leaves_camera_unchanged()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var before = vm.RenderSnapshot.CameraState;
        var center = vm.ObservationCenter;

        vm.RunCommand.Execute("聚焦");

        Assert.Equal(before, vm.RenderSnapshot.CameraState);
        Assert.Equal(center, vm.ObservationCenter);
        Assert.Equal("当前没有可聚焦对象。", vm.FooterMessage);
    }
}
