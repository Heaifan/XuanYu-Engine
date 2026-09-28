using System.Buffers.Binary;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class TerrainHotpathTests
{
    [Fact]
    public async Task Camera_navigation_reuses_imported_terrain_resource()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.UpdateViewportFrame(800, 600);
        var directory = Directory.CreateDirectory(Path.Combine(
            Path.GetTempPath(), $"terrain-hotpath-{Guid.NewGuid():N}"));
        var path = Path.Combine(directory.FullName, "n23e121.hgt");
        WriteHgt(path, 100);
        try
        {
            Assert.True(await vm.ImportTerrainSourceAsync(path));
            var initial = vm.RenderProjection.Projection.TerrainResources[0];
            var initialField = initial.Heightfield;
            var builds = vm.TerrainRenderResourceBuildCount;
            Assert.Equal(1, builds);

            for (var index = 0; index < 20; index++)
                Assert.True(vm.DollyCamera(index % 2 == 0 ? 1 : -1));

            var current = vm.RenderProjection.Projection.TerrainResources[0];
            Assert.Same(initial, current);
            Assert.Same(initialField, current.Heightfield);
            Assert.Equal(builds, vm.TerrainRenderResourceBuildCount);
        }
        finally { directory.Delete(true); }
    }

    [Fact]
    public async Task Camera_navigation_reuses_all_multi_tile_resources()
    {
        var directory = Directory.CreateDirectory(Path.Combine(
            Path.GetTempPath(), $"terrain-hotpath-multi-{Guid.NewGuid():N}"));
        var paths = new[] { "n23e121.hgt", "n23e122.hgt" }
            .Select(name => Path.Combine(directory.FullName, name)).ToArray();
        foreach (var path in paths) WriteHgt(path, 100);
        try
        {
            var vm = new UiVm(null, () => true, seedInitialScene: false);
            vm.UpdateViewportFrame(800, 600);
            Assert.True(await vm.ImportTerrainSourcesAsync(paths));
            var initial = vm.RenderProjection.Projection.TerrainResources.ToArray();
            var builds = vm.TerrainRenderResourceBuildCount;
            Assert.Equal(1, builds);

            Assert.True(vm.DollyCamera(120));
            var current = vm.RenderProjection.Projection.TerrainResources.ToArray();

            Assert.Equal(initial.Length, current.Length);
            Assert.All(initial.Zip(current), pair => Assert.Same(pair.First, pair.Second));
            Assert.Equal(builds, vm.TerrainRenderResourceBuildCount);
        }
        finally { directory.Delete(true); }
    }

    static void WriteHgt(string path, short elevation)
    {
        var bytes = Enumerable.Range(0, 4).SelectMany(_ =>
        {
            Span<byte> value = stackalloc byte[2];
            BinaryPrimitives.WriteInt16BigEndian(value, elevation);
            return value.ToArray();
        }).ToArray();
        File.WriteAllBytes(path, bytes);
    }
}
