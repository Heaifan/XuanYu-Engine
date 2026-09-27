using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class TerrainAutoFrameD1Tests
{
    [Fact]
    public async Task Full_view_with_terrain_and_no_entities_keeps_terrain_visible()
    {
        var vm = await ImportAsync([("n23e121.hgt", 10, 2)]);
        vm.RunCommand.Execute("查看全部");
        AssertTerrainVisible(vm);
        Assert.Equal(TerrainWorldCenter(vm), vm.ObservationCenter);
    }

    [Fact]
    public async Task Terrain_context_hides_default_map_surface_from_render_projection()
    {
        var vm = await ImportAsync([("n23e121.hgt", 10, 2)]);
        Assert.True(vm.IsTerrainContext);
        Assert.False(vm.RenderProjection.Projection.HasMap);
        Assert.DoesNotContain(RenderDrawPlan.GetFrameDrawPlan(vm.RenderProjection.Projection),
            entry => entry.Kind is RenderDrawKind.MapGround or RenderDrawKind.MapBounds);
    }

    [Fact]
    public async Task Hundred_kilometer_full_view_keeps_camera_planes_finite()
    {
        var vm = await ImportAsync([("n23e121.hgt", 10, 2)]);
        var before = vm.RenderSnapshot.CameraState.Mode;
        vm.RunCommand.Execute("查看全部");
        var camera = vm.RenderSnapshot.CameraState;
        Assert.Equal(before, camera.Mode);
        Assert.True(double.IsFinite(camera.NearPlane) && camera.NearPlane > 0);
        Assert.True(double.IsFinite(camera.FarPlane) && camera.FarPlane > camera.NearPlane);
    }

    [Fact]
    public async Task Terrain_remains_visible_after_viewport_resize()
    {
        var vm = await ImportAsync([("n23e121.hgt", 10, 2)]);
        vm.UpdateViewportFrame(1600, 900);
        AssertTerrainVisible(vm);
    }
}
