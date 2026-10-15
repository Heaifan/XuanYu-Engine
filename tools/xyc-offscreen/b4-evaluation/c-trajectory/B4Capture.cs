using System.Security.Cryptography;
using System.Text.Json;
using XuanYu.Core.Diagnostics;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Session;

internal sealed record B4Result(string Device, uint Driver, IReadOnlyList<object> Frames);
internal static class B4Capture
{
    const int Width = 1024, Height = 768;

    public static B4Result Run(B4Options options, TerrainRenderResource terrain, JsonElement plan)
    {
        var frames = new List<object>(); var first = B4TrajectoryFrame.Expand(plan).First();
        using var renderer = new VulkanOffscreenTerrainRenderer(Projection(first.Camera, terrain),
            Width, Height, Console.WriteLine);
        if (!renderer.DeviceName.Contains("RTX 3060", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Expected B3 reference GPU RTX 3060; got {renderer.DeviceName}");
        foreach (var frame in B4TrajectoryFrame.Expand(plan))
        {
            var camera = frame.Camera;
            renderer.UpdateProjection(Projection(camera, terrain));
            var rgba = renderer.RenderRgba(); var stats = renderer.TerrainStats;
            var id = $"{frame.Track}-s{frame.Segment:D2}-f{frame.Index:D3}";
            var png = VulkanPngEncoder.EncodeRgba(Width, Height, rgba);
            File.WriteAllBytes(Path.Combine(options.Output, id + ".png"), png);
            var chunks = B4FrameEvidence.ReadChunks(Path.Combine(options.Output, "probes", "terrain.log"), camera.Revision);
            frames.Add(new { id, frame.Track, frame.Segment, frame.Index, frame.Count,
                cameraRevision = camera.Revision, camera, observationCenter = frame.ObservationCenter,
                gpuTimestampMs = renderer.LastGpuRenderMilliseconds, stats.VisibleChunks,
                stats.CulledChunks, stats.TerrainTriangles, stats.LOD0Chunks, stats.LOD1Chunks,
                stats.LOD2Chunks, stats.LOD3Chunks, stats.LOD4Chunks,
                coveragePixels = B4FrameEvidence.CountTerrain(rgba), chunks, png = id + ".png",
                pngSha256 = Convert.ToHexString(SHA256.HashData(png)) });
        }
        return new(renderer.DeviceName, renderer.DeviceDriverVersion, frames);
    }

    static RenderProjection Projection(RenderCameraProjection camera, TerrainRenderResource terrain) =>
        new(camera, [], false, default, Terrain: terrain);

}
