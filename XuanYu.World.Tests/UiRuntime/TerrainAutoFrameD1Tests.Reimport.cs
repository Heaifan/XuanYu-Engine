using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class TerrainAutoFrameD1Tests
{
    [Fact]
    public async Task Empty_import_does_not_move_camera()
    {
        var vm = NewVm();
        vm.UpdateViewportFrame(800, 600);
        var before = vm.RenderSnapshot.CameraState;

        Assert.False(await vm.ImportTerrainSourcesAsync([]));

        Assert.Equal(before, vm.RenderSnapshot.CameraState);
    }

    [Fact]
    public async Task Reimport_reframes_new_terrain_bounds()
    {
        var vm = NewVm();
        vm.UpdateViewportFrame(800, 600);
        var first = await WriteHgtAsync("n23e121.hgt", 10, 2);
        try
        {
            Assert.True(await vm.ImportTerrainSourcesAsync([first]));
            var firstCenter = vm.ObservationCenter;
            await File.WriteAllBytesAsync(first, HgtBytes(30, 2));
            Assert.True(await vm.ImportTerrainSourcesAsync([first]));
            Assert.NotEqual(firstCenter.Z, vm.ObservationCenter.Z);
            Assert.Equal(TerrainWorldCenter(vm), vm.ObservationCenter);
        }
        finally { File.Delete(first); }
    }

    [Fact]
    public async Task Framing_uses_world_meters_instead_of_latitude_longitude_degrees()
    {
        var vm = await ImportAsync([("n23e121.hgt", 10, 2)]);

        Assert.Equal(TerrainWorldCenter(vm), vm.ObservationCenter);
        Assert.True(vm.ObservationCenter.X > 1_000);
        Assert.True(vm.ObservationCenter.Y > 1_000);
    }

    [Fact]
    public async Task Framing_publishes_updated_render_snapshot_and_projection()
    {
        var vm = NewVm();
        vm.UpdateViewportFrame(800, 600);
        var changes = 0;
        vm.RenderProjectionChanged += _ => changes++;
        await ImportIntoAsync(vm, [("n23e121.hgt", 10, 2)]);

        Assert.True(changes > 0);
        Assert.Equal(vm.RenderSnapshot.CameraState.Revision,
            vm.RenderProjection.Projection.Camera.Revision);
        Assert.True(vm.RenderProjection.Success);
    }

}
