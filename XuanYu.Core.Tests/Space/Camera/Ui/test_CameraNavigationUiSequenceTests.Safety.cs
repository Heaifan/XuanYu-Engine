using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.UI;

namespace XuanYu.Core.Tests.Camera;

// F3-F2（计划 14.5/14.6）：失败安全与状态合同——取消恢复、非法输入拒绝、导航不 Dirty/Undo。
public sealed partial class CameraNavigationUiSequenceTests
{
    [Fact]
    public void Orbit_cancel_then_dolly_restores_start_camera()
    {
        var vm = NewVm();
        var before = vm.RenderSnapshot.CameraState;
        Assert.True(vm.BeginCameraNavigation(7, 300, 300, false, 800, 600));
        Assert.True(vm.PreviewCameraNavigation(7, 400, 200));
        Assert.True(vm.CancelCameraNavigation("测试取消"));
        Assert.Equal(before.Revision, vm.RenderSnapshot.CameraState.Revision);
        vm.DollyCamera(1.0);
        AssertValid(vm.RenderSnapshot.CameraState);
    }

    [Fact]
    public void Dolly_with_invalid_delta_fails_without_touching_state()
    {
        var vm = NewVm();
        var before = vm.RenderSnapshot.CameraState;
        var centerBefore = vm.ObservationCenter;
        Assert.False(vm.DollyCamera(double.NaN));
        Assert.Equal(before, vm.RenderSnapshot.CameraState);
        Assert.Equal(centerBefore, vm.ObservationCenter);
    }

    [Fact]
    public void Navigation_does_not_mark_dirty_or_push_undo()
    {
        var vm = NewVm();
        vm.RunCommand.Execute("视角-顶");
        vm.DollyCamera(1.0);
        Assert.True(vm.BeginCameraNavigation(7, 300, 300, false, 800, 600));
        Assert.True(vm.PreviewCameraNavigation(7, 360, 240));
        Assert.True(vm.EndCameraNavigation(7));
        Assert.False(vm.IsSceneDirty);
        Assert.True(vm.TransformHistoryCount == 0);
        AssertValid(vm.RenderSnapshot.CameraState);
    }

    static UiVm NewVm()
    {
        var vm = new UiVm(null, () => true);
        vm.UpdateViewportFrame(800, 600);
        return vm;
    }

    static void AssertValid(CameraState camera)
    {
        foreach (var v in new[] { camera.Position, camera.Forward, camera.Right, camera.Up })
        {
            Assert.True(double.IsFinite(v.X) && double.IsFinite(v.Y) && double.IsFinite(v.Z));
        }

        Assert.True(System.Math.Abs(camera.Forward.Length - 1.0) < 1e-9);
        Assert.True(System.Math.Abs(camera.Up.Length - 1.0) < 1e-9);
        Assert.True(System.Math.Abs(camera.Forward.Dot(camera.Up)) < 1e-9);
        Assert.True(camera.Forward.Cross(camera.Up).Length > 0.999999);
    }
}
