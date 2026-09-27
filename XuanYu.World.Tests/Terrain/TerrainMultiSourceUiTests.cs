using System.IO.Compression;
using System.Buffers.Binary;
using XuanYu.Editor.UI;
using XuanYu.World.Terrain.Source;

namespace XuanYu.World.Tests.Terrain;

public sealed class TerrainMultiSourceUiTests
{
    [Fact]
    public async Task Mixed_sources_are_retained_in_the_runtime_tile_set()
    {
        var directory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), $"terrain-{Guid.NewGuid():N}"));
        var hgt = Path.Combine(directory.FullName, "n23e121.hgt");
        var zip = Path.Combine(directory.FullName, "n23e122.zip");
        try
        {
            WriteHgt(hgt, 10);
            using (var archive = ZipFile.Open(zip, ZipArchiveMode.Create))
            using (var stream = archive.CreateEntry("n23e122.hgt").Open())
                stream.Write(Bytes(20));
            var vm = new UiVm(null, () => true, seedInitialScene: false);

            Assert.True(await vm.ImportTerrainSourcesAsync([hgt, zip]));

            Assert.Equal(2, vm.TerrainTiles?.Tiles.Count);
            Assert.Equal(20, vm.TerrainTiles?.GetElevation(23.5, 122.5,
                TerrainElevationInterpolation.Nearest).ElevationMeters);
        }
        finally { directory.Delete(true); }
    }

    [Fact]
    public async Task Multi_source_progress_never_regresses()
    {
        var directory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), $"terrain-{Guid.NewGuid():N}"));
        var first = Path.Combine(directory.FullName, "n23e121.hgt");
        var second = Path.Combine(directory.FullName, "n23e122.hgt");
        try
        {
            WriteHgt(first, 10); WriteHgt(second, 20);
            var vm = new UiVm(null, () => true, seedInitialScene: false);
            var values = new List<double>();
            vm.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(vm.TerrainImportPercentage))
                    values.Add(vm.TerrainImportPercentage);
            };

            Assert.True(await vm.ImportTerrainSourcesAsync([first, second]));

            Assert.NotEmpty(values);
            Assert.Equal(100, values[^1]);
            Assert.True(values.Zip(values.Skip(1), (left, right) => right >= left).All(item => item));
        }
        finally { directory.Delete(true); }
    }

    static void WriteHgt(string path, short value) => File.WriteAllBytes(path, Bytes(value));

    static byte[] Bytes(short value)
    {
        using var stream = new MemoryStream();
        Span<byte> sample = stackalloc byte[2];
        for (var index = 0; index < 4; index++)
        {
            BinaryPrimitives.WriteInt16BigEndian(sample, value);
            stream.Write(sample);
        }
        return stream.ToArray();
    }
}
