using System.Buffers.Binary;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class TerrainAutoFrameD1Tests
{
    static UiVm NewVm() => new(null, () => true, seedInitialScene: false);

    static async Task<UiVm> ImportAsync((string Name, short Height, int Side)[] tiles)
    {
        var vm = NewVm();
        vm.UpdateViewportFrame(800, 600);
        await ImportIntoAsync(vm, tiles);
        return vm;
    }

    static async Task ImportIntoAsync(UiVm vm, (string Name, short Height, int Side)[] tiles)
    {
        var paths = new List<string>();
        try
        {
            foreach (var tile in tiles)
            {
                var path = await WriteHgtAsync(tile.Name, tile.Height, tile.Side);
                paths.Add(path);
            }
            Assert.True(await vm.ImportTerrainSourcesAsync(paths));
        }
        finally
        {
            foreach (var path in paths) File.Delete(path);
        }
    }

    static async Task<string> WriteHgtAsync(string name, short height, int side)
    {
        var stem = Path.GetFileNameWithoutExtension(name);
        var path = Path.Combine(Path.GetTempPath(), $"{stem}-d1-{Guid.NewGuid():N}.hgt");
        await File.WriteAllBytesAsync(path, HgtBytes(height, side));
        return path;
    }

    static byte[] HgtBytes(short height, int side) =>
        Enumerable.Range(0, side * side).SelectMany(_ =>
        {
            Span<byte> bytes = stackalloc byte[2];
            BinaryPrimitives.WriteInt16BigEndian(bytes, height);
            return bytes.ToArray();
        }).ToArray();

    static Vector3d TerrainWorldCenter(UiVm vm)
    {
        var corners = TerrainCorners(vm.RenderProjection.Projection.TerrainResources,
            vm.VerticalExaggeration).ToArray();
        return new((corners.Min(p => p.X) + corners.Max(p => p.X)) / 2,
            (corners.Min(p => p.Y) + corners.Max(p => p.Y)) / 2,
            (corners.Min(p => p.Z) + corners.Max(p => p.Z)) / 2);
    }

    static void AssertTerrainVisible(UiVm vm)
    {
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.CameraState,
            new(0, 0, 800, 600, 800, 600, 1, 1));
        Assert.All(TerrainCorners(vm.RenderProjection.Projection.TerrainResources,
            vm.VerticalExaggeration), corner =>
        {
            Assert.True(projection.TryProjectWorldPoint(corner, out var screen));
            Assert.InRange(screen.X, 0, 800);
            Assert.InRange(screen.Y, 0, 600);
        });
    }

    static IEnumerable<Vector3d> TerrainCorners(
        IReadOnlyList<TerrainRenderResource> resources, double exaggeration)
    {
        foreach (var resource in resources)
        {
            var field = resource.Heightfield;
            var heights = field.ElevationMeters.Select(value => value * exaggeration).ToArray();
            var x = (field.Width - 1) * resource.CellSizeMeters;
            var y = (field.Height - 1) * resource.CellSizeMeters;
            var min = heights.Min(); var max = heights.Max();
            yield return new(0, 0, min); yield return new(x, 0, min);
            yield return new(0, y, min); yield return new(x, y, min);
            yield return new(0, 0, max); yield return new(x, 0, max);
            yield return new(0, y, max); yield return new(x, y, max);
        }
    }
}
