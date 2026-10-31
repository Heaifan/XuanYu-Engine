using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.World;

public sealed partial class CameraNavigationUiTests
{
    [Fact]
    public void Camera_controller_owns_pan_state_and_observation_center()
    {
        var camera = new CameraController();
        var start = camera.Camera;
        var center = camera.ObservationCenter;
        Assert.True(camera.TryBeginNavigation(9, 100, 100, CameraSessionMode.Pan, 800, 600, null));
        Assert.True(camera.TryPreviewNavigation(9, 180, 145, out var result, out _, out _));
        camera.ApplyFrame(result);

        Assert.NotEqual(start.Position, camera.Camera.Position);
        Assert.NotEqual(center, camera.ObservationCenter);
        Assert.Equal(result.Camera, camera.Camera);
        Assert.Equal(result.ObservationCenter, camera.ObservationCenter);
    }

    [Fact]
    public void Camera_controller_dolly_keeps_center_and_minimum_distance()
    {
        var camera = new CameraController();
        var center = camera.ObservationCenter;

        Assert.True(camera.TryDolly(1000, null, out var result, out _));
        Assert.Equal(0.25, result.Camera.Position.DistanceTo(result.ObservationCenter), 8);
        Assert.Equal(center, result.ObservationCenter);

        camera.ApplyFrame(result);
        Assert.Equal(center, camera.ObservationCenter);
    }

    [Fact]
    public void Pan_publishes_camera_state_and_can_cancel()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var before = vm.RenderSnapshot.CameraState;
        var center = vm.ObservationCenter;
        Assert.True(vm.BeginCameraNavigation(17, 100, 100, true, 800, 600));
        Assert.True(vm.IsCameraNavigationActive);

        Assert.True(vm.PreviewCameraNavigation(17, 180, 145));

        Assert.NotEqual(before.Position, vm.RenderSnapshot.CameraState.Position);
        Assert.NotEqual(center, vm.ObservationCenter);
        Assert.Equal(before.Forward, vm.RenderSnapshot.CameraState.Forward);
        Assert.True(vm.CancelCameraNavigation("test"));
        Assert.False(vm.IsCameraNavigationActive);
        Assert.Equal(before.Position, vm.RenderSnapshot.CameraState.Position);
        Assert.Equal(center, vm.ObservationCenter);
    }

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
