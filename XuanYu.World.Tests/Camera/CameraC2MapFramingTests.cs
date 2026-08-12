using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.World.Tests.World;

public sealed class CameraC2MapFramingTests
{
    [Fact]
    public void C2_R01_map_view_all_covers_10000_meter_bounds()
    {
        var vm = CameraC2MapFramingTestsHelpers.MapVm();

        vm.RunCommand.Execute("查看全部");

        Assert.Equal(new Vector3d(0, 0, 0), vm.ObservationCenter);
        CameraC2MapFramingTestsHelpers.AssertMapCornersVisible(vm);
    }

    [Fact]
    public void C2_R02_map_view_all_works_without_scene_entities()
    {
        var vm = CameraC2MapFramingTestsHelpers.MapVm(seedInitialScene: false);
        vm.RunCommand.Execute("查看全部");

        Assert.Equal(new Vector3d(0, 0, 0), vm.ObservationCenter);
        CameraC2MapFramingTestsHelpers.AssertMapCornersVisible(vm);
    }

    [Fact]
    public void C2_R05_map_view_all_keeps_orthographic_mode()
    {
        var vm = CameraC2MapFramingTestsHelpers.MapVm();
        vm.RunCommand.Execute("视角-顶视图");

        vm.RunCommand.Execute("查看全部");

        Assert.Equal(ProjectionMode.Orthographic, vm.RenderSnapshot.CameraState.Mode);
        Assert.True(vm.RenderSnapshot.CameraState.OrthographicScale > 0);
        CameraC2MapFramingTestsHelpers.AssertMapCornersVisible(vm);
    }

    [Fact]
    public void C2_R06_map_view_all_keeps_perspective_mode()
    {
        var vm = CameraC2MapFramingTestsHelpers.MapVm();

        vm.RunCommand.Execute("查看全部");

        Assert.Equal(ProjectionMode.Perspective, vm.RenderSnapshot.CameraState.Mode);
    }

    [Fact]
    public void C2_R07_focus_without_draft_or_entity_leaves_camera_unchanged()
    {
        var vm = CameraC2MapFramingTestsHelpers.MapVm(seedInitialScene: false);
        var before = vm.RenderSnapshot.CameraState;

        vm.RunCommand.Execute("聚焦");

        Assert.Equal(before, vm.RenderSnapshot.CameraState);
        Assert.Equal("当前没有可聚焦对象。", vm.FooterMessage);
    }

    [Fact]
    public void C2_R09_view_all_focus_view_all_keeps_camera_finite()
    {
        var setup = CameraC2MapFramingTestsHelpers.DraftVm(3);
        var mapFrames = new List<CameraState>();
        var draftFrames = new List<CameraState>();

        for (var i = 0; i < 3; i++)
        {
            setup.Vm.FocusMap();
            CameraC2MapFramingTestsHelpers.AssertMapCornersVisible(setup.Vm);
            mapFrames.Add(setup.Vm.RenderSnapshot.CameraState);
            setup.Vm.RunCommand.Execute("聚焦");
            CameraC2MapFramingTestsHelpers.AssertDraftPointsVisible(setup.Vm, setup.Points);
            draftFrames.Add(setup.Vm.RenderSnapshot.CameraState);
        }

        CameraC2MapFramingTestsHelpers.AssertSamePose(mapFrames[0], mapFrames[1]);
        CameraC2MapFramingTestsHelpers.AssertSamePose(mapFrames[1], mapFrames[2]);
        CameraC2MapFramingTestsHelpers.AssertSamePose(draftFrames[0], draftFrames[1]);
        CameraC2MapFramingTestsHelpers.AssertSamePose(draftFrames[1], draftFrames[2]);
    }
}
