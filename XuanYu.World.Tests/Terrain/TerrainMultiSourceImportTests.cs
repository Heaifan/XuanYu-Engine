using System.IO.Compression;
using XuanYu.World.Terrain.Import;
using XuanYu.World.Terrain.Source;

namespace XuanYu.World.Tests.Terrain;

public sealed class TerrainMultiSourceImportTests
{
    [Fact]
    public void Mixed_hgt_and_zip_sources_create_one_tile_set()
    {
        var directory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), $"terrain-{Guid.NewGuid():N}"));
        var hgt = Path.Combine(directory.FullName, "n23e121.hgt");
        var zip = Path.Combine(directory.FullName, "n23e122.zip");
        try
        {
            File.WriteAllBytes(hgt, Bytes(10));
            using (var archive = ZipFile.Open(zip, ZipArchiveMode.Create))
            using (var stream = archive.CreateEntry("n23e122.hgt").Open())
                stream.Write(Bytes(20));

            var result = TerrainSourceImport.ReadMany([hgt, zip]);

            Assert.Equal(["n23e121", "n23e122"], result.Tiles.Select(tile => tile.TileId));
            Assert.Equal(20, result.GetElevation(23.5, 122.5, TerrainElevationInterpolation.Nearest).ElevationMeters);
        }
        finally { directory.Delete(true); }
    }

    [Fact]
    public void Unsupported_extension_is_rejected_explicitly()
    {
        var path = Path.Combine(Path.GetTempPath(), $"terrain-{Guid.NewGuid():N}.txt");
        File.WriteAllText(path, "not terrain");
        try
        {
            var error = Assert.Throws<TerrainSourceReadException>(() => TerrainSourceImport.ReadMany([path]));

            Assert.Contains("不支持", error.Message);
        }
        finally { File.Delete(path); }
    }

    static byte[] Bytes(short value)
    {
        using var stream = TerrainImportAcceptanceFixture.HgtStream(value, value, value, value);
        return stream.ToArray();
    }
}
