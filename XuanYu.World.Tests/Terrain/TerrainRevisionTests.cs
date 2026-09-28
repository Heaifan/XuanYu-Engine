using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Terrain;

public sealed class TerrainRevisionTests
{
    [Fact]
    public async Task Reimporting_same_hgt_tile_with_changed_height_advances_revision_and_runtime_height()
    {
        var directory = Directory.CreateDirectory(Path.Combine(
            Path.GetTempPath(), $"terrain-revision-{Guid.NewGuid():N}"));
        var path = Path.Combine(directory.FullName, "n23e121.hgt");
        try
        {
            File.WriteAllBytes(path, HgtBytes(100));
            var vm = new UiVm(null, () => true, seedInitialScene: false);
            Assert.True(await vm.ImportTerrainSourceAsync(path));
            var first = vm.RenderProjection.Projection.TerrainResources.Single();

            File.WriteAllBytes(path, HgtBytes(500));
            Assert.True(await vm.ImportTerrainSourceAsync(path));
            var second = vm.RenderProjection.Projection.TerrainResources.Single();

            Assert.Equal(first.TerrainId, second.TerrainId);
            Assert.True(second.Revision > first.Revision);
            Assert.Equal(500, second.Heightfield.ElevationAt(0, 0));
        }
        finally { directory.Delete(true); }
    }

    [Fact]
    public async Task Camera_zoom_pan_and_orbit_keep_terrain_revision_stable()
    {
        var directory = Directory.CreateDirectory(Path.Combine(
            Path.GetTempPath(), $"terrain-camera-{Guid.NewGuid():N}"));
        var path = Path.Combine(directory.FullName, "n23e121.hgt");
        try
        {
            File.WriteAllBytes(path, HgtBytes(100));
            var vm = new UiVm(null, () => true, seedInitialScene: false);
            Assert.True(await vm.ImportTerrainSourceAsync(path));
            var revision = vm.RenderProjection.Projection.TerrainResources.Single().Revision;

            Assert.True(vm.DollyCamera(120));
            Assert.Equal(revision, vm.RenderProjection.Projection.TerrainResources.Single().Revision);
            Assert.True(vm.BeginCameraNavigation(1, 100, 100, true, 800, 600));
            Assert.True(vm.PreviewCameraNavigation(1, 140, 120));
            Assert.True(vm.EndCameraNavigation(1));
            Assert.Equal(revision, vm.RenderProjection.Projection.TerrainResources.Single().Revision);
            Assert.True(vm.BeginCameraNavigation(2, 100, 100, false, 800, 600));
            Assert.True(vm.PreviewCameraNavigation(2, 140, 120));
            Assert.True(vm.EndCameraNavigation(2));
            Assert.Equal(revision, vm.RenderProjection.Projection.TerrainResources.Single().Revision);
        }
        finally { directory.Delete(true); }
    }

    [Fact]
    public async Task Terrain_edit_changes_render_heightfield_and_revision()
    {
        var directory = Directory.CreateDirectory(Path.Combine(
            Path.GetTempPath(), $"terrain-edit-{Guid.NewGuid():N}"));
        var path = Path.Combine(directory.FullName, "n23e121.hgt");
        try
        {
            File.WriteAllBytes(path, HgtBytes(100));
            var vm = new UiVm(null, () => true, seedInitialScene: false);
            Assert.True(await vm.ImportTerrainSourceAsync(path));
            var first = vm.RenderProjection.Projection.TerrainResources.Single();

            vm.TerrainWorld!.EditDelta.Set(new(0, 1), 400);
            var second = vm.RenderProjection.Projection.TerrainResources.Single();

            Assert.True(second.Revision > first.Revision);
            Assert.Equal(500, second.Heightfield.ElevationAt(0, 0));
        }
        finally { directory.Delete(true); }
    }

    static byte[] HgtBytes(short value) =>
        Enumerable.Range(0, 4).SelectMany(_ => new[] { (byte)(value >> 8), (byte)value }).ToArray();
}
