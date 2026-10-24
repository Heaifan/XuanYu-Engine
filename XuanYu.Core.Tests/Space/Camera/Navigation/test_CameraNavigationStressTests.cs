using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.UI;

namespace XuanYu.Core.Tests.Camera;

// F3-F2（计划 14.4）：重复导航压力测试——固定序列循环 100 次，检测累积误差与逐步失去正交。
public sealed class CameraNavigationStressTests
{
    [Fact]
    public void Repeated_navigation_cycle_stays_orthonormal_100_times()
    {
        var vm = new UiVm(null, () => true);
        vm.UpdateViewportFrame(800, 600);

        for (var i = 0; i < 100; i++)
        {
            vm.DollyCamera(1.0);
            vm.DollyCamera(-1.0);
            Assert.True(vm.BeginCameraNavigation(7, 300, 300, false, 800, 600));
            Assert.True(vm.PreviewCameraNavigation(7, 340, 260));
            Assert.True(vm.EndCameraNavigation(7));
            Assert.True(vm.BeginCameraNavigation(8, 300, 300, true, 800, 600));
            Assert.True(vm.PreviewCameraNavigation(8, 330, 270));
            Assert.True(vm.EndCameraNavigation(8));
            vm.RunCommand.Execute("视角-顶");
            vm.RunCommand.Execute("视角-前");
            AssertValid(vm.RenderSnapshot.CameraState);
        }
    }

    static void AssertValid(CameraState camera)
    {
        Assert.True(System.Math.Abs(camera.Forward.Length - 1.0) < 1e-9);
        Assert.True(System.Math.Abs(camera.Right.Length - 1.0) < 1e-9);
        Assert.True(System.Math.Abs(camera.Up.Length - 1.0) < 1e-9);
        Assert.True(System.Math.Abs(camera.Forward.Dot(camera.Up)) < 1e-9);
        Assert.True(System.Math.Abs(camera.Forward.Dot(camera.Right)) < 1e-9);
        Assert.True(System.Math.Abs(camera.Right.Dot(camera.Up)) < 1e-9);
        Assert.True(camera.Forward.Cross(camera.Up).Length > 0.999999);
    }
}
