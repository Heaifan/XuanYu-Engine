using System.Security.Cryptography;
using System.Text.Json;
using XuanYu.Core.Math;
using XuanYu.Core.Diagnostics;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Session;
using XuanYu.World.Terrain.Import;
using XuanYu.World.Terrain.Source;

internal static class Program
{
    const string SourceHash = "BEF65347232885016C50F73B9EBDC09D52B82FD5EB4517377220F7B8211340E1";

    static int Main(string[] args)
    {
        var options = B4Options.Parse(args);
        var hash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(options.Source)));
        if (hash != SourceHash) throw new InvalidDataException($"Unexpected terrain SHA256: {hash}");
        Environment.SetEnvironmentVariable("XYE_PROBE_VIEWPORT", "1");
        Environment.SetEnvironmentVariable("XYE_TERRAIN_REPLAY", "1");
        Environment.SetEnvironmentVariable("XYE_TERRAIN_AB", options.Variant);
        Environment.SetEnvironmentVariable("XYE_TERRAIN_SSE_ERROR_TABLE", options.ErrorTable);
        Directory.CreateDirectory(options.Output);
        var tile = TerrainSourceImport.ReadMany([options.Source]).Tiles.Single();
        var terrain = tile.ToRenderSnapshot(tile.TileId, 1, 2049);
        var plan = JsonDocument.Parse(File.ReadAllText(options.Trajectory));
        var captures = B4Capture.Run(options, terrain, plan.RootElement);
        var manifest = new { schema = "XYC-B4-C-TRAJECTORY/1", variant = options.Variant,
            sourceSha256 = hash, head = options.Head, dirty = options.Dirty,
            device = captures.Device, driverVersion = captures.Driver, width = 1024, height = 768,
            frames = captures.Frames };
        File.WriteAllText(Path.Combine(options.Output, "frames.json"),
            JsonSerializer.Serialize(manifest, new JsonSerializerOptions
            { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        return 0;
    }
}
