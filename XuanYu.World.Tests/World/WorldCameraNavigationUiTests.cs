using XuanYu.Core.Math;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.World;

public sealed class WorldCameraNavigationUiTests
{
    [Fact]
    public void Frame_selected_updates_observation_center()
    {
        var vm = new UiVm(null, () => true);
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(item => item.Key == "EntityId(5)");

        vm.RunCommand.Execute("聚焦");

        Near(new Vector3d(6, 0, 0), vm.ObservationCenter);
    }

    [Fact]
    public void Frame_all_updates_observation_center()
    {
        var vm = new UiVm(null, () => true);

        vm.RunCommand.Execute("查看全部");

        Near(new Vector3d(3, 0.75, 0), vm.ObservationCenter);
    }

    [Fact]
    public void Camera_cancel_restores_start_camera_and_center()
    {
        var vm = new UiVm(null, () => true);
        var start = vm.RenderSnapshot.CameraState;
        var center = vm.ObservationCenter;

        Assert.True(vm.BeginCameraNavigation(7, 100, 100, false, 800, 600));
        Assert.True(vm.PreviewCameraNavigation(7, 220, 160));
        Assert.True(vm.CancelCameraNavigation("Escape"));

        Near(start.Position, vm.RenderSnapshot.CameraState.Position);
        Near(center, vm.ObservationCenter);
    }

    [Fact]
    public void Camera_rejects_gizmo_capture_and_stale_pointer_end()
    {
        var vm = new UiVm(null, () => true);
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(item => item.Key == "EntityId(1)");
        vm.SelectToolCommand.Execute("移动");
        vm.InteractionCommand.Execute("Begin");

        Assert.False(vm.BeginCameraNavigation(7, 100, 100, false, 800, 600));
        vm.CancelInteractionFromEscape();
        Assert.True(vm.BeginCameraNavigation(8, 100, 100, false, 800, 600));
        Assert.False(vm.EndCameraNavigation(7));
    }

    [Fact]
    public void Dolly_is_ignored_during_camera_capture()
    {
        var vm = new UiVm(null, () => true);
        var before = vm.RenderSnapshot.CameraState.Position;

        Assert.True(vm.BeginCameraNavigation(7, 100, 100, false, 800, 600));
        Assert.False(vm.DollyCamera(1));

        Near(before, vm.RenderSnapshot.CameraState.Position);
    }

    [Fact]
    public void Camera_capture_blocks_picking()
    {
        var vm = new UiVm(null, () => true);
        var before = vm.SelectionKey;

        Assert.True(vm.BeginCameraNavigation(7, 100, 100, false, 800, 600));

        Assert.False(vm.PickViewportPointer(100, 100, 800, 600, 800, 600, 1, 1, true));
        Assert.Equal(before, vm.SelectionKey);
    }

    static void Near(Vector3d expected, Vector3d actual)
    {
        Assert.Equal(expected.X, actual.X, precision: 6);
        Assert.Equal(expected.Y, actual.Y, precision: 6);
        Assert.Equal(expected.Z, actual.Z, precision: 6);
    }
}
