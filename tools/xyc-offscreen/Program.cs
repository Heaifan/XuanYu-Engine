using System.Security.Cryptography;
using System.Text.Json;
using XuanYu.Core.Math;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Session;
using XuanYu.World.Terrain.Import;

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: XuanYu.OffscreenTerrain <terrain.hgt> <output.png> [width] [height]");
    return 2;
}

var source = Path.GetFullPath(args[0]);
var output = Path.GetFullPath(args[1]);
var width = args.Length > 2 ? uint.Parse(args[2]) : 1024;
var height = args.Length > 3 ? uint.Parse(args[3]) : 768;
var tile = TerrainSourceImport.ReadMany([source]).Tiles.Single();
var terrain = tile.ToRenderSnapshot(tile.TileId, 1, 513);
var field = terrain.Heightfield;
var spanX = (field.Width - 1) * field.CellSizeMeters;
var spanY = (field.Height - 1) * field.CellSizeYMeters;
var centerHeight = (field.Metadata.MinElevation + field.Metadata.MaxElevation) * 0.5;
var altitude = Math.Max(spanX, spanY) * 1.15;
var camera = new RenderCameraProjection(
    new Vector3d(spanX * 0.5, spanY * 0.5, centerHeight + altitude),
    new Vector3d(0, 0, -1), new Vector3d(0, 1, 0), 45, 0.1, altitude * 4, 1);
var projection = new RenderProjection(camera, [], false, default, Terrain: terrain);

using var renderer = new VulkanOffscreenTerrainRenderer(projection, width, height, Console.WriteLine);
using var cancellation = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) => { eventArgs.Cancel = true; cancellation.Cancel(); };
var rgba = renderer.RenderRgba(cancellation.Token);
if (renderer.TerrainStats.TerrainTriangles <= 0)
    throw new InvalidOperationException("Terrain RenderPass 未提交任何 Terrain 三角形");
var terrainPixels = rgba.Chunk(4).Count(pixel =>
    pixel[0] != 89 || pixel[1] != 140 || pixel[2] != 204 || pixel[3] != 255);
if (terrainPixels == 0) throw new InvalidOperationException("GPU 读回中未发现 Terrain 像素");
var png = XuanYu.Core.Diagnostics.VulkanPngEncoder.EncodeRgba(
    checked((int)width), checked((int)height), rgba);
Directory.CreateDirectory(Path.GetDirectoryName(output)!);
File.WriteAllBytes(output, png);
var manifest = new
{
    renderId = Guid.NewGuid().ToString("N"),
    sourceHgt = source,
    sourceSha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(source))),
    device = renderer.DeviceName,
    buildIdentity = Environment.GetEnvironmentVariable("XYC_BUILD_ID") ?? "UNDECLARED",
    surface = false,
    swapchain = false,
    present = false,
    terrainId = terrain.TerrainId,
    terrainRevision = terrain.Revision,
    cameraRevision = camera.Revision,
    cameraPosition = camera.Position,
    cameraForward = camera.Forward,
    cameraUp = camera.Up,
    verticalFovDegrees = camera.VerticalFovDegrees,
    width,
    height,
    terrainPixels,
    terrainTriangles = renderer.TerrainStats.TerrainTriangles,
    pngSha256 = Convert.ToHexString(SHA256.HashData(png))
};
File.WriteAllText(Path.ChangeExtension(output, ".json"),
    JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true }));
Console.WriteLine(JsonSerializer.Serialize(manifest));
return 0;
