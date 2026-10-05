using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class TerrainAutoFrameD1Tests
{
    [Fact]
    public async Task Initial_terrain_frame_then_orbit_does_not_rebuild_terrain_or_resolve_surface()
    {
        var vm = await ImportAsync([("n23e121.hgt", 10, 2)]);
        var terrainWorld = vm.TerrainWorld;
        var resource = vm.RenderProjection.Projection.TerrainResources.Single();
        var center = vm.ObservationCenter;
        Assert.True(vm.BeginCameraNavigation(1, 400, 300, false, 800, 600));
        Assert.True(vm.PreviewCameraNavigation(1, 430, 330));

        Assert.Same(terrainWorld, vm.TerrainWorld);
        Assert.Same(resource, vm.RenderProjection.Projection.TerrainResources.Single());
        Assert.All(vm.OrbitProbeEvents, e => Assert.Equal(center, e.Pivot));
        Assert.All(vm.OrbitProbeEvents.Where(e => e.Phase == OrbitProbePhase.Move),
            e => Assert.Equal(0, e.PivotResolveCount));
    }
}
