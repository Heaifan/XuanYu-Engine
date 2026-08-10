using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.World.Tests.World;

public sealed class CameraC2DraftFramingTests
{
    [Fact]
    public void C2_R03_focus_centers_three_point_draft_bounds()
    {
        var setup = CameraC2MapFramingTestsHelpers.DraftVm(3);
        var minX = setup.Points.Min(point => point.X);
        var maxX = setup.Points.Max(point => point.X);
        var minY = setup.Points.Min(point => point.Y);
        var maxY = setup.Points.Max(point => point.Y);

        setup.Vm.RunCommand.Execute("聚焦");

        Assert.Equal((minX + maxX) / 2.0, setup.Vm.ObservationCenter.X, precision: 6);
        Assert.Equal((minY + maxY) / 2.0, setup.Vm.ObservationCenter.Y, precision: 6);
        Assert.Equal(setup.Vm.MapSession.CurrentMap.Surface.BaseHeightMeters,
            setup.Vm.ObservationCenter.Z, precision: 6);
    }

    [Fact]
    public void C2_R04_focus_one_point_draft_uses_safe_distance()
    {
        var setup = CameraC2MapFramingTestsHelpers.DraftVm(1);

        setup.Vm.RunCommand.Execute("聚焦");

        var camera = setup.Vm.RenderSnapshot.CameraState;
        Assert.Equal(ProjectionMode.Perspective, camera.Mode);
        Assert.True(camera.Position.DistanceTo(setup.Vm.ObservationCenter) > 50.0);
        CameraC2MapFramingTestsHelpers.AssertFinite(camera);
    }

    [Fact]
    public void C2_R08_focus_draft_then_pointer_move_is_safe()
    {
        var setup = CameraC2MapFramingTestsHelpers.DraftVm(3);
        setup.Vm.RunCommand.Execute("聚焦");

        var error = Record.Exception(() => setup.Vm.RegionDrawingPointerMoved(
            400, 300, CameraC2MapFramingTestsHelpers.Viewport));

        Assert.Null(error);
    }
}
