using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.UI;

namespace XuanYu.Core.Tests.Camera;

// F3-F2：UiVm 相机导航组合序列——标准视角/Orbit/Pan/Dolly/Resize 任意组合不抛异常且基合法。
public sealed partial class CameraNavigationUiSequenceTests
{
    [Fact]
    public void Top_view_orbit_pan_dolly_chain_keeps_valid_camera()
    {
        var vm = NewVm();
        var before = vm.RenderSnapshot.CameraState;
        vm.RunCommand.Execute("视角-顶");
        AssertValid(vm.RenderSnapshot.CameraState);
        vm.DollyCamera(1.0);
        Assert.True(vm.BeginCameraNavigation(7, 300, 300, false, 800, 600));
        Assert.True(vm.PreviewCameraNavigation(7, 340, 260));
        Assert.True(vm.EndCameraNavigation(7));
        vm.DollyCamera(-1.0);
        AssertValid(vm.RenderSnapshot.CameraState);
        Assert.True(vm.RenderSnapshot.CameraState.Revision > before.Revision);
    }

    [Fact]
    public void Bottom_view_pan_resize_dolly_chain_keeps_valid_camera()
    {
        var vm = NewVm();
        vm.RunCommand.Execute("视角-底");
        vm.DollyCamera(0.5);
        Assert.True(vm.BeginCameraNavigation(7, 300, 300, true, 800, 600));
        Assert.True(vm.PreviewCameraNavigation(7, 330, 270));
        Assert.True(vm.EndCameraNavigation(7));
        vm.UpdateViewportFrame(1000, 700);
        vm.DollyCamera(1.0);
        AssertValid(vm.RenderSnapshot.CameraState);
    }

    [Fact]
    public void Gizmo_orbit_commit_then_dolly_keeps_center()
    {
        var vm = NewVm();
        var centerBefore = vm.ObservationCenter;
        Assert.True(vm.BeginCameraNavigation(7, 300, 300, false, 800, 600));
        Assert.True(vm.PreviewCameraNavigation(7, 380, 220));
        Assert.True(vm.EndCameraNavigation(7));
        Assert.Equal(centerBefore, vm.ObservationCenter);
        vm.DollyCamera(1.0);
        Assert.Equal(centerBefore, vm.ObservationCenter);
        AssertValid(vm.RenderSnapshot.CameraState);
    }
}
