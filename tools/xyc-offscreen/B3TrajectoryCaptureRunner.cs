using System.Security.Cryptography;
using System.Text.Json;
using XuanYu.Core.Math;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Session;
using XuanYu.World.Terrain.Import;

internal static class B3TrajectoryCaptureRunner
{
    public static int Run(string[] args)
    {
        var values = args.Chunk(2).ToDictionary(x => x[0], x => x[1], StringComparer.OrdinalIgnoreCase);
        var source = Path.GetFullPath(values["--source"]); var trajectory = Path.GetFullPath(values["--trajectory"]);
        var output = Path.GetFullPath(values["--out"]); if (Directory.Exists(output)) throw new IOException("拒绝覆盖输出目录");
        if (Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(source))) != "BEF65347232885016C50F73B9EBDC09D52B82FD5EB4517377220F7B8211340E1") throw new InvalidDataException("HGT hash mismatch");
        var tile = TerrainSourceImport.ReadMany([source]).Tiles.Single(); var terrain = tile.ToRenderSnapshot(tile.TileId, 1, 2049);
        using var doc = JsonDocument.Parse(File.ReadAllText(trajectory)); var root = doc.RootElement;
        var poses = root.GetProperty("poses"); var steps = root.GetProperty("steps");
        Directory.CreateDirectory(output); var captures = new List<object>(); var frameRows = new List<object>();
        using var renderer = new VulkanOffscreenTerrainRenderer(Projection(Camera(poses.GetProperty("near-oblique"), 1), terrain), 1024, 768, Console.WriteLine);
        if (!renderer.DeviceName.Contains("RTX 3060", StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException($"Expected RTX 3060; got {renderer.DeviceName}");
        var revision = 1;
        foreach (var step in steps.EnumerateArray())
        {
            var id = step.GetProperty("id").GetString()!; var poseName = step.GetProperty("pose").GetString()!;
            var camera = Camera(poses.GetProperty(poseName), ++revision); renderer.UpdateProjection(Projection(camera, terrain));
            var rgba = renderer.RenderRgba(); var png = XuanYu.Core.Diagnostics.VulkanPngEncoder.EncodeRgba(1024, 768, rgba);
            var image = id + ".png"; File.WriteAllBytes(Path.Combine(output, image), png);
            var stats = renderer.TerrainStats;
            captures.Add(new { id, image, imageSha256 = Convert.ToHexString(SHA256.HashData(png)), camera = CameraJson(camera) });
            frameRows.Add(new { id, cameraRevision = revision, gpuMs = renderer.LastGpuRenderMilliseconds,
                triangles = stats.TerrainTriangles, visibleChunks = stats.VisibleChunks, drawCalls = stats.TerrainDrawCalls,
                lod0 = stats.LOD0Chunks, lod1 = stats.LOD1Chunks, lod2 = stats.LOD2Chunks,
                lod3 = stats.LOD3Chunks, lod4 = stats.LOD4Chunks });
        }
        var bundle = new { schema = "XYC-B3-CAPTURES/1", terrainSha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(source))), captures };
        File.WriteAllText(Path.Combine(output, "captures.json"), JsonSerializer.Serialize(bundle, new JsonSerializerOptions { WriteIndented = true }));
        File.WriteAllText(Path.Combine(output, "runtime.json"), JsonSerializer.Serialize(new { device = renderer.DeviceName,
            driverVersion = renderer.DeviceDriverVersion, buildIdentity = Environment.GetEnvironmentVariable("XYC_BUILD_ID"),
            width = 1024, height = 768, frameRows }, new JsonSerializerOptions { WriteIndented = true }));
        return 0;
    }

    static Vector3d V(JsonElement e) => new(e.GetProperty("x").GetDouble(), e.GetProperty("y").GetDouble(), e.GetProperty("z").GetDouble());
    static RenderCameraProjection Camera(JsonElement e, long revision) => new(V(e.GetProperty("position")), V(e.GetProperty("forward")),
        V(e.GetProperty("up")), e.GetProperty("fovDegrees").GetDouble(), e.GetProperty("nearPlane").GetDouble(),
        e.GetProperty("farPlane").GetDouble(), revision);
    static RenderProjection Projection(RenderCameraProjection camera, TerrainRenderResource terrain) => new(camera, [], false, default, Terrain: terrain);
    static object CameraJson(RenderCameraProjection c) => new { position = new { x = c.Position.X, y = c.Position.Y, z = c.Position.Z },
        forward = new { x = c.Forward.X, y = c.Forward.Y, z = c.Forward.Z }, up = new { x = c.Up.X, y = c.Up.Y, z = c.Up.Z },
        fovDegrees = c.VerticalFovDegrees, nearPlane = c.NearPlane, farPlane = c.FarPlane };
}
