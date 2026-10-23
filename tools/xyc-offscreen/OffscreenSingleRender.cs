using System.Security.Cryptography;
using System.Text.Json;
using XuanYu.Core.Math;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Session;
using XuanYu.Render.Vulkan.Render.Terrain.Offscreen;
using XuanYu.World.Terrain.Import;

internal static class OffscreenSingleRender
{
    public static int Run(string[] args)
    {
        if (args.Length < 2) { Console.Error.WriteLine("Usage: <terrain.hgt> <output.png> [width] [height]"); return 2; }
        var source = Path.GetFullPath(args[0]); var output = Path.GetFullPath(args[1]);
        var width = args.Length > 2 ? uint.Parse(args[2]) : 1024;
        var height = args.Length > 3 ? uint.Parse(args[3]) : 768;
        var tile = TerrainSourceImport.ReadMany([source]).Tiles.Single();
        var terrain = tile.ToRenderSnapshot(tile.TileId, 1, 513);
        var field = terrain.Heightfield; var spanX = (field.Width - 1) * field.CellSizeMeters;
        var spanY = (field.Height - 1) * field.CellSizeYMeters;
        var altitude = Math.Max(spanX, spanY) * 1.15;
        var camera = new RenderCameraProjection(new(spanX * 0.5, spanY * 0.5,
            (field.Metadata.MinElevation + field.Metadata.MaxElevation) * 0.5 + altitude),
            new(0, 0, -1), new(0, 1, 0), 45, 0.1, altitude * 4, 1);
        var projection = new RenderProjection(camera, [], false, default, Terrain: terrain);
        using var renderer = new VulkanOffscreenTerrainRenderer(projection, width, height, Console.WriteLine);
        var rgba = renderer.RenderRgba();
        var pixels = BenchmarkImageAnalysis.CountTerrainPixels(rgba);
        if (renderer.TerrainStats.TerrainTriangles <= 0 || pixels == 0)
            throw new InvalidOperationException("GPU 离屏渲染没有有效 Terrain 像素");
        var png = XuanYu.Core.Diagnostics.VulkanPngEncoder.EncodeRgba((int)width, (int)height, rgba);
        Directory.CreateDirectory(Path.GetDirectoryName(output)!); File.WriteAllBytes(output, png);
        var manifest = new { sourceHgt = source, sourceSha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(source))),
            device = renderer.DeviceName, buildIdentity = Environment.GetEnvironmentVariable("XYC_BUILD_ID") ?? "UNDECLARED",
            surface = false, swapchain = false, present = false, terrainId = terrain.TerrainId,
            terrainRevision = terrain.Revision, cameraRevision = camera.Revision, width, height,
            terrainPixels = pixels, terrainTriangles = renderer.TerrainStats.TerrainTriangles,
            pngSha256 = Convert.ToHexString(SHA256.HashData(png)) };
        File.WriteAllText(Path.ChangeExtension(output, ".json"), JsonSerializer.Serialize(manifest,
            new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine(JsonSerializer.Serialize(manifest)); return 0;
    }
}
