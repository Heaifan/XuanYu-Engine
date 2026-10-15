using System.Diagnostics;
using System.Security.Cryptography;
using XuanYu.Core.Math;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Benchmark;
using XuanYu.Render.Vulkan.Session;
using XuanYu.World.Terrain.Source;

internal static class BenchmarkConfigurationRunner
{
    public static BenchmarkExecution Run(TerrainElevationTile tile, BenchmarkOptions options,
        int sampleCount, string poseName, string runDirectory)
    {
        TerrainRenderResource? terrain = null; RenderCameraProjection? camera = null;
        var configName = $"s{sampleCount}-{poseName}";
        var pngPath = Path.Combine(runDirectory, configName + ".png");
        var sampleTimer = Stopwatch.StartNew(); var totalTimer = Stopwatch.StartNew();
        try
        {
            BenchmarkFailureFactory.CheckMemoryBudget(sampleCount);
            var terrainId = Environment.GetEnvironmentVariable("XYE_B3_FIXED_TERRAIN") == "1"
                ? tile.TileId : $"{tile.TileId}-{sampleCount}-{poseName}";
            terrain = tile.ToRenderSnapshot(terrainId, 1, sampleCount);
            sampleTimer.Stop();
            var pose = BenchmarkPoseFactory.Create(terrain).Single(x => x.Name == poseName);
            camera = pose.Camera;
            var projection = new RenderProjection(pose.Camera, [], false, default, Terrain: terrain);
            var setup = Stopwatch.StartNew();
            using var renderer = new VulkanOffscreenTerrainRenderer(projection, 1024, 768, Console.WriteLine);
            setup.Stop();
            var device = renderer.DeviceName;
            if (!device.Contains("RTX 3060", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"目标 GPU 为 NVIDIA RTX 3060，实际设备为 {device}");
            var first = Stopwatch.StartNew(); var coldRgba = renderer.RenderRgba(); first.Stop();
            var coldGpu = renderer.LastGpuRenderMilliseconds;
            var warm = new List<BenchmarkFrame>(); var gpu = new List<double>(); var wall = new List<double>();
            for (var i = 0; i < options.WarmupFrames; i++) renderer.RenderRgba();
            byte[] finalRgba = coldRgba; var encode = Stopwatch.StartNew();
            _ = XuanYu.Core.Diagnostics.VulkanPngEncoder.EncodeRgba(1024, 768, coldRgba); encode.Stop();
            for (var i = 0; i < options.MeasuredFrames; i++)
            {
                var timer = Stopwatch.StartNew(); var rgba = renderer.RenderRgba(); timer.Stop();
                var gpuMs = renderer.LastGpuRenderMilliseconds;
                var encodeTimer = Stopwatch.StartNew(); var bytes = XuanYu.Core.Diagnostics.VulkanPngEncoder.EncodeRgba(1024, 768, rgba); encodeTimer.Stop();
                if (gpuMs.HasValue) gpu.Add(gpuMs.Value); wall.Add(timer.Elapsed.TotalMilliseconds);
                warm.Add(new(i + 1, gpuMs, timer.Elapsed.TotalMilliseconds,
                    encodeTimer.Elapsed.TotalMilliseconds, Hash(bytes)));
                finalRgba = rgba;
            }
            File.WriteAllBytes(pngPath, XuanYu.Core.Diagnostics.VulkanPngEncoder.EncodeRgba(1024, 768, finalRgba));
            var stats = renderer.TerrainStats; var pixels = BenchmarkImageAnalysis.CountTerrainPixels(finalRgba);
            var errors = BenchmarkImageAnalysis.HeightErrorP95(tile, terrain.Heightfield,
                projection.EffectiveTerrainTransform.VerticalExaggeration);
            var gpuPercentiles = gpu.Count >= 30
                ? VulkanBenchmarkPercentileCalculator.Calculate(gpu) : (VulkanBenchmarkPercentiles?)null;
            var wallPercentiles = VulkanBenchmarkPercentileCalculator.Calculate(wall);
            var probes = BenchmarkProbeReader.Read(Path.Combine(runDirectory, "probes", "terrain.log"), terrain.TerrainId);
            var status = pixels == 0 || stats.TerrainTriangles == 0 ? "INVALID_POSE" :
                options.MeasuredFrames < 30 || gpuPercentiles is null ? "INSUFFICIENT_SAMPLE" : "PASS";
            totalTimer.Stop();
            return new(new BenchmarkConfiguration
            {
                Device = device, DeviceDriverVersion = renderer.DeviceDriverVersion,
                TimestampValidBits = renderer.GpuTimestampValidBits,
                TimestampPeriodNs = renderer.GpuTimestampPeriodNanoseconds,
                Samples = sampleCount, Pose = poseName, Status = status,
                TerrainSampleMs = sampleTimer.Elapsed.TotalMilliseconds,
                TotalWallMs = totalTimer.Elapsed.TotalMilliseconds,
                RendererSetupMs = setup.Elapsed.TotalMilliseconds, ColdFrameMs = first.Elapsed.TotalMilliseconds,
                ColdGpuRenderMs = coldGpu, ColdEncodeMs = encode.Elapsed.TotalMilliseconds,
                CpuMeshBuildMs = renderer.TerrainMeshBuildMilliseconds,
                CpuUploadMs = renderer.TerrainUploadMilliseconds, EstimatedGpuBytes = stats.TerrainResidentGpuBytes,
                MeasuredGpuBytes = "UNKNOWN", CameraInsideChunkAabb = pose.CameraInsideChunkAabb,
                ChunkIdentity = pose.ChunkIdentity, Camera = camera,
                VisibleChunks = stats.VisibleChunks, CulledChunks = stats.CulledChunks,
                Lod0 = stats.LOD0Chunks, Lod1 = stats.LOD1Chunks, Lod2 = stats.LOD2Chunks,
                Lod3 = stats.LOD3Chunks, Lod4 = stats.LOD4Chunks,
                DrawCalls = stats.TerrainDrawCalls, Triangles = stats.TerrainTriangles,
                TerrainPixels = pixels, TerrainCoverage = pixels * 100d / (1024 * 768),
                HeightErrorWorldP95Meters = errors.WorldP95, HeightErrorVisualP95Meters = errors.VisualP95,
                ComparisonToPreviousResolution = null, Png = pngPath, PngSha256 = Hash(File.ReadAllBytes(pngPath)),
                ValidGpuSamples = gpu.Count, GpuMsP50 = gpuPercentiles?.P50, GpuMsP95 = gpuPercentiles?.P95,
                WallMsP50 = wallPercentiles.P50, WallMsP95 = wallPercentiles.P95,
                SamplesData = warm, ChunkProbes = probes
            }, finalRgba);
        }
        catch (Exception error)
        {
            totalTimer.Stop();
            return BenchmarkFailureFactory.Create(sampleCount, poseName, camera, sampleTimer.Elapsed.TotalMilliseconds,
                totalTimer.Elapsed.TotalMilliseconds, pngPath, error);
        }
    }

    static string Hash(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes));
}
