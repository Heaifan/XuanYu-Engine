using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class TerrainAutoFrameD1Tests
{
    [Fact]
    public async Task Single_tile_perspective_frames_rendered_world_bounds()
    {
        var vm = await ImportAsync([("n23e121.hgt", 10, 2)]);

        Assert.Equal(ProjectionMode.Perspective, vm.RenderSnapshot.CameraState.Mode);
        AssertTerrainVisible(vm);
    }

    [Fact]
    public async Task Multi_tile_perspective_frames_all_rendered_resources()
    {
        var vm = await ImportAsync([("n23e121.hgt", 10, 2), ("n23e122.hgt", 20, 3)]);

        Assert.Equal(2, vm.RenderProjection.Projection.TerrainResources.Count);
        Assert.NotEqual(vm.RenderProjection.Projection.TerrainResources[0].WorldOrigin,
            vm.RenderProjection.Projection.TerrainResources[1].WorldOrigin);
        AssertTerrainVisible(vm);
    }

    [Fact]
    public async Task Single_tile_orthographic_keeps_projection_and_frames_bounds()
    {
        var vm = NewVm();
        vm.UpdateViewportFrame(800, 600);
        vm.RunCommand.Execute("视角-顶视图");
        await ImportIntoAsync(vm, [("n23e121.hgt", 10, 2)]);

        Assert.Equal(ProjectionMode.Orthographic, vm.RenderSnapshot.CameraState.Mode);
        AssertTerrainVisible(vm);
    }

    [Fact]
    public async Task Multi_tile_orthographic_keeps_projection_and_frames_all_tiles()
    {
        var vm = NewVm();
        vm.UpdateViewportFrame(800, 600);
        vm.RunCommand.Execute("视角-顶视图");
        await ImportIntoAsync(vm, [("n23e121.hgt", 10, 2), ("n23e122.hgt", 20, 3)]);

        Assert.Equal(ProjectionMode.Orthographic, vm.RenderSnapshot.CameraState.Mode);
        AssertTerrainVisible(vm);
    }

    [Fact]
    public async Task Import_updates_navigation_center_and_camera_revision()
    {
        var vm = NewVm();
        vm.UpdateViewportFrame(800, 600);
        var revision = vm.RenderSnapshot.CameraState.Revision;

        await ImportIntoAsync(vm, [("n23e121.hgt", 10, 2)]);

        Assert.Equal(TerrainWorldCenter(vm), vm.ObservationCenter);
        Assert.True(vm.RenderSnapshot.CameraState.Revision > revision);
    }

}
